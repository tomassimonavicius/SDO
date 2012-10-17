using System;
using System.Collections;
using System.Net.Sockets;
using System.Text;

namespace Common
{
    public class Helper
    {
        private static readonly BitArray MARK = new BitArray(new [] { true, true, true, true, true, false, false, false });
        private static readonly BitArray MARKLENGTH = new BitArray(new[] { false, false, false, false, false, true, true, true });

        public static byte[] SDOSerializeClientData(ClientData data)
        {
            byte[] id = SDOSerializeString(data.Id);
            byte[] text = SDOSerializeString(data.Text);
            byte[] key = SDOSerializeString(data.Key);
            byte[] iv = SDOSerializeString(data.IV);
            byte[] algorithm = SDOSerializeString(data.Algorithm);
            byte[] action = SDOSerializeString(data.Action);

            BitArray bitArray = new BitArray(new[] { (byte)6 });
            bitArray.Set(3, true);
            bitArray.Set(4, true);
            bitArray.Set(5, true);
            bitArray.Set(6, true);
            bitArray.Set(7, true);

            byte[] bytes = new byte[id.Length + text.Length + key.Length + iv.Length + algorithm.Length + action.Length + 1];

            bitArray.CopyTo(bytes, 0);

            int index = 1;
            id.CopyTo(bytes, index);
            index += id.Length;
            text.CopyTo(bytes, index);
            index += text.Length;
            key.CopyTo(bytes, index);
            index += key.Length;
            iv.CopyTo(bytes, index);
            index += iv.Length;
            algorithm.CopyTo(bytes, index);
            index += algorithm.Length;
            action.CopyTo(bytes, index);

            return bytes;
        }

        public static ClientData SDODeserializeClientData(NetworkStream stream)
        {
            ClientData data = new ClientData();

            while (true)
            {
                int result = stream.ReadByte();

                if (result == -1)
                    break;

                byte b = (byte) result;

                BitArray firstByte = new BitArray(new []{b});
                if (firstByte.And(MARK) == MARK) //MARK byte
                {
                    byte[] markLength = new byte[1];
                    firstByte.And(MARKLENGTH).CopyTo(markLength, 0);

                    int length = BitConverter.ToInt32(markLength, 0);
                }
            }

            return data;
        }

        public static byte[] SDOSerializeServerData(ServerData data)
        {
            byte[] id = SDOSerializeString(data.Id);
            byte[] text = SDOSerializeString(data.Text);

            BitArray bitArray = new BitArray(new[] { (byte)2 });
            bitArray.Set(3, true);
            bitArray.Set(4, true);
            bitArray.Set(5, true);
            bitArray.Set(6, true);
            bitArray.Set(7, true);

            byte[] bytes = new byte[id.Length + text.Length + 1];

            bitArray.CopyTo(bytes, 0);

            int index = 1;
            id.CopyTo(bytes, index);
            index += id.Length;
            text.CopyTo(bytes, index);

            return bytes;
        }

        public static ServerData SDODeserializeServerData(NetworkStream stream)
        {
            ServerData data = new ServerData();

            return data;
        }

        private static byte[] SDOSerializeString(string text)
        {
            byte[] textBytes = Encoding.UTF8.GetBytes(text);
            int textBytesLength = textBytes.Length;
            byte[] textBytesLengthBytes = IntBytes(textBytesLength);
            int textBytesLengthBytesLength = textBytesLengthBytes.Length;

            byte[] result = new byte[textBytesLengthBytesLength + textBytesLength + 1];

            BitArray bitArray;

            if (textBytesLength < 16)
            {
                bitArray = new BitArray(new[] { (byte)textBytesLength });
                bitArray.Set(4, false);
            }
            else
            {
                bitArray = new BitArray(new[] { (byte)textBytesLengthBytesLength });
                bitArray.Set(4, true);
            }
            bitArray.Set(5, true);
            bitArray.Set(6, true);
            bitArray.Set(7, false);

            bitArray.CopyTo(result, 0);
            int resultIndex = 1;

            for (int i = 0; i < textBytesLengthBytesLength; i++)
            {
                result[resultIndex] = textBytesLengthBytes[i];
                resultIndex++;
            }

            for (int i = 0; i < textBytesLength; i++)
            {
                result[resultIndex] = textBytes[i];
                resultIndex++;
            }

            return result;
        }

        private static string SDODeserializeString(byte[] data)
        {
            string text = "";

            return text;
        }

        public static string ProcessClientData(ClientData clientData)
        {
            switch (clientData.Action)
            {
                case "Encrypt":
                    return EncryptDecrypt.Encrypt(clientData.Text, clientData.Algorithm, clientData.Key, clientData.IV);
                case "Decrypt":
                    return EncryptDecrypt.Decrypt(clientData.Text, clientData.Algorithm, clientData.Key, clientData.IV);
                default:
                    throw new Exception("Not supported client action.");
            }
        }

        private static byte[] IntBytes(int value)
        {
            if (sbyte.MinValue <= value && value <= sbyte.MaxValue)
                return BitConverter.GetBytes((sbyte)value);
            if (byte.MinValue <= value && value <= byte.MaxValue)
                return BitConverter.GetBytes((byte)value);
            if (short.MinValue <= value && value <= short.MaxValue)
                return BitConverter.GetBytes((short)value);
            if (ushort.MinValue <= value && value <= ushort.MaxValue)
                return BitConverter.GetBytes((ushort)value);
            if (int.MinValue <= value && value <= int.MaxValue)
                return BitConverter.GetBytes(value);

            return BitConverter.GetBytes(value);
        }
    }
}
