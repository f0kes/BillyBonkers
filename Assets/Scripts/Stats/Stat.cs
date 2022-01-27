using System.Linq;
using GameState;

namespace Structures
{
  
	using System.Collections.Generic;
	using UnityEngine;

	namespace Structures
	{
		[System.Serializable]
		public class Stat
		{
			[SerializeField]
			private float baseValue;

			public float BaseValue => baseValue;

			private float _lastValue;
			private List<StatModifier> _modifiers;

			public List<StatModifier> Modifiers => _modifiers;

			public Stat()
			{
				_modifiers = new List<StatModifier>();
			}

			public Stat(float baseValue)
			{
				_modifiers = new List<StatModifier>();
				this.baseValue = baseValue;
			}
			public float GetValue()
			{
				float result = baseValue;
				foreach (var mod in _modifiers )
				{
					mod.ApplyMod(ref result, baseValue);
				}

				_lastValue = result;
				return result;
			}

			public void SetBaseValue(float val)
			{
				baseValue = val;
			}

			public float GetLastValue()
			{
				return _lastValue;
			}

			public void AddTemporalMod(StatModifier mod, float time)
			{
				AddMod(mod);
				TimeTicker.I.InvokeInTime(()=>{RemoveMod(mod);}, time);
			}
			
			public void AddMod(StatModifier mod)
			{
				_modifiers.Add(mod);
				_modifiers = _modifiers.OrderBy(m => m.priority).ToList();

			}

			public void RemoveMod(StatModifier mod)
			{
				if (_modifiers.Contains(mod))
				{
					_modifiers.Remove(mod);
					_modifiers = _modifiers.OrderBy(m => m.priority).ToList();
				}
			}
		}
	}
}