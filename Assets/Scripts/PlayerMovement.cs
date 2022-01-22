using System;
using DefaultNamespace;
using Enums;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	public Ball Ball { get; private set; }
	private Rigidbody _rb;
	
	[SerializeField] private Cue cue;
	private PlayerInputHandler _inputHandler;
	private PlayerInputHandler.PlayerInp _frameInp;

	private void Awake()
	{
		_rb = gameObject.GetComponentInParent<Rigidbody>();
		//_inputHandler = gameObject.GetComponentInParent<PlayerInputHandler>();
		Ball = GetComponentInParent<Ball>();
	}

	private void FixedUpdate()
	{
		_frameInp = _inputHandler.GetFrameInput();
		ProcessInput(_frameInp);
		
	}

	public void SetInputHandler(PlayerInputHandler handler)
	{
		_inputHandler = handler;
	}
	private void ProcessInput(PlayerInputHandler.PlayerInp inp)
	{
		Vector2 dir = new Vector2(inp.XMove, inp.ZMove);
		AccelerateTowards(dir);
		cue.ProcessInput(inp);
	}
	

	private void AccelerateTowards(Vector2 dir)
	{
		Vector3 rbVelocity = _rb.velocity;
		Vector3 accel = new Vector3(dir.x, 0, dir.y) * Ball.Stats[BallStat.Acceleration];
		Vector3 newVel = rbVelocity + accel * Time.deltaTime;
		
		rbVelocity = rbVelocity.magnitude * newVel.normalized;
		
		_rb.velocity = newVel.magnitude <= Ball.Stats[BallStat.Speed] ? newVel : rbVelocity;
	}

	
}