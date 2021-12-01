using System;
using System.IO;
using System.Collections.Generic;
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

        /// <summary>
        /// Posts data and files to an url
        /// </summary>
        /// <param name="url">the full url of the POSt request</param>
        /// <param name="fields">a Dictionary containing key, value pairs to POST</param>
        /// <param name="filePaths">the full filepaths to add to the post requests</param>
        /// <returns>the HTTP request response</returns>
        public async Task<string> PostFiles(string url,Dictionary<string, object> fields, string[] filePaths)
        {
            try
            {
                List<IMultipartFormSection> formData = new List<IMultipartFormSection>(); //create a new form section

                //add the form fields from the dictionary
                foreach (var field in fields)
                {
                    formData.Add(new MultipartFormDataSection(field.Key, field.Value.ToString()));
                }

                //add the files to the form section
                foreach (string filePath in filePaths)
                {
                    formData.Add(new MultipartFormFileSection("file", File.ReadAllBytes(filePath), Path.GetFileName(filePath), "file"));
                }

                UnityWebRequest www = UnityWebRequest.Post(url, formData);
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
