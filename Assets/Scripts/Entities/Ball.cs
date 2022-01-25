using System;
using System.Collections.Generic;
using DefaultNamespace;
using Enums;
using GameState;
using Structures;
using Structures.Structures;
using UnityEngine;
using UnityEngine.Serialization;

public class Ball : MonoBehaviour
{
	public Action OnDeath;
	public Action OnDeathEnd;
	public Action<Strike> OnHit;
	public Action<Collision> OnCollision;
	public Action<Skin> OnSetSkin;

	public StatDict<BallStat> Stats = new StatDict<BallStat>();
	public float Health { get; private set; }
	[SerializeField] private List<EnumeratedStat> enumeratedStats;

	private Rigidbody _rb;
	private GameObject _floor;

	public Skin Skin { get; private set; }

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

	public void SetSkin(Skin skin)
	{
		this.Skin = skin;
		OnSetSkin?.Invoke(skin);
	}


	public virtual void Hit(Strike strike, bool addForce = true)
	{
		OnHit?.Invoke(strike);
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
		Destroy(gameObject, 1f);
		TimeTicker.I.InvokeInTime((() => { OnDeathEnd?.Invoke(); }), 1f);
	}

	public virtual void CollisionFromChild(Collision collision)
	{
		OnCollision?.Invoke(collision);
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

	public virtual void KillTrigger(KillTrigger killTrigger)
	{
		Die();
	}
}