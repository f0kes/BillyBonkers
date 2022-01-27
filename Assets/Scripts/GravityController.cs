using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GravityController : MonoBehaviour
{
	private Rigidbody _rb;
	private float _gravityScale = -9.81f;
	private float _defaultGravity = -9.81f;
	private float _fallGravity = -100f;

	private void Awake()
	{
		_rb = GetComponent<Rigidbody>();
		_rb.useGravity = false;
	}


	private void FixedUpdate()
	{
		Vector3 gravity = _gravityScale * Vector3.up;
		_rb.AddForce(gravity, ForceMode.Acceleration);
	}

	private void SetGravityScale(float gravityScale = -9.81f)
	{
		_gravityScale = gravityScale;
	}

	public void Fall()
	{
		Vector3 vel = _rb.velocity;
		SetGravityScale(_fallGravity);
	}

	public void StopFalling()
	{
		Vector3 vel = _rb.velocity;
		SetGravityScale(_defaultGravity);
	}
}