using System.Linq;
using Enums;

namespace GameState
{
	public abstract class WinCondition
	{
		public abstract bool Check(out RoundFinishMessage finishMessage);
	}

	public class OneSurvivorWinCondition : WinCondition
	{
		public override bool Check(out RoundFinishMessage finishMessage)
		{
			int playersAlive = 0;
			Player contender = null;
			foreach (var p in Player.Players.Where(p => p.RoundStarted && !p.Dead))
			{
				contender = p;
				playersAlive += 1;
				if (playersAlive > 1)
				{
					finishMessage = default;
					return false;
				}
			}

			if (playersAlive == 0)
			{
				finishMessage = new RoundFinishMessage(true, contender, RoundFinishType.Tie);
				return true;	
			}
			finishMessage = new RoundFinishMessage(false, contender, RoundFinishType.OneSurvivor);
			return true;
		}
	}
}