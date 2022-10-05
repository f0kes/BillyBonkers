using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameState;
using Mirror;
using Telepathy;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Visuals;
using Random = UnityEngine.Random;

namespace UI
{
	public class Lobby : NetworkBehaviour
	{
		public static Lobby I;

		[Serializable]
		public struct LevelBool
		{
			[Scene] public string Level;
			public bool On;
		}

		[SerializeField] private Player _fakePlayerPrefab;
		[SerializeField] private int _minPlayers = 1;
		[SerializeField] private List<LevelBool> Levels;
		[SerializeField] private List<PlayerMenuRepresenter> playerMenuRepresenters = new List<PlayerMenuRepresenter>();
		[SerializeField] private TextMeshProUGUI timer;

		private static readonly List<LobbyPlayer> Players = new List<LobbyPlayer>();

		private IEnumerable<bool> ReadyStates
		{
			get { return Players.Select(player => player.IsReady).ToList(); }
		}

		public void Awake()
		{
			if (I == null)
				I = this;
			else
				Destroy(gameObject);
		}


		public void AddPlayer(LobbyPlayer player)
		{
			Debug.Log("Add player");
			if (Players.Contains(player))
				return;

			Players.Add(player);
			Debug.Log("Player added");
		}

		public void RemovePlayer(LobbyPlayer lobbyPlayer)
		{
			Debug.Log("Removing player");
			Players.Remove(lobbyPlayer);
		}

		private void Start()
		{
			List<LevelBool> updatedLevels = Levels.Where(level => level.On).ToList();
			Levels = updatedLevels;
			EnableRepresenters();
			EnableTimer();
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
			foreach (var player in Players)
			{
				reps[0].ShowPlayer(player.Player, player.IsReady);
				reps.Remove(reps[0]);
			}
		}

		private async void ReadyCheck(float timeToWait)
		{
			float time = 0;

			while (time < timeToWait)
			{
				if (ReadyStates.All(x => x) && Players.Count >= _minPlayers)
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

		private void EnableRepresenters()
		{
			foreach (var representer in playerMenuRepresenters)
			{
				representer.gameObject.SetActive(true);
			}
		}

		private void EnableTimer()
		{
			timer.gameObject.SetActive(true);
		}

		private void HideTimer()
		{
			timer.text = "";
		}

		private void StartRound()
		{
			var randLevel = Random.Range(0, Levels.Count);
			if (Players.Count < 2)
			{
				Player p = Instantiate(_fakePlayerPrefab);
				var lobbyPlayer = p.GetComponent<LobbyPlayer>();
				lobbyPlayer.SetReady(true);
			}

			LoadScene(randLevel);
		}

		private void LoadScene(int scene)
		{
			var levelName = Levels[scene].Level;
			NetworkManager.singleton.ServerChangeScene(levelName);
		}

		private void ShowTimer(float time)
		{
			timer.text = time.ToString();
		}
	}
}