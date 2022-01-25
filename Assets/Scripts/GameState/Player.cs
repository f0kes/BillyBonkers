using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameState
{
	public class Player : MonoBehaviour
	{
		[SerializeField] public PlayerInputHandler inputHandler;
		[SerializeField] public Skin Skin;
		public static List<Player> Players = new List<Player>();
		private static int _playerCount = 0;

		public Ball PlayerBall { get; private set; }
		public int PlayerId { get; private set; }
		public int PlayerWins { get; private set; } = 0;

		public bool RoundStarted { get; private set; } = false;
		public bool Dead { get; private set; } = false;
		
		


		void OnEnable()
		{
			SceneManager.sceneUnloaded += OnSceneUnloaded;
		}
		

		private void Awake()
		{
			Players.Add(this);
			_playerCount += 1;
			PlayerId = _playerCount;
			DontDestroyOnLoad(gameObject);
		}

		public void SetBall(Ball playerBall)
		{
			PlayerBall = playerBall;
			PlayerBall.OnDeathEnd += SetDead;
			PlayerBall.SetSkin(Skin);
			RoundStarted = true;
		}

		public void ChangeSkin(Skin skin)
		{
			Skin = skin;
		}

		public void AddWin()
		{
			PlayerWins += 1;
		}

		void OnSceneUnloaded(Scene scene)
		{
			if (PlayerBall != null)
			{
				PlayerBall.OnDeathEnd -= SetDead;
			}

			inputHandler.OnShootPressed = null;
			inputHandler.OnMoveStarted = null;
			PlayerBall = null;
			RoundStarted = false;
			Dead = false;
		}

		private void SetDead()
		{
			Dead = true;
		}
	}
}