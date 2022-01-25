using System;
using DefaultNamespace;
using UnityEngine;

namespace Particles
{
	[RequireComponent(typeof(ParticleSystem))]
	public class OnHitParticle : MonoBehaviour
	{
		private ParticleSystem _particleSystem;
		private Ball _ball;
		private ParticleSystem.MinMaxCurve _initialCurve;
		private void Awake()
		{
			_particleSystem = gameObject.GetComponent<ParticleSystem>();
		}

		private void Start()
		{
			_ball = gameObject.GetComponentInParent<Ball>();
			_ball.OnHit += LaunchParticles;
			//_ball.OnCollision += LaunchParticlesCollision;
			_initialCurve = _particleSystem.main.startSize;
		}

		

		private void LaunchParticles(Strike strike)
		{
			var main = _particleSystem.main;
			float mult = strike.OverallDamage / 60;
			mult = Mathf.Min(mult, 1);
			main.startSize=new ParticleSystem.MinMaxCurve(_initialCurve.constantMin*mult,_initialCurve.constantMax*mult);
			_particleSystem.Play();
		}
	}
}