using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetSmallerOnTime : MonoBehaviour
{
	[SerializeField] private float _value;

	void Update()
	{
		var localScale = transform.localScale;
		var frameValue = _value * Time.deltaTime;
		localScale = new Vector3(localScale.x - frameValue, localScale.y,
			localScale.z - frameValue);
		transform.localScale = localScale;
	}
}