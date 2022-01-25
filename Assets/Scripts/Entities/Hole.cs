using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hole : MonoBehaviour
{
	[SerializeField] private LayerMask ballLayer;
	private void OnTriggerEnter(Collider other)
	{
		
		if ((ballLayer.value & (1 << other.gameObject.layer)) > 0)
		{
			GameObject[] floor = GameObject.FindGameObjectsWithTag("Floor");
			foreach (var f in floor)
			{
				Physics.IgnoreCollision(other,f.GetComponent<Collider>(), true);
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if ((ballLayer.value & (1 << other.gameObject.layer)) > 0)
		{
			GameObject[] floor = GameObject.FindGameObjectsWithTag("Floor");
			foreach (var f in floor)
			{
				Physics.IgnoreCollision(other,f.GetComponent<Collider>(), false);
			}
		}
	}
}
