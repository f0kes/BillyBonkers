using Entities;
using Enums;
using GameState;
using Structures;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

namespace NPBallEffect
{
	[CreateAssetMenu(fileName = "New Increase Damage Effect", menuName = "Damage Effect", order = 2)]
	public class IncreasedHitDamage : Effect
	{
		[SerializeField] private int bonusDamage;
		[SerializeField] private int multiplyDamage = 1;
		private bool _damageIncreased = false;

		public override void Apply(NpBall ball)
		{
			ball.OnChangeOwner += (Ball o) => { Score(ball); };
		}

		private void Score(NpBall ball)
		{
			if (!_damageIncreased)
			{
				ball.Stats.GetStat(BallStat.CollisionDamageMultiplier).AddMod(new StatModifierAdd(bonusDamage, 1));
				ball.Stats.GetStat(BallStat.CollisionDamageMultiplier).AddMod(new StatModifierMultiply(multiplyDamage, 1));
				_damageIncreased = true;
			}
		}
	}
}