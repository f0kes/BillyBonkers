using System;
using Mirror;
using UI;
using UnityEngine;
using Visuals;

namespace GameState
{
	public class LobbyPlayer : NetworkBehaviour
	{
		public Player Player { get; private set; }
		public bool IsReady { get; private set; }
		
		[SyncVar(hook = nameof(ChangeSkin))]
		private int _skinIndex;

		private void Awake()
		{
			Player = GetComponent<Player>();
		}

		private void Start()
		{
			Lobby.I.AddPlayer(this);
			ChangeSkin(0);
			if(!isServer) return;
			Player.inputHandler.OnShootPressed += ToggleReady;
			Player.inputHandler.OnMoveStarted += ChangeSkin;
		}

		[ClientRpc]
		private void ToggleReady()
		{
			IsReady = !IsReady;
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
		private void ChangeSkin( int index)
		{
			if (index < 0) index += SkinList.I.Skins.Count;
			index %= SkinList.I.Skins.Count;
			_skinIndex = index;
			Player.ChangeSkin(index);
		}
	}
}