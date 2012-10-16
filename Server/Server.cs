using System;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.Xml.Serialization;
using System.IO;
using System.Collections.Generic;

namespace Server
{
    class Server
    {
        private TcpListener tcpListener;
        private Thread listenerThread;

        public Server()
        {
            this.tcpListener = new TcpListener(IPAddress.Any, 3000);
            this.listenerThread = new Thread(new ThreadStart(WaitForClients));
            this.listenerThread.Start();
        }

        private void WaitForClients()
        {
            this.tcpListener.Start();

            while (true)
            {
                TcpClient client = this.tcpListener.AcceptTcpClient();

                Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClient));
                clientThread.Start(client);
            }
        }

        private void HandleClient(object client)
        {
            TcpClient tcpClient = (TcpClient)client;
            NetworkStream clientStream = tcpClient.GetStream();

            byte[] data = GetData(clientStream);

            ASCIIEncoding encoder = new ASCIIEncoding();

            Console.WriteLine(encoder.GetString(data, 0, data.Length));

            Data deserializedData = DeserializeData(data);

            byte[] processedData = GetResult(deserializedData);

            byte[] buffer = encoder.GetBytes("Hello Client!");

            clientStream.Write(buffer, 0, buffer.Length);
            clientStream.Flush();

            tcpClient.Close();
        }

        public byte[] GetData(NetworkStream stream)
        {
            byte[] message = new byte[4096];
            int bytesRead;

            while (true)
            {
                bytesRead = 0;

                try
                {
                    bytesRead = stream.Read(message, 0, 4096);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }

                if (bytesRead == 0)
                {
                    break;
                }

                byte[] data = new byte[bytesRead];
                for (int i = 0; i < bytesRead; i++)
                {
                    data[i] = message[i];
                }

                return data;
            }

            return message;
        }

        public void WriteData(byte[] data, NetworkStream stream)
        {

        }

        public Data DeserializeData(byte[] data)
        {
            Data result = null;

            XmlSerializer serializer = new XmlSerializer(typeof(Data));

            MemoryStream reader = new MemoryStream(data);

            result = (Data)serializer.Deserialize(reader);
            reader.Close();

            return result;
        }

        public byte[] GetResult(Data data)
        {
            switch (data.Action)
            {
                case "Encrypt":
                    return EncryptDecrypt.Encrypt(Encoding.ASCII.GetBytes(data.Text), data.Algorithm, Encoding.ASCII.GetBytes(data.Key), Encoding.ASCII.GetBytes(data.IV));
                case "Decrypt":
                    return EncryptDecrypt.Decrypt(Encoding.ASCII.GetBytes(data.Text), data.Algorithm, Encoding.ASCII.GetBytes(data.Key), Encoding.ASCII.GetBytes(data.IV));
                default:
                    throw new Exception(string.Format("Unknown action: {0}", data.Action));
            }
        }
    }
}
