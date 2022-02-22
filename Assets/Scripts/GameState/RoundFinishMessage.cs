using Enums;

namespace GameState
{
	public struct RoundFinishMessage
	{
		public Player Winner { get; private set; }
		public bool Tie{ get;  }
		public  RoundFinishType Type { get; private set; }
		
		public string Context { get; }

		public RoundFinishMessage(bool tie, Player winner, RoundFinishType type, string context)
		{
			Tie = tie;
			Winner = winner;
			Type = type;
			Context = context;
		}
	}
}