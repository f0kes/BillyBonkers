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
		public bool DamageOwner = false;
		public List<NPBallEffect.Effect> effects = new List<NPBallEffect.Effect>();
		public Ball Owner { get; private set; }

		public override void CollisionFromChild(Collision collision)
		{
			Ball other = collision.gameObject.GetComponentInParent<Ball>();
			if (other != null && (other != Owner || DamageOwner))
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
			if (strike.Source != StrikeSource.Collision)
			{
				ChangeOwner(strike.Striker);
			}
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
				OnChangeOwner?.Invoke(Owner);
			}
		}

		

		protected override void Die()
		{
			base.Die();
		}
	}
}