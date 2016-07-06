using System;
using WebGL;

namespace THREE
{
	public class Texture : JSEventDispatcher
	{
		public int id;
		public string name;
		public dynamic image;
		public JSArray mipmaps;
		public Func<object> mapping;
		public int wrapS;
		public int wrapT;
		public int magFilter;
		public int minFilter;
		public int anisotropy;
		public int format;
		public int type;
		public Vector2 offset;
		public Vector2 repeat;
		public bool generateMipmaps;
		public bool premultiplyAlpha;
		public bool flipY;
		public int unpackAlignment;
		public bool needsUpdate;
		public dynamic onUpdate;

		public bool __webglInit;
		public WebGLTexture __webglTexture;
		public int __oldAnisotropy;

		public Texture(Image image = null, Func<object> mapping = null, int wrapS = THREE.ClampToEdgeWrapping,
		               int wrapT = THREE.ClampToEdgeWrapping, int magFilter = THREE.LinearFilter,
		               int minFilter = THREE.LinearMipMapLinearFilter, int format = THREE.RGBAFormat,
		               int type = THREE.UnsignedByteType, int anisotropy = 1)
		{
			id = THREE.TextureIdCount++;

			name = "";

			this.image = image;
			mipmaps = new JSArray();

			this.mapping = mapping ?? THREE.UVMapping;

			this.wrapS = wrapS;
			this.wrapT = wrapT;

			this.magFilter = magFilter;
			this.minFilter = minFilter;

			this.anisotropy = anisotropy;

			this.format = format;
			this.type = type;

			offset = new Vector2();
			repeat = new Vector2(1, 1);

			generateMipmaps = true;
			premultiplyAlpha = false;
			flipY = true;
			unpackAlignment = 4;

			needsUpdate = false;
			onUpdate = null;
		}

		public Texture clone(Texture texture = null)
		{
			if (texture == null)
			{
				texture = new Texture();
			}

			texture.image = image;
			texture.mipmaps = mipmaps.slice(0);

			texture.mapping = mapping;

			texture.wrapS = wrapS;
			texture.wrapT = wrapT;

			texture.magFilter = magFilter;
			texture.minFilter = minFilter;

			texture.anisotropy = anisotropy;

			texture.format = format;
			texture.type = type;

			texture.offset.copy(offset);
			texture.repeat.copy(repeat);

			texture.generateMipmaps = generateMipmaps;
			texture.premultiplyAlpha = premultiplyAlpha;
			texture.flipY = flipY;
			texture.unpackAlignment = unpackAlignment;

			return texture;
		}

		public void dispose()
		{
			dispatchEvent(new JSEvent(this, "dispose"));
		}
	}
}
