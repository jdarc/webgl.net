using System;
using WebGL;

namespace THREE
{
	public static class ImageUtils
	{
		public static Texture loadTexture(string url, dynamic mapping = null, Action<dynamic> onLoad = null, dynamic onError = null)
		{
			var image = new Image();
			dynamic texture = new Texture(image, mapping);

			var loader = new ImageLoader();

			loader.addEventListener("load", evt =>
			{
				texture.image = ((dynamic)evt).content;
				texture.needsUpdate = true;

				if (onLoad != null)
				{
					onLoad(texture);
				}
			});

			loader.addEventListener("error", evt =>
			{
				if (onError != null)
				{
					onError(((dynamic)evt).message);
				}
			});

			loader.load(url, image);

			texture.sourceFile = url;

			return texture;
		}

		public static Texture loadTextureCube(dynamic array, dynamic mapping = null, dynamic onLoad = null, dynamic onError = null)
		{
			var images = new JSArray();
			var loadCount = 0;

			var texture = new Texture();
			texture.image = images;
			if (mapping != null)
			{
				texture.mapping = mapping;
			}

			texture.flipY = false;

			for (int i = 0, il = array.length; i < il;++ i)
			{
				var cubeImage = new Image();
				images[i] = cubeImage;

				cubeImage.addEventListener("load", delegate
				{
					loadCount += 1;

					if (loadCount == 6)
					{
						texture.needsUpdate = true;
						if (onLoad != null)
						{
							onLoad(texture);
						}
					}
				});

				cubeImage.addEventListener("error", onError);

				cubeImage.src = array[i];
			}

			return texture;
		}

		public static Texture loadCompressedTexture(dynamic url, dynamic mapping = null, dynamic onLoad = null, dynamic onError = null)
		{
			var texture = new CompressedTexture();
			texture.mapping = mapping;

			var request = new XMLHttpRequest();

			request.onload = () =>
			{
				var buffer = new Uint8Array(request.response);

				var dds = parseDDS(buffer.buffer, true);

				texture.format = dds.format;

				texture.mipmaps = dds.mipmaps;
				texture.image.width = dds.width;
				texture.image.height = dds.height;

				texture.generateMipmaps = false;

				texture.needsUpdate = true;

				if (onLoad != null)
				{
					onLoad(texture);
				}
			};

			request.onerror = onError;

			request.open("GET", url, true);
			request.responseType = "arraybuffer";
			request.send();

			return texture;
		}

		public static Texture loadCompressedTextureCube(dynamic array, Func<object> mapping, Action<dynamic> onLoad, Action onError = null)
		{
			dynamic images = new JSArray();
			images.loadCount = 0;

			dynamic texture = new CompressedTexture();
			texture.image = images;
			if (mapping != null)
			{
				texture.mapping = mapping;
			}

			texture.flipY = false;

			texture.generateMipmaps = false;

			Action<dynamic, dynamic> generateCubeFaceCallback = (rq, img) =>
			{
				var buffer = rq.response;
				var dds = ImageUtils.parseDDS(buffer, true);

				img.format = dds.format;

				img.mipmaps = dds.mipmaps;
				img.width = dds.width;
				img.height = dds.height;

				images.loadCount += 1;

				if (images.loadCount == 6)
				{
					texture.format = dds.format;
					texture.needsUpdate = true;
					if (onLoad != null)
					{
						onLoad(texture);
					}
				}
			};

			if (array is JSArray)
			{
				for (int i = 0, il = array.length; i < il;++ i)
				{
					var cubeImage = new JSObject();
					images[i] = cubeImage;

					var request = new XMLHttpRequest();

					request.onload = () => generateCubeFaceCallback(request, cubeImage);
					request.onerror = () => onError();

					var url = array[i];

					request.open("GET", url, true);
					request.responseType = "arraybuffer";
					request.send();
				}
			}
			else
			{
				var url = array;
				var request = new XMLHttpRequest();

				request.onload = () =>
				{
					var dds = parseDDS(new Uint8Array(request.response).buffer, true);

					if (dds.isCubemap)
					{
						var faces = dds.mipmaps.length / dds.mipmapCount;

						for (var f = 0; f < faces; f++)
						{
							images[f] = JSObject.create(new {mipmaps = new JSArray()});

							for (var i = 0; i < dds.mipmapCount; i++)
							{
								images[f].mipmaps.push(dds.mipmaps[f * dds.mipmapCount + i]);
								images[f].format = dds.format;
								images[f].width = dds.width;
								images[f].height = dds.height;
							}
						}

						texture.format = dds.format;
						texture.needsUpdate = true;
						if (onLoad != null)
						{
							onLoad(texture);
						}
					}
				};

				request.onerror = onError;

				request.open("GET", url, true);
				request.responseType = "arraybuffer";
				request.send();
			}

			return texture;
		}

		public static dynamic parseDDS(ArrayBuffer buffer, dynamic loadMipmaps)
		{
			var dds = JSObject.create(new {mipmaps = new JSArray(), width = 0, height = 0, format = (object)null, mipmapCount = 1});

			const int DDS_MAGIC = 0x20534444;

			const int DDSD_CAPS = 0x1;
			const int DDSD_HEIGHT = 0x2;
			const int DDSD_WIDTH = 0x4;
			const int DDSD_PITCH = 0x8;
			const int DDSD_PIXELFORMAT = 0x1000;
			const int DDSD_MIPMAPCOUNT = 0x20000;
			const int DDSD_LINEARSIZE = 0x80000;
			const int DDSD_DEPTH = 0x800000;

			const int DDSCAPS_COMPLEX = 0x8;
			const int DDSCAPS_MIPMAP = 0x400000;
			const int DDSCAPS_TEXTURE = 0x1000;

			const int DDSCAPS2_CUBEMAP = 0x200;
			const int DDSCAPS2_CUBEMAP_POSITIVEX = 0x400;
			const int DDSCAPS2_CUBEMAP_NEGATIVEX = 0x800;
			const int DDSCAPS2_CUBEMAP_POSITIVEY = 0x1000;
			const int DDSCAPS2_CUBEMAP_NEGATIVEY = 0x2000;
			const int DDSCAPS2_CUBEMAP_POSITIVEZ = 0x4000;
			const int DDSCAPS2_CUBEMAP_NEGATIVEZ = 0x8000;
			const int DDSCAPS2_VOLUME = 0x200000;

			const int DDPF_ALPHAPIXELS = 0x1;
			const int DDPF_ALPHA = 0x2;
			const int DDPF_FOURCC = 0x4;
			const int DDPF_RGB = 0x40;
			const int DDPF_YUV = 0x200;
			const int DDPF_LUMINANCE = 0x20000;

			var FOURCC_DXT1 = fourCCToInt32("DXT1");
			var FOURCC_DXT3 = fourCCToInt32("DXT3");
			var FOURCC_DXT5 = fourCCToInt32("DXT5");

			const int headerLengthInt = 31;

			const int off_magic = 0;

			const int off_size = 1;
			const int off_flags = 2;
			const int off_height = 3;
			const int off_width = 4;

			const int off_mipmapCount = 7;

			const int off_pfFlags = 20;
			const int off_pfFourCC = 21;

			const int off_caps = 27;
			const int off_caps2 = 28;
			const int off_caps3 = 29;
			const int off_caps4 = 30;

			var header = new Int32Array(buffer, 0, headerLengthInt);

			if (header[off_magic] != DDS_MAGIC)
			{
				JSConsole.error("ImageUtils.parseDDS(): Invalid magic number in DDS header");
				return dds;
			}

			if ((header[off_pfFlags] & DDPF_FOURCC) == 0)
			{
				JSConsole.error("ImageUtils.parseDDS(): Unsupported format, must contain a FourCC code");
				return dds;
			}

			int blockBytes;

			var fourCC = header[off_pfFourCC];

			if (fourCC == FOURCC_DXT1)
			{
				blockBytes = 8;
				dds.format = THREE.RGB_S3TC_DXT1_Format;
			}
			else if (fourCC == FOURCC_DXT3)
			{
				blockBytes = 16;
				dds.format = THREE.RGBA_S3TC_DXT3_Format;
			}
			else if (fourCC == FOURCC_DXT5)
			{
				blockBytes = 16;
				dds.format = THREE.RGBA_S3TC_DXT5_Format;
			}
			else
			{
				JSConsole.error("ImageUtils.parseDDS(): Unsupported FourCC code: " + int32ToFourCC(fourCC));
				return dds;
			}

			dds.mipmapCount = 1;

			if ((header[off_flags] & DDSD_MIPMAPCOUNT) != 0 && loadMipmaps != false)
			{
				dds.mipmapCount = System.Math.Max(1, header[off_mipmapCount]);
			}

			dds.isCubemap = (header[off_caps2] & DDSCAPS2_CUBEMAP) != 0;

			dds.width = header[off_width];
			dds.height = header[off_height];

			var dataOffset = header[off_size] + 4;

			int width = dds.width;
			int height = dds.height;

			var faces = dds.isCubemap ? 6 : 1;

			for (var face = 0; face < faces; face++)
			{
				for (var i = 0; i < dds.mipmapCount; i++)
				{
					int dataLength = (int)(System.Math.Max(4, width) / 4 * System.Math.Max(4, height) / 4 * blockBytes);
					var byteArray = new Uint8Array(buffer, dataOffset, dataLength);

					var mipmap = JSObject.create(new {data = byteArray, width, height});
					dds.mipmaps.push(mipmap);

					dataOffset += dataLength;

					width = (int)System.Math.Max(width * 0.5, 1);
					height = (int)System.Math.Max(height * 0.5, 1);
				}

				width = dds.width;
				height = dds.height;
			}

			return dds;
		}

		public static Func<string, int> fourCCToInt32 = value => value[0] + (value[1] << 8) + (value[2] << 16) + (value[3] << 24);

		public static Func<int, string> int32ToFourCC = value => new String(new[] {(char)(value & 0xff), (char)((value >> 8) & 0xff), (char)((value >> 16) & 0xff), (char)((value >> 24) & 0xff)});
	}
}
