using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace JelleKUL.MeshAlignment
{
    public class HttpClient
    {
        public async Task<string> Get(string url)
        {
            try
            {
                using var www = UnityWebRequest.Get(url);
                var operation = www.SendWebRequest();

                while (!operation.isDone)
                    await Task.Yield();

                if (www.result != UnityWebRequest.Result.Success)
                    Debug.LogError($"Failed: {www.error}");

                var result = www.downloadHandler.text;

                return result;
            }
            catch (Exception ex)
            {
                Debug.LogError($"{nameof(Get)} failed: {ex.Message}");
                return default;
            }
        }

        public async Task<string> Post(string url, object data)
        {
            try
            {
                string serializedData = (data.GetType() == typeof(string)) ? (string)data : JsonUtility.ToJson(data);

                Debug.Log(serializedData);

                /*
                using var www = UnityWebRequest.Post(url, serializedData);

                www.SetRequestHeader("Content-Type", "application/json");
                */

                var www = new UnityWebRequest(url, "POST");
                byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(serializedData);
                www.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
                www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
                www.SetRequestHeader("Content-Type", "application/json");

                Debug.Log(www.url);

                var operation = www.SendWebRequest();

                while (!operation.isDone)
                    await Task.Yield();

                if (www.result != UnityWebRequest.Result.Success)
                    Debug.LogError($"Failed: {www.error}");

                var result = www.downloadHandler.text;

                return result;
            }
            catch (Exception ex)
            {
                Debug.LogError($"{nameof(Get)} failed: {ex.Message}");
                return default;
            }
        }
    }
}
