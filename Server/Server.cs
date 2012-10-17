using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Net;
using Common;

namespace Server
{
    class Server
    {
        private TcpListener tcpListener;
        private Thread listenerThread;

        public void Listen()
        {
            this.tcpListener = new TcpListener(IPAddress.Any, 3000);
            this.listenerThread = new Thread(WaitForClients);
            this.listenerThread.Start();
        }

        private void WaitForClients()
        {
            this.tcpListener.Start();

            while (true)
            {
                TcpClient client = this.tcpListener.AcceptTcpClient();

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
                StringBuilder myCompleteMessage = new StringBuilder();

                // Incoming message may be larger than the buffer size. 
                do
                {
                    int numberOfBytesRead = stream.Read(myReadBuffer, 0, myReadBuffer.Length);
                    bytes.AddRange(myReadBuffer.Take(numberOfBytesRead));
                    myCompleteMessage.AppendFormat("{0}", Encoding.ASCII.GetString(myReadBuffer, 0, numberOfBytesRead));
                } while (stream.DataAvailable);

                // Print out the received message to the console.
                Console.WriteLine("You received the following message : " + myCompleteMessage);
            }
            else
            {
                Console.WriteLine("Sorry.  You cannot read from this NetworkStream.");
            }


            return new ClientData();
            //return Helper.SDODeserializeClientData(stream);
        }

        public void SendResponse(ClientData clientData, NetworkStream stream)
        {
            ServerData serverData = new ServerData
                                        {
                                            Id = clientData.Id,
                                            Text = Helper.ProcessClientData(clientData)
                                        };

            byte[] buffer = Helper.SDOSerializeServerData(serverData);

            stream.Write(buffer, 0, buffer.Length);
            stream.Flush();
        }
    }
}
