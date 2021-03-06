﻿namespace Unosquare.FFME.MacOS.Rendering
{
    using AppKit;
    using CoreGraphics;
    using Shared;
    using System;
    using System.Runtime.InteropServices;
    using Unosquare.FFME.Primitives;
    using Unosquare.FFME.MacOS.Platform;
    using System.Diagnostics;

    /// <summary>
    /// Provides Video Image Rendering via NSImage.
    /// </summary>
    /// <seealso cref="Unosquare.FFME.Rendering.IRenderer" />
    class VideoRenderer : IMediaRenderer
    {
        private readonly AtomicBoolean IsRenderingInProgress = new AtomicBoolean(false);

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Unosquare.FFME.MacOS.Rendering.VideoRenderer"/> class.
        /// </summary>
        /// <param name="mediaEngine">Media element core.</param>
        public VideoRenderer(MediaEngine mediaEngine)
        {
            MediaCore = mediaEngine;
        }

        /// <summary>
        /// Gets the media element core player component.
        /// </summary>
        /// <value>The media element core.</value>
        public MediaEngine MediaCore { get; }

        public void Close()
        {
            // placeholder
        }

        public void Pause()
        {
            // placeholder
        }

        public void Play()
        {
            // placeholder
        }

        public void Render(MediaBlock mediaBlock, TimeSpan clockPosition)
        {
            var block = mediaBlock as VideoBlock;
            if (block == null) return;

            if (IsRenderingInProgress.Value == true)
            {
                // frame skipped
                return;
            }

            try
            {
                IsRenderingInProgress.Value = true;
                var size = block.BufferLength;
                var bytes = new byte[size];
                Marshal.Copy(block.Buffer, bytes, 0, size);
                Transform(bytes, block.PixelWidth, block.PixelHeight);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                IsRenderingInProgress.Value = false;
            }
        }

        private void Transform(byte[] bytes, int width, int height)
        {
            try
            {
                var space = CGColorSpace.CreateDeviceRGB();
                var provider = new CGDataProvider(bytes);
                var i = new CGImage(
                    width,
                    height,
                    Constants.Video.BitsPerComponent,
                    Constants.Video.BitsPerPixel,
                    width * Constants.Video.BytesPerPixel,
                    space,
                    CGBitmapFlags.ByteOrderDefault,
                    provider,
                    null,
                    false,
                    CGColorRenderingIntent.Default);

                var nsImage = new NSImage(i, new CGSize(width, height));
                MacPlatform.Current.GuiInvoke(() =>
                {
                    ((MediaCore.Parent) as MediaElement).ImageView.Image = nsImage;
                });
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }

        public void Seek()
        {
            // placeholder
        }

        public void Stop()
        {
            // placeholder
        }

        public void Update(TimeSpan clockPosition)
        {
            // placeholder
        }

        public void WaitForReadyState()
        {
            // placeholder
        }
    }
}
