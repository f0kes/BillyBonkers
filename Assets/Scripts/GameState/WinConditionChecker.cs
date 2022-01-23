using System;
using System.Collections.Generic;
using UI;
using UnityEngine;

namespace GameState
{
	public class WinConditionChecker : MonoBehaviour
	{
		private List<WinCondition> _conditions = new List<WinCondition>();
		[SerializeField] private RoundEndScreen _endScreenToSpawn;
		private bool _roundFinished = false;

		public void AddWinCondition(WinCondition condition)
		{
			_conditions.Add(condition);
		}

		//tochange
		private void Awake()
		{
			AddWinCondition(new OneSurvivorWinCondition());
		}

		private void Update()
		{
			if (!_roundFinished)
			{
				foreach (var con in _conditions)
				{
					RoundFinishMessage message;
					if (con.Check(out message))
					{
						InitializeRoundFinish(message);
					}
				}
			}
		}

		private void InitializeRoundFinish(RoundFinishMessage victoryMessage)
		{
			_roundFinished = true;
			if (victoryMessage.Tie == false)
			{
				victoryMessage.Winner.AddWin();
			}

			RoundEndScreen endScreen = Instantiate(_endScreenToSpawn);
			endScreen.ShowRoundResults(victoryMessage);
		}
	}
}