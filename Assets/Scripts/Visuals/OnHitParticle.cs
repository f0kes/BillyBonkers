using System;
using System.Collections.Generic;
using DefaultNamespace;
using Entities;
using Enums;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Particles
{
	[RequireComponent(typeof(ParticleSystem))]
	public class OnHitParticle : MonoBehaviour
	{
		[SerializeField] private AudioSource _audioSource;
		[SerializeField] private List<AudioClip> _audioClipsBall = new List<AudioClip>();
		[SerializeField] private List<AudioClip> _audioClipsCue = new List<AudioClip>();
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
			_ball.OnHit += OnHit;
			//_ball.OnCollision += LaunchParticlesCollision;
			_initialCurve = _particleSystem.main.startSize;
		}

		

		private void OnHit(Strike strike)
		{
			LaunchParticles(strike);
			MakeSound(strike);
		}

		private void MakeSound(Strike strike)
		{
			AudioClip toPlay = GetStrikeSound(strike);
			float soundVolume = Sigmoid(strike.OverallDamage/30, xzero: 1);
			_audioSource.PlayOneShot(toPlay, soundVolume);
			
		}

		private AudioClip GetStrikeSound(Strike strike)
		{
			AudioClip strikeSound;
			int numSound;
			switch (strike.Source)
			{
				case StrikeSource.Ball:
					numSound = Random.Range(0, _audioClipsBall.Count);
					strikeSound = _audioClipsBall[numSound];
					break;
				case StrikeSource.Cue:
					numSound = Random.Range(0, _audioClipsCue.Count);
					strikeSound = _audioClipsCue[numSound];
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			return strikeSound;
		}

		private void LaunchParticles(Strike strike)
		{
			var main = _particleSystem.main;
			float mult = strike.OverallDamage / 60;
			mult = Mathf.Min(mult, 1);
			main.startSize=new ParticleSystem.MinMaxCurve(_initialCurve.constantMin*mult,_initialCurve.constantMax*mult);
			_particleSystem.Play();
		}
		public static float Sigmoid(float value, float L=1f, float xzero=0, float k=1f)
		{
			return L / (1.0f + (float) Math.Exp(-k * (value - xzero)));
		}
	}
}