using System;
using System.Collections.Generic;
using DefaultNamespace;
using Enums;
using NPBallEffect;
using Structures;
using UnityEngine;


namespace Entities
{
	public class NpBall : Ball
	{
		public Action<Ball> OnChangeOwner;
		public List<Effect> effects = new List<Effect>();
		public Ball Owner { get; private set; }
		private bool _damageIncreased = false;

		public override void CollisionFromChild(Collision collision)
		{
			Ball other = collision.gameObject.GetComponentInParent<Ball>();
			if (other != null && other != Owner)
			{
				base.CollisionFromChild(collision);
				ChangeOwner(other);
			}
		}

		protected override void Awake()
		{
			base.Awake();
			foreach (var effect in effects)
			{
				effect.Apply(this);
			}
		}

		public override void ApplyDamage(Strike strike, bool addForce = true)
		{
			base.ApplyDamage(strike, addForce);
			ChangeOwner(strike.Striker);
		}

		private void ChangeOwner(Ball owner)
		{
			Ball newOwner;
			if (owner is NpBall npBall)
			{
				newOwner = npBall.Owner;
			}
			else
			{
				newOwner = owner;
			}

			if (newOwner != Owner && newOwner!=null)
			{
				Owner = newOwner;
				OnChangeOwner?.Invoke(owner);
				if (!_damageIncreased)
				{
					Stats.GetStat(BallStat.CollisionDamageMultiplier).AddMod(new StatModifierMultiply(5, 1));
					_damageIncreased = true;
				}
			}
		}

		protected override void Die()
		{
			if (Owner != null)
			{
				Owner.ScoreBall();
			}
			base.Die();
		}
	}
}