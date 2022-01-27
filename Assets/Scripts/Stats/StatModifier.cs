using System;
using System.Collections.Generic;
using Enums;

namespace Structures
{
	public class StatModifier
	{
		public float Value { get; set; }

		public string Name => GetType().ToString();
		public float priority;

		public StatModifier(float value, float priority)
		{
			this.Value = value;
			this.priority = priority;
		}

		public virtual void ApplyMod(ref float finalStat, float baseValue)
		{
			//	finalStat = finalStat;
		}
	}

	public class StatModifierAdd : StatModifier
	{
		public StatModifierAdd(float value, float priority) : base(value, priority)
		{
		}

		public override void ApplyMod(ref float finalStat, float baseValue)
		{
			finalStat += Value;
		}
	}

	public class StatModifierMultiply : StatModifier
	{
		public StatModifierMultiply(float value, float priority) : base(value, priority)
		{
		}

		public override void ApplyMod(ref float finalStat, float baseValue)
		{
			finalStat *= Value;
		}
	}

	public class StatMultiplyBase : StatModifier
	{
		public StatMultiplyBase(float value, float priority) : base(value, priority)
		{
		}

		public override void ApplyMod(ref float finalStat, float baseValue)
		{
			finalStat += (baseValue * Value) - baseValue;
		}
	}

	public class StatMultiplyBasePositive : StatModifier
	{
		public StatMultiplyBasePositive(float value, float priority) : base(value, priority)
		{
		}

		public override void ApplyMod(ref float finalStat, float baseValue)
		{
			finalStat += baseValue * (1 + Value) - baseValue;
		}
	}
}