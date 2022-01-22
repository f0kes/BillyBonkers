using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameState
{
	public class WinConditionChecker : MonoBehaviour
	{
		private List<WinCondition> _conditions = new List<WinCondition>();

		public void AddWinCondition(WinCondition condition)
		{
			_conditions.Add(condition);
		}

		private void Update()
		{
			foreach (var con in _conditions)
			{
				
			}
		}
	}
}