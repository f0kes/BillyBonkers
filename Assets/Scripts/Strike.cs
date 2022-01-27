using Entities;
using Enums;
using UnityEngine;

namespace DefaultNamespace
{
	public struct Strike
	{
		public Ball Striker { get; }

		public Ball Victim{ get; }
		public Vector3 HitVector{ get; }
		public float DamageMultiplier { get; set; }
		public StrikeSource Source{ get; }

		public Strike(Ball striker, Ball victim, Vector3 hitVector, float damageMultiplier, StrikeSource source)
		{
			Striker = striker;
			Victim = victim;
			HitVector = hitVector;
			DamageMultiplier = damageMultiplier;
			Source = source;
		}

		public float OverallDamage => DamageMultiplier * HitVector.magnitude;
	}
}