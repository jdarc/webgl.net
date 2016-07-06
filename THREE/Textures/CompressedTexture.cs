using WebGL;

namespace THREE
{
	public class CompressedTexture : Texture
	{
		public CompressedTexture(JSArray mipmaps = null, int width = 0, int height = 0)
		{
			image = new Image(width, height);
			this.mipmaps = mipmaps;

			generateMipmaps = false;
		}

		public CompressedTexture clone()
		{
			return base.clone(new CompressedTexture()) as CompressedTexture;
		}
	}
}
