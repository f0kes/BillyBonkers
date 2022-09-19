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

		private static ushort NextId { get; set; }
		
		public static void TryGetValue(ushort id, out T value)
		{
			Table.TryGetValue(id, out value);
		}

		public static T GetValue(ushort id)
		{
			return Table[id];
		}
		
		public static void Add(T value)
		{
			if (Table.ContainsValue(value))
				return;

			Table.Add(NextId, value);
			ReverseTable.Add(value, NextId);

			NextId++;
		}

		public static void Remove(T value)
		{
			if (!Table.ContainsValue(value))
				return;

			Table.Remove(ReverseTable[value]);
			ReverseTable.Remove(value);
		}

		public static ushort GetId(T value)
		{
			return ReverseTable[value];
		}

		public static void Clear()
		{
			Table.Clear();
			ReverseTable.Clear();
			NextId = 0;
		}

		public static List<T> GetValues()
		{
			return Table.Values.ToList();
		}
	}
}