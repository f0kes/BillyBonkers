using Enums;

namespace GameState
{
	public struct RoundFinishMessage
	{
		public Player Winner { get; private set; }
		public bool Tie{ get;  }
		public  RoundFinishType Type { get; private set; }

		public RoundFinishMessage(bool tie, Player winner, RoundFinishType type)
		{
			Tie = tie;
			Winner = winner;
			Type = type;
		}
	}
}