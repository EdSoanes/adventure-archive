using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK;
using MonoTouch.GLKit;
using System.Drawing;
using OpenTK.Graphics.ES20;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using MonoTouch.OpenGLES;
using MonoTouch.CoreFoundation;
using BlackDragon.Core.IoC;
using BlackDragon.Core;

namespace BlackDragon.Fx.MapGL
{
    public abstract class MapShape
    {
        public enum StateType
        {
            Initialized,
            Downloading,
            ReadyToRender
        }

        private object _stateLock = new object();
        private StateType _state = StateType.Initialized;
        public StateType State
        {
            get
            {
                lock (_stateLock)
                    return _state;
            }
            private set
            {
                lock (_stateLock)
                    _state = value;
            }
        }

        private bool _configuredForTextures;

        protected GLKBaseEffect BaseEffect = new GLKBaseEffect();
        protected List<Vector2> _shapeVertices = new List<Vector2>();
        protected List<Vector2> _textureVertices = new List<Vector2>();

        protected GLKTextureInfo _texture;

        public PointF Position
        {
            get;
            set;
        }

        public float Scale
        {
            get;
            set;
        }

        public float Alpha
        {
            get;
            set;
        }

        public bool IsToBeRendered
        {
            get;
            set;
        }

        public int NoOfVertices
        { 
            get { return _shapeVertices.Count(); }
        }

		public DateTime LRUTime
		{
			get;
		    set;
		}

        public MapShape()
        {
			LRUTime = DateTime.Now;
            Scale = 1;
            Position = new PointF(0, 0);
            Alpha = 1f;
        }

        public void LoadImage(string fileName)
        {
            lock (_stateLock)
            {
                using (var image = UIImage.FromFile(fileName))
                {
                    NSError error;
                    NSDictionary options = NSDictionary.FromObjectAndKey(NSNumber.FromBoolean(false), GLKTextureLoader.OriginBottomLeft);

                    DiscardTexture();
                    _texture = GLKTextureLoader.FromImage(image.CGImage, options, out error);

                    if (error != null)
                        Console.WriteLine("Error loading texture from image: {0}", error);
                    else
                    {
                        if (!_configuredForTextures)
                        {
                            BaseEffect.Texture2d0.EnvMode = GLKTextureEnvMode.Modulate;
                            BaseEffect.Texture2d0.Target = GLKTextureTarget.Texture2D;
                            BaseEffect.Texture2d0.GLName = _texture.Name;

                            _configuredForTextures = true;
                        }

                        State = StateType.ReadyToRender;
                    }           
                }
            }
        }

        public void SetDownloadingState()
        {
            State = StateType.Downloading;
        }

        public void SetTexture(GLKTextureInfo texture)
        {
            lock (_stateLock)
            {
                DiscardTexture();
                _texture = texture;

				if (!_configuredForTextures)
				{
					BaseEffect.Texture2d0.EnvMode = GLKTextureEnvMode.Modulate;
					BaseEffect.Texture2d0.Target = GLKTextureTarget.Texture2D;

					_configuredForTextures = true;
				}

				BaseEffect.Texture2d0.GLName = _texture.Name;

                State = StateType.ReadyToRender;
            }
        }

        public void DiscardTexture()
        {
            lock (_stateLock)
            {
                if (_texture != null)
                {
                    GL.DeleteTextures(1, new int[] { (int)_texture.Name });

                    var err = GL.GetError();
                    if (err != 0)
                        Console.WriteLine("DiscardTexture: Failed with error {0}", err);
					else
						Console.WriteLine("Discarded texture: " + _texture.Name);

                    _texture.Dispose();
                    _texture = null;

                }

                State = StateType.Initialized;
            }
        }

		public int GetTextureToDiscard()
		{
			lock (_stateLock)
			{
				int id = -1;
				if (_texture != null)
				{
					id = (int)_texture.Name;
					BaseEffect.Texture2d0.GLName = 0;
					_texture = null;
					State = StateType.Initialized;
				}

				return id;
			}
		}

        public void Render(Matrix4 projectionMatrix)
        {
            lock (_stateLock)
            {
                if (_shapeVertices.Any() && State == StateType.ReadyToRender)
                {
                    //If the projection matrix changes, then update the base effect
                    if (BaseEffect.Transform.ProjectionMatrix != projectionMatrix)
                        BaseEffect.Transform.ProjectionMatrix = projectionMatrix;

                    var translate = Matrix4.CreateTranslation(Position.X, Position.Y, 0);
                    var scale = Matrix4.Scale(Scale);
                    var res = Matrix4.Mult(scale, translate);

                    BaseEffect.Transform.ModelViewMatrix = res;
                    BaseEffect.UseConstantColor = true;
                    BaseEffect.ConstantColor = new Vector4(Alpha, Alpha, Alpha, Alpha);
                    BaseEffect.PrepareToDraw();

                    if (_texture != null)
                    {
                        GL.EnableVertexAttribArray((int)GLKVertexAttrib.TexCoord0);
                        GL.VertexAttribPointer((int)GLKVertexAttrib.TexCoord0, 2, VertexAttribPointerType.Float, false, 0, _textureVertices.ToArray());
                    }

                    GL.EnableVertexAttribArray((int)GLKVertexAttrib.Position);
                    GL.VertexAttribPointer((int)GLKVertexAttrib.Position, 2, VertexAttribPointerType.Float, false, 0, _shapeVertices.ToArray());
                    GL.DrawArrays(BeginMode.TriangleStrip, 0, this.NoOfVertices);
                    GL.DisableVertexAttribArray((int)GLKVertexAttrib.Position);

                    if (_texture != null)
                    {
                        GL.DisableVertexAttribArray((int)GLKVertexAttrib.TexCoord0);
                    }
                }
            }
        }
    }
        
}
