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
				finishMessage = new RoundFinishMessage(true, contender, RoundFinishType.Tie, "Tie");
				return true;
			}

			finishMessage = new RoundFinishMessage(false, contender, RoundFinishType.OneSurvivor, "One survivor left");
			return true;
		}
	}

	public class ScoreWinCondition : WinCondition
	{
		private int _scoreToWin;

		public ScoreWinCondition(int scoreToWin)
		{
			_scoreToWin = scoreToWin;
		}

		public override bool Check(out RoundFinishMessage finishMessage)
		{
			foreach (var p in Player.Players.Where(p => p.RoundStarted && !p.Dead))
			{
				if (p.PlayerBall.ScoredBalls >= _scoreToWin)
				{
					finishMessage = new RoundFinishMessage(false, p, RoundFinishType.EnoughBalls,
						"Enough balls were scored");
					return true;
				}
			}

			finishMessage = default;
			return false;
		}
	}
}