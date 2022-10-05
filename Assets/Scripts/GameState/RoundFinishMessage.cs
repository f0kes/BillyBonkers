using System.Collections.Generic;
using Enums;
using Mirror;

namespace GameState
{
	public struct RoundFinishMessage
	{
		public Player Winner { get; private set; }
		public bool Tie { get; }
		public RoundFinishType Type { get; private set; }

		public string Context { get; }

		public RoundFinishMessage(bool tie, Player winner, RoundFinishType type, string context)
		{
			Tie = tie;
			Winner = winner;
			Type = type;
			Context = context;
		}
	}

	public static class RoundFinishMessageReadWrite
	{
		public static void WriteMyType(this NetworkWriter writer, RoundFinishMessage value)
		{
			writer.WriteBool(value.Tie);
			writer.WriteUShort((ushort) value.Winner);
			writer.WriteUShort((ushort) value.Type);
			writer.WriteString(value.Context);
		}

		public static RoundFinishMessage ReadMyType(this NetworkReader reader)
		{
			return new RoundFinishMessage(reader.ReadBool(), (Player) reader.ReadUShort(),
				(RoundFinishType) reader.ReadUShort(), reader.ReadString());
		}
	}
}