using System;
using WebGL;

namespace THREE
{
	public class ParticleCanvasMaterial : Material
	{
		public Color color;
		public Action<dynamic, dynamic> program;

		public ParticleCanvasMaterial(dynamic parameters = null)
		{
			parameters = parameters ?? new JSObject();

			color = new Color(parameters.color ?? 0xffffff);
			program = parameters.program ?? (Action<dynamic, dynamic>)((context, clr) => { });

			setValues(parameters);
		}

		public ParticleCanvasMaterial clone()
		{
			var material = (ParticleCanvasMaterial)clone(new ParticleCanvasMaterial());

			material.color.copy(color);
			material.program = program;

			return material;
		}
	}
}
