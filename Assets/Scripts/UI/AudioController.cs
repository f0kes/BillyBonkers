using UnityEngine;

namespace UI
{
	public class AudioController : MonoBehaviour
	{
		public void OnValueChanged(float value)
		{
			AudioListener.volume = value;
		}
	}
}