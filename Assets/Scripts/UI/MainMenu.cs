using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameState;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace UI
{
	public class MainMenu : MonoBehaviour
	{
		[SerializeField] private List<Skin> SkinList;
		private List<Skin> _unusedSkins = new List<Skin>();
		[SerializeField] private List<PlayerMenuRepresenter> playerMenuRepresenters = new List<PlayerMenuRepresenter>();
		[SerializeField] private TextMeshProUGUI timer;

		private readonly List<Player> _players = new List<Player>();
		private readonly Dictionary<Player, bool> _readyStates = new Dictionary<Player, bool>();


		private void AddPlayer(Player player)
		{
			if (!_players.Contains(player))
			{
				_players.Add(player);
				if (_unusedSkins.Count != 0)
				{
					player.ChangeSkin(_unusedSkins[0]);
					_unusedSkins.Remove(_unusedSkins[0]);
				}

				_readyStates[player] = false;
				player.inputHandler.OnShootPressed += () => { _readyStates[player] = !_readyStates[player]; };
				player.inputHandler.OnMoveStarted += dir => { ChangeSkin(player, dir); };
			}
		}

		private void ChangeSkin(Player player, Vector2 dir)
		{
			if (!_readyStates[player])
			{
				int currentIndex = SkinList.FindIndex(skin => skin == player.Skin);
				int index = (currentIndex + 1) % SkinList.Count;
				player.ChangeSkin(SkinList[index]);
			}
		}

		public void OnPlayerJoin(PlayerInput input)
		{
			Player player = input.gameObject.GetComponent<Player>();

			AddPlayer(player);
		}

		private void Start()
		{
			_unusedSkins = new List<Skin>(SkinList);
			var players = Player.Players;
			foreach (var player in players)
			{
				AddPlayer(player);
			}

			ReadyCheck(1f);
		}


		private void Update()
		{
			ShowPlayers();
		}


		private void ShowPlayers()
		{
			foreach (var representer in playerMenuRepresenters)
			{
				representer.Hide();
			}

			var reps = new List<PlayerMenuRepresenter>(playerMenuRepresenters);
			foreach (var player in _players)
			{
				reps[0].ShowPlayer(player, _readyStates[player]);
				reps.Remove(reps[0]);
			}
		}

		private async void ReadyCheck(float timeToWait)
		{
			float time = 0;

			while (time < timeToWait)
			{
				if (_readyStates.Values.All(x => x) && _players.Count > 1)
				{
					time += Time.deltaTime;
					ShowTimer(Mathf.CeilToInt(timeToWait - time));
				}
				else
				{
					HideTimer();
					time = 0;
				}

				await Task.Yield();
			}

			StartRound();
		}

		private void HideTimer()
		{
			timer.text = "";
		}

		private void StartRound()
		{
			SceneManager.LoadScene(1);
		}

		private void ShowTimer(float time)
		{
			timer.text = time.ToString();
		}
	}
}