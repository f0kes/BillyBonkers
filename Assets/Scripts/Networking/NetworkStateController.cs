using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GameState;
using Mirror;
using UnityEngine;
using Debug = UnityEngine.Debug;

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
		private bool _reconciliationScheduled;

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

			if (!args.Simulating)
			{
				foreach (var inputHandler in _inputHandlers)
				{
					RememberInput(args.Tick, IdTable<PlayerInputHandler>.GetId(inputHandler),
						inputHandler.GetFrameInput());
				}
			}

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

			foreach (var behaviour in _networkBehaviours)
			{
				behaviour.Deserialize(snapshot.Get(behaviour.NetworkId));
			}

			foreach (var inputHandler in _inputHandlers)
			{
				inputHandler.CacheInput();
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

			foreach (var inputHandler in _inputHandlers)
			{
				inputHandler.RestoreInput();
			}
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
			var tick = TimeTicker.I.CurrentTick;
			var snapshot = _stateCache[tick % StateCacheSize];
			OnClientRecieveState(tick, snapshot);
		}

		[ClientRpc]
		private void OnClientRecieveState(int tick, Snapshot snapshot)
		{
			if (isServer) return;
			if (tick > _lastReceivedTick)
			{
				_lastReceivedTick = tick;
				if (tick < TimeTicker.I.CurrentTick)
				{
					ScheduleReconciliation(tick, snapshot);
					//SetState(tick, snapshot);
				}
				else
				{
					SetState(tick, snapshot);
					TimeTicker.I.SyncTick();
				}
			}
		}

		private void ScheduleReconciliation(int tick, Snapshot snapshot)
		{
			if (_reconciliationScheduled)
			{
				return;
			}

			_reconciliationScheduled = true;
			TimeTicker.OnTickStart += Tick;

			void Tick(TimeTicker.OnTickEventArgs args)
			{
				if(args.Simulating) return;
				TimeTicker.OnTickStart -= Tick;
				Reconcile(tick, snapshot);
				_reconciliationScheduled = false;
			}
		}
	}
}