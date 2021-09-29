using UnityEngine;
using UnityEngine.Networking;
using System;
using System.IO;
using System.Net;
using System.Threading;


namespace JelleKUL.MeshAlignment
{
	/// <summary>
    /// Sets up a server and receives GET and POST requests
    /// </summary>
	public class UnityHttpListener : MonoBehaviour
	{

		[SerializeField]
		[Min(0)]
		private string httpPort = "1234";

		[SerializeField]
		private TextAsset htmlResponseFile;

		private HttpListener listener;
		private Thread listenerThread;

		string htmlString = "";

		public string lastReceivedData = "";

		public StringEvent OnDataReceived = new StringEvent();

		private bool hasNewData = false;

		void Start()
		{
			StartListening();
		}

        private void Update()
        {
            if (hasNewData)
            {
				hasNewData = false;
				OnDataReceived.Invoke(lastReceivedData);
			}
		}

        /// <summary>
        /// Start up the server
        /// </summary>
        public void StartListening()
		{
			listener = new HttpListener();
			listener.Prefixes.Add("http://localhost:" + httpPort + "/");
			listener.Prefixes.Add("http://127.0.0.1:" + httpPort + "/");
			listener.Prefixes.Add("http://" + IPLogger.GetLocalIPv4() + ":" + httpPort + "/");
			listener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;

			listener.Start();

			listenerThread = new Thread(StartListener);
			listenerThread.Start();
			Debug.Log("Server Started @ " + IPLogger.GetLocalIPv4() + ":" + httpPort + "/");

			if (htmlResponseFile != null)
			{
				Debug.Log("Preparing to send:");
				Debug.Log(htmlResponseFile.text);
				htmlString = htmlResponseFile.text;
			}
		}

		/// <summary>
        /// Stops the server
        /// </summary>
		public void StopListening()
		{
			listener.Stop();
		}

		private void StartListener()
		{
			while (true)
			{
				var result = listener.BeginGetContext(ListenerCallback, listener);
				result.AsyncWaitHandle.WaitOne();
			}
		}

		private void ListenerCallback(IAsyncResult result)
		{
			var context = listener.EndGetContext(result);

			Debug.Log("Method: " + context.Request.HttpMethod);
			Debug.Log("LocalUrl: " + context.Request.Url.LocalPath);

			// Obtain a response object.
			HttpListenerResponse response = context.Response;

			// enable CORS to allow other sites to send a request
			if (context.Request.HttpMethod == "OPTIONS")
			{
				response.AddHeader("Access-Control-Allow-Headers", "Content-Type, Accept, X-Requested-With");
				response.AddHeader("Access-Control-Allow-Methods", "GET, POST");
				response.AddHeader("Access-Control-Max-Age", "1728000");
			}
			response.AppendHeader("Access-Control-Allow-Origin", "*");

			// Construct a response.
			string responseString = "<!DOCTYPE html><html><body> <p>Request Received</p></body></html>";
			// if a HTML file is added, return that
			if (htmlString != "")
			{
				responseString = htmlString;
			}
			byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);

			// Get a response stream and write the response to it.
			response.ContentLength64 = buffer.Length;
			System.IO.Stream output = response.OutputStream;
			output.Write(buffer, 0, buffer.Length);

			if (context.Request.QueryString.AllKeys.Length > 0)
            {
				foreach (var key in context.Request.QueryString.AllKeys)
				{
					Debug.Log("Key: " + key + ", Value: " + context.Request.QueryString.GetValues(key)[0]);
				}
			}
				
			if (context.Request.HttpMethod == "POST")
			{
				Thread.Sleep(1000);
				var data_text = new StreamReader(context.Request.InputStream,
									context.Request.ContentEncoding).ReadToEnd();
				Debug.Log(data_text);
				lastReceivedData = data_text.ToString();
				hasNewData = true;
			}

			context.Response.Close();
		}

	}
}