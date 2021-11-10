using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace JelleKUL.MeshAlignment
{
    [RequireComponent(typeof(GlobalPosition))]
    [RequireComponent(typeof(UnityHttpSender))]
    public class LocationSender : MonoBehaviour
    {
        [SerializeField]
        private Text locationText;

        private GlobalPosition globalPosition;
        private UnityHttpSender sender;

        [SerializeField]
        private string baseUrl = "http://192.168.0.237", port = "1234";

        [SerializeField]
        private string getUrl = "", postUrl = "", postMessage = "";


        // Start is called before the first frame update
        void Start()
        {
            globalPosition = GetComponent<GlobalPosition>();
            sender = GetComponent<UnityHttpSender>();
        }

        public void SendLocation()
        {
            locationText.text = "Finding Location...";
            StartCoroutine(globalPosition.FindGlobalPosition(OnLocationFound));
        }

        void OnLocationFound(bool succes)
        {
            if (succes)
            {
                locationText.text = "LocationInfo:" + JsonUtility.ToJson(globalPosition.positionInfo);

                Debug.Log("Sending LocationInfo:" + JsonUtility.ToJson(globalPosition.positionInfo));
                sender.SendPostRequest(globalPosition.positionInfo,"", baseUrl + ":" + port);
                //sender.SendGetRequest("hello");
            }
        }

        public void SetUrl(string url)
        {
            baseUrl = url;
        }

        public void SetPort(string port)
        {
            this.port = port;
        }

        [ContextMenu("Send Position")]
        public async void SendPosition()
        {
            HttpClient newClient = new HttpClient();

            string result = await newClient.Post(postUrl,postMessage);

            Debug.Log(result);
        }

        [ContextMenu("Get Position")]
        public async void GetPosition()
        {
            HttpClient newClient = new HttpClient();

            string result = await newClient.Get(postUrl);

            Debug.Log(result);
        }
    }
}