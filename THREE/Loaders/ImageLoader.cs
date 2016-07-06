using WebGL;

namespace THREE
{
	public class ImageLoader : JSEventDispatcher
	{
		public string crossOrigin;

		public void load(string url, Image image = null)
		{
			if (image == null)
			{
				image = new Image();
			}

			image.addEventListener("load", evt =>
			{
				dynamic loadEvent = new JSEvent(this, "load");
				loadEvent.content = image;
				dispatchEvent(loadEvent);
			});

			image.addEventListener("error", evt =>
			{
				dynamic errorEvent = new JSEvent(this, "error");
				errorEvent.message = "Couldn\'t load URL [" + url + "]";
				dispatchEvent(errorEvent);
			});

			if (eval(crossOrigin)) ;

			image.src = url;
		}
	}
}
