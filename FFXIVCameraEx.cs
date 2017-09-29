namespace RB3DOverlayed
{
    using ff14bot;
    using ff14bot.Managers;
    using Overlay;
    using SlimDX;
    
    internal static class FFXIVCameraEx
    {
        private static Matrix Convert(System.Numerics.Matrix4x4 mat)
        {
            return new Matrix
                   {
                       M11 = mat.M11, M12 = mat.M12, M13 = mat.M13, M14 = mat.M14,
                       M21 = mat.M21, M22 = mat.M22, M23 = mat.M23, M24 = mat.M24,
                       M31 = mat.M31, M32 = mat.M32, M33 = mat.M33, M34 = mat.M34,
                       M41 = mat.M41, M42 = mat.M42, M43 = mat.M43, M44 = mat.M44,
                   };
        }

        public static Matrix View => FF14Camera.View;
        public static Matrix Projection => Matrix.PerspectiveFovRH(CameraManager.FoV, RenderForm.AspectRatio, 0.1f, 150);
    }

    /// <summary>
    /// 64bit model
    /// </summary>
    public static class FF14Camera
    {
        public static Vector3 CameraLocation => Core.Memory.Read<Vector3>(CameraManager.CameraLocationPtr);

        public static Vector3 Up = new Vector3(0, 1, 0);

        public static Matrix View
        {
            get
            {
                //var cameraPtr = Core.Memory.Read<IntPtr>(Core.Memory.ImageBase + 0x16bf8c0);

                var camera = Core.Memory.Read<SlimDX.Vector3>(CameraManager.CameraLocationPtr);//Core.Memory.Read<System.Numerics.Vector3>(cameraPtr + 0x50); //104
                var focus = Core.Memory.Read<SlimDX.Vector3>(CameraManager.FocusPtr);//Core.Memory.Read<System.Numerics.Vector3>(cameraPtr + 0x80); //0x80

                //var rot = Core.Memory.Read<float>(Core.Memory.Read<IntPtr>(Core.Memory.ImageBase + 0x16bf8c0) + 0x134);

                //var rotation = Matrix.RotationZ(MathEx.NormalizeRadian(rot));
                //var transformed = SlimDX.Vector3.TransformCoordinate(dir.Convert(), rotation);
                //var final = camera.Convert() + transformed;

                //SlimDX.Matrix.LookAtRH()

                return Matrix.LookAtRH(camera, focus, Up);
                //return Matrix4x4.CreateLookAt(camera, focus, Up);
            }
        }

        public static Matrix Projection
        {
            get
            {
                //var cameraPtr = Core.Memory.Read<IntPtr>(Core.Memory.ImageBase + 0x16bf8c0);
                //var fov = Core.Memory.Read<float>(cameraPtr + 0x124);
                //return Matrix4x4.CreatePerspectiveFieldOfView(fov, RenderForm.AspectRatio, 0.1f, 150);


                return Matrix.PerspectiveFovRH(CameraManager.FoV, RenderForm.AspectRatio, 0.1f, 150);
            }
        }
    }

    /**
     *       this._PlayerTurnAccel = (float) (pointer + 78 * 4);
      this._PlayerTiltAccel = (float) (pointer + 79 * 4);
      this._FocusX = (float) (pointer + 108 * 4);
      this._FocusY = (float) (pointer + 109 * 4);
      this._FocusZ = (float) (pointer + 110 * 4);
      this._CameraX = (float) (pointer + 104 * 4);
      this._CameraY = (float) (pointer + 105 * 4);
      this._CameraZ = (float) (pointer + 106 * 4);
    */
}
