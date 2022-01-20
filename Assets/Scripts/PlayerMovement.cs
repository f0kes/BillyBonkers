using DefaultNamespace;
using DefaultNamespace.Enums;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	private Ball _ball;
	private Rigidbody _rb;
	
	[SerializeField] private Cue cue;
	private PlayerInputHandler _inputHandler;
	private PlayerInputHandler.BallInput _frameInput;

	private void Awake()
	{
		_rb = gameObject.GetComponentInParent<Rigidbody>();
		_inputHandler = gameObject.GetComponentInParent<PlayerInputHandler>();
		_ball = GetComponentInParent<Ball>();
	}

	private void FixedUpdate()
	{
		_frameInput = _inputHandler.GetFrameInput();
		ProcessInput(_frameInput);
		
	}

	
	private void ProcessInput(PlayerInputHandler.BallInput input)
	{
		Vector2 dir = new Vector2(input.XMove, input.ZMove);
		AccelerateTowards(dir);
		cue.ProcessInput(input);
	}
	

	private void AccelerateTowards(Vector2 dir)
	{
		Vector3 rbVelocity = _rb.velocity;
		Vector3 accel = new Vector3(dir.x, 0, dir.y) * _ball.Stats[BallStat.Acceleration];
		Vector3 newVel = rbVelocity + accel * Time.deltaTime;
		
		rbVelocity = rbVelocity.magnitude * newVel.normalized;
		
		_rb.velocity = newVel.magnitude <= _ball.Stats[BallStat.Speed] ? newVel : rbVelocity;
	}
}