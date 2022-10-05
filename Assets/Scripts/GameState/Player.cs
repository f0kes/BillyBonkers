using System;
using System.Collections.Generic;
using Entities;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;
using Visuals;

namespace GameState
{
	public class Player : NetworkBehaviour
	{
		[SerializeField] public PlayerInputHandler inputHandler;
		[SerializeField] public Skin Skin;
		
		public static List<Player> Players = new List<Player>();
		private static ushort _playerCount = 0;

		public Ball PlayerBall { get; private set; }
		public ushort PlayerId { get; private set; }
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
		private void Start()
		{
			if(!isServer) return;
			SetId(PlayerId);
		}
		
		[ClientRpc]
		private void SetId(ushort id)
		{
			PlayerId = id;
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
		public void ChangeSkin(int skinId)
		{
			Skin = SkinList.I.GetSkin(skinId);
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

		private void OnDestroy()
		{
			Players.Remove(this);
			_playerCount -= 1;
		}

		private void SetDead()
		{
			Dead = true;
		}
		public static explicit operator ushort(Player player)
		{
			return player.PlayerId;
		}
		public static explicit operator Player(ushort id)
		{
			return Players.Find(x => x.PlayerId == id);
		}
	}
}