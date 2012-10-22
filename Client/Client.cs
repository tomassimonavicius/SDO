using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Xml;
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
            byte[] buffer = Helper.SdoSerializeClientData(ReadClientData());

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
                    byte[] tmp = new byte[numberOfBytesRead];
                    Array.Copy(myReadBuffer, 0, tmp, 0, numberOfBytesRead);
                    bytes.AddRange(tmp);
                } while (stream.DataAvailable);
            }
            else
            {
                Console.WriteLine("Sorry. You can't read from this NetworkStream.");
            }

            WriteServerData(Helper.SdoDeserializeServerData(bytes.ToArray()));
        }

        private ClientData ReadClientData()
        {
            ClientData clientData = new ClientData();

            XmlReaderSettings settings = new XmlReaderSettings {IgnoreWhitespace = true, IgnoreComments = true};
            using (XmlReader reader = XmlReader.Create("clientdata.xml", settings))
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element && reader.LocalName == "id")
                    {
                        clientData.Id = reader.ReadElementContentAsString();
                        clientData.Text = reader.ReadElementContentAsString();
                        clientData.Key = reader.ReadElementContentAsString();
                        clientData.Iv = reader.ReadElementContentAsString();
                        clientData.Algorithm = reader.ReadElementContentAsString();
                        clientData.Action = reader.ReadElementContentAsString();
                    }
                }
            }

            return clientData;
        }

        private void WriteServerData(ServerData data)
        {
            XmlWriter xmlWriter = new XmlTextWriter("serverdata.xml", Encoding.UTF8);

            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("serverdata");
            xmlWriter.WriteStartElement("id");
            xmlWriter.WriteString(data.Id);
            xmlWriter.WriteEndElement();
            xmlWriter.WriteStartElement("status");
            xmlWriter.WriteString(data.Status);
            xmlWriter.WriteEndElement();
            xmlWriter.WriteStartElement("text");
            xmlWriter.WriteString(data.Text);
            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndElement();

            xmlWriter.Flush();
            xmlWriter.Close();
        }
    }
}
