using System.Collections.Generic;

namespace Networking
{
	public class Snapshot
	{
		private Dictionary<ushort,byte[]> _data = new Dictionary<ushort, byte[]>();
		public static Snapshot FromMessages(Message[] messages)
		{
			Snapshot snapshot = new Snapshot();
			foreach (Message message in messages)
			{
				snapshot._data.Add(message.Id, message.Data);
			}
			return snapshot;
		}
	}
}