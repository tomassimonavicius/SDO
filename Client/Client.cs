using System;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using Common;

namespace Client
{
    class Client
    {
        public Client()
        {
            TcpClient client = new TcpClient();

            IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 3000);

            client.Connect(serverEndPoint);

            NetworkStream clientStream = client.GetStream();

            byte[] buffer = Helper.SDOSerializeClientData(GetData());

            clientStream.Write(buffer, 0, buffer.Length);
            clientStream.Flush();

            byte[] message = new byte[4096];
            int bytesRead;

            while (true)
            {
                bytesRead = 0;

                try
                {
                    //blocks until a client sends a message
                    bytesRead = clientStream.Read(message, 0, 4096);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }

                if (bytesRead == 0)
                {
                    //the client has disconnected from the server
                    break;
                }

                //message has successfully been received
                ASCIIEncoding encoder = new ASCIIEncoding();
                Console.WriteLine(encoder.GetString(message, 0, bytesRead));
            }
        }

        public ClientData GetData()
        {
            ClientData data = null;
            string path = "data.xml";

            XmlSerializer serializer = new XmlSerializer(typeof(ClientData));

            StreamReader reader = new StreamReader(path);
            data = (ClientData)serializer.Deserialize(reader);
            reader.Close();

            return data;
        }
    }
}
