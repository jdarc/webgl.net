using WebGL;

namespace THREE
{
	public class Clock
	{
		public bool autoStart;
		public double startTime;
		public double oldTime;
		public double elapsedTime;
		public bool running;

		public Clock(dynamic autoStart = null)
		{
			this.autoStart = autoStart ?? true;

			startTime = 0;
			oldTime = 0;
			elapsedTime = 0;

			running = false;
		}

		public void start()
		{
			startTime = JSDate.now();
			oldTime = startTime;

			running = true;
		}

		public void stop()
		{
			getElapsedTime();

			running = false;
		}

		public dynamic getElapsedTime()
		{
			getDelta();

			return elapsedTime;
		}

		public dynamic getDelta()
		{
			var diff = 0.0;

			if (autoStart && ! running)
			{
				start();
			}

			if (running)
			{
				var newTime = JSDate.now();
				diff = 0.001 * (newTime - oldTime);
				oldTime = newTime;

				elapsedTime += diff;
			}

			return diff;
		}
	}
}
