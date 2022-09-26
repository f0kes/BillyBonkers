using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cinemachine;
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

		private void Awake()
		{
			TimeTicker.I.Pause();
		}

		private void Start()
		{
			_cinemachineTargetGroup = FindObjectOfType<CinemachineTargetGroup>();
		}

		public override void OnStartClient()
		{
			base.OnStartClient();
			Debug.Log("OnStartClient");
			SendReady();
		}

		public override void OnStartServer()
		{
			base.OnStartServer();
			Debug.Log("OnStartServer");
			InitWhenReady();
		}

		private async void InitWhenReady()
		{
			while (_readyStates.Count < NetworkManager.singleton.numPlayers)
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
			var balls = FindObjectsOfType<BallMovement>().ToList().Where((x) => x.IsPlayer);
			var unusedPlayers = new List<Player>(Player.Players);
			foreach (var mover in balls)
			{
				InitBall(unusedPlayers, mover);
			}

			RpcStartTicker();
		}

		private void InitBall(IList<Player> unusedPlayers, BallMovement mover)
		{
			if (unusedPlayers.Count > 0)
			{
				var player = unusedPlayers[0];
				SetBallToPlayerWithIds(player.PlayerId, IdTable<BallMovement>.GetId(mover));
				unusedPlayers.Remove(unusedPlayers[0]);
			}
			else
			{
				NetworkServer.Destroy(mover.transform.root.gameObject);
			}
		}

		private void SetBallToPlayer(Player player, BallMovement movement)
		{
			movement.SetInputHandler(player.inputHandler);
			player.SetBall(movement.Ball);
			_cinemachineTargetGroup.AddMember(movement.transform, 1, 4);
		}

		[ClientRpc]
		private void SetBallToPlayerWithIds(ushort playerID, ushort ballID)
		{
			Debug.Log($"Setting ball {ballID} to player {playerID}");
			var player = (Player) playerID;
			var ball = IdTable<BallMovement>.GetValue(ballID);
			SetBallToPlayer(player, ball);
		}

		[ClientRpc]
		private void RpcStartTicker()
		{
			TimeTicker.I.Reset();
			TimeTicker.I.Unpause();
		}

		[Command(requiresAuthority = false)]
		private void SendReady(NetworkConnectionToClient sender = null)
		{
			Debug.Log($"Player {sender.connectionId} is ready");
			_readyStates[sender.connectionId] = true;
		}
	}
}