using System;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
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
			var balls = FindObjectsOfType<PlayerMovement>().ToList();
			var cinemaChineTargetGroup = FindObjectOfType<CinemachineTargetGroup>();
			List<Player> unusedPlayers = new List<Player>(Player.Players);
			foreach (var mover in balls)
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
}