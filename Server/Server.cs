using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using Common;

namespace Server
{
    class Server
    {
        private TcpListener _tcpListener;
        private Thread _listenerThread;

        public void Listen()
        {
            _tcpListener = new TcpListener(IPAddress.Any, 3000);
            _listenerThread = new Thread(WaitForClients);
            _listenerThread.Start();
        }

        private void WaitForClients()
        {
            _tcpListener.Start();

            while (true)
            {
                TcpClient client = _tcpListener.AcceptTcpClient();

                Thread clientThread = new Thread(HandleClient);
                clientThread.Start(client);
            }
        }

        private void HandleClient(object client)
        {
            TcpClient tcpClient = (TcpClient)client;
            NetworkStream clientStream = tcpClient.GetStream();

            ClientData clientData = WaitRequest(clientStream);
            SendResponse(clientData, clientStream);

            tcpClient.Close();
        }

        private ClientData WaitRequest(NetworkStream stream)
        {
            List<byte> bytes = new List<byte>(); 

            if (stream.CanRead)
            {
                byte[] myReadBuffer = new byte[1024];

                do
                {
                    int numberOfBytesRead = stream.Read(myReadBuffer, 0, myReadBuffer.Length);
                    bytes.AddRange(myReadBuffer.Take(numberOfBytesRead));
                } while (stream.DataAvailable);
            }
            else
            {
                Console.WriteLine("Sorry. You cannot read from this NetworkStream.");
            }

            return Helper.SDODeserializeClientData(bytes.ToArray());
        }

        public void SendResponse(ClientData clientData, NetworkStream stream)
        {
            ServerData serverData = new ServerData
            {
                Id = clientData.Id
            };

            try
            {
                serverData.Text = Helper.ProcessClientData(clientData);
                serverData.Status = "Success";
            }
            catch (Exception ex)
            {
                serverData.Text = ex.Message;
                serverData.Status = "Error";
            }

            byte[] buffer = Helper.SDOSerializeServerData(serverData);

            stream.Write(buffer, 0, buffer.Length);
            stream.Flush();
        }
    }
}
