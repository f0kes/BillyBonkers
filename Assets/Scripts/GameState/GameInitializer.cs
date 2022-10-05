using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cinemachine;
using Entities;
using Mirror;
using Networking;
using Steamworks;
using UnityEngine;

namespace GameState
{
	public class GameInitializer : NetworkBehaviour
	{
		private CinemachineTargetGroup _cinemachineTargetGroup;
		private Dictionary<int, bool> _readyStates = new Dictionary<int, bool>();
		private bool _isGameStarted = false;
		public static event Action OnGameStart;

		private void Awake()
		{
			TimeTicker.I.Pause();
		}


		private void Start()
		{
			_cinemachineTargetGroup = FindObjectOfType<CinemachineTargetGroup>();
			_isGameStarted = true;
		}

		public override void OnStartClient()
		{
			base.OnStartClient();
			SendReady();
		}

		public override void OnStartServer()
		{
			base.OnStartServer();
			InitWhenReady();
		}

		private async void InitWhenReady()
		{
			while (_readyStates.Count < NetworkManager.singleton.numPlayers )
			{
				await Task.Yield();
			}

			Debug.Log("All players ready");
			Init();
		}

		private void Init()
		{
			_cinemachineTargetGroup = FindObjectOfType<CinemachineTargetGroup>();
			if (!isServer) return;

			IdTable<NetworkEntity>.Clear();
			RpcClearNetworkEntities();
			var networkEntities = FindObjectsOfType<NetworkEntity>().ToList();
			foreach (var networkEntity in networkEntities)
			{
				networkEntity.Init();
			}

			var balls = FindObjectsOfType<Ball>().ToList().Where((x) => x.IsPlayer);
			var unusedPlayers = new List<Player>(Player.Players);
			foreach (var mover in balls)
			{
				InitBall(unusedPlayers, mover);
			}

			RpcInitClient();
		}

		private void InitBall(IList<Player> unusedPlayers, NetworkEntity ball)
		{
			if (unusedPlayers.Count > 0)
			{
				var player = unusedPlayers[0];
				SetBallToPlayerWithIds(player.PlayerId, IdTable<NetworkEntity>.GetId(ball));
				unusedPlayers.Remove(unusedPlayers[0]);
			}
			else
			{
				NetworkServer.Destroy(ball.gameObject);
				Destroy(ball.transform.root.gameObject);
			}
		}

		private void SetBallToPlayer(Player player, Ball ball)
		{
			ball.SetInputHandler(player.inputHandler);
			player.SetBall(ball);
			_cinemachineTargetGroup.AddMember(ball.RbTransform, 1, 4);
		}

		[ClientRpc]
		private void SetBallToPlayerWithIds(ushort playerID, ushort ballID)
		{
			Debug.Log($"Setting ball {ballID} to player {playerID}");
			var player = (Player) playerID;
			var ball = (Ball) IdTable<NetworkEntity>.GetValue(ballID);
			SetBallToPlayer(player, ball);
		}

		[ClientRpc]
		private void RpcInitClient()
		{
			TimeTicker.I.SyncTick();
			TimeTicker.I.Unpause();
			OnGameStart?.Invoke();
		}

		[ClientRpc]
		private void RpcClearNetworkEntities()
		{
			IdTable<NetworkEntity>.Clear();
		}

		[Command(requiresAuthority = false)]
		private void SendReady(NetworkConnectionToClient sender = null)
		{
			Debug.Log($"Player {sender.connectionId} is ready");
			_readyStates[sender.connectionId] = true;
		}
	}
}