// <auto-generated/>
using System;

namespace Telegram.Api.TL
{
	public partial class TLInputPeerChannel : TLInputPeerBase 
	{
		public Int32 ChannelId { get; set; }
		public Int64 AccessHash { get; set; }

		public TLInputPeerChannel() { }
		public TLInputPeerChannel(TLBinaryReader from, bool cache = false)
		{
			Read(from, cache);
		}

		public override TLType TypeId { get { return TLType.InputPeerChannel; } }

		public override void Read(TLBinaryReader from, bool cache = false)
		{
			ChannelId = from.ReadInt32();
			AccessHash = from.ReadInt64();
			if (cache) ReadFromCache(from);
		}

		public override void Write(TLBinaryWriter to, bool cache = false)
		{
			to.Write(0x20ADAEF8);
			to.Write(ChannelId);
			to.Write(AccessHash);
			if (cache) WriteToCache(to);
		}
	}
}