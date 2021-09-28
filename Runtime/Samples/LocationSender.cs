using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JelleKUL.MeshAlignment
{
    [RequireComponent(typeof(GlobalPosition))]
    [RequireComponent(typeof(UnityHttpSender))]
    public class LocationSender : MonoBehaviour
    {
        private GlobalPosition globalPosition;
        private UnityHttpSender sender;

        // Start is called before the first frame update
        void Start()
        {
            globalPosition = GetComponent<GlobalPosition>();
            sender = GetComponent<UnityHttpSender>();
        }

        public void SendLocation()
        {
            StartCoroutine(globalPosition.FindGlobalPosition(OnLocationFound));
        }

        void OnLocationFound(bool succes)
        {
            if (succes)
            {
                Debug.Log("Sending LocationInfo:" + JsonUtility.ToJson(globalPosition.lastLocation));
                sender.SendPostRequest(globalPosition.positionInfo, "");
                //sender.SendGetRequest("hello");
            }
        }
    }
}