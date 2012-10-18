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

        private void WaitResponse(NetworkStream clientStream)
        {
            //byte[] message = new byte[4096];

            //while (true)
            //{
            //    int bytesRead = 0;

            //    try
            //    {
            //        //blocks until a client sends a message
            //        bytesRead = clientStream.Read(message, 0, 4096);
            //    }
            //    catch (Exception ex)
            //    {
            //        Console.WriteLine(ex.ToString());
            //    }

            //    if (bytesRead == 0)
            //    {
            //        //the client has disconnected from the server
            //        break;
            //    }
            //}

            WriteServerData(Helper.SDODeserializeServerData(clientStream));
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
