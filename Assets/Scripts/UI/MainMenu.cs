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
		[SerializeField] private List<PlayerMenuRepresenter> playerMenuRepresenters = new List<PlayerMenuRepresenter>();
		[SerializeField] private TextMeshProUGUI timer;
		
		private List<Player> _players = new List<Player>();
		private Dictionary<Player, bool> _readyStates = new Dictionary<Player, bool>();

		private void AddPlayer(Player player)
		{
			_players.Add(player);
			_readyStates[player] = false;
			player.inputHandler.OnShootPressed += () => { _readyStates[player] = !_readyStates[player]; };
		}

		public void OnPlayerJoin(PlayerInput input)
		{
			Player player = input.gameObject.GetComponent<Player>();
			AddPlayer(player);
		}

		private void Start()
		{
			var players = FindObjectsOfType<Player>().ToList();
			foreach (var player in players)
			{
				AddPlayer(player);
			}
			ReadyCheck(5f);
		}


		private void Update()
		{
			ReadInputs();
			ShowPlayers();
		}


		private void ReadInputs()
		{
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
					ShowTimer(Mathf.CeilToInt(timeToWait-time));
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