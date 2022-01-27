using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using Entities;
using Enums;
using Unity.Mathematics;
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

	public void ProcessInput(PlayerInputHandler.PlayerInp inp)
	{
		if (inp.Shoot)
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

		Quaternion rotation = quaternion.identity;
		Vector3 fixedDir = new Vector3(_dir.x, 0, _dir.z);
		if (fixedDir != Vector3.zero)
		{
			rotation = Quaternion.LookRotation(fixedDir, Vector3.up);
		}

		if (rotation != Quaternion.identity)
		{
			transform.rotation = rotation;
		}
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
			Vector3 fixedNormal = new Vector3(hit.normal.x, 0, hit.normal.z);
			Vector3 finalForce = -fixedNormal * force;
			Ball victim = hit.transform.gameObject.GetComponentInParent<Ball>();
			Strike strike = new Strike(_ball, victim, finalForce, _ball.Stats[BallStat.DamageMultiplier],
				StrikeSource.Cue);
			victim.ApplyDamage(strike);
			ballBody.AddForce(-finalForce * _ball.Stats[BallStat.KnockBack], ForceMode.VelocityChange);
		}
		else if (Physics.Raycast(ballBody.position, _dir, out hit, 3, wallMask))
		{
			Vector3 finalForce = -_dir * force;
			ballBody.AddForce(finalForce, ForceMode.VelocityChange);
		}
	}
}