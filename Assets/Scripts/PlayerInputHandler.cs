using System;
using System.Collections;
using System.Collections.Generic;
using GameState;
using Interfaces;
using Mirror;
using Networking;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : NetworkBehaviour, IInputHandler
{
	public Action OnShootPressed;
	public Action<Vector2> OnMoveStarted;
	private PlayerInp _frameInp;

	private void Start()
	{
		IdTable<PlayerInputHandler>.Add(this);
		TimeTicker.OnTickEnd += OnTickEnd;
	}

	private void OnDestroy()
	{
		IdTable<PlayerInputHandler>.Remove(this);
		TimeTicker.OnTickEnd -= OnTickEnd;
	}

	private void OnTickEnd(TimeTicker.OnTickEventArgs args)
	{
		if (_frameInp.ShootReleased)
		{
			OnShootPressed?.Invoke();
		}

		if (_frameInp.MoveStarted != Vector2.zero)
		{
			OnMoveStarted?.Invoke(_frameInp.MoveStarted);
		}

		if (!hasAuthority) return;
		
		CmdSendInput(_frameInp);
		_frameInp.MoveStarted = Vector2.zero;
		_frameInp.ShootReleased = false;
	}

	public void OnMove(InputAction.CallbackContext context)
	{
		if (!hasAuthority) return;
		if (context.started)
		{
			_frameInp.MoveStarted = context.ReadValue<Vector2>();
		}

		var moveAxis = Vector2.ClampMagnitude(context.ReadValue<Vector2>(), 1f);
		;
		_frameInp.XMove = moveAxis.x;
		_frameInp.ZMove = moveAxis.y;
	}

	public void OnShoot(InputAction.CallbackContext context)
	{
		if (!hasAuthority) return;
		if (context.started)
		{
			_frameInp.Shoot = true;
		}

		if (context.canceled)
		{
			_frameInp.Shoot = false;
			_frameInp.ShootReleased = true;
		}
	}

	public PlayerInp GetFrameInput()
	{
		return _frameInp;
	}

	public void SetFrameInput(PlayerInp inp)
	{
		_frameInp = inp;
	}

	[Command]
	public void CmdSendInput(PlayerInp inp)
	{
		SetFrameInput(inp);
	}
}