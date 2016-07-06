using System;
using System.IO;
using System.Text;
using WebGL;

namespace THREE
{
	public class XMLHttpRequest
	{
		private enum ReadyState
		{
			Uninitialized = 0,
			Open = 1,
			Sent = 2,
			Receiving = 3,
			Loaded = 4
		}

		private int _readyState = (int)ReadyState.Uninitialized;
		private string _url;
		private int _contentLength;

		public Action onreadystatechange;
		public Action onload;
		public Action onerror;

		public string responseType;

		public dynamic response { get; private set; }
		public string responseText { get; private set; }
		public string responseXML { get; private set; }

		public int status { get; private set; }
		public string statusText { get; private set; }

		public int readyState
		{
			get { return _readyState; }
			private set
			{
				_readyState = value;
				if (onreadystatechange != null)
				{
					onreadystatechange();
				}
			}
		}

		public void open(string method, string url, bool async = false)
		{
			_url = url;
			readyState = (int)ReadyState.Open;
			responseText = null;
			responseXML = null;
			response = null;
			responseType = string.Empty;
			status = 0;
			statusText = string.Empty;
		}

		public void send()
		{
			readyState = (int)ReadyState.Sent;

			responseText = responseXML = response = null;

			var data = File.ReadAllBytes(_url);
			_contentLength = data.Length;

			readyState = (int)ReadyState.Receiving;

			switch (responseType)
			{
				case "":
				{
					var sb = new StringBuilder();
					foreach (var t in data)
					{
						sb.Append((char)t);
					}
					responseText = sb.ToString();
				}
					break;
				case "arraybuffer":
					response = new Uint8Array(data);
					break;
				default:
					throw new NotImplementedException();
			}

			readyState = (int)ReadyState.Loaded;
			status = 200;

			if (onload != null)
			{
				onload();
			}
		}

		public string getResponseHeader(string header)
		{
			return header.Equals("Content-Length") ? _contentLength.ToString() : string.Empty;
		}
	}
}
