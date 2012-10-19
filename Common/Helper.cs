using System;
using System.Collections;
using System.Text;

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

            byte[] bytes = new byte[id.Length + text.Length + key.Length + iv.Length + algorithm.Length + action.Length + 7];

            bitArray.CopyTo(bytes, 0);

            int index = 1;
            bytes[index] = 1;
            index += 1;
            id.CopyTo(bytes, index);
            index += id.Length;
            bytes[index] = 2;
            index += 1;
            text.CopyTo(bytes, index);
            index += text.Length;
            bytes[index] = 3;
            index += 1;
            key.CopyTo(bytes, index);
            index += key.Length;
            bytes[index] = 4;
            index += 1;
            iv.CopyTo(bytes, index);
            index += iv.Length;
            bytes[index] = 5;
            index += 1;
            algorithm.CopyTo(bytes, index);
            index += algorithm.Length;
            bytes[index] = 6;
            index += 1;
            action.CopyTo(bytes, index);

            return bytes;
        }

        public static ClientData SDODeserializeClientData(byte[] data)
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
                        clientData.Id = SDODeserializeString(data, ref position);
                        break;
                    case 2:
                        clientData.Text = SDODeserializeString(data, ref position);
                        break;
                    case 3:
                        clientData.Key = SDODeserializeString(data, ref position);
                        break;
                    case 4:
                        clientData.IV = SDODeserializeString(data, ref position);
                        break;
                    case 5:
                        clientData.Algorithm = SDODeserializeString(data, ref position);
                        break;
                    case 6:
                        clientData.Action = SDODeserializeString(data, ref position);
                        break;
                }
            }

            return clientData;
        }

        private static int IsMarkByte(byte b)
        {
            BitArray bitArray = new BitArray(new[] { b });
            bool isMarkByte = true;

            for (int i = 7; i > 3; i--)
            {
                if (!bitArray.Get(i))
                {
                    isMarkByte = false;
                    break;
                }
            }

            if (isMarkByte)
            {
                byte[] markLength = new byte[1];
                bitArray.And(new BitArray(new[] { true, true, true, false, false, false, false, false })).CopyTo(markLength, 0);
                return markLength[0];
            }

            return -1;
        }

        public static byte[] SDOSerializeServerData(ServerData data)
        {
            byte[] id = SDOSerializeString(data.Id);
            byte[] status = SDOSerializeString(data.Status);
            byte[] text = SDOSerializeString(data.Text);

            BitArray bitArray = new BitArray(new[] { (byte)3 });
            bitArray.Set(3, true);
            bitArray.Set(4, true);
            bitArray.Set(5, true);
            bitArray.Set(6, true);
            bitArray.Set(7, true);

            byte[] bytes = new byte[id.Length + status.Length + text.Length + 4];

            bitArray.CopyTo(bytes, 0);

            int index = 1;
            bytes[index] = 1;
            index += 1;
            id.CopyTo(bytes, index);
            index += id.Length;
            bytes[index] = 2;
            index += 1;
            status.CopyTo(bytes, index);
            index += status.Length;
            bytes[index] = 3;
            index += 1;
            text.CopyTo(bytes, index);

            return bytes;
        }

        public static ServerData SDODeserializeServerData(byte[] data)
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
                        serverData.Id = SDODeserializeString(data, ref position);
                        break;
                    case 2:
                        serverData.Status = SDODeserializeString(data, ref position);
                        break;
                    case 3:
                        serverData.Text = SDODeserializeString(data, ref position);
                        break;
                }
            }

            return serverData;
        }

        private static byte[] SDOSerializeString(string text)
        {
            byte[] textBytes = Encoding.UTF8.GetBytes(text);
            int textBytesLength = textBytes.Length;
            byte[] textBytesLengthBytes = IntBytes(textBytesLength);
            int textBytesLengthBytesLength = textBytesLengthBytes.Length;

            byte[] result;

            BitArray bitArray;

            if (textBytesLength < 16)
            {
                result = new byte[textBytesLength + 1];
                bitArray = new BitArray(new[] { (byte)textBytesLength });
                bitArray.Set(4, false);
            }
            else
            {
                result = new byte[textBytesLengthBytesLength + textBytesLength + 1];
                bitArray = new BitArray(new[] { (byte)textBytesLengthBytesLength });
                bitArray.Set(4, true);
            }
            bitArray.Set(5, true);
            bitArray.Set(6, true);
            bitArray.Set(7, false);

            bitArray.CopyTo(result, 0);
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

        private static string SDODeserializeString(byte[] data, ref int position)
        {
            BitArray type = new BitArray(new []{ data[position] });

            if(!type.Get(7) && type.Get(6) && type.Get(5))
            {
                bool lengthNextInBytes = type.Get(4);
                byte[] length = new byte[1];
                type.And(new BitArray(new[] { true, true, true, true, false, false, false, false })).CopyTo(length, 0);
                int l = length[0];

                position++;

                if(lengthNextInBytes)
                {
                    byte[] tl = new byte[8];
                    for (int i = 0; i < l; i++)
                    {
                        tl[i] = data[position];
                        position++;
                    }

                    long tlCount = BitConverter.ToInt64(tl, 0);

                    byte[] t = new byte[tlCount];
                    for (int i = 0; i < tlCount; i++)
                    {
                        t[i] = data[position];
                        position++;
                    }

                    return Encoding.UTF8.GetString(t, 0, t.Length);
                }
                else
                {
                    byte[] t = new byte[l];
                    for (int i = 0; i < l; i++)
                    {
                        t[i] = data[position];
                        position++;
                    }

                    return Encoding.UTF8.GetString(t, 0, t.Length);
                }
            }
            throw new Exception("Expected string, nut received unknown type.");
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
            if (sbyte.MinValue <= value && value <= byte.MaxValue)
                return new [] {(byte)value};
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
