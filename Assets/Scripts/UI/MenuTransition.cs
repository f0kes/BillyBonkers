using UnityEngine;

namespace UI
{
	public class MenuTransition : MonoBehaviour
	{
		[SerializeField] private GameObject _menuToGoTo;
		[SerializeField] private GameObject _menuToGoFrom;
		public void Transition()
		{
			_menuToGoTo.SetActive(true);
			_menuToGoFrom.SetActive(false);
		}
	}
}
