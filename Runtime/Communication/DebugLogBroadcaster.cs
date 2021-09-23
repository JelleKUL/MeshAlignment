using UnityEngine;
using System;
using System.Text;
using System.Net.Sockets;
using System.Net;
#if WINDOWS_UWP
using Windows.Networking.Sockets;
using Windows.Networking.Connectivity;
using Windows.Networking;
#endif

/*
 * Broadcast all Debug Log messages on the current WiFi network
 * By Peter Koch <peterept@gmail.com>
 * 
 * Use this with any UDP Listener on your PC 
 * eg: SocketTest
 *     http://sourceforge.net/projects/sockettest/
 *     Launch the app, go to UDP tab, set port to 9999 and press Start Listening
 * 
 * Important Note:
 *  - Callstacks are only sent in non-editor builds when "Development Build" is checkmarked in Build Settings
 */
public class DebugLogBroadcaster : MonoBehaviour
{
    public int broadcastPort = 9999;

#if WINDOWS_UWP
    HostName hostName;
    DatagramSocket client;
#else
    IPEndPoint remoteEndPoint;
    UdpClient client;
#endif

    void OnEnable()
    {
#if WINDOWS_UWP
        hostName = new Windows.Networking.HostName("255.255.255.255");
        client = new DatagramSocket();
#else
        remoteEndPoint = new IPEndPoint(IPAddress.Broadcast, broadcastPort);
        client = new UdpClient();
#endif
        Application.logMessageReceived += HandlelogMessageReceived;
        Debug.Log("DebugLogBroadcaster started on port:" + broadcastPort);
        Debug.Log("Current Local IP adress: " + IPLogger.GetLocalIPv4());
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandlelogMessageReceived;
#if !WINDOWS_UWP
        client.Close();
        remoteEndPoint = null;
#endif
        client = null;
    }

    // DatagramSocket needs to run in an async routine to dispatch an async threading task
#if WINDOWS_UWP
    async
#endif
    void HandlelogMessageReceived(string condition, string stackTrace, LogType type)
    {
        string msg = string.Format("[{0}] {1}{2}",
                                   type.ToString().ToUpper(),
                                   condition,
                                   "\n    " + stackTrace.Replace("\n", "\n    "));
#if WINDOWS_UWP
         await SendUdpMessage(msg);

#else
        byte[] data = Encoding.UTF8.GetBytes(msg);
        client.Send(data, data.Length, remoteEndPoint);
#endif
    }

#if WINDOWS_UWP
    private async System.Threading.Tasks.Task SendUdpMessage(string message) {
        using (var stream = await client.GetOutputStreamAsync(hostName, broadcastPort.ToString())) {
            using (var writer = new Windows.Storage.Streams.DataWriter(stream)) {
                var data = Encoding.UTF8.GetBytes(message);
                writer.WriteBytes(data);
                await writer.StoreAsync();
            }
        }
    }
#endif
    
}
