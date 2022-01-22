using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
	public Action OnShootPressed;

	public struct PlayerInp
	{
		public float XMove;
		public float ZMove;
		public bool Shoot;
	}

	private PlayerInp _frameInp;


	public void OnMove(InputAction.CallbackContext context)
	{
		Vector2 moveAxis = context.ReadValue<Vector2>();
		_frameInp.XMove = moveAxis.x;
		_frameInp.ZMove = moveAxis.y;
	}

	public void OnShoot(InputAction.CallbackContext context)
	{
		if (context.started)
		{
			_frameInp.Shoot = true;
			
		}

		if (context.canceled)
		{
			_frameInp.Shoot = false;
			OnShootPressed?.Invoke();
		}
	}

	public PlayerInp GetFrameInput()
	{
		return _frameInp;
	}
}