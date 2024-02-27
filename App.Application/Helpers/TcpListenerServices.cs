using Microsoft.Extensions.Configuration;
using MimeKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
//using System.Windows.Forms;

namespace App.Application.Helpers
{
    public class TcpListenerServices
    {
        public static string Send(string filePath,IConfiguration _configuration, tcpType type = tcpType.exporting)
        {
            int port = 0;
            string ip = "";
            Encoding iso = Encoding.GetEncoding("ISO-8859-6");
            if (type == tcpType.exporting)
            {
                port = int.Parse(_configuration["ApplicationSetting:ExportingToolPort"]);
                ip = _configuration["ApplicationSetting:ExportingToolIp"];
                iso = Encoding.GetEncoding("ISO-8859-6");
            }
            else if (type == tcpType.EInvoice)
            {
                port = int.Parse(_configuration["ApplicationSetting:EInvoiceToolPort"]);
                //ip = GetLocalIPAddress();
                ip = "192.168.1.240";
                iso = Encoding.GetEncoding("UTF-8");
            }


            try
            {
               
                TcpClient clientSocket = new TcpClient();
                clientSocket.Connect(ip, port);
                NetworkStream serverStream = clientSocket.GetStream();

                // Request
                byte[] outStream = iso.GetBytes(filePath);
                serverStream.Write(outStream, 0, outStream.Length);
                serverStream.Flush();

                //Response
                byte[] bytesToRead = new byte[clientSocket.ReceiveBufferSize];
                int bytesRead = serverStream.Read(bytesToRead, 0, clientSocket.ReceiveBufferSize);
                string resp = Encoding.ASCII.GetString(bytesToRead, 0, bytesRead);
                return resp;

            }
            catch (Exception EXC)
            {
                return EXC.Message;
            }
        }

        static string GetLocalIPAddress()
        {
            string ipAddress = string.Empty;

            // Get all network interfaces on the local machine
            NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

            foreach (NetworkInterface networkInterface in networkInterfaces)
            {
                // Consider only operational interfaces
                if (networkInterface.OperationalStatus == OperationalStatus.Up)
                {
                    // Filter out loopback and non-IPv4 addresses
                    if (networkInterface.NetworkInterfaceType != NetworkInterfaceType.Loopback &&
                        networkInterface.NetworkInterfaceType != NetworkInterfaceType.Tunnel &&
                        networkInterface.Supports(NetworkInterfaceComponent.IPv4))
                    {
                        // Get the IP properties of the current interface
                        IPInterfaceProperties ipProperties = networkInterface.GetIPProperties();

                        // Get the collection of unicast addresses assigned to the interface
                        UnicastIPAddressInformationCollection unicastAddresses = ipProperties.UnicastAddresses;

                        // Iterate through the unicast addresses
                        foreach (UnicastIPAddressInformation unicastAddress in unicastAddresses)
                        {
                            // Check if the address is a private IP address
                            if (IsPrivateIPAddress(unicastAddress.Address))
                            {
                                ipAddress = unicastAddress.Address.ToString();
                                break; // Stop iterating once a private IP is found
                            }
                        }
                    }
                }
            }

            return ipAddress;
        }

        static bool IsPrivateIPAddress(IPAddress ipAddress)
        {
            byte[] addressBytes = ipAddress.GetAddressBytes();

            // Check if the IP address belongs to a private address range
            return (addressBytes[0] == 10) ||
                   (addressBytes[0] == 172 && (addressBytes[1] >= 16 && addressBytes[1] <= 31)) ||
                   (addressBytes[0] == 192 && addressBytes[1] == 168);
        }
    }
}
