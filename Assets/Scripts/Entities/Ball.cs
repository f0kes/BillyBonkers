using System;
using System.Collections.Generic;
using DefaultNamespace;
using Enums;
using GameState;
using Mirror;
using Networking;
using Structures;
using Structures.Structures;
using UnityEngine;
using Visuals;

namespace Entities
{
	public class Ball : NetworkEntity
	{
		#region Events

		public Action OnDeath;
		public Action OnDeathEnd;
		public Action<Strike> OnHit;
		public Action<Collision> OnCollision;
		public Action<Skin> OnSetSkin;
		public Action<float> OnOutlineIntensityChange;

		#endregion
		
		public StatDict<BallStat> Stats = new StatDict<BallStat>();
		public float Health { get; private set; }
		public int ScoredBalls { get; private set; }
		[SerializeField] private List<EnumeratedStat> enumeratedStats;

		private Rigidbody _rb;
		private GameObject _floor;

		private bool _dead = false;

		public float OutlineIntensity { get; private set; }

		public Skin Skin { get; private set; }

		[Serializable]
		public struct EnumeratedStat
		{
			public BallStat Name;
			public Stat Value;
		}

		protected override void Awake()
		{
			base.Awake();
			foreach (var stat in enumeratedStats)
			{
				Stats.SetStat(stat.Name, stat.Value);
			}

			Health = Stats[BallStat.Health];
			_rb = GetComponentInChildren<Rigidbody>();
		}

		public override Message Serialize()
		{
			var message = new Message();
			message.AddFloat(_rb.position.x);
			message.AddFloat(_rb.position.y);
			message.AddFloat(_rb.position.z);
			message.AddFloat(_rb.rotation.x);
			message.AddFloat(_rb.rotation.y);
			message.AddFloat(_rb.rotation.z);
			message.AddFloat(_rb.rotation.w);
			message.AddFloat(_rb.velocity.x);
			message.AddFloat(_rb.velocity.y);
			message.AddFloat(_rb.velocity.z);
			message.AddFloat(_rb.angularVelocity.x);
			message.AddFloat(_rb.angularVelocity.y);
			message.AddFloat(_rb.angularVelocity.z);
			message.AddFloat(Health);
			return message;
		}

		public override void Deserialize(Message message)
		{
			var position = new Vector3(message.GetFloat(), message.GetFloat(), message.GetFloat());
			var rotation = new Quaternion(message.GetFloat(), message.GetFloat(), message.GetFloat(),
				message.GetFloat());
			var velocity = new Vector3(message.GetFloat(), message.GetFloat(), message.GetFloat());
			var angularVelocity = new Vector3(message.GetFloat(), message.GetFloat(), message.GetFloat());
			Health = message.GetFloat();
			_rb.position = position;
			_rb.rotation = rotation;
			_rb.velocity = velocity;
			_rb.angularVelocity = angularVelocity;
		}

		public void SetSkin(Skin skin)
		{
			this.Skin = skin;
			OnSetSkin?.Invoke(skin);
		}

		public void SetOutlineIntensity(float intensity)
		{
			OutlineIntensity = intensity;
			OnOutlineIntensityChange?.Invoke(intensity);
		}

		public void AddScore(int score)
		{
			ScoredBalls += score;
		}

		public Vector3 GetPosition()
		{
			return _rb != null ? _rb.position : Vector3.zero;
		}

		public virtual void ApplyDamage(Strike strike, bool addForce = true)
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
		protected virtual void Die()
		{
			if (!_dead)
			{
				OnDeath?.Invoke();
				_dead = true;
				
				TimeTicker.I.InvokeInTime((() => { OnDeathEnd?.Invoke(); }), 1f);
				TimeTicker.I.InvokeInTime((() => { NetworkServer.Destroy(gameObject); }), 1f);
			}
		}

		public virtual void CollisionFromChild(Collision collision)
		{
			OnCollision?.Invoke(collision);
			Ball other = collision.gameObject.GetComponentInParent<Ball>();
			if (other != null)
			{
				Vector3 collSpeed = collision.relativeVelocity;
				Strike strike = new Strike(this, other, collSpeed, Stats[BallStat.CollisionDamageMultiplier],
					StrikeSource.Collision);
				other.ApplyDamage(strike, false);
			}
		}

		public virtual void KillTrigger(KillTrigger killTrigger)
		{
			Die();
		}
	}
}