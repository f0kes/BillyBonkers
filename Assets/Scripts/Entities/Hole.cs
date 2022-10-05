using System;
using System.Collections;
using System.Collections.Generic;
using Entities;
using Networking;
using UnityEngine;

public class Hole : MonoBehaviour
{
	[SerializeField] private LayerMask ballLayer;

	private void OnTriggerEnter(Collider other)
	{
		Ball ball = other.gameObject.GetComponentInParent<Ball>();
		if (ball != null)
		{
			GameObject[] floor = GameObject.FindGameObjectsWithTag("Floor");
			foreach (var f in floor)
			{
				Physics.IgnoreCollision(other, f.GetComponent<Collider>(), true);
			}

			GravityController gravityController = ball.GetComponentInChildren<GravityController>();
			if (gravityController != null)
			{
				gravityController.Fall();
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		Ball ball = other.gameObject.GetComponentInParent<Ball>();
		if (ball != null)
		{
			GameObject[] floor = GameObject.FindGameObjectsWithTag("Floor");
			foreach (var f in floor)
			{
				Physics.IgnoreCollision(other, f.GetComponent<Collider>(), false);
			}

			GravityController gravityController = ball.GetComponentInChildren<GravityController>();
			if (gravityController != null)
			{
				gravityController.StopFalling();
			}
		}
	}

	
}