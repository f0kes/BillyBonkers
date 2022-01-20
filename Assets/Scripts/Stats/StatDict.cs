using System.Collections.Generic;
using DefaultNamespace.Enums;
using Structures.Structures;

namespace Structures
{
	public class StatDict<T>
	{
		private Dictionary<T, Stat> _stats = new Dictionary<T, Stat>();
		public float this[T name] => _stats[name].GetValue();

		public Stat GetStat(T name)
		{
			return _stats[name];
		}

		public void SetStat(T name, Stat stat)
		{
			_stats[name] = stat;
		}
	}
}