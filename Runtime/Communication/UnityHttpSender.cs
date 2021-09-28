using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace JelleKUL.MeshAlignment
{
    public class UnityHttpSender : MonoBehaviour
    {
        [SerializeField]
        private string baseUrl = "";


        public void SendGetRequest(string getUrl, string _baseUrl = "")
        {
            if (_baseUrl == "") _baseUrl = baseUrl;

            StartCoroutine(GetRequest(CombinedUrl(_baseUrl, getUrl)));
        }

        IEnumerator GetRequest(string uri)
        {
            UnityWebRequest uwr = UnityWebRequest.Get(uri);
            yield return uwr.SendWebRequest();

            if (uwr.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log("Error While Sending: " + uwr.error);
            }
            else
            {
                Debug.Log("Received: " + uwr.downloadHandler.text);
            }
        }

        public void SendPostRequest(object jsonObj, string postUrl, string _baseUrl = "")
        {
            if (_baseUrl == "") _baseUrl = baseUrl;

            StartCoroutine(PostRequest(CombinedUrl(_baseUrl, postUrl), JsonUtility.ToJson(jsonObj)));
        }

        IEnumerator PostRequest(string url, string json)
        {
            var uwr = new UnityWebRequest(url, "POST");
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
            uwr.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
            uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            uwr.SetRequestHeader("Content-Type", "application/json");

            //Send the request then wait here until it returns
            yield return uwr.SendWebRequest();

            if (uwr.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log("Error While Sending: " + uwr.error);
            }
            else
            {
                Debug.Log("Received: " + uwr.downloadHandler.text);
            }
        }

        string CombinedUrl(string baseU, string extraU)
        {
            if (baseU.EndsWith("/")) baseU = baseU.Substring(0, baseU.Length - 1);
            if (extraU.StartsWith("/")) extraU = extraU.Substring(1);

            return (baseU + "/" + extraU);
        }
    }
}