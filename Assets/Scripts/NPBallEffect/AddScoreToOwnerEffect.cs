using Entities;
using Enums;
using GameState;
using Structures;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

namespace NPBallEffect
{
	[CreateAssetMenu(fileName = "New Score Effect", menuName = "Score Effect", order = 2)]
	public class AddScoreToOwnerEffect : Effect
	{
		[SerializeField] private int score;


		public override void Apply(NpBall ball)
		{
			ball.OnDeath += () => { Score(ball); };
		}

		private void Score(NpBall ball)
		{
			if (ball.Owner != null)
			{
				ball.Owner.AddScore(score);
			}
		}
	}
}