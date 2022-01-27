using DefaultNamespace;
using Entities;
using UnityEngine;

namespace Visuals
{
	[RequireComponent(typeof(ChangeMaterialsRenderQueue))]
	[RequireComponent(typeof(MeshRenderer))]
	public class OutLineChanger : MonoBehaviour
	{
		private NpBall _ball;
		private MeshRenderer _renderer;
		private ChangeMaterialsRenderQueue _fixer;
		private static readonly int Emission = Shader.PropertyToID("_emission");

		private void Awake()
		{
			_ball = GetComponentInParent<NpBall>();
			_renderer = GetComponent<MeshRenderer>();
			_fixer = GetComponent<ChangeMaterialsRenderQueue>();
			_ball.OnChangeOwner += ChangeOutLine;
		}

		private void ChangeOutLine(Ball ball)
		{
			Material material = new Material(_renderer.material);
			Color newColor = ball.Skin.mainColor*2;
			material.SetColor(Emission, newColor);
			_renderer.material = material;
			_fixer.Fix();
		}
	}
}