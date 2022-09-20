using System;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using Steamworks;
using UnityEngine;

namespace GameState
{
	public class GameInitializer : MonoBehaviour
	{
		private void Awake()
		{
			Init();
		}

		public void Init()
		{
			var balls = FindObjectsOfType<BallMovement>().ToList().Where((x)=>x.IsPlayer);
			var cinemaChineTargetGroup = FindObjectOfType<CinemachineTargetGroup>();
			List<Player> unusedPlayers = new List<Player>(Player.Players);
			foreach (var mover in balls)
			{
				InitBall(unusedPlayers, mover, cinemaChineTargetGroup);
			}
		}

		private static void InitBall(List<Player> unusedPlayers, BallMovement mover, CinemachineTargetGroup cinemaChineTargetGroup)
		{
			if (unusedPlayers.Count > 0)
			{
				mover.SetInputHandler(unusedPlayers[0].inputHandler);
				cinemaChineTargetGroup.AddMember(mover.transform, 1, 4);
				unusedPlayers[0].SetBall(mover.Ball);
				unusedPlayers.Remove(unusedPlayers[0]);
			}
			else
			{
				Destroy(mover.transform.root.gameObject);
			}
		}
	}
}