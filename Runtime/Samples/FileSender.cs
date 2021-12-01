using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


namespace JelleKUL.MeshAlignment
{
    public class FileSender : MonoBehaviour
    {
        private string filesUrl = "localhost:5000/add-session";
        [SerializeField]
        private string postFolderPath = "";

        [SerializeField]
        private StringEvent OnFilesSend = new StringEvent();

        public void SetUrl(string url)
        {
            filesUrl = url;
        }

        public void SetPath(string path)
        {
            postFolderPath = path;
        }

        /// <summary>
        /// Async POST all the files in a folder to a server using a Dictionary & filepaths
        /// </summary>
        /// <returns>The response string</returns>
        [ContextMenu("Send Files")]
        public async Task<string> SendFiles()
        {
            HttpClient newClient = new HttpClient();

            string sessionName = System.IO.Path.GetFileName(postFolderPath);
            Dictionary<string, object> postDict = new Dictionary<string, object>();
            postDict.Add("session", sessionName);

            string[] filePaths = System.IO.Directory.GetFiles(postFolderPath);
            string result = await newClient.PostFiles(filesUrl, postDict, filePaths);

            Debug.Log(result);
            OnFilesSend.Invoke(result);
            return result;
        }
    }
}
