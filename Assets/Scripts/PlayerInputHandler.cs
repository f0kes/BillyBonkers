using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
	public struct BallInput
	{
		public float XMove;
		public float ZMove;
		public bool Shoot;
	}

	private BallInput _frameInput;

	
	public void OnMove(InputAction.CallbackContext context)
	{
		Vector2 moveAxis = context.ReadValue<Vector2>();
		_frameInput.XMove = moveAxis.x;
		_frameInput.ZMove = moveAxis.y;
	}

	public void OnShoot(InputAction.CallbackContext context)
	{
		if (context.started)
		{
			_frameInput.Shoot = true;
		}

		if (context.canceled)
		{
			_frameInput.Shoot = false;
		}
	}

	public BallInput GetFrameInput()
	{
		return _frameInput;
	}
}