using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using DefaultNamespace.Enums;
using UnityEngine;

public class Cue : MonoBehaviour
{
	private Ball _ball;

	[SerializeField] private Rigidbody ballBody;
	[SerializeField] private LayerMask ballMask;
	[SerializeField] private LayerMask wallMask;

	private Vector3 _dir = Vector3.zero;
	private Vector3 _bodyOldPos = Vector3.zero;

	private Vector3 _chargeDisplacement = Vector3.zero;
	private float _currentChargeTime = 0;


	private void Awake()
	{
		_ball = GetComponentInParent<Ball>();
	}

	private void FixedUpdate()
	{
		AimAndPosition();
	}

	public void ProcessInput(PlayerInputHandler.BallInput input)
	{
		if (input.Shoot)
		{
			Charge(_dir);
		}
		else if (_currentChargeTime != 0)
		{
			Release();
		}
	}

	private void AimAndPosition()
	{
		var position = ballBody.position;
		_dir = (position - _bodyOldPos).normalized;
		_bodyOldPos = position;

		transform.position = ballBody.position + _dir + Vector3.up * 0.5f + _chargeDisplacement;

		Quaternion rotation = Quaternion.LookRotation(_dir, Vector3.up);
		transform.rotation = rotation;
	}

	private void Charge(Vector3 dir)
	{
		_currentChargeTime += Time.deltaTime;
		if (_currentChargeTime > _ball.Stats[BallStat.ChargeTime])
		{
			_currentChargeTime = _ball.Stats[BallStat.ChargeTime];
		}

		_chargeDisplacement = -dir * _currentChargeTime / _ball.Stats[BallStat.ChargeTime];
	}

	private void Release()
	{
		float force = _currentChargeTime / _ball.Stats[BallStat.ChargeTime] * _ball.Stats[BallStat.HitPower];

		_currentChargeTime = 0;
		_chargeDisplacement = Vector3.zero;

		RaycastHit hit;

		if (Physics.Raycast(ballBody.position, _dir, out hit, 3, ballMask))
		{
			Vector3 finalForce = -hit.normal * force;
			Ball ball = hit.transform.gameObject.GetComponentInParent<Ball>();
			Strike strike = new Strike() {Striker = _ball, Victim = ball, HitVector = finalForce};
			ball.Hit(strike);
			ballBody.AddForce(-finalForce * _ball.Stats[BallStat.KnockBack], ForceMode.VelocityChange);
		}
		else if (Physics.Raycast(ballBody.position, _dir, out hit, 3, wallMask))
		{
			Vector3 finalForce = -hit.normal * force;
			ballBody.AddForce(-finalForce, ForceMode.VelocityChange);
		}
	}
}