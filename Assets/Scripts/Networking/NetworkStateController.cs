using System;
using System.Collections.Generic;
using System.Linq;
using GameState;
using UnityEngine;

namespace Networking
{
	public class NetworkStateController : MonoBehaviour
	{
		private const int StateCacheSize = 1024;
		private const int MaxNetworkBehaviours = 1024;

		private List<NetworkBehaviour> _networkBehaviours = new List<NetworkBehaviour>();
		private List<PlayerInputHandler> _inputHandlers = new List<PlayerInputHandler>();

		private readonly Message[][]
			_stateCache = new Message[StateCacheSize][]; // [tick][networkBehaviourIndex]

		private readonly PlayerInp[][]
			_inputCache = new PlayerInp[StateCacheSize][]; // [tick][InputHandlerIndex]

		private readonly HashSet<int> _reconciledTicks = new HashSet<int>();

		private void Start()
		{
			for (var i = 0; i < StateCacheSize; i++)
			{
				_stateCache[i] = new Message[MaxNetworkBehaviours];
				_inputCache[i] = new PlayerInp[MaxNetworkBehaviours];
			}

			TimeTicker.OnTick += OnTick;
		}

		private void OnDisable()
		{
			TimeTicker.OnTick -= OnTick;
		}

		private void OnTick(TimeTicker.OnTickEventArgs args)
		{
			_networkBehaviours = IdTable<NetworkBehaviour>.GetValues();
			_inputHandlers = IdTable<PlayerInputHandler>.GetValues();

			foreach (var behaviour in _networkBehaviours)
			{
				SetState(args.Tick, behaviour.NetworkId, behaviour.Serialize());
			}

			foreach (var inputHandler in _inputHandlers)
			{
				SetInput(args.Tick, IdTable<PlayerInputHandler>.GetId(inputHandler), inputHandler.GetFrameInput());
			}

			if (args.Tick % TimeTicker.SecondsToTicks(2) == 0 && !_reconciledTicks.Contains(args.Tick))
			{
				_reconciledTicks.Add(args.Tick);
				Reconcile(args.Tick - TimeTicker.SecondsToTicks(0.3f),
					_stateCache[(args.Tick - TimeTicker.SecondsToTicks(0.3f)) % StateCacheSize]);
			}
		}

		public void RollBack(int tick)
		{
			_networkBehaviours = IdTable<NetworkBehaviour>.GetValues();
			foreach (var behaviour in _networkBehaviours)
			{
				behaviour.Deserialize(GetState(tick, behaviour.NetworkId));
			}
		}

		public void Reconcile(int tick, Message[] state)
		{
			_networkBehaviours = IdTable<NetworkBehaviour>.GetValues();
			_inputHandlers = IdTable<PlayerInputHandler>.GetValues();

			var inputCacheCopy = _inputCache.Clone() as PlayerInp[][];
			
			//DebugPrintInputCache("first ");
			foreach (var behaviour in _networkBehaviours)
			{
				behaviour.Deserialize(state[behaviour.NetworkId]);
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

				TimeTicker.I.Tick();
			}

			//Debug.Log("Rollback complete, tick before rollback: " + currentTick + ", tick after rollback: " +
			//         TimeTicker.I.CurrentTick);
			//DebugPrintInputCache("second ");
		}

		private Message GetState(int tick, int networkBehaviourIndex)
		{
			return _stateCache[tick % StateCacheSize][networkBehaviourIndex];
		}

		private PlayerInp GetInput(int tick, int inputHandlerIndex)
		{
			return _inputCache[tick % StateCacheSize][inputHandlerIndex];
		}

		private void SetState(int tick, int networkBehaviourIndex, Message message)
		{
			_stateCache[tick % StateCacheSize][networkBehaviourIndex] = message;
		}

		private void SetInput(int tick, int inputHandlerIndex, PlayerInp message)
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
	}
}