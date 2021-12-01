using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using UnityEngine;

namespace JelleKUL.MeshAlignment
{

    public class HttpServer : MonoBehaviour
    {
		[Header("Server Parameters")]
		[SerializeField]
		private bool startAtStart = true;
		[SerializeField]
		private bool logData = false;
		[SerializeField]
		[Min(0)]
		private string httpPort = "1234";
		[SerializeField]
		[Tooltip("The HTML file to send when a request is made to the port")]
		private TextAsset htmlResponseFile;

		[Header("Incomming data")]
		public string lastReceivedData = "";
		public StringEvent OnDataReceived = new StringEvent();

		private bool hasNewData = false;
		private bool listening = false;
		private HttpListener listener;
		private Thread listenerThread;
		private string htmlString = "";

		void Start()
		{
			if(startAtStart) StartListening();
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
			listening = true;

			if (htmlResponseFile != null)
			{
				htmlString = htmlResponseFile.text;
			}
		}

		/// <summary>
		/// Stops the server
		/// </summary>
		public void StopListening()
		{
			if(listening)listener.Stop();
			listening = false;
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

			if (logData)
			{
				Debug.Log("Method: " + context.Request.HttpMethod);
				Debug.Log("LocalUrl: " + context.Request.Url.LocalPath);
			}

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
			Stream output = response.OutputStream;
			output.Write(buffer, 0, buffer.Length);

			if (logData && (context.Request.QueryString.AllKeys.Length > 0))
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
				if(logData) Debug.Log(data_text);
				lastReceivedData = data_text.ToString();
				hasNewData = true; //use extra variable to invoke the event on the main thread
			}

			context.Response.Close();
		}
	}
}
