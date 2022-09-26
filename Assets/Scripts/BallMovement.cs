using System;
using DefaultNamespace;
using Entities;
using Enums;
using GameState;
using Interfaces;
using Networking;
using UnityEngine;

public class BallMovement : MonoBehaviour
{
	public bool IsPlayer = true;
	public Ball Ball { get; private set; }
	private Rigidbody _rb;

	[SerializeField] private Cue cue;
	private IInputHandler _inputHandler;
	private PlayerInp _frameInp;

	private bool _controlDisabled = true;

	private void Awake()
	{
		_rb = gameObject.GetComponentInParent<Rigidbody>();
		//_inputHandler = gameObject.GetComponentInParent<PlayerInputHandler>();
		Ball = GetComponentInParent<Ball>();
		Ball.OnDeath += DisableControl;
		IdTable<BallMovement>.Add(this);
	}

	private void Start()
	{
		TimeTicker.OnTick += OnTick;
	}

	private void OnDestroy()
	{
		TimeTicker.OnTick -= OnTick;
	}

	private void DisableControl()
	{
		_controlDisabled = true;
	}

	private void OnTick(TimeTicker.OnTickEventArgs eventArgs)
	{
		if (!_controlDisabled)
		{
			_frameInp = _inputHandler.GetFrameInput();
			ProcessInput(_frameInp);
		}
	}

	public void SetInputHandler(IInputHandler handler)
	{
		_inputHandler = handler;
		_controlDisabled = false;
	}

	private void ProcessInput(PlayerInp inp)
	{
		Vector2 dir = new Vector2(inp.XMove, inp.ZMove);
		AccelerateTowards(dir);
		cue.ProcessInput(inp);
	}


	private void AccelerateTowards(Vector2 dir)
	{
		Vector3 rbVelocity = _rb.velocity;
		Vector3 accel = new Vector3(dir.x, 0, dir.y) * Ball.Stats[BallStat.Acceleration];
		Vector3 newVel = rbVelocity + accel * TimeTicker.TickInterval;

		rbVelocity = rbVelocity.magnitude * newVel.normalized;

		_rb.velocity = newVel.magnitude <= Ball.Stats[BallStat.Speed] ? newVel : rbVelocity;
	}
}