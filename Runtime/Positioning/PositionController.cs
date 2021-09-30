using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JelleKUL.MeshAlignment
{
    public class PositionController : MonoBehaviour
    {
        [SerializeField]
        private Vector3 globalPositionOffset = Vector3.zero;

        [SerializeField]
        private Transform worldReferenceCenter;

        [SerializeField]
        private bool logData = true;

        [SerializeField]
        private CoordinateSystem coordinateSystem;

        [SerializeField]
        private PositionInfo lastReceivedPosition = new PositionInfo();


        // Start is called before the first frame update
        void Start()
        {
            if(worldReferenceCenter == null)
            {
                Log("No worldReferenceCenter set, using this object to move");
                worldReferenceCenter = transform;
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void UpdateGlobalPosition(string positionInfoString)
        {
            Log("Received a posiiotnInfoString");
            PositionInfo parsedObject = JsonUtility.FromJson<PositionInfo>(positionInfoString);
            UpdateGlobalPosition(parsedObject);
        }

        public void UpdateGlobalPosition(PositionInfo positionInfo)
        {
            Log("Received a posiiotnInfo");
            lastReceivedPosition = positionInfo;
            Vector3 newGlobalPosition = CoordinateConverter.ConvertCoordinates(positionInfo.position, (CoordinateSystem)positionInfo.coordinateSystem, coordinateSystem);
            
            worldReferenceCenter.position = newGlobalPosition - globalPositionOffset;
        }

        void Log(string log)
        {
            if (logData) Debug.Log("<color=orange>" + name + ": </color>" + log);
        }

        private void OnDrawGizmosSelected()
        {
            if (Application.isPlaying)
            {
                Gizmos.DrawSphere(worldReferenceCenter.position, lastReceivedPosition.errorRadius);

            }
        }

    }
}
