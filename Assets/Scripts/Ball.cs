using System;
using System.Collections.Generic;
using DefaultNamespace;
using DefaultNamespace.Enums;
using Structures;
using Structures.Structures;
using UnityEngine;

public class Ball : MonoBehaviour
{
	public float Health {  get; private set; }

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

		Health -= strike.HitVector.magnitude;
		if (Health <= 0)
		{
			Destroy(gameObject);
		}
	}

	private void DealDamage(Strike strike)
	{
	}

	protected virtual void OnCollisionEnter(Collision collision)
	{
		Ball other = collision.gameObject.GetComponentInParent<Ball>();
		if (other != null)
		{
			Vector3 collSpeed = collision.impulse / Time.fixedDeltaTime;
			Strike strike = new Strike() {Striker = this, Victim = other, HitVector = collSpeed};
			other.Hit(strike,false);
		}
	}
}