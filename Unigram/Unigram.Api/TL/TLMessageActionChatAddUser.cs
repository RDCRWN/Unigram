// <auto-generated/>
using System;

namespace Telegram.Api.TL
{
	public partial class TLMessageActionChatAddUser : TLMessageActionBase 
	{
		public TLVector<Int32> Users { get; set; }

		public TLMessageActionChatAddUser() { }
		public TLMessageActionChatAddUser(TLBinaryReader from, bool cache = false)
		{
			Read(from, cache);
		}

		public override TLType TypeId { get { return TLType.MessageActionChatAddUser; } }

		public override void Read(TLBinaryReader from, bool cache = false)
		{
			Users = TLFactory.Read<TLVector<Int32>>(from, cache);
			if (cache) ReadFromCache(from);
		}

		public override void Write(TLBinaryWriter to, bool cache = false)
		{
			to.Write(0x488A7337);
			to.WriteObject(Users, cache);
			if (cache) WriteToCache(to);
		}
	}
}