using System;
using System.Collections.Generic;
using DefaultNamespace.Enums;
using Structures;
using Structures.Structures;
using UnityEngine;

public class Ball : MonoBehaviour
{
	private float _health;
		
	[SerializeField] private List<EnumeratedStat> enumeratedStats;
	private Rigidbody _rb;
	public StatDict<BallStat> Stats = new StatDict<BallStat>();
		
	[Serializable]
	public struct EnumeratedStat
	{
		public BallStat Name;
		public Stat Value;
	}

	private void Awake()
	{
		foreach (var stat in enumeratedStats)
		{
			Stats.SetStat(stat.Name, stat.Value);
		}

		_health = Stats[BallStat.Health];
		_rb = GetComponentInChildren<Rigidbody>();
	}

	public void Hit(Vector3 hitVector)
	{
		_rb.AddForce(hitVector, ForceMode.VelocityChange);
		_health -= hitVector.magnitude;
		if (_health <= 0)
		{
			Destroy(gameObject);
		}
	}
}