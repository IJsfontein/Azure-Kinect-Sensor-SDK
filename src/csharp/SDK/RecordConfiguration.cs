using System;

namespace Microsoft.Azure.Kinect.Sensor
{
    /// <summary>
    /// 
    /// </summary>
    public class RecordConfiguration
    {
        /// <summary>
        /// 
        /// </summary>
        public ImageFormat ColorFormat { get; internal set; } = ImageFormat.ColorMJPG;
        /// <summary>
        /// 
        /// </summary>
        public ColorResolution ColorResolution { get; internal set; } = ColorResolution.Off;
        /// <summary>
        /// 
        /// </summary>
        public DepthMode DepthMode { get; internal set; } = DepthMode.Off;
        /// <summary>
        /// 
        /// </summary>
        public FPS CameraFPS { get; internal set; } = FPS.FPS30;
        /// <summary>
        /// 
        /// </summary>
        public bool ColorTrackEnabled { get; internal set; } = false;
        /// <summary>
        /// 
        /// </summary>
        public bool DepthTrackEnabled { get; internal set; } = false;
        /// <summary>
        /// 
        /// </summary>
        public bool IRTrackEnabled { get; internal set; } = false;
        /// <summary>
        /// 
        /// </summary>
        public bool ImuTrackEnabled { get; internal set; } = false;
        /// <summary>
        /// 
        /// </summary>
        public TimeSpan DepthDelayOffColor { get; internal set; } = TimeSpan.Zero;
        /// <summary>
        /// 
        /// </summary>
        public WiredSyncMode WiredSyncMode { get; internal set; } = WiredSyncMode.Standalone;
        /// <summary>
        /// 
        /// </summary>
        public TimeSpan SubordinateDelayOffMaster { get; internal set; } = TimeSpan.Zero;
        /// <summary>
        /// 
        /// </summary>
        public TimeSpan StartTimestampOffset { get; internal set; } = TimeSpan.Zero;
    }
}
