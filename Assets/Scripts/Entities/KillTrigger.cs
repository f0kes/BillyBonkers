using System;
using System.Collections;
using System.Collections.Generic;
using Entities;
using UnityEngine;

public class KillTrigger : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		Ball ball = other.gameObject.GetComponentInParent<Ball>();
		if (ball != null)
		{
			ball.KillTrigger(this);
		}
	}
}
