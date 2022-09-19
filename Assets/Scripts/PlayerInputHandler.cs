using System;
using System.Collections;
using System.Collections.Generic;
using GameState;
using Interfaces;
using Networking;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour, IInputHandler
{
	public Action OnShootPressed;
	public Action<Vector2> OnMoveStarted;
	private PlayerInp _frameInp;

	private void Start()
	{
		IdTable<PlayerInputHandler>.Add(this);
		TimeTicker.OnTick += OnTick;
	}

	private void OnDestroy()
	{
		IdTable<PlayerInputHandler>.Remove(this);
		TimeTicker.OnTick -= OnTick;
	}

	private void OnTick(TimeTicker.OnTickEventArgs args)
	{
		if (_frameInp.ShootReleased)
		{
			
		}
		_frameInp = new PlayerInp(_frameInp)
		{
			Tick = args.Tick
		};
	}
	public void OnMove(InputAction.CallbackContext context)
	{
		if (context.started)
		{
			OnMoveStarted?.Invoke(context.ReadValue<Vector2>());
		}
		var moveAxis = Vector2.ClampMagnitude(context.ReadValue<Vector2>(), 1f); ;
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
			_frameInp.ShootReleased = true;
			OnShootPressed?.Invoke(); //TODO move to tick
			
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
}