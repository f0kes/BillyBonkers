using System;
using System.Collections.Generic;
using System.Linq;
using GameState;
using Mirror;
using UnityEngine;

namespace Networking
{
	public class NetworkStateController : NetworkBehaviour
	{
		private const int StateCacheSize = 1024;
		private const int MaxNetworkBehaviours = 1024;

		private List<NetworkEntity> _networkBehaviours = new List<NetworkEntity>();
		private List<PlayerInputHandler> _inputHandlers = new List<PlayerInputHandler>();

		//TODO convert to dictionary
		private readonly Snapshot[]
			_stateCache = new Snapshot[StateCacheSize]; // [tick][networkBehaviourIndex]

		private readonly PlayerInp[][]
			_inputCache = new PlayerInp[StateCacheSize][]; // [tick][InputHandlerIndex]

		private int _lastReceivedTick;

		private void Start()
		{
			for (var i = 0; i < StateCacheSize; i++)
			{
				_stateCache[i] = new Snapshot();
				_inputCache[i] = new PlayerInp[MaxNetworkBehaviours];
			}
		}

		private void OnDisable()
		{
			TimeTicker.OnTick -= OnTick;
		}

		private void OnEnable()
		{
			TimeTicker.OnTick += OnTick;
		}

		private void OnTick(TimeTicker.OnTickEventArgs args)
		{
			_networkBehaviours = IdTable<NetworkEntity>.GetValues();
			_inputHandlers = IdTable<PlayerInputHandler>.GetValues();

			foreach (var behaviour in _networkBehaviours)
			{
				RememberState(args.Tick, behaviour.NetworkId, behaviour.Serialize());
			}

			foreach (var inputHandler in _inputHandlers)
			{
				RememberInput(args.Tick, IdTable<PlayerInputHandler>.GetId(inputHandler), inputHandler.GetFrameInput());
			}

			/*if (args.Tick % TimeTicker.SecondsToTicks(2) == 0 && !_reconciledTicks.Contains(args.Tick))
			{
				_reconciledTicks.Add(args.Tick);
				Reconcile(args.Tick - TimeTicker.SecondsToTicks(0.3f),
					_stateCache[(args.Tick - TimeTicker.SecondsToTicks(0.3f)) % StateCacheSize]);
			}*/
			if (isServer)
			{
				PrepareStateAndSend();
			}
		}

		public void SetState(int tick, Snapshot state)
		{
			//_stateCache[tick % StateCacheSize] = snapshotData;
			TimeTicker.I.CurrentTick = tick;
			foreach (var behaviour in _networkBehaviours)
			{
				behaviour.Deserialize(state.Get(behaviour.NetworkId));
			}
		}

		public void Reconcile(int tick, Snapshot snapshot)
		{
			_networkBehaviours = IdTable<NetworkEntity>.GetValues();
			_inputHandlers = IdTable<PlayerInputHandler>.GetValues();

			var inputCacheCopy = _inputCache.Clone() as PlayerInp[][];

			//DebugPrintInputCache("first ");
			foreach (var behaviour in _networkBehaviours)
			{
				behaviour.Deserialize(snapshot.Get(behaviour.NetworkId));
			}

			var currentTick = TimeTicker.I.CurrentTick;
			TimeTicker.I.CurrentTick = tick;
			while (TimeTicker.I.CurrentTick < currentTick)
			{
				foreach (var inputHandler in _inputHandlers)
				{
					inputHandler.SetFrameInput(GetInput(TimeTicker.I.CurrentTick,
						IdTable<PlayerInputHandler>.GetId(inputHandler)));
				}

				TimeTicker.I.Tick(true);
			}

			//Debug.Log("Rollback complete, tick before rollback: " + currentTick + ", tick after rollback: " +
			//         TimeTicker.I.CurrentTick);
			//DebugPrintInputCache("second ");
		}

		private Message GetState(int tick, ushort networkBehaviourIndex)
		{
			return _stateCache[tick % StateCacheSize].Get(networkBehaviourIndex);
		}

		private PlayerInp GetInput(int tick, int inputHandlerIndex)
		{
			return _inputCache[tick % StateCacheSize][inputHandlerIndex];
		}

		private void RememberState(int tick, ushort networkBehaviourIndex, Message message)
		{
			_stateCache[tick % StateCacheSize].Set(networkBehaviourIndex, message);
		}

		private void RememberInput(int tick, int inputHandlerIndex, PlayerInp message)
		{
			_inputCache[tick % StateCacheSize][inputHandlerIndex] = message;
		}

		private void DebugPrintInputCache(string toAdd = "")
		{
			for (var i = 0; i < StateCacheSize; i++)
			{
				var ic = _inputCache[i][0];
				var toPrint = toAdd + "tick: " + i + " controller: " + 0 + "\n";
				toPrint += ic.Tick + " tick" + "\n";
				toPrint += ic.Shoot + " shoot" + "\n";
				toPrint += ic.XMove + " x move" + "\n";
				toPrint += ic.ZMove + " z move" + "\n";
				Debug.Log(toPrint);
			}
		}

		[Server]
		private void PrepareStateAndSend()
		{
			int tick = TimeTicker.I.CurrentTick;
			Snapshot snapshot = _stateCache[tick % StateCacheSize];
			RecieveState(tick, snapshot);
		}

		[ClientRpc]
		private void RecieveState(int tick, Snapshot snapshot)
		{
			if (tick > _lastReceivedTick)
			{
				if (tick < TimeTicker.I.CurrentTick)
				{
					Reconcile(tick, snapshot);
					Debug.Log("Reconciled");
				}
				else
				{
					SetState(tick, snapshot);
				}

				_lastReceivedTick = tick;
			}
		}
	}
}