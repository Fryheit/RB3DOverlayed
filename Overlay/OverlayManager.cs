namespace RB3DOverlayed.Overlay
{
    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using ff14bot;
    using ff14bot.Behavior;
    using ff14bot.Helpers;
    using ff14bot.Managers;
    using ff14bot.Overlay3D;
    using SlimDX;
    using SlimDX.Direct3D9;
    
    public static class OverlayManager
    {
        public static async Task RunOverlay(Form window, CancellationToken ct)
        {
            IntPtr handle = window.Handle;
            int width = window.Width;
            int height = window.Height;
            await Task.Run(() => RunRenderLoop(handle, width, height, ct));
        }

        public static event Action<DrawingContext> Drawing;

        // This is our actual loop function
        private static void RunRenderLoop(IntPtr window, int width, int height, CancellationToken ct)
        {
            using (var d3d = new Direct3D())
            {
                PresentParameters presentParams = CreatePresentParams(d3d, window, width, height);

                using (Device device = new Device(d3d, 0, DeviceType.Hardware, window,CreateFlags.HardwareVertexProcessing,presentParams))
                {
                    using (DrawingContext ctx = new DrawingContext(device))
                    {
                        while (!ct.IsCancellationRequested)
                        {
                            Stopwatch timer = Stopwatch.StartNew();
                            Render(device, ctx);

                            timer.Stop();
                            int toSleep = 1000 / 60 - (int) timer.ElapsedMilliseconds;
                            if (toSleep > 0)
                                ct.WaitHandle.WaitOne(toSleep);
                        }
                    }
                }
            }
        }

        private static void Render(Device device, DrawingContext ctx)
        {
            try
            {
                SetupRenderStates(device);
            }
            catch (Exception ex)
            {
                Logging.Write(LogLevel.Normal, "[D3d Overlay] Exception {0}", ex);
            }

            // Clear the backbuffer to a black color.
            device.Clear(ClearFlags.All, Color.FromArgb(0, 0, 0, 0), 1.0f, 0);

            device.BeginScene();
            try
            {
                //if (TreeRoot.IsRunning)
                RaptureAtkUnitManager.Update();

                if(!CommonBehaviors.IsLoading)
                {
                    using (Core.Memory.AcquireFrame(true))
                    {
                        ctx.UpdateZBuffer();
                        //device.SetRenderState(RenderState.ZWriteEnable, false);
                        Drawing?.Invoke(ctx);
                        Overlay3D.InvokeDrawing(new DrawingEventArgs(ctx));
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.Write(LogLevel.Normal, "[D3d Overlay] Exception {0}", ex);
            }

            device.EndScene();
            device.Present();
            
        }

        private static PresentParameters CreatePresentParams(Direct3D d3d, IntPtr window, int width, int height)
        {
            PresentParameters presentParams =
                new PresentParameters
                {
                    Windowed = true,
                    SwapEffect = SwapEffect.Discard,
                    DeviceWindowHandle = window,
                    BackBufferFormat = Format.A8R8G8B8,
                    BackBufferWidth = width,
                    BackBufferHeight = height,
                    EnableAutoDepthStencil = true,
                    AutoDepthStencilFormat = Format.D24S8,
                };

            int msQuality;
            if (d3d.CheckDeviceMultisampleType(0, DeviceType.Hardware, Format.A8R8G8B8, true,
                                               MultisampleType.NonMaskable, out msQuality))
            {
                presentParams.Multisample = MultisampleType.NonMaskable;
                presentParams.MultisampleQuality = msQuality - 1;
            }

            return presentParams;
        }

        private static void SetupRenderStates(Device device)
        {
            device.SetTransform(TransformState.View, FFXIVCameraEx.View);
            device.SetTransform(TransformState.Projection, FFXIVCameraEx.Projection);
            device.SetTransform(TransformState.World, Matrix.Identity);

            // Required to enabled 3D drawing
            device.SetRenderState(RenderState.ColorWriteEnable, ColorWriteEnable.All);

            // Depth
            //device.SetRenderState(RenderState.ZEnable, ZBufferType.UseZBuffer);
            //device.SetRenderState(RenderState.ZWriteEnable, true);
            //device.SetRenderState(RenderState.ZFunc, Compare.LessEqual);

            device.SetRenderState(RenderState.CullMode, Cull.None);

            // Lighting / Alpha Stuff
            device.SetRenderState(RenderState.Lighting, false);
            device.SetRenderState(RenderState.AlphaBlendEnable, true);
            device.SetRenderState(RenderState.SourceBlend, Blend.SourceAlpha);
            device.SetRenderState(RenderState.DestinationBlend, Blend.InverseSourceAlpha);

            // Multisampling
            device.SetRenderState(RenderState.MultisampleAntialias, true);
        }
    }
}
