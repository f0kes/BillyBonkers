using Entities;
using UnityEngine;

namespace Visuals
{
	public class OutLineIntensityChanger : MonoBehaviour
	{
		private Ball _ball;
		private MeshRenderer _renderer;
		private ChangeMaterialsRenderQueue _fixer;
		private static readonly int Emission = Shader.PropertyToID("_emission");

		private void Awake()
		{
			_ball = GetComponentInParent<Ball>();
			_renderer = GetComponent<MeshRenderer>();
			_fixer = GetComponent<ChangeMaterialsRenderQueue>();
			_ball.OnOutlineIntensityChange += ChangeOutLine;
		}

		private void ChangeOutLine(float intensity)
		{
			Material material = new Material(_renderer.material);
			Color newColor = _ball.Skin.mainColor*intensity;
			material.SetColor(Emission, newColor);
			_renderer.material = material;
			_fixer.Fix();
		}
	}
}