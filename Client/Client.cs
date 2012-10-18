using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Xml.Serialization;
using System.IO;
using Common;

namespace Client
{
    class Client
    {
        public void Connect()
        {
            TcpClient client = new TcpClient();
            IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 3000);
            client.Connect(serverEndPoint);
            NetworkStream clientStream = client.GetStream();

            SendRequest(clientStream);
            WaitResponse(clientStream);
        }

        private void SendRequest(NetworkStream clientStream)
        {
            byte[] buffer = Helper.SDOSerializeClientData(ReadClientData());

            clientStream.Write(buffer, 0, buffer.Length);
            clientStream.Flush();
        }

        private void WaitResponse(NetworkStream stream)
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

            WriteServerData(Helper.SDODeserializeServerData(bytes.ToArray()));
        }

        private ClientData ReadClientData()
        {
            const string path = "clientdata.xml";

            XmlSerializer serializer = new XmlSerializer(typeof(ClientData));

            StreamReader reader = new StreamReader(path);
            ClientData data = (ClientData)serializer.Deserialize(reader);
            reader.Close();

            return data;
        }

        private void WriteServerData(ServerData data)
        {
            const string path = "serverdata.xml";

            StreamWriter writer = new StreamWriter(path);

            XmlSerializer serializer = new XmlSerializer(typeof(ServerData));
            serializer.Serialize(writer, data);

            writer.Flush();
            writer.Close();
        }
    }
}
