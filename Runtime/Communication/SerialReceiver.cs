using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports; // this enables the IO port namespace

namespace JelleKUL.MeshAlignment
{
    /// <summary>
    /// Functions to receive and send serial data, set the api compatibilty level to .NET 4.x
    /// </summary>
    public class SerialReceiver : MonoBehaviour
    {
        [Header("ConnectionSettings")]
        [SerializeField]
        private bool OpenSerialAtStart = false;
        [SerializeField]
        private bool listenToSerial = false;
        [SerializeField]
        private string IOPort = "/dev/cu.usbmodem1424301"; //"/dev/cu.HC05-SPPDev"; // Change this to whatever port your device is connected to
        [SerializeField]
        private int baudeRate = 115200; //this must match the bauderate of the other device

        [HideInInspector]
        public SerialPort sp;

        public string recievedData { get; private set; }

        // Start is called before the first frame update
        void Start()
        {
            if (OpenSerialAtStart) ActivateSP(IOPort);
        }

        // Update is called once per frame
        void Update()
        {
            if (sp.IsOpen && listenToSerial) ReceiveData();
        }

        /// <summary>
        /// Open a serial port with an optional port name
        /// </summary>
        /// <param name="port">the port name, uses default if left open</param>
        public void ActivateSP(string port = "")
        {
            if (port != "") IOPort = port;
            sp = new SerialPort(IOPort, baudeRate, Parity.None, 8, StopBits.One);
            sp.Open();
            sp.ReadTimeout = 25;
        }

        /// <summary>
        /// send data through the serial port
        /// </summary>
        /// <param name="message">the message to send</param>
        /// <returns>The succes of the write</returns>
        public bool WriteToSerial(string message)
        {
            if (sp == null) return false;

            if (sp.IsOpen)
            {
                sp.Write(message);
                string mess = "";

                foreach (var item in message)
                {
                    mess += item + ", ";
                }
                Debug.Log(mess);
                sp.BaseStream.Flush();
                return true;
            }

            return false;
        }

        void ReceiveData()
        {
            if (sp == null) return;

            if (sp.IsOpen)
            {
                try
                {
                    recievedData = sp.ReadLine(); //reads the serial input
                    Debug.Log(recievedData);
                }
                catch (System.Exception)
                {
                    Debug.LogWarning(name + ": unable to read data");
                }
            }
        }

        /// <summary>
        /// returns a list of available serial ports
        /// </summary>
        /// <returns></returns>
        public List<string> GetConnectedDevices()
        {
            List<string> deviceList = new List<string>();

            foreach (string port in SerialPort.GetPortNames())
            {
                deviceList.Add(port);
            }

            return deviceList;
        }

    }
}