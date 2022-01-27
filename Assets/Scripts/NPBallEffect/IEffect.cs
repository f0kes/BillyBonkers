using Entities;
using UnityEngine;

namespace NPBallEffect
{
	public abstract class Effect :  ScriptableObject
	{
		public abstract void Apply(NpBall ball);
	}
}