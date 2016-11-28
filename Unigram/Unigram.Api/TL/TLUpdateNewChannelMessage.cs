// <auto-generated/>
using System;

namespace Telegram.Api.TL
{
	public partial class TLUpdateNewChannelMessage : TLUpdateBase, ITLMultiPts 
	{
		public TLMessageBase Message { get; set; }
		public Int32 Pts { get; set; }
		public Int32 PtsCount { get; set; }

		public TLUpdateNewChannelMessage() { }
		public TLUpdateNewChannelMessage(TLBinaryReader from, bool cache = false)
		{
			Read(from, cache);
		}

		public override TLType TypeId { get { return TLType.UpdateNewChannelMessage; } }

		public override void Read(TLBinaryReader from, bool cache = false)
		{
			Message = TLFactory.Read<TLMessageBase>(from, cache);
			Pts = from.ReadInt32();
			PtsCount = from.ReadInt32();
			if (cache) ReadFromCache(from);
		}

		public override void Write(TLBinaryWriter to, bool cache = false)
		{
			to.Write(0x62BA04D9);
			to.WriteObject(Message, cache);
			to.Write(Pts);
			to.Write(PtsCount);
			if (cache) WriteToCache(to);
		}
	}
}