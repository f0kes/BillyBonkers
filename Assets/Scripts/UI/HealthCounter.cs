using System;
using System.Collections.Generic;
using System.Linq;
using Entities;
using GameState;
using TMPro;
using UnityEngine;

namespace UI
{
	public class HealthCounter : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI text;
		private static List<Player> _usedPlayers = new List<Player>();
		private int _playerNumber = 0;
		private Ball _ballToCount;


		private void Awake()
		{
			_usedPlayers = new List<Player>();
		}

		public void Init()
		{
			var players = Player.Players;
			foreach (var player in players)
			{
				if (FindPlayerToCount(player))
				{
					break;
				}
			}

			if (_ballToCount == null)
			{
				gameObject.SetActive(false);
			}
		}
		private void Update()
		{
			UpdatePlayerText();
		}

		private bool FindPlayerToCount(Player player)
		{
			if (!_usedPlayers.Contains(player))
			{
				_usedPlayers.Add(player);
				_ballToCount = player.PlayerBall;
				_playerNumber = player.PlayerId;
				return true;
			}

			return false;
		}

		

		private void UpdatePlayerText()
		{
			if (_ballToCount.Health > 0)
			{
				text.text = "<color=#8c1f33>" + "P" + _playerNumber + " " + "</color>" +
				            Mathf.FloorToInt(_ballToCount.Health) + "%";
			}
			else
			{
				text.text = "D E A D";
			}
		}
	}
}