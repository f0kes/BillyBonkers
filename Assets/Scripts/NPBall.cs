using Enums;
using GameState;
using Structures;
using UnityEngine;

namespace DefaultNamespace
{
	public class NpBall: Ball
	{
		public Ball Owner { get; private set; }
		public override void CollisionFromChild(Collision collision)
		{
			Ball other = collision.gameObject.GetComponentInParent<Ball>();
			if (other != null && other!=Owner)
			{
				base.CollisionFromChild(collision);
			}
		}

		public override void Hit(Strike strike, bool addForce = true)
		{
			base.Hit(strike, addForce);
			if (strike.Striker.gameObject.CompareTag("Player"))
			{
				Owner = strike.Striker;
				Debug.Log(Owner + " now is allowed to fuck me");
				Stats.GetStat(BallStat.CollisionDamageMultiplier).AddMod(new StatModifierMultiply(5,1));
			}
		}
	}
}