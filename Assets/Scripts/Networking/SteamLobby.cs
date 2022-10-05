using System;
using Mirror;
using Steamworks;
using UnityEngine;

namespace Networking
{
	public class SteamLobby : MonoBehaviour
	{
		private Callback<LobbyCreated_t> _lobbyCreated;
		private Callback<GameLobbyJoinRequested_t> _gameLobbyJoinRequested;
		private Callback<LobbyEnter_t> _lobbyEnter;

		private NetworkManager _networkManager;
		private ulong _lobbyId;
		private bool _isInLobby;
		private bool _isHost;

		private void Start()
		{
			_networkManager = GetComponent<NetworkManager>();
			if (!SteamManager.Initialized) return;
			_lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
			_gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
			_lobbyEnter = Callback<LobbyEnter_t>.Create(OnLobbyEnter);
			HostLobby();
		}

		private void OnLobbyEnter(LobbyEnter_t param)
		{
			if (NetworkServer.active || NetworkClient.isConnected) return;
			var hostAddress = SteamMatchmaking.GetLobbyData(new CSteamID(param.m_ulSteamIDLobby), "HostAddress");
			_lobbyId = param.m_ulSteamIDLobby;
			_networkManager.networkAddress = hostAddress;
			_networkManager.StartClient();
			_isInLobby = true;
			_isHost = false;
		}

		private void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t param)
		{
			if (_isInLobby)
			{
				LeaveLobby();
			}
			SteamMatchmaking.JoinLobby(param.m_steamIDLobby);
		}

		private void OnLobbyCreated(LobbyCreated_t param)
		{
			if (param.m_eResult != EResult.k_EResultOK) return;
			_networkManager.StartHost();
			SteamMatchmaking.SetLobbyData(new CSteamID(param.m_ulSteamIDLobby), "HostAddress",
				SteamUser.GetSteamID().ToString());
			_lobbyId = param.m_ulSteamIDLobby;
			_isInLobby = true;
			_isHost = true;
		}

		public void HostLobby()
		{
			if (_isInLobby)
			{
				LeaveLobby();
			}

			SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, _networkManager.maxConnections);
		}

		public void LeaveLobby()
		{
			SteamMatchmaking.LeaveLobby(new CSteamID(_lobbyId));
			if (_isHost)
			{
				_networkManager.StopHost();
			}
			else
			{
				_networkManager.StopClient();
			}
		}
	}
}