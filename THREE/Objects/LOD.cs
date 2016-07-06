using WebGL;

namespace THREE
{
	public class LOD : Object3D
	{
		public JSArray LODs;

		public LOD()
		{
			LODs = new JSArray();
		}

		public void addLevel(Object3D object3D, double visibleAtDistance = 0.0)
		{
			visibleAtDistance = System.Math.Abs(visibleAtDistance);

			var l = 0;
			for (; l < LODs.length; l++)
			{
				if (visibleAtDistance < LODs[l].visibleAtDistance)
				{
					break;
				}
			}

			LODs.splice(l, 0, create(new {object3D, visibleAtDistance}));
			add(object3D);
		}

		public void update(Camera camera)
		{
			if (LODs.length > 1)
			{
				camera.matrixWorldInverse.getInverse(camera.matrixWorld);

				var inverse = camera.matrixWorldInverse;
				var distance = -(inverse.elements[2] * matrixWorld.elements[12] + inverse.elements[6] * matrixWorld.elements[13] + inverse.elements[10] * matrixWorld.elements[14] + inverse.elements[14]);

				LODs[0].object3D.visible = true;

				var l = 1;
				for (; l < LODs.length; l++)
				{
					if (distance >= LODs[l].visibleAtDistance)
					{
						LODs[l - 1].object3D.visible = false;
						LODs[l].object3D.visible = true;
					}
					else
					{
						break;
					}
				}

				for (; l < LODs.length; l++)
				{
					LODs[l].object3D.visible = false;
				}
			}
		}
	}
}
