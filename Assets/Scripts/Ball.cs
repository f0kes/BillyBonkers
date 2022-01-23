using System;
using System.Collections.Generic;
using DefaultNamespace;
using Enums;
using Structures;
using Structures.Structures;
using UnityEngine;

public class Ball : MonoBehaviour
{
	public Action OnDeath;
	public float Health { get; private set; }

	[SerializeField] private List<EnumeratedStat> enumeratedStats;
	private Rigidbody _rb;
	public StatDict<BallStat> Stats = new StatDict<BallStat>();

	[Serializable]
	public struct EnumeratedStat
	{
		public BallStat Name;
		public Stat Value;
	}

	private void Awake()
	{
		foreach (var stat in enumeratedStats)
		{
			Stats.SetStat(stat.Name, stat.Value);
		}

		Health = Stats[BallStat.Health];
		_rb = GetComponentInChildren<Rigidbody>();
	}

	public virtual void Hit(Strike strike, bool addForce = true)
	{
		if (addForce)
		{
			_rb.AddForce(strike.HitVector, ForceMode.VelocityChange);
		}

		Health -= strike.HitVector.magnitude * strike.DamageMultiplier;
		if (Health <= 0)
		{
			Die();
		}
	}

	private void Die()
	{
		OnDeath?.Invoke();
		Destroy(gameObject);
	}

	public virtual void CollisionFromChild(Collision collision)
	{
		Ball other = collision.gameObject.GetComponentInParent<Ball>();
		if (other != null)
		{
			Vector3 collSpeed = collision.relativeVelocity;
			Strike strike = new Strike()
			{
				Striker = this, Victim = other, HitVector = collSpeed,
				DamageMultiplier = Stats[BallStat.CollisionDamageMultiplier]
			};
			other.Hit(strike, false);
		}
	}
}