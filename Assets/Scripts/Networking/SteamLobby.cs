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

		private void Start()
		{
			_networkManager = GetComponent<NetworkManager>();
			if (!SteamManager.Initialized) return;
			_lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
			_gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
			_lobbyEnter = Callback<LobbyEnter_t>.Create(OnLobbyEnter);
		}

		private void OnLobbyEnter(LobbyEnter_t param)
		{
			if(NetworkServer.active || NetworkClient.isConnected) return;
			var hostAddress = SteamMatchmaking.GetLobbyData(new CSteamID(param.m_ulSteamIDLobby), "HostAddress");
			_networkManager.networkAddress = hostAddress;
			_networkManager.StartClient();
			
		}

		private void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t param)
		{
			SteamMatchmaking.JoinLobby(param.m_steamIDLobby);
		}

		private void OnLobbyCreated(LobbyCreated_t param)
		{
			if (param.m_eResult != EResult.k_EResultOK) return;
			_networkManager.StartHost();
			SteamMatchmaking.SetLobbyData(new CSteamID(param.m_ulSteamIDLobby), "HostAddress",
				SteamUser.GetSteamID().ToString());
		}

		public void HostLobby()
		{
			SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, _networkManager.maxConnections);
		}
	}
}