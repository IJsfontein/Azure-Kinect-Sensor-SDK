//------------------------------------------------------------------------------
// <copyright file="Record.cs" company="Microsoft">
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
    public class Record : IDisposable
    {
        private IntPtr handle = IntPtr.Zero;
        private NativeMethods.k4a_device_configuration_t nativeConfig;
        private bool disposedValue;

        /// <summary>
        /// 
        /// </summary>
        public Record()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="device"></param>
        /// <param name="config"></param>
        public Record(string filePath, Device device, DeviceConfiguration config) =>
            this.Create(filePath, device, config);

        /// <summary>
        /// 
        /// </summary>
        ~Record() => this.Dispose(false);

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
        /// <param name="device"></param>
        /// <param name="config"></param>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="Exception"></exception>
        public void Create(string filePath, Device device, DeviceConfiguration config)
        {
            if (this.handle != IntPtr.Zero)
            {
                throw new InvalidOperationException("Recording file is open. Please close it first!");
            }

            if (device == null)
            {
                throw new InvalidOperationException("Device should not be null.");
            }

            this.nativeConfig = config?.GetNativeConfiguration() ??
                               throw new Exception("DeviceConfiguration should not be null.");
            AzureKinectException.ThrowIfNotSuccess(() =>
                NativeMethods.k4a_record_create(filePath, device.DangerousGetHandle(), this.nativeConfig, out this.handle));
            AzureKinectException.ThrowIfNotSuccess(() => NativeMethods.k4a_record_write_header(this.handle));
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

            try
            {
                AzureKinectException.ThrowIfNotSuccess(() => NativeMethods.k4a_record_flush(this.handle));
            }
            finally
            {
                NativeMethods.k4a_record_close(this.handle);
                this.handle = IntPtr.Zero;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="capture"></param>
        /// <returns></returns>
        public bool WriteCapture(Capture capture)
        {
            if (capture == null)
            {
                return false;
            }

            lock (this)
            {
                AzureKinectException.ThrowIfNotSuccess(() =>
                    NativeMethods.k4a_record_write_capture(this.handle, capture.DangerousGetHandle()));
            }

            return true;
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
                NativeMethods.k4a_record_close(this.handle);
                this.handle = IntPtr.Zero;
            }

            this.disposedValue = true;
        }
    }
}
