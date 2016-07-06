using WebGL;

namespace THREE
{
	public class WebGLRenderTargetCube : WebGLRenderTarget
	{
		public WebGLRenderTargetCube(int width, int height, dynamic options) : base(width, height, (JSObject)options)
		{
			activeCubeFace = 0;
		}
	}
}
