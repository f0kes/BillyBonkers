using Entities;
using Enums;
using GameState;
using Structures;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

namespace NPBallEffect
{
	[CreateAssetMenu(fileName = "New Buff Owner Effect", menuName = "Buff Owner Effect", order = 2)]
	public class BuffOwnerOnScoreEffect : Effect
	{
		[SerializeField] private BallStat statToChange;
		[SerializeField] private GameObject iconStart;
		[SerializeField] private GameObject iconFinish;
		[SerializeField] private float value;
		[SerializeField] private float time;

		public override void Apply(NpBall ball)
		{
			ball.OnDeath += () => { ModifyStat(ball); };
		}

		private void ModifyStat(NpBall npBall)
		{
			StatModifierAdd addMod = new StatModifierAdd(value, 2);
			SpawnIcon(iconStart, npBall.Owner);
			TimeTicker.I.InvokeInTime((() => { SpawnIcon(iconFinish, npBall.Owner); }), time);
			npBall.Owner.Stats.GetStat(statToChange).AddTemporalMod(addMod, time);
		}

		private void SpawnIcon(GameObject icon, Ball ball)
		{
			Debug.Log("spawn");
			Vector3 pos = ball.GetPosition();
			pos = new Vector3(pos.x, pos.y + 5);
			Instantiate(icon, ball.GetPosition(), quaternion.identity);
		}
	}
}