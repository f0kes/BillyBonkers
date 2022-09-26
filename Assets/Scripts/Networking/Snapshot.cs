using System;
using System.Collections.Generic;
using Mirror;

namespace Networking
{
	public class Snapshot
	{
		private Dictionary<ushort, byte[]> _data = new Dictionary<ushort, byte[]>();
		public Dictionary<ushort, byte[]> Data => new Dictionary<ushort, byte[]>(_data);
		public Snapshot() { }
		public Snapshot(IDictionary<ushort, byte[]> data)
		{
			_data = new Dictionary<ushort, byte[]>(data);
		}
		public byte[] this[ushort netID]
		{
			get => _data[netID];
			set => _data[netID] = value;
		}

		public void Add(ushort netID, Message data)
		{
			_data.Add(netID, data.Bytes);
		}

		public Message Get(ushort netID)
		{
			var msg = new Message();
			Array.Copy(_data[netID], msg.Bytes, _data[netID].Length);
			return msg;
		}

		public void Set(ushort netID, Message data)
		{
			_data[netID] = data.Bytes;
		}

		public void Remove(ushort netID)
		{
			_data.Remove(netID);
		}
		
	}

	public static class SnapshotReadWrite
	{
		public static void WriteMyType(this NetworkWriter writer, Snapshot value)
		{
			writer.WriteUShort((ushort)value.Data.Count);
			foreach (var item in value.Data)
			{
				writer.WriteUShort(item.Key);
				writer.WriteBytesAndSize(item.Value, 0, item.Value.Length);
			}
		}

		public static Snapshot ReadMyType(this NetworkReader reader)
		{
			var count = reader.ReadUShort();
			var data = new Dictionary<ushort, byte[]>();
			for (int i = 0; i < count; i++)
			{
				var key = reader.ReadUShort();
				var value = reader.ReadBytesAndSize();
				data.Add(key, value);
			}
			return new Snapshot(data);
		}
	}
}