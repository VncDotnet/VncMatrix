using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace VncMatrix
{
    public class WakeOnLan
    {
        private static readonly IPEndPoint BroadcastAddress = new IPEndPoint(IPAddress.Parse("255.255.255.255"), 0);
        private static readonly Socket Socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        static WakeOnLan()
        {
            Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontRoute, 1);
            Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
            //Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Mu true);
        }

        public static void SendMagicPacket(string mac, IPAddress? expectedAddress = null)
        {
            Debug.WriteLine($"Waking {mac}");
            var macBytes = mac.Split(':', '-').Select(m => Byte.Parse(m, NumberStyles.AllowHexSpecifier)).ToArray();
            byte[] payload = new byte[102];

            for (int i = 0; i < 6; i++)
            {
                payload[i] = 0xff;
            }

            for (int i = 1; i <= 16; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    payload[(i * 6) + j] = macBytes[j];
                }
            }

            IPEndPoint dest;
            if (expectedAddress != null)
                dest = new IPEndPoint(expectedAddress, 0);
            else
                dest = BroadcastAddress;

            Socket.SendTo(payload, dest);
        }
    }
}
