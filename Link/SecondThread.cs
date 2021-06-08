using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Link
{
    public class SecondThread
    {

        private Semaphore _sendSemaphore;
        private Semaphore _receiveSemaphore;
        private BitArray _receivedMessage;
		private PostToFirstWT _post;
		private Encoding _encoding;
		private static int indexPack = 0;

		public SecondThread(ref Semaphore sendSemaphore, ref Semaphore receiveSemaphore, Encoding encoding)
        {
            _sendSemaphore = sendSemaphore;
            _receiveSemaphore = receiveSemaphore;
            _encoding = encoding;
        }

        public void SecondThreadMain(Object obj)
        {
			_post = (PostToFirstWT)obj;
			ConsoleHelper.WriteToConsole("2 поток", "Начинаю работу.Жду запрос на соединение.");
            _receiveSemaphore.WaitOne();
            SetData();

        }

		private void SetData()
		{
			PackWindow itemWindow = (PackWindow)StaticFunction.DeserializeObject(StaticFunction.BitArrayToByteArray(_receivedMessage));
			List<Receipt> receipts = new List<Receipt>();
			for(int el = 0; el < itemWindow.UsefulPack; el++)
            {
				receipts.Add(CheckStatus(itemWindow.Packs[el]));
            }
			Receipt receipt = null;
			
			foreach(var el in receipts)
            {
				if(el.Status != new BitArray(BitConverter.GetBytes(StaticFunction.RR)))
                {
					receipt = el;
					break;
                }
            }
			if(receipt != null)
            {
				_post(new BitArray(StaticFunction.SerializeObject(receipt)));
			}
            else
            {
				_post(new BitArray(StaticFunction.SerializeObject(receipts.TakeLast(1))));
			}
			_sendSemaphore.Release();
			_receiveSemaphore.WaitOne();
			SetData();
		}
		public void ReceiveData(BitArray array)
        {
            _receivedMessage = array;
        }

		public int CheckSum(bool[] array)
		{
			var checkSum = 0;
			
			for (int i = 0; i < array.Length; i++)
			{
				checkSum += array[i] == false ? 0 : 1;
			}
			return checkSum;
		}

		public Receipt CheckStatus(Pack item)
        {
			Receipt receipt = null;

			bool packId = true;
			if (item.Id != indexPack + 1)
				if (item.Id != 0 && indexPack == 7)
					packId = false;
			if (item == null)
			{
				receipt = new Receipt(status: new BitArray(BitConverter.GetBytes(StaticFunction.REJ)));
				ConsoleHelper.WriteToConsole("2 поток", "Возникла ошибка. Запрашиваю пакет заново");
			}
			else
			{
				switch (BitConverter.ToInt32(StaticFunction.BitArrayToByteArray(item.Status), 0))
				{
					case 1: //RR
						ConsoleHelper.WriteToConsole("2 поток", $"Получен кадр #{item.Id}");
						var value = new bool[item.Data.Count];
						for (int m = 0; m < item.Data.Count; m++)
							value[m] = item.Data[m];
						var checkSum = CheckSum(value);
						if (checkSum == item.CheckSum && packId)
						{
							indexPack = item.Id;
							if (item.RepeatId == null)
								StaticFunction.AddData(null, item.Data);
							else
								StaticFunction.AddData(item.RepeatId, item.Data);
							receipt = new Receipt(item.Id, new BitArray(BitConverter.GetBytes(StaticFunction.RR)));
						}
						else
						{
							receipt = new Receipt(item.Id, new BitArray(BitConverter.GetBytes(StaticFunction.REJ)));
							ConsoleHelper.WriteToConsole("2 поток", "Возникла ошибка. Запрашиваю пакет заново.");
						}
						break;
					case 4: //RD
						receipt = new Receipt(item.Id, new BitArray(BitConverter.GetBytes(StaticFunction.DISC)));
						var booleans = new List<bool>();
						for (int i = 0; i < StaticFunction.Result.Count; i++)
							for (int j = 0; j < StaticFunction.Result[i].Length; j++)
								booleans.Add(StaticFunction.Result[i][j]);
						ConsoleHelper.WriteToConsole("2 поток", "Разъединение одобрено. Завершение работы.");
						ConsoleHelper.WriteToConsole("2 поток", $"Полученные данные:  {_encoding.GetString(StaticFunction.BitArrayToByteArray(new BitArray(booleans.ToArray())))}");
						break;
					case 6: //RIM
						ConsoleHelper.WriteToConsole("2 поток", "Пришел запрос на соединение. Подтверждаю.");
						receipt = new Receipt(status: new BitArray(BitConverter.GetBytes(StaticFunction.SIM)));
						break;
					case 8: //UP
						receipt = new Receipt(item.Id, new BitArray(BitConverter.GetBytes(StaticFunction.UA)));
						ConsoleHelper.WriteToConsole("2 поток", "Пришел запрос на передачу. Подтверждаю.");
						break;
					default:
						receipt = new Receipt(status: new BitArray(BitConverter.GetBytes(StaticFunction.REJ)));
						ConsoleHelper.WriteToConsole("2 поток", "Возникла ошибка. Запрашиваю пакет заново");
						break;
				}
			}
			return receipt;
		}
	}
}
