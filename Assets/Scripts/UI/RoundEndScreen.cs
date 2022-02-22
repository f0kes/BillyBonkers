using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Enums;
using GameState;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
	public class RoundEndScreen : MonoBehaviour
	{
		[SerializeField]private TextMeshProUGUI condition;
		[SerializeField]private TextMeshProUGUI winner;
		[SerializeField]private List<TextMeshProUGUI> scores ;
		[SerializeField] private Animator _toWait;
		public void ShowRoundResults(RoundFinishMessage message)
		{

			condition.text = message.Context;
			if (message.Tie == false)
			{
				winner.text = "P" + message.Winner.PlayerId + " Wins";
			}
			else
			{
				winner.text = "No winner";
			}

			var unusedPlayers = new List<Player>(Player.Players);
			foreach (var text in scores)
			{
				if (unusedPlayers.Count == 0)
				{
					text.gameObject.SetActive(false);
				}
				else
				{
					Player p = unusedPlayers[0];
					text.text = "P" + p.PlayerId + "\n" + "\n" + "\n" + p.PlayerWins + " wins";
					unusedPlayers.Remove(p);
				}
			}
			//CheckForAnimation();
			
		}

		// private async void CheckForAnimation()
		// {
		// 	while (!_toWait.GetCurrentAnimatorStateInfo(0).IsTag("Finished"))
		// 	{
		// 		Debug.Log(_toWait.GetCurrentAnimatorStateInfo(0).IsName("Finished"));
		// 		await Task.Yield();
		// 	}
		// 	SceneManager.LoadScene(0);
		// }
	}
}