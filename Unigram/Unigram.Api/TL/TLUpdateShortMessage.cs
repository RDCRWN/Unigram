// <auto-generated/>
using System;

namespace Telegram.Api.TL
{
	public partial class TLUpdateShortMessage : TLUpdatesBase, ITLMultiPts 
	{
		[Flags]
		public enum Flag : Int32
		{
			Out = (1 << 1),
			Mentioned = (1 << 4),
			MediaUnread = (1 << 5),
			Silent = (1 << 13),
			FwdFrom = (1 << 2),
			ViaBotId = (1 << 11),
			ReplyToMsgId = (1 << 3),
			Entities = (1 << 7),
		}

		public bool IsOut { get { return Flags.HasFlag(Flag.Out); } set { Flags = value ? (Flags | Flag.Out) : (Flags & ~Flag.Out); } }
		public bool IsMentioned { get { return Flags.HasFlag(Flag.Mentioned); } set { Flags = value ? (Flags | Flag.Mentioned) : (Flags & ~Flag.Mentioned); } }
		public bool IsMediaUnread { get { return Flags.HasFlag(Flag.MediaUnread); } set { Flags = value ? (Flags | Flag.MediaUnread) : (Flags & ~Flag.MediaUnread); } }
		public bool IsSilent { get { return Flags.HasFlag(Flag.Silent); } set { Flags = value ? (Flags | Flag.Silent) : (Flags & ~Flag.Silent); } }
		public bool HasFwdFrom { get { return Flags.HasFlag(Flag.FwdFrom); } set { Flags = value ? (Flags | Flag.FwdFrom) : (Flags & ~Flag.FwdFrom); } }
		public bool HasViaBotId { get { return Flags.HasFlag(Flag.ViaBotId); } set { Flags = value ? (Flags | Flag.ViaBotId) : (Flags & ~Flag.ViaBotId); } }
		public bool HasReplyToMsgId { get { return Flags.HasFlag(Flag.ReplyToMsgId); } set { Flags = value ? (Flags | Flag.ReplyToMsgId) : (Flags & ~Flag.ReplyToMsgId); } }
		public bool HasEntities { get { return Flags.HasFlag(Flag.Entities); } set { Flags = value ? (Flags | Flag.Entities) : (Flags & ~Flag.Entities); } }

		public Flag Flags { get; set; }
		public Int32 Id { get; set; }
		public Int32 UserId { get; set; }
		public String Message { get; set; }
		public Int32 Pts { get; set; }
		public Int32 PtsCount { get; set; }
		public Int32 Date { get; set; }
		public TLMessageFwdHeader FwdFrom { get; set; }
		public Int32? ViaBotId { get; set; }
		public Int32? ReplyToMsgId { get; set; }
		public TLVector<TLMessageEntityBase> Entities { get; set; }

		public TLUpdateShortMessage() { }
		public TLUpdateShortMessage(TLBinaryReader from, bool cache = false)
		{
			Read(from, cache);
		}

		public override TLType TypeId { get { return TLType.UpdateShortMessage; } }

		public override void Read(TLBinaryReader from, bool cache = false)
		{
			Flags = (Flag)from.ReadInt32();
			Id = from.ReadInt32();
			UserId = from.ReadInt32();
			Message = from.ReadString();
			Pts = from.ReadInt32();
			PtsCount = from.ReadInt32();
			Date = from.ReadInt32();
			if (HasFwdFrom) FwdFrom = TLFactory.Read<TLMessageFwdHeader>(from, cache);
			if (HasViaBotId) ViaBotId = from.ReadInt32();
			if (HasReplyToMsgId) ReplyToMsgId = from.ReadInt32();
			if (HasEntities) Entities = TLFactory.Read<TLVector<TLMessageEntityBase>>(from, cache);
			if (cache) ReadFromCache(from);
		}

		public override void Write(TLBinaryWriter to, bool cache = false)
		{
			UpdateFlags();

			to.Write(0x914FBF11);
			to.Write((Int32)Flags);
			to.Write(Id);
			to.Write(UserId);
			to.Write(Message);
			to.Write(Pts);
			to.Write(PtsCount);
			to.Write(Date);
			if (HasFwdFrom) to.WriteObject(FwdFrom, cache);
			if (HasViaBotId) to.Write(ViaBotId.Value);
			if (HasReplyToMsgId) to.Write(ReplyToMsgId.Value);
			if (HasEntities) to.WriteObject(Entities, cache);
			if (cache) WriteToCache(to);
		}

		private void UpdateFlags()
		{
			HasFwdFrom = FwdFrom != null;
			HasViaBotId = ViaBotId != null;
			HasReplyToMsgId = ReplyToMsgId != null;
			HasEntities = Entities != null;
		}
	}
}