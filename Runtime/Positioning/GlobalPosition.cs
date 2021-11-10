using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JelleKUL.MeshAlignment
{
    /// <summary>
    /// functions to get the global position of the device
    /// </summary>
    public class GlobalPosition : MonoBehaviour
    {
        [Tooltip("The current coordinate system to store the position in")]
        public CoordinateSystem coordinateSystem = CoordinateSystem.Lambert72;
        public Vector3 position { get; private set; } = Vector3.zero; //the position of the device
        public float errorRadius { get; private set; }  = 0f; //the error radius of the device
        public PositionInfo positionInfo = new PositionInfo();
        public LocationInfo lastLocation { get; private set; } //the last locationinfo, containing all kinds of info
        public bool hasData { get; private set; } = false; //has there been a succesfull connect

        [SerializeField]
        [Tooltip("Log all the Data and processes")]
        private bool logData = false;
        [SerializeField]
        [Tooltip("Start the location finding at start")]
        private bool findLocationAtStart = false;
        [SerializeField]
        [Tooltip("The maximum amount of time the device should check for a location")]
        [Range(0,60)]
        private int maxWaitTime = 10;

        // Start is called before the first frame update
        void Start()
        {
            if (findLocationAtStart) StartFindGlobalPosition();
        }

        public void StartFindGlobalPosition()
        {
            StartCoroutine(FindGlobalPosition(OnPositionFound));
        }

        //todo check if device supports location getting
        public IEnumerator FindGlobalPosition(System.Action<bool> callbackOnFinish)
        {
            Log("Starting Location search");
            bool gotLocation = false;


            // First, check if user has location service enabled
            if (!Input.location.isEnabledByUser)
                callbackOnFinish(gotLocation);

            // Start service before querying location
            Input.location.Start();

            Log("LocationStatus: " + Input.location.status);

            // Wait until service initializes
            int maxWait = maxWaitTime;
            while ((Input.location.status == LocationServiceStatus.Initializing || Input.location.status == LocationServiceStatus.Stopped) && maxWait > 0)
            {
                yield return new WaitForSeconds(1);
                Log("Connecting...");
                maxWait--;
            }

            // Service didn't initialize in 20 seconds
            if (maxWait < 1)
            {
                Log("Timed out");
                callbackOnFinish(gotLocation);
            }

            Log("LocationSatus: " + Input.location.status);

            // Connection has failed
            if (Input.location.status == LocationServiceStatus.Failed || Input.location.status == LocationServiceStatus.Stopped)
            {
                Log("Unable to determine device location");
                callbackOnFinish(gotLocation);
            }
            else
            {
                Log("Found Location!");
                // Access granted and location value could be retrieved
                hasData = true;
                gotLocation = true;

                lastLocation = Input.location.lastData;
                errorRadius = Mathf.Max(Input.location.lastData.horizontalAccuracy, Input.location.lastData.verticalAccuracy);

                //convert the longitude and latitude to lambert
                Vector3 gpsPos = new Vector3(Input.location.lastData.longitude, Input.location.lastData.latitude, Input.location.lastData.altitude);
                position = CoordinateConverter.ConvertCoordinates(gpsPos, CoordinateSystem.WGS84, coordinateSystem);

                positionInfo.position = position;
                positionInfo.errorRadius = errorRadius;
                positionInfo.coordinateSystem = (int)coordinateSystem;
                
            }

            // Stop service if there is no need to query location updates continuously
            Input.location.Stop();

            //send the succes back to the callback
            callbackOnFinish(gotLocation);
        }

        public void SetCoordinateSystem(CoordinateSystem coordinateSystem)
        {
            this.coordinateSystem = coordinateSystem;
        }

        //todo add safety check
        public void SetCoordinateSystem(int coordinateSystem)
        {
            this.coordinateSystem = (CoordinateSystem)coordinateSystem;
        }

        void OnPositionFound(bool succes)
        {
            if (succes)
            {
                Log("The device's current location = " + position + "\t With an error radius of: " + errorRadius + "m");
            }
            else
            {
                Log("Unable to determine device location");
            }
        }

        void Log(string log)
        {
            if(logData) Debug.Log("<color=orange>" + name + ": </color>" + log);
        }
    }

}