using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Link
{
    public class StaticFunction
    {
        public static int PackLength = 56;
		public static int PackId = 0;
		public static int Index = 0;
		public static List<BitArray> Result = new List<BitArray>();
		public static bool[][] Data;
		private static Random Random = new Random();
		public static int RR = 1; //готовность к приему
		public static int RNR = 2; // неготовность к приему
		public static int REJ = 3; // отказ
		public static int RD = 4; // запрос разъединения
		public static int DISC = 5; //разъединить соединение
		public static int RIM = 6; // запрос инициализации
		public static int SIM = 7; // установить режим инициализации
		public static int UP = 8; // запрос передачи (ненумерованный)
		public static int UA = 9; //подтверждение (ненумерованное)

		public static void AddData(int? repeatIndex, BitArray data)
		{
			var LockObject = new object();
			lock (LockObject)
			{
				try
				{
					if (repeatIndex == null)
						Result.Add(data);
					else
						Result.Insert((int)repeatIndex, data);
				}
				catch (Exception) { }
			}
		}

		public static int GetIdPack => PackId;
		public static int IndexPack()
		{
			if (PackId == 7)
            {
				PackId = 0;
			}
            else
            {
				PackId++;
			}
			return PackId;
		}
		public static BitArray SetNoiseRandom(BitArray data)
		{
			if (Random.Next(1, 100) < 10)
				for (int i = 0; i < data.Length; i++)
					if (i % Random.Next(1, 5) == 0)
						data[i] = Random.Next(1, 10) < 5;

			return data;
		}
		public static byte[] BitArrayToByteArray(BitArray data)
		{
			if (data != null)
			{
				byte[] array = new byte[(data.Length - 1) / 8 + 1];
				data.CopyTo(array, 0);
				return array;
			}
            else
            {
				return null;
            }
		}

		public static object DeserializeObject(byte[] allBytes)
		{
			if (allBytes != null)
			{
				using (var stream = new MemoryStream(allBytes))
				{
					return DeserializeFromStream(stream);
				}
			}
            else
            {
				return null;
            }
		}

        private static object DeserializeFromStream(MemoryStream stream)
        {
			IFormatter formatter = new BinaryFormatter();
			stream.Seek(0, SeekOrigin.Begin);

			return formatter.Deserialize(stream);
		}

		public static byte[] SerializeObject(object obj)
		{
			BinaryFormatter bf = new BinaryFormatter();
			using (var ms = new MemoryStream())
			{
				bf.Serialize(ms, obj);
				return ms.ToArray();
			}
		}
	}
}
