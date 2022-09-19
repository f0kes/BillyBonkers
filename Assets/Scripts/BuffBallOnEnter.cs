using System;
using System.Collections;
using System.Collections.Generic;
using Entities;
using Enums;
using Structures;
using Unity.Mathematics;
using UnityEngine;

public class BuffBallOnEnter : MonoBehaviour
{
	[Serializable]
	public struct StatModifierBall
	{
		public BallStat ToBuff;
		public float Value;
	}

	[SerializeField] private GameObject iconOnBuff;
	
	[SerializeField] private List<StatModifierBall> _modifiers = new List<StatModifierBall>();
	[SerializeField] private float timeToBuff;
	private float _currentTime;

	private List<Ball> _ballsToBuff = new List<Ball>();

	private void Update()
	{
		_currentTime += Time.deltaTime;
		if (_currentTime >= timeToBuff)
		{
			foreach (var kv in _modifiers)
			{
				foreach (var ball in _ballsToBuff)
				{
					StatModifierAdd mod = new StatModifierAdd(kv.Value, 1);
					ball.Stats.GetStat(kv.ToBuff).AddMod(mod);
					SpawnIcon(iconOnBuff, ball);
				}
			}

			_currentTime = 0;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		Ball ball = other.gameObject.GetComponentInParent<Ball>();
		if (ball != null)
		{
			_ballsToBuff.Add(ball);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		Ball ball = other.gameObject.GetComponentInParent<Ball>();
		if (ball != null && _ballsToBuff.Contains(ball))
		{
			_ballsToBuff.Remove(ball);
		}
	}
	private static void SpawnIcon(GameObject icon, Ball ball)
	{
		Vector3 pos = ball.GetPosition();
		pos = new Vector3(pos.x, pos.y + 5, pos.z);
		Instantiate(icon, pos, quaternion.identity);
	}
}