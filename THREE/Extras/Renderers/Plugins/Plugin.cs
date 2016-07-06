namespace THREE
{
	public abstract class Plugin
	{
		public abstract void init(dynamic renderer);

		public abstract void update(dynamic scene, dynamic camera);

		public abstract void render(dynamic scene, dynamic camera, int viewportWidth = 0, int viewportHeight = 0);
	}
}
