using System;
using System.Text;

namespace Common
{
    //TODO: take little endian into consideration
    public class Helper
    {
        public static byte[] SdoSerializeClientData(ClientData clientData)
        {
            byte[][] data = new byte[6][];

            data[0] = SdoSerializeString(clientData.Id);
            data[1] = SdoSerializeString(clientData.Text);
            data[2] = SdoSerializeString(clientData.Key);
            data[3] = SdoSerializeString(clientData.Iv);
            data[4] = SdoSerializeString(clientData.Algorithm);
            data[5] = SdoSerializeString(clientData.Action);

            byte[] bytes = new byte[data[0].Length + data[1].Length + data[2].Length + data[3].Length + data[4].Length + data[5].Length + 7];
            bytes[0] = 0xFE;

            int index = 1;
            for (int i = 1; i < 7; i++)
            {
                byte[] d = data[i - 1];

                bytes[index] = (byte)i;
                index++;
                d.CopyTo(bytes, index);
                index += d.Length;
            }

            return bytes;
        }

        public static ClientData SdoDeserializeClientData(byte[] data)
        {
            ClientData clientData = new ClientData();

            int dataLength = data.Length;
            int position = 0;
            int markCount = IsMarkByte(data[position]);

            while (markCount < 0 || dataLength < position || data[position] == 0)
            {
                position++;
                markCount = IsMarkByte(data[position]);
            }

            position++;

            for (int i = 0; i < markCount; i++)
            {
                while (data[position] != i + 1 || dataLength < position || data[position] == 0)
                {
                    position++;
                }

                if (dataLength < position || data[position] == 0)
                    return clientData;

                position++;

                switch (i + 1)
                {
                    case 1:
                        clientData.Id = SdoDeserializeString(data, ref position);
                        break;
                    case 2:
                        clientData.Text = SdoDeserializeString(data, ref position);
                        break;
                    case 3:
                        clientData.Key = SdoDeserializeString(data, ref position);
                        break;
                    case 4:
                        clientData.Iv = SdoDeserializeString(data, ref position);
                        break;
                    case 5:
                        clientData.Algorithm = SdoDeserializeString(data, ref position);
                        break;
                    case 6:
                        clientData.Action = SdoDeserializeString(data, ref position);
                        break;
                }
            }

            return clientData;
        }

        private static int IsMarkByte(byte b)
        {
            if ((b & 0xF8) == 0xF8)
            {
                return b & 0x07;
            }

            return -1;
        }

        public static byte[] SdoSerializeServerData(ServerData serverData)
        {
            byte[][] data = new byte[3][];

            data[0] = SdoSerializeString(serverData.Id);
            data[1] = SdoSerializeString(serverData.Status);
            data[2] = SdoSerializeString(serverData.Text);

            byte[] bytes = new byte[data[0].Length + data[1].Length + data[2].Length + 4];
            bytes[0] = 0xFB;

            int index = 1;
            for (int i = 1; i < 4; i++)
            {
                byte[] d = data[i - 1];

                bytes[index] = (byte)i;
                index++;
                d.CopyTo(bytes, index);
                index += d.Length;
            }

            return bytes;
        }

        public static ServerData SdoDeserializeServerData(byte[] data)
        {
            ServerData serverData = new ServerData();

            int dataLength = data.Length;
            int position = 0;
            int markCount = IsMarkByte(data[position]);

            while (markCount < 0 || dataLength < position || data[position] == 0)
            {
                position++;
                markCount = IsMarkByte(data[position]);
            }

            position++;

            for (int i = 0; i < markCount; i++)
            {
                while (data[position] != i + 1 || dataLength < position || data[position] == 0)
                {
                    position++;
                }

                if (dataLength < position || data[position] == 0)
                    return serverData;

                position++;

                switch (i + 1)
                {
                    case 1:
                        serverData.Id = SdoDeserializeString(data, ref position);
                        break;
                    case 2:
                        serverData.Status = SdoDeserializeString(data, ref position);
                        break;
                    case 3:
                        serverData.Text = SdoDeserializeString(data, ref position);
                        break;
                }
            }

            return serverData;
        }

        private static byte[] SdoSerializeString(string text)
        {
            byte[] textBytes = Encoding.UTF8.GetBytes(text);
            int textBytesLength = textBytes.Length;
            byte[] textBytesLengthBytes = IntBytes(textBytesLength);
            int textBytesLengthBytesLength = textBytesLengthBytes.Length;

            byte[] result;

            if (textBytesLength < 16)
            {
                result = new byte[textBytesLength + 1];
                result[0] = (byte)(((byte)textBytesLength) | 0x60);
            }
            else
            {
                result = new byte[textBytesLengthBytesLength + textBytesLength + 1];
                result[0] = (byte)(((byte)textBytesLengthBytesLength) | 0x70);
            }

            int resultIndex = 1;

            if (textBytesLength > 15)
            {
                for (int i = 0; i < textBytesLengthBytesLength; i++)
                {
                    result[resultIndex] = textBytesLengthBytes[i];
                    resultIndex++;
                }
            }

            for (int i = 0; i < textBytesLength; i++)
            {
                result[resultIndex] = textBytes[i];
                resultIndex++;
            }

            return result;
        }

        private static string SdoDeserializeString(byte[] data, ref int position)
        {
            byte type = data[position];

            if((type & 0x60) == 0x60)
            {
                bool lengthInBytes = (type & 0x10) == 0x10;
                int length = type & 0x0F;

                position++;
                byte[] text;

                if (lengthInBytes)
                {
                    byte[] lengthBytes = new byte[8];
                    for (int i = 0; i < length; i++)
                    {
                        lengthBytes[i] = data[position];
                        position++;
                    }

                    long lengthBytesLength = BitConverter.ToInt64(lengthBytes, 0);

                    text = new byte[lengthBytesLength];
                    for (int i = 0; i < lengthBytesLength; i++)
                    {
                        text[i] = data[position];
                        position++;
                    }
                }
                else
                {
                    text = new byte[length];
                    for (int i = 0; i < length; i++)
                    {
                        text[i] = data[position];
                        position++;
                    }
                }

                return Encoding.UTF8.GetString(text, 0, text.Length);
            }

            throw new Exception("Expected string, but received unknown type.");
        }

        public static string ProcessClientData(ClientData clientData)
        {
            switch (clientData.Action)
            {
                case "Encrypt":
                    return EncryptDecrypt.Encrypt(clientData.Text, clientData.Algorithm, clientData.Key, clientData.Iv);
                case "Decrypt":
                    return EncryptDecrypt.Decrypt(clientData.Text, clientData.Algorithm, clientData.Key, clientData.Iv);
                default:
                    throw new Exception("Not supported client action.");
            }
        }

        private static byte[] IntBytes(int value)
        {
            byte[] bytes;

            if (sbyte.MinValue <= value && value <= byte.MaxValue)
            {
                return new[] {(byte) value};
            }
            if (short.MinValue <= value && value <= short.MaxValue)
            {
                bytes = new byte[2];
                bytes[1] = (byte) (((short) value) >> 8);
                bytes[0] = (byte) ((short) value);
                return bytes;
            }
            if (ushort.MinValue <= value && value <= ushort.MaxValue)
            {
                bytes = new byte[2];
                bytes[1] = (byte) (((ushort) value) >> 8);
                bytes[0] = (byte) ((ushort) value);
                return bytes;
            }

            bytes = new byte[4];
            bytes[3] = (byte) (value >> 24);
            bytes[2] = (byte) (value >> 16);
            bytes[1] = (byte) (value >> 8);
            bytes[0] = (byte) value;
            return bytes;
        }
    }
}
