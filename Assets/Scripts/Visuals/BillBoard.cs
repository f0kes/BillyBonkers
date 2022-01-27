using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Visuals
{
	public class BillBoard : MonoBehaviour
	{
		private Camera _camera;
		public float _alpha = 1;

		public bool rotate180;

		// Start is called before the first frame update
		void Start()
		{
			_camera = Camera.main;
		}

		// Update is called once per frame
		void Update()
		{
			Transform t;
			(t = transform).LookAt(_camera.transform);
			int add = rotate180 ? 180 : 0;
			var rotation = t.rotation;
			t.Rotate(Vector3.up, 180);
			ChangeTransparency();
		}

		private void ChangeTransparency()
		{
		}
	}
}