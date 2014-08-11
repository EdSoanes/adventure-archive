using System;
using System.Drawing;
using BlackDragon.Core.Entities;
using BlackDragon.Fx.Extensions;
using MonoTouch.UIKit;
using MonoTouch.OpenGLES;
using OpenTK.Graphics.ES20;
using MonoTouch.CoreGraphics;
using System.Threading;

namespace BlackDragon.Fx.MapGL
{
    public class MapTileHelper
    {
//        public static EAGLContext RenderContext
//        {
//            get;
//            private set;
//        }
//
        public static EAGLContext UploadContext
        {
            get;
            private set;
        }

        // This handles our ProcessImageDelegate
        // just a simple way for us to be able to do whatever we want to our image
        // before it gets cached, so here's where you want to resize, etc.
        public static UIImage CropTile(UIImage image)
        {
			var croppedImage = image.CropImage(0, 0, Map.TileWidth, Map.TileHeight);
            image.Dispose();

            return croppedImage;
        }

        public static void InitializeOpenGL()
        {
            //RenderContext = new EAGLContext(EAGLRenderingAPI.OpenGLES2);
            UploadContext = new EAGLContext(EAGLRenderingAPI.OpenGLES2);
            //EAGLContext.SetCurrentContext(RenderContext);
            GL.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.OneMinusSrcAlpha);

			Thread.CurrentThread.Name = "MainThreadName";
        }
    }
}

