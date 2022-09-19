using System;
using System.Collections.Generic;
using System.Linq;
using Entities;
using Interfaces;
using UnityEngine;

namespace Bots
{
	public class SimpleBotBrain : MonoBehaviour, IInputHandler
	{
		private List<Ball> _playerBalls = new List<Ball>();
		private PlayerInp _frameInp;

		private void Start()
		{
			var movement = GetComponent<BallMovement>();
			movement.SetInputHandler(this);
			foreach (var mover in FindObjectsOfType<BallMovement>().ToList().Where((x) => x.IsPlayer))
			{
				_playerBalls.Add(mover.Ball);
			}
			
		}

		private Ball GetClosestPlayer()
		{
			var closestBall =
				_playerBalls.OrderBy((ball) => Vector3.Distance(ball.GetPosition(), transform.position)).First();
			return closestBall;
		}


		public PlayerInp GetFrameInput()
		{
			Vector3 moveDir = (GetClosestPlayer().GetPosition() - transform.position).normalized;
			_frameInp = new PlayerInp() {XMove = moveDir.x, ZMove = moveDir.z, Shoot = false};
			return _frameInp;
		}
	}
}