using System;
using Enums;
using GameState;
using Structures;
using UnityEngine;

namespace DefaultNamespace
{
	public class NpBall: Ball
	{
		public Action<Ball> OnChangeOwner;
		
		public Ball Owner { get; private set; }
		private bool _damageIncreased = false;
		public override void CollisionFromChild(Collision collision)
		{
			Ball other = collision.gameObject.GetComponentInParent<Ball>();
			if (other != null && other!=Owner)
			{
				base.CollisionFromChild(collision);
				ChangeOwner(other);
			}
		}

		public override void Hit(Strike strike, bool addForce = true)
		{
			base.Hit(strike, addForce);
			if (strike.Striker.gameObject.CompareTag("Player") && (Owner==null))
			{
				ChangeOwner(strike.Striker);	
			}
		}

		public override void KillTrigger(KillTrigger killTrigger)
		{
			if (Owner != null)
			{
				
			}
			base.KillTrigger(killTrigger);
		}

		private void ChangeOwner(Ball owner)
		{
			Owner = owner;
			OnChangeOwner?.Invoke(owner);
			if (!_damageIncreased)
			{
				Stats.GetStat(BallStat.CollisionDamageMultiplier).AddMod(new StatModifierMultiply(5, 1));
				_damageIncreased = true;
			}
			
		}
	}
}