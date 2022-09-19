using System.Collections.Generic;
using Entities;
using Enums;
using GameState;
using Structures;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using Visuals;

namespace NPBallEffect
{
	[CreateAssetMenu(fileName = "New Buff Owner NPBallEffect", menuName = "Buff Owner NPBallEffect", order = 2)]
	public class BuffOwnerOnScoreEffect : Effect
	{
		[System.Serializable]
		public struct StatEffect
		{
			public BallStat ToChange;
			public float Value;
		}
		[SerializeField] private BallStat statToChange;
		[SerializeField] private List<StatEffect> statsToChange = new List<StatEffect>();
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
			npBall.Owner.Stats.GetStat(statToChange).AddTemporalMod(addMod, time);
			foreach (var statEffect in statsToChange)
			{
				 addMod = new StatModifierAdd(statEffect.Value*value, 2);
				npBall.Owner.Stats.GetStat(statEffect.ToChange).AddTemporalMod(addMod, time);
				npBall.Owner.SetOutlineIntensity(npBall.Owner.OutlineIntensity+5);
			}
			
			SpawnIcon(iconStart, npBall.Owner);
			TimeTicker.I.InvokeInTime((() => { OnBuffEnd(iconFinish, npBall.Owner); }), time);
			
		}

		private static void OnBuffEnd(GameObject icon, Ball ball)
		{
			SpawnIcon(icon,ball);
			ball.SetOutlineIntensity(ball.OutlineIntensity - 5);
		}

		private static void SpawnIcon(GameObject icon, Ball ball)
		{
			Vector3 pos = ball.GetPosition();
			pos = new Vector3(pos.x, pos.y + 5, pos.z);
			Instantiate(icon, pos, quaternion.identity);
		}
	}
}