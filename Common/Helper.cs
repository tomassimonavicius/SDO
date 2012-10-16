using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Helper
    {
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

        public static ClientData SDODeserializeClientData(byte[] bytes)
        {
            ClientData data = new ClientData();

            return data;
        }

        public static byte[] SDOSerializeServerData(ServerData data)
        {
            byte[] bytes = new byte[0];

            return bytes;
        }

        public static ServerData SDODeserializeServerData(byte[] bytes)
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
