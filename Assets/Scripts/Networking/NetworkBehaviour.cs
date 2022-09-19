using System;
using System.Collections.Generic;
using GameState;
using UnityEngine;

namespace Networking
{
	public abstract class NetworkBehaviour : MonoBehaviour
	{
		public ushort NetworkId { get; private set; }

		protected virtual void Awake()
		{
			IdTable<NetworkBehaviour>.Add(this);
			NetworkId = IdTable<NetworkBehaviour>.GetId(this);
		}

		protected virtual void Start()
		{
			TimeTicker.OnTick += OnTick;
		}

		protected virtual void OnDisable()
		{
			IdTable<NetworkBehaviour>.Remove(this);
			TimeTicker.OnTick -= OnTick;
		}
		protected virtual void OnDestroy()
		{
			
		}

		private void OnTick(TimeTicker.OnTickEventArgs args)
		{
			OnTick();
		}

		protected virtual void OnTick()
		{
		}

		public abstract Message Serialize();
		public abstract void Deserialize(Message message);
	}
}