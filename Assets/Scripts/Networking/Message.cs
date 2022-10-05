using System;
using System.Text;
using UnityEngine;

namespace Networking
{
	public class Message
	{
		public const int MaxMessageSize = 1250;
		public byte[] Bytes { get; private set; }

		private ushort _writePos = 0;
		private ushort _readPos = 0;

		public int UnreadLength => _writePos - _readPos;
		public ushort WrittenLength
		{
			get => _writePos;
			set => _writePos = value;
		}

		internal int UnwrittenLength => Bytes.Length - _writePos;

		public Message(int maxSize = MaxMessageSize) => Bytes = new byte[maxSize];

		public byte[] ToArray()
		{
			var array = new byte[_writePos];
			Buffer.BlockCopy(Bytes, 0, array, 0, _writePos);
			return array;	
		}

		#region Byte

		public Message AddByte(byte value) => Add(value);

		public Message Add(byte value)
		{
			if (UnwrittenLength < 1)
				throw new Exception(
					$"Message has insufficient remaining capacity ({UnwrittenLength}) to add type 'byte'!");

			Bytes[_writePos++] = value;
			return this;
		}


		public byte GetByte()
		{
			if (UnreadLength < 1)
			{
				Debug.LogError(
					$"Message contains insufficient unread bytes ({UnreadLength}) to read type 'byte', returning 0!");
				return 0;
			}

			return Bytes[_readPos++]; // GetValue the byte at _readPos' position
		}


		public Message AddBytes(byte[] value, bool includeLength = true, bool isBigArray = false) =>
			Add(value, includeLength, isBigArray);


		public Message Add(byte[] array, bool includeLength = true, bool isBigArray = false)
		{
			if (includeLength)
			{
				if (isBigArray)
					Add((ushort) array.Length);
				else
				{
					if (array.Length > byte.MaxValue)
						throw new Exception(
							$"Array is too long for the length to be stored in a single byte! Set isBigArray to true when calling Add & GetBytes to store the length in 2 bytes instead.");
					Add((byte) array.Length);
				}
			}

			if (UnwrittenLength < array.Length)
				throw new Exception(
					$"Message has insufficient remaining capacity ({UnwrittenLength}) to add type 'byte[]'!");

			Array.Copy(array, 0, Bytes, _writePos, array.Length);
			_writePos += (ushort) array.Length;
			return this;
		}


		public byte[] GetBytes(bool isBigArray = false)
		{
			if (isBigArray)
				return GetBytes(GetUShort());
			else
				return GetBytes(GetByte());
		}


		public byte[] GetBytes(int amount)
		{
			byte[] array = new byte[amount];
			ReadBytes(amount, array);
			return array;
		}


		public void GetBytes(int amount, byte[] array, int startIndex = 0)
		{
			if (startIndex + amount > array.Length)
				throw new ArgumentOutOfRangeException(
					$"Destination array isn't long enough to fit {amount} bytes, starting at index {startIndex}!");

			ReadBytes(amount, array, startIndex);
		}


		private void ReadBytes(int amount, byte[] array, int startIndex = 0)
		{
			if (UnreadLength < amount)
			{
				Debug.LogError(
					$"Message contains insufficient unread bytes ({UnreadLength}) to read type 'byte[]', array will contain default elements!");
				amount = UnreadLength;
			}

			Array.Copy(Bytes, _readPos, array, startIndex,
				amount); // Copy the bytes at _readPos' position to the array that will be returned
			_readPos += (ushort) amount;
		}

		#endregion

		#region Bool

		public Message AddBool(bool value) => Add(value);


		public Message Add(bool value)
		{
			if (UnwrittenLength < ByteConverter.BoolLength)
				throw new Exception(
					$"Message has insufficient remaining capacity ({UnwrittenLength}) to add type 'bool'!");

			Bytes[_writePos++] = (byte) (value ? 1 : 0);
			return this;
		}


		public bool GetBool()
		{
			if (UnreadLength < ByteConverter.BoolLength)
			{
				Debug.LogError(
					$"Message contains insufficient unread bytes ({UnreadLength}) to read type 'bool', returning false!");
				return false;
			}

			return Bytes[_readPos++] == 1; // Convert the byte at _readPos' position to a bool
		}


		public Message AddBools(bool[] value, bool includeLength = true, bool isBigArray = false) =>
			Add(value, includeLength, isBigArray);


		public Message Add(bool[] array, bool includeLength = true, bool isBigArray = false)
		{
			if (includeLength)
			{
				if (isBigArray)
					Add((ushort) array.Length);
				else
				{
					if (array.Length > byte.MaxValue)
						throw new Exception(
							$"Array is too long for the length to be stored in a single byte! Set isBigArray to true when calling Add & GetBools to store the length in 2 bytes instead.");
					Add((byte) array.Length);
				}
			}

			ushort byteLength = (ushort) (array.Length / 8 + (array.Length % 8 == 0 ? 0 : 1));
			if (UnwrittenLength < byteLength)
				throw new Exception(
					$"Message has insufficient remaining capacity ({UnwrittenLength}) to add type 'bool[]'!");

			// Pack 8 bools into each byte
			bool isLengthMultipleOf8 = array.Length % 8 == 0;
			for (int i = 0; i < byteLength; i++)
			{
				byte nextByte = 0;
				int bitsToWrite = 8;
				if ((i + 1) == byteLength && !isLengthMultipleOf8)
					bitsToWrite = array.Length % 8;

				for (int bit = 0; bit < bitsToWrite; bit++)
					nextByte |= (byte) ((array[i * 8 + bit] ? 1 : 0) << bit);

				Bytes[_writePos + i] = nextByte;
			}

			_writePos += byteLength;
			return this;
		}


		public bool[] GetBools(bool isBigArray = false)
		{
			if (isBigArray)
				return GetBools(GetUShort());
			else
				return GetBools(GetByte());
		}


		public bool[] GetBools(int amount)
		{
			bool[] array = new bool[amount];

			int byteAmount = amount / 8 + (amount % 8 == 0 ? 0 : 1);
			if (UnreadLength < byteAmount)
			{
				Debug.LogError(
					$"Message contains insufficient unread bytes ({UnreadLength}) to read type 'bool[]', array will contain default elements!");
				byteAmount = UnreadLength;
			}

			ReadBools(byteAmount, array);
			return array;
		}


		public void GetBools(int amount, bool[] array, int startIndex = 0)
		{
			if (startIndex + amount > array.Length)
				throw new ArgumentOutOfRangeException(
					$"Destination array isn't long enough to fit {amount} bools, starting at index {startIndex}!");

			int byteAmount = amount / 8 + (amount % 8 == 0 ? 0 : 1);
			if (UnreadLength < byteAmount)
				Debug.LogError(
					$"Message contains insufficient unread bytes ({UnreadLength}) to read type 'bool[]', array will contain default elements!");

			ReadBools(byteAmount, array, startIndex);
		}


		private void ReadBools(int byteAmount, bool[] array, int startIndex = 0)
		{
			// Read 8 bools from each byte
			bool isLengthMultipleOf8 = array.Length % 8 == 0;
			for (int i = 0; i < byteAmount; i++)
			{
				int bitsToRead = 8;
				if ((i + 1) == byteAmount && !isLengthMultipleOf8)
					bitsToRead = array.Length % 8;

				for (int bit = 0; bit < bitsToRead; bit++)
					array[startIndex + (i * 8 + bit)] = (Bytes[_readPos + i] >> bit & 1) == 1;
			}

			_readPos += (ushort) byteAmount;
		}

		#endregion

		#region Short & UShort

		public Message AddShort(short value) => Add(value);


		public Message Add(short value)
		{
			if (UnwrittenLength < ByteConverter.ShortLength)
				throw new Exception(
					$"Message has insufficient remaining capacity ({UnwrittenLength}) to add type 'short'!");

			ByteConverter.FromShort(value, Bytes, _writePos);
			_writePos += ByteConverter.ShortLength;
			return this;
		}


		public Message AddUShort(ushort value) => Add(value);


		public Message Add(ushort value)
		{
			if (UnwrittenLength < ByteConverter.UShortLength)
				throw new Exception(
					$"Message has insufficient remaining capacity ({UnwrittenLength}) to add type 'ushort'!");

			ByteConverter.FromUShort(value, Bytes, _writePos);
			_writePos += ByteConverter.UShortLength;
			return this;
		}


		public short GetShort()
		{
			if (UnreadLength < ByteConverter.ShortLength)
			{
				Debug.LogError(
					$"Message contains insufficient unread bytes ({UnreadLength}) to read type 'short', returning 0!");
				return 0;
			}

			short value = ByteConverter.ToShort(Bytes, _readPos);
			_readPos += ByteConverter.ShortLength;
			return value;
		}


		public ushort GetUShort()
		{
			if (UnreadLength < ByteConverter.UShortLength)
			{
				Debug.LogError(
					$"Message contains insufficient unread bytes ({UnreadLength}) to read type 'ushort', returning 0!");
				return 0;
			}

			ushort value = ByteConverter.ToUShort(Bytes, _readPos);
			_readPos += ByteConverter.UShortLength;
			return value;
		}


		internal ushort PeekUShort()
		{
			if (UnreadLength < ByteConverter.UShortLength)
			{
				Debug.LogError(
					$"Message contains insufficient unread bytes ({UnreadLength}) to peek type 'ushort', returning 0!");
				return 0;
			}

			return ByteConverter.ToUShort(Bytes, _readPos);
		}


		public Message AddShorts(short[] value, bool includeLength = true, bool isBigArray = false) =>
			Add(value, includeLength, isBigArray);


		public Message Add(short[] array, bool includeLength = true, bool isBigArray = false)
		{
			if (includeLength)
			{
				if (isBigArray)
					Add((ushort) array.Length);
				else
				{
					if (array.Length > byte.MaxValue)
						throw new Exception(
							$"Array is too long for the length to be stored in a single byte! Set isBigArray to true when calling Add & GetShorts to store the length in 2 bytes instead.");
					Add((byte) array.Length);
				}
			}

			if (UnwrittenLength < array.Length * ByteConverter.ShortLength)
				throw new Exception(
					$"Message has insufficient remaining capacity ({UnwrittenLength}) to add type 'short[]'!");

			for (int i = 0; i < array.Length; i++)
				Add(array[i]);

			return this;
		}


		public Message AddUShorts(ushort[] value, bool includeLength = true, bool isBigArray = false) =>
			Add(value, includeLength, isBigArray);


		public Message Add(ushort[] array, bool includeLength = true, bool isBigArray = false)
		{
			if (includeLength)
			{
				if (isBigArray)
					Add((ushort) array.Length);
				else
				{
					if (array.Length > byte.MaxValue)
						throw new Exception(
							$"Array is too long for the length to be stored in a single byte! Set isBigArray to true when calling Add & GetUShorts to store the length in 2 bytes instead.");
					Add((byte) array.Length);
				}
			}

			if (UnwrittenLength < array.Length * ByteConverter.UShortLength)
				throw new Exception(
					$"Message has insufficient remaining capacity ({UnwrittenLength}) to add type 'ushort[]'!");

			for (int i = 0; i < array.Length; i++)
				Add(array[i]);

			return this;
		}


		public short[] GetShorts(bool isBigArray = false)
		{
			if (isBigArray)
				return GetShorts(GetUShort());
			else
				return GetShorts(GetByte());
		}


		public short[] GetShorts(int amount)
		{
			short[] array = new short[amount];
			ReadShorts(amount, array);
			return array;
		}


		public void GetShorts(int amount, short[] array, int startIndex = 0)
		{
			if (startIndex + amount > array.Length)
				throw new ArgumentOutOfRangeException(
					$"Destination array isn't long enough to fit {amount} shorts, starting at index {startIndex}!");

			ReadShorts(amount, array, startIndex);
		}


		public ushort[] GetUShorts(bool isBigArray = false)
		{
			if (isBigArray)
				return GetUShorts(GetUShort());
			else
				return GetUShorts(GetByte());
		}


		public ushort[] GetUShorts(int amount)
		{
			ushort[] array = new ushort[amount];
			ReadUShorts(amount, array);
			return array;
		}


		public void GetUShorts(int amount, ushort[] array, int startIndex = 0)
		{
			if (startIndex + amount > array.Length)
				throw new ArgumentOutOfRangeException(
					$"Destination array isn't long enough to fit {amount} ushorts, starting at index {startIndex}!");

			ReadUShorts(amount, array, startIndex);
		}


		private void ReadShorts(int amount, short[] array, int startIndex = 0)
		{
			if (UnreadLength < amount * ByteConverter.ShortLength)
			{
				Debug.LogError(
					$"Message contains insufficient unread bytes ({UnreadLength}) to read type 'short[]', array will contain default elements!");
				amount = UnreadLength / ByteConverter.ShortLength;
			}

			for (int i = 0; i < amount; i++)
			{
				array[startIndex + i] = ByteConverter.ToShort(Bytes, _readPos);
				_readPos += ByteConverter.ShortLength;
			}
		}


		private void ReadUShorts(int amount, ushort[] array, int startIndex = 0)
		{
			if (UnreadLength < amount * ByteConverter.UShortLength)
			{
				Debug.LogError(
					$"Message contains insufficient unread bytes ({UnreadLength}) to read type 'ushort[]', array will contain default elements!");
				amount = UnreadLength / ByteConverter.ShortLength;
			}

			for (int i = 0; i < amount; i++)
			{
				array[startIndex + i] = ByteConverter.ToUShort(Bytes, _readPos);
				_readPos += ByteConverter.UShortLength;
			}
		}

		#endregion

		#region Int & UInt

		public Message AddInt(int value) => Add(value);


		public Message Add(int value)
		{
			if (UnwrittenLength < ByteConverter.IntLength)
				throw new Exception(
					$"Message has insufficient remaining capacity ({UnwrittenLength}) to add type 'int'!");

			ByteConverter.FromInt(value, Bytes, _writePos);
			_writePos += ByteConverter.IntLength;
			return this;
		}


		public Message AddUInt(uint value) => Add(value);


		public Message Add(uint value)
		{
			if (UnwrittenLength < ByteConverter.UIntLength)
				throw new Exception(
					$"Message has insufficient remaining capacity ({UnwrittenLength}) to add type 'uint'!");

			ByteConverter.FromUInt(value, Bytes, _writePos);
			_writePos += ByteConverter.UIntLength;
			return this;
		}


		public int GetInt()
		{
			if (UnreadLength < ByteConverter.IntLength)
			{
				Debug.LogError(
					$"Message contains insufficient unread bytes ({UnreadLength}) to read type 'int', returning 0!");
				return 0;
			}

			int value = ByteConverter.ToInt(Bytes, _readPos);
			_readPos += ByteConverter.IntLength;
			return value;
		}


		public uint GetUInt()
		{
			if (UnreadLength < ByteConverter.UIntLength)
			{
				Debug.LogError(
					$"Message contains insufficient unread bytes ({UnreadLength}) to read type 'uint', returning 0!");
				return 0;
			}

			uint value = ByteConverter.ToUInt(Bytes, _readPos);
			_readPos += ByteConverter.UIntLength;
			return value;
		}


		public Message AddInts(int[] value, bool includeLength = true, bool isBigArray = false) =>
			Add(value, includeLength, isBigArray);


		public Message Add(int[] array, bool includeLength = true, bool isBigArray = false)
		{
			if (includeLength)
			{
				if (isBigArray)
					Add((ushort) array.Length);
				else
				{
					if (array.Length > byte.MaxValue)
						throw new Exception(
							$"Array is too long for the length to be stored in a single byte! Set isBigArray to true when calling Add & GetInts to store the length in 2 bytes instead.");
					Add((byte) array.Length);
				}
			}

			if (UnwrittenLength < array.Length * ByteConverter.IntLength)
				throw new Exception(
					$"Message has insufficient remaining capacity ({UnwrittenLength}) to add type 'int[]'!");

			for (int i = 0; i < array.Length; i++)
				Add(array[i]);

			return this;
		}


		public Message AddUInts(uint[] value, bool includeLength = true, bool isBigArray = false) =>
			Add(value, includeLength, isBigArray);


		public Message Add(uint[] array, bool includeLength = true, bool isBigArray = false)
		{
			if (includeLength)
			{
				if (isBigArray)
					Add((ushort) array.Length);
				else
				{
					if (array.Length > byte.MaxValue)
						throw new Exception(
							$"Array is too long for the length to be stored in a single byte! Set isBigArray to true when calling Add & GetUInts to store the length in 2 bytes instead.");
					Add((byte) array.Length);
				}
			}

			if (UnwrittenLength < array.Length * ByteConverter.UIntLength)
				throw new Exception(
					$"Message has insufficient remaining capacity ({UnwrittenLength}) to add type 'uint[]'!");

			for (int i = 0; i < array.Length; i++)
				Add(array[i]);

			return this;
		}


		public int[] GetInts(bool isBigArray = false)
		{
			if (isBigArray)
				return GetInts(GetUShort());
			else
				return GetInts(GetByte());
		}


		public int[] GetInts(int amount)
		{
			int[] array = new int[amount];
			ReadInts(amount, array);
			return array;
		}


		public void GetInts(int amount, int[] array, int startIndex = 0)
		{
			if (startIndex + amount > array.Length)
				throw new ArgumentOutOfRangeException(
					$"Destination array isn't long enough to fit {amount} ints, starting at index {startIndex}!");

			ReadInts(amount, array, startIndex);
		}


		public uint[] GetUInts(bool isBigArray = false)
		{
			if (isBigArray)
				return GetUInts(GetUShort());
			else
				return GetUInts(GetByte());
		}


		public uint[] GetUInts(int amount)
		{
			uint[] array = new uint[amount];
			ReadUInts(amount, array);
			return array;
		}


		public void GetUInts(int amount, uint[] array, int startIndex = 0)
		{
			if (startIndex + amount > array.Length)
				throw new ArgumentOutOfRangeException(
					$"Destination array isn't long enough to fit {amount} uints, starting at index {startIndex}!");

			ReadUInts(amount, array, startIndex);
		}


		private void ReadInts(int amount, int[] array, int startIndex = 0)
		{
			if (UnreadLength < amount * ByteConverter.IntLength)
			{
				Debug.LogError(
					$"Message contains insufficient unread bytes ({UnreadLength}) to read type 'int[]', array will contain default elements!");
				amount = UnreadLength / ByteConverter.IntLength;
			}

			for (int i = 0; i < amount; i++)
			{
				array[startIndex + i] = ByteConverter.ToInt(Bytes, _readPos);
				_readPos += ByteConverter.IntLength;
			}
		}


		private void ReadUInts(int amount, uint[] array, int startIndex = 0)
		{
			if (UnreadLength < amount * ByteConverter.UIntLength)
			{
				Debug.LogError(
					$"Message contains insufficient unread bytes ({UnreadLength}) to read type 'uint[]', array will contain default elements!");
				amount = UnreadLength / ByteConverter.UIntLength;
			}

			for (int i = 0; i < amount; i++)
			{
				array[startIndex + i] = ByteConverter.ToUInt(Bytes, _readPos);
				_readPos += ByteConverter.UIntLength;
			}
		}

		#endregion

		#region Long & ULong

		public Message AddLong(long value) => Add(value);


		public Message Add(long value)
		{
			if (UnwrittenLength < ByteConverter.LongLength)
				throw new Exception(
					$"Message has insufficient remaining capacity ({UnwrittenLength}) to add type 'long'!");

			ByteConverter.FromLong(value, Bytes, _writePos);
			_writePos += ByteConverter.LongLength;
			return this;
		}


		public Message AddULong(ulong value) => Add(value);


		public Message Add(ulong value)
		{
			if (UnwrittenLength < ByteConverter.ULongLength)
				throw new Exception(
					$"Message has insufficient remaining capacity ({UnwrittenLength}) to add type 'ulong'!");

			ByteConverter.FromULong(value, Bytes, _writePos);
			_writePos += ByteConverter.ULongLength;
			return this;
		}


		public long GetLong()
		{
			if (UnreadLength < ByteConverter.LongLength)
			{
				Debug.LogError(
					$"Message contains insufficient unread bytes ({UnreadLength}) to read type 'long', returning 0!");
				return 0;
			}

			long value = ByteConverter.ToLong(Bytes, _readPos);
			_readPos += ByteConverter.LongLength;
			return value;
		}


		public ulong GetULong()
		{
			if (UnreadLength < ByteConverter.ULongLength)
			{
				Debug.LogError(
					$"Message contains insufficient unread bytes ({UnreadLength}) to read type 'ulong', returning 0!");
				return 0;
			}

			ulong value = ByteConverter.ToULong(Bytes, _readPos);
			_readPos += ByteConverter.ULongLength;
			return value;
		}


		public Message AddLongs(long[] value, bool includeLength = true, bool isBigArray = false) =>
			Add(value, includeLength, isBigArray);


		public Message Add(long[] array, bool includeLength = true, bool isBigArray = false)
		{
			if (includeLength)
			{
				if (isBigArray)
					Add((ushort) array.Length);
				else
				{
					if (array.Length > byte.MaxValue)
						throw new Exception(
							$"Array is too long for the length to be stored in a single byte! Set isBigArray to true when calling Add & GetLongs to store the length in 2 bytes instead.");
					Add((byte) array.Length);
				}
			}

			if (UnwrittenLength < array.Length * ByteConverter.LongLength)
				throw new Exception(
					$"Message has insufficient remaining capacity ({UnwrittenLength}) to add type 'long[]'!");

			for (int i = 0; i < array.Length; i++)
				Add(array[i]);

			return this;
		}


		public Message AddULongs(ulong[] value, bool includeLength = true, bool isBigArray = false) =>
			Add(value, includeLength, isBigArray);


		public Message Add(ulong[] array, bool includeLength = true, bool isBigArray = false)
		{
			if (includeLength)
			{
				if (isBigArray)
					Add((ushort) array.Length);
				else
				{
					if (array.Length > byte.MaxValue)
						throw new Exception(
							$"Array is too long for the length to be stored in a single byte! Set isBigArray to true when calling Add & GetULongs to store the length in 2 bytes instead.");
					Add((byte) array.Length);
				}
			}

			if (UnwrittenLength < array.Length * ByteConverter.ULongLength)
				throw new Exception(
					$"Message has insufficient remaining capacity ({UnwrittenLength}) to add type 'ulong[]'!");

			for (int i = 0; i < array.Length; i++)
				Add(array[i]);

			return this;
		}


		public long[] GetLongs(bool isBigArray = false)
		{
			if (isBigArray)
				return GetLongs(GetUShort());
			else
				return GetLongs(GetByte());
		}


		public long[] GetLongs(int amount)
		{
			long[] array = new long[amount];
			ReadLongs(amount, array);
			return array;
		}


		public void GetLongs(int amount, long[] array, int startIndex = 0)
		{
			if (startIndex + amount > array.Length)
				throw new ArgumentOutOfRangeException(
					$"Destination array isn't long enough to fit {amount} longs, starting at index {startIndex}!");

			ReadLongs(amount, array, startIndex);
		}


		public ulong[] GetULongs(bool isBigArray = false)
		{
			if (isBigArray)
				return GetULongs(GetUShort());
			else
				return GetULongs(GetByte());
		}


		public ulong[] GetULongs(int amount)
		{
			ulong[] array = new ulong[amount];
			ReadULongs(amount, array);
			return array;
		}


		public void GetULongs(int amount, ulong[] array, int startIndex = 0)
		{
			if (startIndex + amount > array.Length)
				throw new ArgumentOutOfRangeException(
					$"Destination array isn't long enough to fit {amount} ulongs, starting at index {startIndex}!");

			ReadULongs(amount, array, startIndex);
		}


		private void ReadLongs(int amount, long[] array, int startIndex = 0)
		{
			if (UnreadLength < amount * ByteConverter.LongLength)
			{
				Debug.LogError(
					$"Message contains insufficient unread bytes ({UnreadLength}) to read type 'long[]', array will contain default elements!");
				amount = UnreadLength / ByteConverter.LongLength;
			}

			for (int i = 0; i < amount; i++)
			{
				array[startIndex + i] = ByteConverter.ToLong(Bytes, _readPos);
				_readPos += ByteConverter.LongLength;
			}
		}


		private void ReadULongs(int amount, ulong[] array, int startIndex = 0)
		{
			if (UnreadLength < amount * ByteConverter.ULongLength)
			{
				Debug.LogError(
					$"Message contains insufficient unread bytes ({UnreadLength}) to read type 'ulong[]', array will contain default elements!");
				amount = UnreadLength / ByteConverter.ULongLength;
			}

			for (int i = 0; i < amount; i++)
			{
				array[startIndex + i] = ByteConverter.ToULong(Bytes, _readPos);
				_readPos += ByteConverter.ULongLength;
			}
		}

		#endregion

		#region Float

		public Message AddFloat(float value) => Add(value);


		public Message Add(float value)
		{
			if (UnwrittenLength < ByteConverter.FloatLength)
				throw new Exception(
					$"Message has insufficient remaining capacity ({UnwrittenLength}) to add type 'float'!");

			ByteConverter.FromFloat(value, Bytes, _writePos);
			_writePos += ByteConverter.FloatLength;
			return this;
		}


		public float GetFloat()
		{
			if (UnreadLength < ByteConverter.FloatLength)
			{
				Debug.LogError(
					$"Message contains insufficient unread bytes ({UnreadLength}) to read type 'float', returning 0!");
				return 0;
			}

			float value = ByteConverter.ToFloat(Bytes, _readPos);
			_readPos += ByteConverter.FloatLength;
			return value;
		}


		public Message AddFloats(float[] value, bool includeLength = true, bool isBigArray = false) =>
			Add(value, includeLength, isBigArray);


		public Message Add(float[] array, bool includeLength = true, bool isBigArray = false)
		{
			if (includeLength)
			{
				if (isBigArray)
					Add((ushort) array.Length);
				else
				{
					if (array.Length > byte.MaxValue)
						throw new Exception(
							$"Array is too long for the length to be stored in a single byte! Set isBigArray to true when calling Add & GetFloats to store the length in 2 bytes instead.");
					Add((byte) array.Length);
				}
			}

			if (UnwrittenLength < array.Length * ByteConverter.FloatLength)
				throw new Exception(
					$"Message has insufficient remaining capacity ({UnwrittenLength}) to add type 'float[]'!");

			for (int i = 0; i < array.Length; i++)
				Add(array[i]);

			return this;
		}


		public float[] GetFloats(bool isBigArray = false)
		{
			if (isBigArray)
				return GetFloats(GetUShort());
			else
				return GetFloats(GetByte());
		}


		public float[] GetFloats(int amount)
		{
			float[] array = new float[amount];
			ReadFloats(amount, array);
			return array;
		}


		public void GetFloats(int amount, float[] array, int startIndex = 0)
		{
			if (startIndex + amount > array.Length)
				throw new ArgumentOutOfRangeException(
					$"Destination array isn't long enough to fit {amount} floats, starting at index {startIndex}!");

			ReadFloats(amount, array, startIndex);
		}


		private void ReadFloats(int amount, float[] array, int startIndex = 0)
		{
			if (UnreadLength < amount * ByteConverter.FloatLength)
			{
				Debug.LogError(
					$"Message contains insufficient unread bytes ({UnreadLength}) to read type 'float[]', array will contain default elements!");
				amount = UnreadLength / ByteConverter.FloatLength;
			}

			for (int i = 0; i < amount; i++)
			{
				array[startIndex + i] = ByteConverter.ToFloat(Bytes, _readPos);
				_readPos += ByteConverter.FloatLength;
			}
		}

		#endregion

		#region Double

		public Message AddDouble(double value) => Add(value);


		public Message Add(double value)
		{
			if (UnwrittenLength < ByteConverter.DoubleLength)
				throw new Exception(
					$"Message has insufficient remaining capacity ({UnwrittenLength}) to add type 'double'!");

			ByteConverter.FromDouble(value, Bytes, _writePos);
			_writePos += ByteConverter.DoubleLength;
			return this;
		}


		public double GetDouble()
		{
			if (UnreadLength < ByteConverter.DoubleLength)
			{
				Debug.LogError(
					$"Message contains insufficient unread bytes ({UnreadLength}) to read type 'double', returning 0!");
				return 0;
			}

			double value = ByteConverter.ToDouble(Bytes, _readPos);
			_readPos += ByteConverter.DoubleLength;
			return value;
		}


		public Message AddDoubles(double[] value, bool includeLength = true, bool isBigArray = false) =>
			Add(value, includeLength, isBigArray);


		public Message Add(double[] array, bool includeLength = true, bool isBigArray = false)
		{
			if (includeLength)
			{
				if (isBigArray)
					Add((ushort) array.Length);
				else
				{
					if (array.Length > byte.MaxValue)
						throw new Exception(
							$"Array is too long for the length to be stored in a single byte! Set isBigArray to true when calling Add & GetDoubles to store the length in 2 bytes instead.");
					Add((byte) array.Length);
				}
			}

			if (UnwrittenLength < array.Length * ByteConverter.DoubleLength)
				throw new Exception(
					$"Message has insufficient remaining capacity ({UnwrittenLength}) to add type 'double[]'!");

			for (int i = 0; i < array.Length; i++)
				Add(array[i]);

			return this;
		}


		public double[] GetDoubles(bool isBigArray = false)
		{
			if (isBigArray)
				return GetDoubles(GetUShort());
			else
				return GetDoubles(GetByte());
		}


		public double[] GetDoubles(int amount)
		{
			double[] array = new double[amount];
			ReadDoubles(amount, array);
			return array;
		}


		public void GetDoubles(int amount, double[] array, int startIndex = 0)
		{
			if (startIndex + amount > array.Length)
				throw new ArgumentOutOfRangeException(
					$"Destination array isn't long enough to fit {amount} doubles, starting at index {startIndex}!");

			ReadDoubles(amount, array, startIndex);
		}


		private void ReadDoubles(int amount, double[] array, int startIndex = 0)
		{
			if (UnreadLength < amount * ByteConverter.DoubleLength)
			{
				Debug.LogError(
					$"Message contains insufficient unread bytes ({UnreadLength}) to read type 'double[]', array will contain default elements!");
				amount = UnreadLength / ByteConverter.DoubleLength;
			}

			for (int i = 0; i < amount; i++)
			{
				array[startIndex + i] = ByteConverter.ToDouble(Bytes, _readPos);
				_readPos += ByteConverter.DoubleLength;
			}
		}

		#endregion

		#region String

		public Message AddString(string value) => Add(value);


		public Message Add(string value)
		{
			byte[] stringBytes = Encoding.UTF8.GetBytes(value);
			Add((ushort) stringBytes.Length); // Add the length of the string (in bytes) to the message

			if (UnwrittenLength < stringBytes.Length)
				throw new Exception(
					$"Message has insufficient remaining capacity ({UnwrittenLength}) to add type 'string'!");

			Add(stringBytes, false); // Add the string itself
			return this;
		}


		public string GetString()
		{
			ushort length = GetUShort(); // GetValue the length of the string (in bytes, NOT characters)
			if (UnreadLength < length)
			{
				Debug.LogError(
					$"Message contains insufficient unread bytes ({UnreadLength}) to read type 'string', result will be truncated!");
				length = (ushort) UnreadLength;
			}

			string value =
				Encoding.UTF8.GetString(Bytes, _readPos, length); // Convert the bytes at _readPos' position to a string
			_readPos += length;
			return value;
		}


		public Message AddStrings(string[] value, bool includeLength = true, bool isBigArray = false) =>
			Add(value, includeLength, isBigArray);


		public Message Add(string[] array, bool includeLength = true, bool isBigArray = false)
		{
			if (includeLength)
			{
				if (isBigArray)
					Add((ushort) array.Length);
				else
				{
					if (array.Length > byte.MaxValue)
						throw new Exception(
							$"Array is too long for the length to be stored in a single byte! Set isBigArray to true when calling Add & GetStrings to store the length in 2 bytes instead.");
					Add((byte) array.Length);
				}
			}

			for (int i = 0; i < array.Length; i++)
				Add(array[i]);

			return this;
		}


		public string[] GetStrings(bool isBigArray = false)
		{
			if (isBigArray)
				return GetStrings(GetUShort());
			else
				return GetStrings(GetByte());
		}


		public string[] GetStrings(int amount)
		{
			string[] array = new string[amount];
			for (int i = 0; i < array.Length; i++)
				array[i] = GetString();

			return array;
		}


		public void GetStrings(int amount, string[] array, int startIndex = 0)
		{
			if (startIndex + amount > array.Length)
				throw new ArgumentOutOfRangeException(
					$"Destination array isn't long enough to fit {amount} strings, starting at index {startIndex}!");

			for (int i = 0; i < amount; i++)
				array[startIndex + i] = GetString();
		}

		#endregion
	}
}