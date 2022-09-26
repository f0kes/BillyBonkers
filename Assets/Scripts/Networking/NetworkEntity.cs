﻿using System;
using System.Collections.Generic;
using GameState;
using Mirror;
using UnityEngine;

namespace Networking
{
	public abstract class NetworkEntity : NetworkBehaviour
	{
		public ushort NetworkId { get; private set; }

		protected virtual void Awake()
		{
			IdTable<NetworkEntity>.Add(this);
			NetworkId = IdTable<NetworkEntity>.GetId(this);
		}

		protected virtual void Start()
		{
			TimeTicker.OnTick += OnTick;
		}

		protected virtual void OnDisable()
		{
			IdTable<NetworkEntity>.Remove(this);
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
		
		public static explicit operator ushort(NetworkEntity entity)
		{
			return entity.NetworkId;
		}
		public static explicit operator NetworkEntity(ushort id)
		{
			return IdTable<NetworkEntity>.GetValue(id);
		}
	}
}