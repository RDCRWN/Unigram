// <auto-generated/>
using System;

namespace Telegram.Api.TL
{
	public partial class TLUpdateMessageID : TLUpdateBase 
	{
		public Int32 Id { get; set; }
		public Int64 RandomId { get; set; }

		public TLUpdateMessageID() { }
		public TLUpdateMessageID(TLBinaryReader from, bool cache = false)
		{
			Read(from, cache);
		}

		public override TLType TypeId { get { return TLType.UpdateMessageID; } }

		public override void Read(TLBinaryReader from, bool cache = false)
		{
			Id = from.ReadInt32();
			RandomId = from.ReadInt64();
			if (cache) ReadFromCache(from);
		}

		public override void Write(TLBinaryWriter to, bool cache = false)
		{
			to.Write(0x4E90BFD6);
			to.Write(Id);
			to.Write(RandomId);
			if (cache) WriteToCache(to);
		}
	}
}