using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuBallRotator : MonoBehaviour
{
	// Start is called before the first frame update
	void Start()
	{
	}

	// Update is called once per frame
	void Update()
	{
		Vector3 axis = (Vector3.up + Vector3.right).normalized;
		transform.Rotate(axis, 30*Time.deltaTime);
	}
}