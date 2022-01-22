using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameState
{
	public class Player : MonoBehaviour
	{
		[SerializeField] public PlayerInputHandler inputHandler;
		public static List<Player> Players = new List<Player>();
		private static int _playerCount = 0;

		public Ball PlayerBall { get; private set; }
		public int PlayerId { get; private set; }
		public int PlayerWins { get; private set; } = 0;


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
		}

		void OnSceneUnloaded(Scene scene)
		{
			PlayerBall = null;
		}
	}
}