using Enums;
using UnityEngine;

namespace DefaultNamespace
{
	public class NpBall:Ball
	{
		private Ball _owner;
		public override void Hit(Strike strike, bool addForce = true)
		{
			_owner = strike.Striker;
			base.Hit(strike, addForce);
		}

		protected override void OnCollisionEnter(Collision collision)
		{
			if (_owner == null)
			{
				base.OnCollisionEnter(collision);
			}
			else
			{
				Ball other = collision.gameObject.GetComponentInParent<Ball>();
				if (other != null)
				{
					Vector3 collSpeed = collision.impulse / Time.fixedDeltaTime * Stats[BallStat.HitPower];
					Strike strike = new Strike() {Striker = _owner, Victim = other, HitVector = collSpeed};
					other.Hit(strike,false);
				}
			}
		}
	}
}