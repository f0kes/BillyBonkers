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
		private bool _roundStarted = false;

		public void AddWinCondition(WinCondition condition)
		{
			_conditions.Add(condition);
		}

		//to change
		private void Awake()
		{
			AddWinCondition(new OneSurvivorWinCondition());
			AddWinCondition(new ScoreWinCondition(10));
			TimeTicker.OnTick += OnTick;
		}

		private void OnDisable()
		{
			TimeTicker.OnTick -= OnTick;
		}

		private void Start()
		{
			//_roundStarted = true;
		}

		private void OnTick(TimeTicker.OnTickEventArgs args)
		{
			if (_roundFinished || !_roundStarted) return;
			foreach (var condition in _conditions)
			{
				RoundFinishMessage message;
				if (condition.IsSatisfied(out message))
				{
					InitializeRoundFinish(message);
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