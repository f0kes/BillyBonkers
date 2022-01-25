using GameState;
using TMPro;
using UnityEngine;

namespace UI
{
	public class PlayerMenuRepresenter : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI nameText;
		[SerializeField] private TextMeshProUGUI readyText;
		[SerializeField] private MeshRenderer doll;

		public void ShowPlayer(Player player, bool ready)
		{
			gameObject.SetActive(true);
			nameText.text = "P" + player.PlayerId;
			readyText.text = ready ? "!" : "...";
			doll.material = player.Skin.material;
		}

		public void Hide()
		{
			gameObject.SetActive(false);
		}
	}
}