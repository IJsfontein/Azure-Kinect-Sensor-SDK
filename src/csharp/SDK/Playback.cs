//------------------------------------------------------------------------------
// <copyright file="Playback.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
// </copyright>
//------------------------------------------------------------------------------
using System;

namespace Microsoft.Azure.Kinect.Sensor
{
    /// <summary>
    /// 
    /// </summary>
    public class Playback : IDisposable
    {
        private IntPtr handle = IntPtr.Zero;
        private Calibration calibration;
        private RecordConfiguration configuration;
        private NativeMethods.k4a_stream_result_t lastResult;
        private bool disposedValue;

        /// <summary>
        /// 
        /// </summary>
        public Playback()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        public Playback(string filePath) => this.Open(filePath);

        /// <summary>
        /// 
        /// </summary>
        ~Playback() => this.Dispose(false);

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public void Open(string filePath)
        {
            if (this.handle != IntPtr.Zero)
            {
                throw new InvalidOperationException("Playback file is open. Please close it first!");
            }

            AzureKinectException.ThrowIfNotSuccess(() =>
                NativeMethods.k4a_playback_open(filePath, out this.handle));

            NativeMethods.k4a_record_configuration_t nativeConfig = default;
            AzureKinectException.ThrowIfNotSuccess(() =>
                NativeMethods.k4a_playback_get_record_configuration(this.handle, out nativeConfig));
            this.configuration = nativeConfig.ToRecordConfiguration();

            AzureKinectException.ThrowIfNotSuccess(() =>
                NativeMethods.k4a_playback_get_calibration(this.handle, out this.calibration));
            if (this.calibration.ColorResolution == ColorResolution.Off)
            {
                return;
            }

            AzureKinectException.ThrowIfNotSuccess(() =>
                NativeMethods.k4a_playback_set_color_conversion(this.handle, ImageFormat.ColorBGRA32));
        }

        /// <summary>
        /// 
        /// </summary>
        public void Close()
        {
            if (!(this.handle != IntPtr.Zero))
            {
                return;
            }

            NativeMethods.k4a_playback_close(this.handle);
            this.handle = IntPtr.Zero;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public Calibration GetCalibration()
        {
            if (this.handle == IntPtr.Zero)
            {
                throw new InvalidOperationException("Playback file hasn't been opened.");
            }

            return this.calibration;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public RecordConfiguration GetConfiguration()
        {
            if (this.handle == IntPtr.Zero)
            {
                throw new InvalidOperationException("Playback file hasn't been opened.");
            }

            return configuration;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="AzureKinectException"></exception>
        public Capture GetNextCapture()
        {
            lock (this)
            {
                this.lastResult = NativeMethods.k4a_playback_get_next_capture(this.handle, out NativeMethods.k4a_capture_t captureHandle);
                return this.lastResult switch
                {
                    NativeMethods.k4a_stream_result_t.K4A_STREAM_RESULT_EOF => null,
                    NativeMethods.k4a_stream_result_t.K4A_STREAM_RESULT_FAILED => throw new AzureKinectException($"result = {this.lastResult}"),
                    _ => !captureHandle.IsInvalid
                        ? new Capture(captureHandle)
                        : throw new AzureKinectException("k4a_playback_get_next_capture did not return a valid capture handle")
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ImuSample GetNextImuSample()
        {
            lock (this)
            {
                ImuSample imuSample = new ImuSample();
                this.lastResult = NativeMethods.k4a_playback_get_next_imu_sample(this.handle, imuSample);
                if (this.lastResult == NativeMethods.k4a_stream_result_t.K4A_STREAM_RESULT_EOF)
                {
                    return null;
                }

                AzureKinectException.ThrowIfNotSuccess(() => this.lastResult);
                return imuSample;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsStreamReadSuccess() => this.lastResult == NativeMethods.k4a_stream_result_t.K4A_STREAM_RESULT_SUCCEEDED;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsEndOfStream() => this.lastResult == NativeMethods.k4a_stream_result_t.K4A_STREAM_RESULT_EOF;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ulong GetLastTimestamp() => NativeMethods.k4a_playback_get_last_timestamp_usec(this.handle);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        public bool SeekTimestamp(ulong timestamp)
        {
            try
            {
                AzureKinectException.ThrowIfNotSuccess(() =>
                    NativeMethods.k4a_playback_seek_timestamp(this.handle, timestamp, NativeMethods.k4a_playback_seek_origin_t.K4A_PLAYBACK_SEEK_BEGIN));
                return true;
            }
            catch (AzureKinectException)
            {
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (this.disposedValue)
            {
                return;
            }

            if (this.handle != IntPtr.Zero)
            {
                NativeMethods.k4a_playback_close(this.handle);
                this.handle = IntPtr.Zero;
            }

            this.disposedValue = true;
        }
    }
}