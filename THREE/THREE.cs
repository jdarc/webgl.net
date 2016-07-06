using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace THREE
{
	public partial class THREE
	{
		public const string REVISION = "55";

		public const int CullFaceNone = 0;
		public const int CullFaceBack = 1;
		public const int CullFaceFront = 2;
		public const int CullFaceFrontBack = 3;

		public const int FrontFaceDirectionCW = 0;
		public const int FrontFaceDirectionCCW = 1;

		public const int BasicShadowMap = 0;
		public const int PCFShadowMap = 1;
		public const int PCFSoftShadowMap = 2;

		public const int FrontSide = 0;
		public const int BackSide = 1;
		public const int DoubleSide = 2;

		public const int NoShading = 0;
		public const int FlatShading = 1;
		public const int SmoothShading = 2;

		public const int NoColors = 0;
		public const int FaceColors = 1;
		public const int VertexColors = 2;

		public const int NoBlending = 0;
		public const int NormalBlending = 1;
		public const int AdditiveBlending = 2;
		public const int SubtractiveBlending = 3;
		public const int MultiplyBlending = 4;
		public const int CustomBlending = 5;

		public const int AddEquation = 100;
		public const int SubtractEquation = 101;
		public const int ReverseSubtractEquation = 102;

		public const int ZeroFactor = 200;
		public const int OneFactor = 201;
		public const int SrcColorFactor = 202;
		public const int OneMinusSrcColorFactor = 203;
		public const int SrcAlphaFactor = 204;
		public const int OneMinusSrcAlphaFactor = 205;
		public const int DstAlphaFactor = 206;
		public const int OneMinusDstAlphaFactor = 207;

		public const int DstColorFactor = 208;
		public const int OneMinusDstColorFactor = 209;
		public const int SrcAlphaSaturateFactor = 210;

		public const int MultiplyOperation = 0;
		public const int MixOperation = 1;
		public const int AddOperation = 2;

		public const int RepeatWrapping = 1000;
		public const int ClampToEdgeWrapping = 1001;
		public const int MirroredRepeatWrapping = 1002;

		public const int NearestFilter = 1003;
		public const int NearestMipMapNearestFilter = 1004;
		public const int NearestMipMapLinearFilter = 1005;
		public const int LinearFilter = 1006;
		public const int LinearMipMapNearestFilter = 1007;
		public const int LinearMipMapLinearFilter = 1008;

		public const int UnsignedByteType = 1009;
		public const int ByteType = 1010;
		public const int ShortType = 1011;
		public const int UnsignedShortType = 1012;
		public const int IntType = 1013;
		public const int UnsignedIntType = 1014;
		public const int FloatType = 1015;

		public const int UnsignedShort4444Type = 1016;
		public const int UnsignedShort5551Type = 1017;
		public const int UnsignedShort565Type = 1018;

		public const int AlphaFormat = 1019;
		public const int RGBFormat = 1020;
		public const int RGBAFormat = 1021;
		public const int LuminanceFormat = 1022;
		public const int LuminanceAlphaFormat = 1023;

		public const int RGB_S3TC_DXT1_Format = 2001;
		public const int RGBA_S3TC_DXT1_Format = 2002;
		public const int RGBA_S3TC_DXT3_Format = 2003;
		public const int RGBA_S3TC_DXT5_Format = 2004;

		public static readonly Func<object> UVMapping = () => null;
		public static readonly Func<object> CubeReflectionMapping = () => null;
		public static readonly Func<object> CubeRefractionMapping = () => null;
		public static readonly Func<object> SphericalReflectionMapping = () => null;
		public static readonly Func<object> SphericalRefractionMapping = () => null;

		public static int TextureIdCount;
		public static int Object3DIdCount;
		public static int MaterialIdCount;
		public static int GeometryIdCount;

		public static readonly int LineStrip;
		public static readonly int LinePieces = 1;

		public static dynamic ShaderLib = WebGLShaders.ShaderLib;
		private static readonly Dictionary<string, dynamic> stuff;

		public static dynamic get(string name)
		{
			return stuff.ContainsKey(name) ? stuff[name] : null;
		}

		public static class SpriteAlignment
		{
			public static readonly Vector2 topLeft = new Vector2(1, -1);
			public static readonly Vector2 topCenter = new Vector2(0, -1);
			public static readonly Vector2 topRight = new Vector2(-1, -1);
			public static readonly Vector2 centerLeft = new Vector2(1, 0);
			public static readonly Vector2 center = new Vector2(0, 0);
			public static readonly Vector2 centerRight = new Vector2(-1, 0);
			public static readonly Vector2 bottomLeft = new Vector2(1, 1);
			public static readonly Vector2 bottomCenter = new Vector2(0, 1);
			public static readonly Vector2 bottomRight = new Vector2(-1, 1);
		}

		static THREE()
		{
			var fieldInfos = typeof(THREE).GetFields(BindingFlags.Static | BindingFlags.Public);
			stuff = fieldInfos.ToDictionary<FieldInfo, string, dynamic>(fieldInfo => fieldInfo.Name, fieldInfo => fieldInfo.GetValue(null));
		}
	}
}
