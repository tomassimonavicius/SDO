using System.Net.Sockets;
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
            //byte[] message = new byte[4096];

            //while (true)
            //{
            //    int bytesRead = 0;

            //    try
            //    {
            //        bytesRead = stream.Read(message, 0, 4096);
            //    }
            //    catch (Exception ex)
            //    {
            //        Console.WriteLine(ex.ToString());
            //    }

            //    if (bytesRead == 0)
            //    {
            //        break;
            //    }

            //    byte[] data = new byte[bytesRead];
            //    for (int i = 0; i < bytesRead; i++)
            //    {
            //        data[i] = message[i];
            //    }

            //    return data;
            //}

            return Helper.SDODeserializeClientData(stream);
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
