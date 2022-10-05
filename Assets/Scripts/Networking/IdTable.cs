using System.Collections.Generic;
using System.Linq;

namespace Networking
{
	public static class IdTable<T>
	{
		private static readonly Dictionary<ushort, T> Table =
			new Dictionary<ushort, T>();

		private static readonly Dictionary<T, ushort> ReverseTable =
			new Dictionary<T, ushort>();

		private static ushort _nextId { get; set; }

		public static ushort MaxId => _nextId;

		public static void TryGetValue(ushort id, out T value)
		{
			Table.TryGetValue(id, out value);
		}

		public static T GetValue(ushort id)
		{
			return Table[id];
		}

		public static ushort Add(T value)
		{
			if (Table.ContainsValue(value))
				return ReverseTable[value];

			Table.Add(_nextId, value);
			ReverseTable.Add(value, _nextId);

			_nextId++;
			return (ushort) (_nextId - 1);
		}
		public static ushort Add(T value, ushort id)
		{
			Table[id] = value;
			ReverseTable[value] = id;

			if (id >= _nextId)
				_nextId = (ushort) (id + 1);

			return id;
		}

		public static void Remove(T value)
		{
			if (!Table.ContainsValue(value))
				return;

			Table.Remove(ReverseTable[value]);
			ReverseTable.Remove(value);
		}
		public static List<T> GetAll()
		{
			return Table.Values.ToList();
		}

		public static ushort GetId(T value)
		{
			return ReverseTable[value];
		}

		public static void Clear()
		{
			Table.Clear();
			ReverseTable.Clear();
			_nextId = 0;
		}

		public static List<T> GetValues()
		{
			return Table.Values.ToList();
		}
	}
}