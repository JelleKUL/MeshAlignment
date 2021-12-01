using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace JelleKUL.MeshAlignment
{
    [RequireComponent(typeof(GlobalPosition))]
    public class LocationSender : MonoBehaviour
    {
        [SerializeField]
        private string locationUrl = "localhost:5000/geolocation";

        [SerializeField]
        private StringEvent OnLocationSend = new StringEvent(), OnLocationGet = new StringEvent();

        private GlobalPosition globalPosition;


        // Start is called before the first frame update
        void Awake()
        {
            globalPosition = GetComponent<GlobalPosition>();
        }

        public void SetUrl(string url)
        {
            locationUrl = url;
        }

        [ContextMenu("Send PositionInfo")]
        public async Task<string> SendPositionInfo()
        {
            bool succes = await globalPosition.FindGlobalPosition();
            if (!succes) return "ERROR";

            HttpClient newClient = new HttpClient();

            string result = await newClient.Post(locationUrl, globalPosition.positionInfo);
            Debug.Log(result);
            OnLocationSend.Invoke(result);
            return result;
        }

        [ContextMenu("Get Position")]
        public async Task<string> GetPosition()
        {
            HttpClient newClient = new HttpClient();

            string result = await newClient.Get(locationUrl);
            Debug.Log(result);
            OnLocationGet.Invoke(result);
            return result;
        }
    }
}