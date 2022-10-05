using System;
using Mirror;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using Visuals;

namespace GameState
{
	public class LobbyPlayer : NetworkBehaviour
	{
		public Player Player { get; private set; }
		public bool IsReady { get; private set; }

		[SyncVar(hook = nameof(ChangeSkin))] private int _skinIndex;

		private void Awake()
		{
			Player = GetComponent<Player>();
			SceneManager.sceneLoaded += OnSceneLoaded;
		}

		public void Start()
		{
			Player = GetComponent<Player>();
			Lobby.I.AddPlayer(this);
			ChangeSkin(0);
			
			if (!isServer) return;
			Player.inputHandler.OnShootPressed += ToggleReady;
			Player.inputHandler.OnMoveStarted += ChangeSkin;
		}
		private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
		{
			if (scene.name == "Lobby")
			{
				IsReady = false;
				Lobby.I.AddPlayer(this);
				
				if (!isServer) return;
				Player.inputHandler.OnShootPressed += ToggleReady;
				Player.inputHandler.OnMoveStarted += ChangeSkin;
			}
		}

		private void OnDestroy()
		{
			Lobby.I.RemovePlayer(this);
			if (!isServer) return;
			Player.inputHandler.OnShootPressed -= ToggleReady;
			Player.inputHandler.OnMoveStarted -= ChangeSkin;
			SceneManager.sceneLoaded -= OnSceneLoaded;
		}

		[ClientRpc]
		private void ToggleReady()
		{
			Debug.Log("ToggleReady");
			IsReady = !IsReady;
		}
		public void SetReady(bool ready)
		{
			IsReady = ready;
		}

		private void ChangeSkin(Vector2 dir)
		{
			if (!IsReady)
			{
				var currentIndex = SkinList.I.Skins.FindIndex(skin => skin == Player.Skin);
				ChangeSkin(currentIndex + (int) Mathf.Sign(dir.x));
			}
		}

		private void ChangeSkin(int oldIndex, int index)
		{
			ChangeSkin(index);
		}

		private void ChangeSkin(int index)
		{
			if (index < 0) index += SkinList.I.Skins.Count;
			index %= SkinList.I.Skins.Count;
			_skinIndex = index;
			Player.ChangeSkin(index);
		}
	}
}