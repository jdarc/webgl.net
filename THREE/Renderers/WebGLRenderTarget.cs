using WebGL;

namespace THREE
{
	public class WebGLRenderTarget : JSEventDispatcher
	{
		public int width;
		public int height;
		public int wrapS;
		public int wrapT;
		public int magFilter;
		public int minFilter;
		public int anisotropy;
		public Vector2 offset;
		public Vector2 repeat;
		public int format;
		public int type;
		public bool depthBuffer;
		public bool stencilBuffer;
		public bool generateMipmaps;
		public dynamic shareDepthFrom;

		public WebGLTexture __webglTexture;
		public dynamic __webglFramebuffer;
		public dynamic __webglRenderbuffer;
		public int activeCubeFace;

		public WebGLRenderTarget(int width, int height, dynamic options = null)
		{
			this.width = width;
			this.height = height;

			options = options ?? new JSObject();

			wrapS = options.wrapS ?? THREE.ClampToEdgeWrapping;
			wrapT = options.wrapT ?? THREE.ClampToEdgeWrapping;

			magFilter = options.magFilter ?? THREE.LinearFilter;
			minFilter = options.minFilter ?? THREE.LinearMipMapLinearFilter;

			anisotropy = options.anisotropy ?? 1;

			offset = new Vector2(0, 0);
			repeat = new Vector2(1, 1);

			format = options.format ?? THREE.RGBAFormat;
			type = options.type ?? THREE.UnsignedByteType;

			depthBuffer = options.depthBuffer ?? true;
			stencilBuffer = options.stencilBuffer ?? true;

			generateMipmaps = true;

			shareDepthFrom = null;
		}

		public WebGLRenderTarget clone()
		{
			var tmp = new WebGLRenderTarget(width, height)
			{
				wrapS = wrapS,
				wrapT = wrapT,
				magFilter = magFilter,
				minFilter = minFilter,
				anisotropy = anisotropy
			};

			tmp.offset.copy(offset);
			tmp.repeat.copy(repeat);

			tmp.format = format;
			tmp.type = type;

			tmp.depthBuffer = depthBuffer;
			tmp.stencilBuffer = stencilBuffer;

			tmp.generateMipmaps = generateMipmaps;

			tmp.shareDepthFrom = shareDepthFrom;

			return tmp;
		}

		public void dispose()
		{
			dispatchEvent(new JSEvent(this, "dispose"));
		}
	}
}
