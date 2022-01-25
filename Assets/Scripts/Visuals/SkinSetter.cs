using System;
using UnityEngine;

namespace Visuals
{
	[RequireComponent(typeof(ChangeMaterialsRenderQueue))]
	[RequireComponent(typeof(MeshRenderer))]
	public class SkinSetter : MonoBehaviour
	{
		[SerializeField] private Skin deadSkin;

		private Ball _ball;
		private MeshRenderer _renderer;
		private ChangeMaterialsRenderQueue _fixer;

		private void Awake()
		{
			_ball = GetComponentInParent<Ball>();
			_renderer = GetComponent<MeshRenderer>();
			_fixer = GetComponent<ChangeMaterialsRenderQueue>();
			_ball.OnSetSkin += SetSkin;
			_ball.OnDeath += () => { SetSkin(deadSkin); };
		}

		private void SetSkin(Skin skin)
		{
			_renderer.material = skin.material;
			_fixer.Fix();
		}
	}
}