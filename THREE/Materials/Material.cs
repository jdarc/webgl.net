using WebGL;

namespace THREE
{
	public class Material : JSEventDispatcher
	{
		public int id;
		public string name;
		public int side;
		public double opacity;
		public bool transparent;
		public int blending;
		public int blendSrc;
		public int blendDst;
		public int blendEquation;
		public bool depthTest;
		public bool depthWrite;
		public bool polygonOffset;
		public double polygonOffsetFactor;
		public double polygonOffsetUnits;
		public double alphaTest;
		public bool overdraw;
		public bool visible;
		public bool needsUpdate;

		public Material()
		{
			id = THREE.MaterialIdCount++;

			name = "";

			side = THREE.FrontSide;

			opacity = 1.0;
			transparent = false;

			blending = THREE.NormalBlending;

			blendSrc = THREE.SrcAlphaFactor;
			blendDst = THREE.OneMinusSrcAlphaFactor;
			blendEquation = THREE.AddEquation;

			depthTest = true;
			depthWrite = true;

			polygonOffset = false;
			polygonOffsetFactor = 0.0;
			polygonOffsetUnits = 0.0;

			alphaTest = 0.0;

			overdraw = false;

			visible = true;

			needsUpdate = true;
		}

		public void setValues(dynamic vals = null)
		{
			if (vals != null)
			{
				name = (string)(vals.name ?? name);
				side = (int)(vals.side ?? side);
				opacity = (double)(vals.opacity ?? opacity);
				transparent = (bool)(vals.transparent ?? transparent);
				blending = (int)(vals.blending ?? blending);
				blendSrc = (int)(vals.blendSrc ?? blendSrc);
				blendDst = (int)(vals.blendDst ?? blendDst);
				blendEquation = (int)(vals.blendEquation ?? blendEquation);
				depthTest = (bool)(vals.depthTest ?? depthTest);
				depthWrite = (bool)(vals.depthWrite ?? depthWrite);
				polygonOffset = (bool)(vals.polygonOffset ?? polygonOffset);
				polygonOffsetFactor = (double)(vals.polygonOffsetFactor ?? polygonOffsetFactor);
				polygonOffsetUnits = (double)(vals.polygonOffsetUnits ?? polygonOffsetUnits);
				alphaTest = (double)(vals.alphaTest ?? alphaTest);
				overdraw = (bool)(vals.overdraw ?? overdraw);
				visible = (bool)(vals.visible ?? visible);
				needsUpdate = (bool)(vals.needsUpdate ?? needsUpdate);
			}
		}

		public Material clone(Material material = null)
		{
			if (material == null)
			{
				material = new Material();
			}

			material.name = name;

			material.side = side;

			material.opacity = opacity;
			material.transparent = transparent;

			material.blending = blending;

			material.blendSrc = blendSrc;
			material.blendDst = blendDst;
			material.blendEquation = blendEquation;

			material.depthTest = depthTest;
			material.depthWrite = depthWrite;

			material.polygonOffset = polygonOffset;
			material.polygonOffsetFactor = polygonOffsetFactor;
			material.polygonOffsetUnits = polygonOffsetUnits;

			material.alphaTest = alphaTest;

			material.overdraw = overdraw;

			material.visible = visible;

			return material;
		}

		public void dispose()
		{
			dispatchEvent(new JSEvent(this, "dispose"));
		}
	}
}
