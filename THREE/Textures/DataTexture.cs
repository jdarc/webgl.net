using System;

namespace THREE
{
	public class DataTexture : Texture
	{
		public DataTexture(dynamic data, int width, int height, int format, int type = THREE.UnsignedByteType,
		                   Func<object> mapping = null, int wrapS = THREE.ClampToEdgeWrapping, int wrapT = THREE.ClampToEdgeWrapping,
		                   int magFilter = THREE.LinearFilter, int minFilter = THREE.LinearMipMapLinearFilter, int anisotropy = 1)
			: base(null, mapping, wrapS, wrapT, magFilter, minFilter, format, type, anisotropy)
		{
			image = create(new {data, width, height});
		}

		public DataTexture clone()
		{
			return base.clone(new DataTexture(null, 0, 0, 0)) as DataTexture;
		}
	}
}
