using System;
using System.Collections.Generic;
using System.Drawing;
using System.Dynamic;
using WebGL;
using Image = WebGL.Image;

namespace THREE
{
	public class Loader
	{
		public const string crossOrigin = "anonymous";

		public bool showStatus;
		public dynamic statusDomElement;
		public Action onLoadStart;
		public Action onLoadProgress;
		public Action onLoadComplete;

		public Loader(bool showStatus)
		{
			this.showStatus = showStatus;
			statusDomElement = showStatus ? addStatusElement() : null;

			onLoadStart = () => { };
			onLoadProgress = () => { };
			onLoadComplete = () => { };
		}

		public dynamic addStatusElement()
		{
			return new ExpandoObject();
		}

		public virtual void updateProgress(dynamic progress)
		{
			var message = "Loaded ";

			if (progress.total)
			{
				message += (100.0 * progress.loaded / progress.total).toFixed(0) + "%";
			}
			else
			{
				message += (progress.loaded / 1000.0).toFixed(2) + " KB";
			}

			statusDomElement.innerHTML = message;
		}

		public virtual string extractUrlBase(string url)
		{
			var parts = new List<string>(url.Split('/'));
			if (parts.Count > 0)
			{
				parts.RemoveAt(parts.Count - 1);
			}
			return (parts.Count < 1 ? "." : string.Join("/", parts)) + '/';
		}

		public virtual JSArray initMaterials(dynamic materials, dynamic texturePath)
		{
			var array = new JSArray();

			for (var i = 0; i < materials.length; ++i)
			{
				array[i] = createMaterial(materials[i], texturePath);
			}

			return array;
		}

		public virtual bool needsTangents(dynamic materials)
		{
			for (int i = 0, il = materials.length; i < il; i++)
			{
				var m = materials[i];

				if (m is ShaderMaterial)
				{
					return true;
				}
			}

			return false;
		}

		public virtual dynamic createMaterial(dynamic m, dynamic texturePath)
		{
			// defaults
			var mtype = "MeshLambertMaterial";
			var mpars = JSObject.create(new
			{
				color = 0xeeeeee,
				opacity = 1.0,
				wireframe = false
			});

			// parameters from model file

			if (JSObject.eval(m.shading))
			{
				var shading = m.shading.ToLower();

				if (shading == "phong")
				{
					mtype = "MeshPhongMaterial";
				}
				else if (shading == "basic")
				{
					mtype = "MeshBasicMaterial";
				}
			}

			if (m.blending != null && THREE.get(m.blending) != null)
			{
				mpars.blending = THREE.get(m.blending);
			}

			if (m.transparent != null || m.opacity < 1.0)
			{
				mpars.transparent = m.transparent;
			}

			if (m.depthTest != null)
			{
				mpars.depthTest = m.depthTest;
			}

			if (m.depthWrite != null)
			{
				mpars.depthWrite = m.depthWrite;
			}

			if (m.visible != null)
			{
				mpars.visible = m.visible;
			}

			if (m.flipSided != null)
			{
				mpars.side = THREE.BackSide;
			}

			if (m.doubleSided != null)
			{
				mpars.side = THREE.DoubleSide;
			}

			if (m.wireframe != null)
			{
				mpars.wireframe = m.wireframe;
			}

			if (m.vertexColors != null)
			{
				if (m.vertexColors is string && m.vertexColors.equals("face"))
				{
					mpars.vertexColors = THREE.FaceColors;
				}
				else if (JSObject.eval(m.vertexColors))
				{
					mpars.vertexColors = THREE.VertexColors;
				}
			}

			// colors

			if (JSObject.eval(m.colorDiffuse))
			{
				mpars.color = rgb2Hex(m.colorDiffuse);
			}
			else if (JSObject.eval(m.DbgColor))
			{
				mpars.color = m.DbgColor;
			}

			if (JSObject.eval(m.colorSpecular))
			{
				mpars.specular = rgb2Hex(m.colorSpecular);
			}

			if (JSObject.eval(m.colorAmbient))
			{
				mpars.ambient = rgb2Hex(m.colorAmbient);
			}

			// modifiers

			if (JSObject.eval(m.transparency))
			{
				mpars.opacity = m.transparency;
			}

			if (JSObject.eval(m.specularCoef))
			{
				mpars.shininess = m.specularCoef;
			}

			// textures

			if (JSObject.eval(m.mapDiffuse) && JSObject.eval(texturePath))
			{
				createTexture(mpars, "map", m.mapDiffuse, texturePath, m.mapDiffuseRepeat, m.mapDiffuseOffset, m.mapDiffuseWrap, m.mapDiffuseAnisotropy);
			}

			if (JSObject.eval(m.mapLight) && JSObject.eval(texturePath))
			{
				createTexture(mpars, "lightMap", m.mapLight, texturePath, m.mapLightRepeat, m.mapLightOffset, m.mapLightWrap, m.mapLightAnisotropy);
			}

			if (JSObject.eval(m.mapBump) && JSObject.eval(texturePath))
			{
				createTexture(mpars, "bumpMap", m.mapBump, texturePath, m.mapBumpRepeat, m.mapBumpOffset, m.mapBumpWrap, m.mapBumpAnisotropy);
			}

			if (JSObject.eval(m.mapNormal) && JSObject.eval(texturePath))
			{
				createTexture(mpars, "normalMap", m.mapNormal, texturePath, m.mapNormalRepeat, m.mapNormalOffset, m.mapNormalWrap, m.mapNormalAnisotropy);
			}

			if (JSObject.eval(m.mapSpecular) && JSObject.eval(texturePath))
			{
				createTexture(mpars, "specularMap", m.mapSpecular, texturePath, m.mapSpecularRepeat, m.mapSpecularOffset, m.mapSpecularWrap, m.mapSpecularAnisotropy);
			}

			if (JSObject.eval(m.mapBumpScale))
			{
				mpars.bumpScale = m.mapBumpScale;
			}

			// special case for normal mapped material

			dynamic material = new MeshBasicMaterial();

			if (JSObject.eval(m.mapNormal))
			{
				var shader = THREE.ShaderLib["normalmap"];
				var uniforms = WebGLShaders.UniformsUtils.clone(shader.uniforms);

				uniforms["tNormal"].value = mpars.normalMap;

				if (JSObject.eval(m.mapNormalFactor))
				{
					uniforms["uNormalScale"].value.set(m.mapNormalFactor, m.mapNormalFactor);
				}

				if (JSObject.eval(mpars.map))
				{
					uniforms["tDiffuse"].value = mpars.map;
					uniforms["enableDiffuse"].value = true;
				}

				if (JSObject.eval(mpars.specularMap))
				{
					uniforms["tSpecular"].value = mpars.specularMap;
					uniforms["enableSpecular"].value = true;
				}

				if (JSObject.eval(mpars.lightMap))
				{
					uniforms["tAO"].value = mpars.lightMap;
					uniforms["enableAO"].value = true;
				}

				// for the moment don't handle displacement texture

				uniforms["uDiffuseColor"].value.setHex(mpars.color);
				uniforms["uSpecularColor"].value.setHex(mpars.specular);
				uniforms["uAmbientColor"].value.setHex(mpars.ambient);

				uniforms["uShininess"].value = mpars.shininess;

				if (mpars.opacity != null)
				{
					uniforms["uOpacity"].value = mpars.opacity;
				}

				var parameters = JSObject.create(new {shader.fragmentShader, shader.vertexShader, uniforms, lights = true, fog = true});
				material = new ShaderMaterial(parameters);

				if (JSObject.eval(mpars.transparent))
				{
					material.transparent = true;
				}
			}
			else
			{
				if (mtype == "MeshLambertMaterial")
				{
					material = new MeshLambertMaterial(mpars);
				}
				else if (mtype == "MeshPhongMaterial")
				{
					material = new MeshPhongMaterial(mpars);
				}
				else if (mtype == "MeshBasicMaterial")
				{
					material = new MeshBasicMaterial(mpars);
				}
			}

			if (m.DbgName != null && material != null)
			{
				material.name = m.DbgName;
			}

			return material;
		}

		private static bool isPow2(double n)
		{
			var l = System.Math.Log(n) / Math.LN2;
			return System.Math.Floor(l) == l;
		}

		private static int nearestPow2(double n)
		{
			var l = System.Math.Log(n) / Math.LN2;
			return (int)System.Math.Pow(2, System.Math.Round(l));
		}

		private static int rgb2Hex(JSArray rgb)
		{
			return ((int)rgb[0] * 255 << 16) + ((int)rgb[1] * 255 << 8) + (int)rgb[2] * 255;
		}

		private static void loadImage(dynamic where, string url)
		{
			var image = new Image();

			image.addEventListener("load", evt =>
			{
				if (!isPow2(image.width) || !isPow2(image.height))
				{
//					var width = nearestPow2(image.width);
//					var height = nearestPow2(image.height);

//					where.image.width = width;
//					where.image.height = height;
//					where.image.getContext("2d").drawImage(image, 0, 0, width, height);

						where.image = image;
				}
				else
				{
					where.image = image;
				}

				where.needsUpdate = true;
			});

		    image.src = url;
		}

		private static void createTexture(dynamic where, string name, string sourceFile, string texturePath, dynamic repeat, dynamic offset, dynamic wrap, dynamic anisotropy)
		{
			var isCompressed = sourceFile.ToLower().EndsWith(".dds");
			var fullPath = texturePath + "/" + sourceFile;

			if (isCompressed)
			{
				var texture = ImageUtils.loadCompressedTexture(fullPath);
				where[name] = texture;
			}
			else
			{
				var texture = new Image {src = fullPath};
				where[name] = new Texture(texture);
			}

			where[name].sourceFile = sourceFile;
			where[name].name = name;

			if (JSObject.eval(repeat))
			{
				where[name].repeat.set(repeat[0], repeat[1]);

				if (repeat[0] != 1)
				{
					where[name].wrapS = THREE.RepeatWrapping;
				}
				if (repeat[1] != 1)
				{
					where[name].wrapT = THREE.RepeatWrapping;
				}
			}

			if (JSObject.eval(offset))
			{
				where[name].offset.set(offset[0], offset[1]);
			}

			if (JSObject.eval(wrap))
			{
				var wrapMap = JSObject.create(new
				{
					repeat = THREE.RepeatWrapping,
					mirror = THREE.MirroredRepeatWrapping
				});

				if (wrapMap[wrap[0]] != null)
				{
					where[name].wrapS = wrapMap[wrap[0]];
				}
				if (wrapMap[wrap[1]] != null)
				{
					where[name].wrapT = wrapMap[wrap[1]];
				}
			}

			if (JSObject.eval(anisotropy))
			{
				where[name].anisotropy = anisotropy;
			}

			if (!isCompressed)
			{
				loadImage(where[name], fullPath);
			}
		}
	}
}
