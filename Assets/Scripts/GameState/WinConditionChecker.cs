using System;
using System.Collections.Generic;
using Mirror;
using UI;
using UnityEngine;

namespace GameState
{
	public class WinConditionChecker : NetworkBehaviour
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
			GameInitializer.OnGameStart += OnGameStart;
		}

		private void OnDisable()
		{
			TimeTicker.OnTick -= OnTick;
		}

		private void OnGameStart()
		{
			if (isServer)
			{
				_roundStarted = true;
				Debug.Log("Round started");
			}
			TimeTicker.OnTick += OnTick;
			GameInitializer.OnGameStart -= OnGameStart;
		}

		private void OnTick(TimeTicker.OnTickEventArgs args)
		{
			if (_roundFinished || !_roundStarted)
			{
				return;
			}
			foreach (var condition in _conditions)
			{
				RoundFinishMessage message;
				if (condition.IsSatisfied(out message))
				{
					InitializeRoundFinish(message);
				}
			}
		}

		[ClientRpc]
		private void InitializeRoundFinish(RoundFinishMessage victoryMessage)
		{
			_roundFinished = true;
			if (victoryMessage.Tie == false)
			{
				Debug.Log("Not a tie");
				victoryMessage.Winner.AddWin();
			}

			RoundEndScreen endScreen = Instantiate(_endScreenToSpawn);
			endScreen.ShowRoundResults(victoryMessage);
		}
	}
}