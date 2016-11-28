// <auto-generated/>
using System;

namespace Telegram.Api.TL
{
	public partial class TLInputEncryptedFile : TLInputEncryptedFileBase 
	{
		public Int64 Id { get; set; }
		public Int64 AccessHash { get; set; }

		public TLInputEncryptedFile() { }
		public TLInputEncryptedFile(TLBinaryReader from, bool cache = false)
		{
			Read(from, cache);
		}

		public override TLType TypeId { get { return TLType.InputEncryptedFile; } }

		public override void Read(TLBinaryReader from, bool cache = false)
		{
			Id = from.ReadInt64();
			AccessHash = from.ReadInt64();
			if (cache) ReadFromCache(from);
		}

		public override void Write(TLBinaryWriter to, bool cache = false)
		{
			to.Write(0x5A17B5E5);
			to.Write(Id);
			to.Write(AccessHash);
			if (cache) WriteToCache(to);
		}
	}
}