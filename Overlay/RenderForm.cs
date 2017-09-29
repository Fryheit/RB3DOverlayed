using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ff14bot.Helpers;

namespace RB3DOverlayed.Overlay
{
    public partial class RenderForm : Form
    {
        private readonly IntPtr _trackedWindow;
        private Task _updatePositionTask;
        private Task _overlayTask;
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        public RenderForm(IntPtr trackedWindow)
        {
            InitializeComponent();

            _trackedWindow = trackedWindow;
        }

        internal static float AspectRatio = 0.0f;
        private async Task UpdatePositionAsync(IntPtr ourHandle, IntPtr trackingWindow)
        {
            while (true)
            {
                Rect clientRect;
                Imports.GetClientRect(trackingWindow, out clientRect);

                Point start = Point.Empty;
                Imports.ClientToScreen(trackingWindow, ref start);

                int x = start.X;
                int y = start.Y;
                int width = clientRect.Right - clientRect.Left;
                int height = clientRect.Bottom - clientRect.Top;

                AspectRatio = (float)width / height;
                //Logging.Write(LogLevel.Normal, "Aspect Ratio: {0}", AspectRatio);
                const int gwHwndprev = 3;
                IntPtr prev = Imports.GetWindow(trackingWindow, gwHwndprev);

                const int doNotActivate = 0x10;
                Imports.SetWindowPos(ourHandle, prev, x, y, width, height, doNotActivate);

                try
                {
                    await Task.Delay(50, _cts.Token);
                }                
                catch (TaskCanceledException)
                {
                    return;
                }
                catch (OperationCanceledException)
                {
                    return;
                }
            }
        }

        private void RenderForm_Load(object sender, EventArgs e)
        {
            // Keep position of form updated. Note that we depend
            // on this running once for the margins below!
            _updatePositionTask = UpdatePositionAsync(Handle, _trackedWindow);

            // Make the window's border completely transparant
            Imports.SetWindowLong(Handle, Constants.GWL_EXSTYLE, (IntPtr)(Imports.GetWindowLong(Handle, Constants.GWL_EXSTYLE) ^ Constants.WS_EX_LAYERED ^ Constants.WS_EX_TRANSPARENT));

            Imports.SetLayeredWindowAttributes(Handle, 0, 255, Constants.LWA_ALPHA);
            //Imports.SetLayeredWindowAttributes(Handle, 0, 0, Constants.LWA_COLORKEY);

            // Expand the Aero Glass Effect Border to the WHOLE form.
            // since we have already had the border invisible we now
            // have a completely invisible window - apart from the DirectX
            // renders NOT in black.
            Margins marg = new Margins
            {
                Left = 0,
                Top = 0,
                Right = Width,
                Bottom = Height
            };

            Imports.DwmExtendFrameIntoClientArea(Handle, ref marg);

            _overlayTask = OverlayManager.RunOverlay(this, _cts.Token);
        }

        public async Task ShutdownAsync()
        {
            _cts.Cancel();
            await _updatePositionTask;
            await _overlayTask;
            _cts.Dispose();

            BeginInvoke(new Action(Close));
        }
    }
}
