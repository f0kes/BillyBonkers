using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionHelper : MonoBehaviour
{
	protected virtual void OnCollisionEnter(Collision collision)
	{
		Ball ball = GetComponentInParent<Ball>();
		ball.CollisionFromChild(collision);
	}
}
