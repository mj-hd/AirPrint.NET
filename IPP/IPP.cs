using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using MiscUtil.IO;
using MiscUtil.Conversion;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

using IPP.Tags;
using IPP.Enums;


namespace IPP
{
	public enum StatusCodes : UInt16
	{
		// Success
		SuccessfulOK = 0x0000,
		SuccessfulOKIgnoredOrSubstitutedAttributes,
		SuccessfulOKConflictingAttributes,
		// Redirection Not Defined Yet,
		// Client Error
		ClientErrorBadRequest = 0x0400,
		ClientErrorForbidden,
		ClientErrorNotAuthenticated,
		ClientErrorNotAuthorized,
		ClientErrorNotPossible,
		ClientErrorTimeout,
		ClientErrorNotFound,
		ClientErrorGone,
		ClientErrorRequestEntityTooLarge,
		ClientErrorRequestValueTooLong,
		ClientErrorDocumentFormatNotSupported,
		ClientErrorAttributesOrValuesNotSupported,
		ClientErrorUriSchemeNotSupported,
		ClientErrorCharsetNotSupported,
		ClientErrorConflictingAttributes,
		ClientErrorCompressionNotSupported,
		ClientErrorCompressionError,
		ClinetErrorDocumentFormatError,
		ClientErrorDocumentAccessError,
		// Server Error
		ServerErrorInternalError = 0x0500,
		ServerErrorOperationNotSupported,
		ServerErrorServiceUnavailable,
		ServerErrorVersionNotSupported,
		ServerErrorDeviceError,
		ServerErrorTemporaryError,
		ServerErrorNotAcceptingJobs,
		ServerErrorBusy,
		ServerErrorJobCanceled,
		ServerErrorMultipleDocumentJobsNotSupported,
		// Default
		None = 0xFFFF
	}

	public enum AttrGroups : byte
	{
		OperationAttributesTag = 0x01,
		JobAttributesTag,
		EndOfAttributesTag,
		PrinterAttributesTag,
		UnsupportedAttributesTag,
		SubscriptionAttributesTag,
		EventNotificationAttributesTag,
		ResourceAttributesTag,
		DocumentAttributesTag,
	}

	public class IPP
	{

		public struct Attribute
		{
			public Attribute(Tag tag, String value)
				: this(tag, TagsExt.ToString(tag), value)
			{
			}

			public Attribute(Tag tag, String name, String value)
			{
				this.Tag = tag;
				this.Name = name;
				this.Value = System.Text.Encoding.UTF8.GetBytes(value);
			}

			public Attribute(Tag tag, String name, UInt32 value)
			{
				this.Tag = tag;
				this.Name = name;
				this.Value = EndianBitConverter.Big.GetBytes(value);
			}

			public Tag Tag;
			public String Name;
			public Byte[] Value;

			public dynamic ValueAsObject
			{
				get
				{
					switch (this.Tag)
					{
						case Tag.Enum:
							if (this.Type == typeof(object)) return EndianBitConverter.Big.ToUInt32(this.Value, 0);
							return Enum.ToObject(this.Type, EndianBitConverter.Big.ToUInt32(this.Value, 0));
						case Tag.Integer:
							return EndianBitConverter.Big.ToInt32(this.Value, 0);
						case Tag.Boolean:
							return BitConverter.ToBoolean(this.Value, 0);
						case Tag.RangeOfInteger:
							return new RangeOfInteger(this.Value);
						case Tag.Resolution:
							return new Resolution(this.Value);
						case Tag.DateTime:
							return new DateTime(this.Value);
						case Tag.TextWithLanguage:
						case Tag.NameWithLanguage:
							return new TextWithLanguage(this.Value);
						case Tag.TextWithoutLanguage:
						case Tag.NameWithoutLanguage:
						case Tag.OctetString:
						case Tag.MemberAttrName:
						case Tag.Keyword:
						case Tag.URI:
						case Tag.URIScheme:
						case Tag.Charset:
						case Tag.NaturalLanguage:
						case Tag.MimeMediaType:
						//case Tags.BegCollection: // TODO
						//	return typeof();
						default:
							return System.Text.Encoding.UTF8.GetString(this.Value, 0, this.Value.Length);
					}
				}
			}

			public Type Type
			{
				get
				{
					switch (this.Tag)
					{
						case Tag.Enum:

							switch (this.Name)
							{
								case "operations-supported":
									return typeof(OperationId);
								case "printer-state":
									return typeof(PrinterState);
								case "job-state":
									return typeof(JobState);
								case "orientation-requested":
								case "orientation-requested-supported":
									return typeof(OrientationRequested);
							}

							return typeof(object);
						case Tag.Integer:
							return typeof(Int32);
						case Tag.Boolean:
							return typeof(Boolean);
						case Tag.RangeOfInteger:
							return typeof(RangeOfInteger);
						case Tag.Resolution:
							return typeof(Resolution);
						case Tag.DateTime:
							return typeof(DateTime);
						case Tag.TextWithLanguage:
						case Tag.NameWithLanguage:
							return typeof(TextWithLanguage);
						case Tag.TextWithoutLanguage:
						case Tag.NameWithoutLanguage:
						case Tag.OctetString:
						case Tag.MemberAttrName:
							return typeof(String);
						case Tag.Keyword:
						case Tag.URI:
						case Tag.URIScheme:
						case Tag.Charset:
						case Tag.NaturalLanguage:
						case Tag.MimeMediaType:
							return typeof(String);
						case Tag.BegCollection: // TODO
							return typeof(object);
						default:
							return typeof(String);
					}
				}
			}

			public struct SetOf
			{
				
			}


			public struct RangeOfInteger
			{
				public RangeOfInteger(byte[] data)
				{
					this.Start = EndianBitConverter.Big.ToInt32(data, 0);
					this.End = EndianBitConverter.Big.ToInt32(data, 4);
				}

				public new String ToString()
				{
					var result = new StringWriter();

					result.Write("({0} - {1})", this.Start, this.End);

					return result.ToString();
				}

				public Int32 Start;
				public Int32 End;
			}

			public struct Resolution
			{
				public Resolution(byte[] data)
				{
					this.Width = EndianBitConverter.Big.ToInt32(data, 0);
					this.Height = EndianBitConverter.Big.ToInt32(data, 4);
					this.Unit = (Units)data[8];
				}

				public enum Units : Byte
				{
					DPI = 0x03,
					DPCM
				}

				public new String ToString()
				{
					var result = new StringWriter();

					result.Write("({0}, {1}) {2}", this.Width, this.Height, this.Unit);

					return result.ToString();
				}

				public Int32 Width;
				public Int32 Height;
				public Units Unit;
			}

			public struct DateTime
			{
				public UInt16 Year;
				public Byte Month;
				public Byte Day;
				public Byte Hour;
				public Byte Minutes;
				public Byte Seconds;
				public Byte DeciSeconds;
				public Byte DirectionFromUTC;
				public Byte HoursFromUTC;
				public Byte MinutesFromUTC;

				public DateTime(byte[] data)
				{
					this.Year = EndianBitConverter.Big.ToUInt16(data, 0);
					this.Month = data[2];
					this.Day = data[3];
					this.Hour = data[4];
					this.Minutes = data[5];
					this.Seconds = data[6];
					this.DeciSeconds = data[7];
					this.DirectionFromUTC = data[8];
					this.HoursFromUTC = data[9];
					this.MinutesFromUTC = data[10];
				}

				public System.DateTime ToDateTime()
				{
					var result = new System.DateTime(
						this.Year,
						this.Month,
						this.Day,
						this.Hour,
						this.Minutes,
						this.Seconds);

					return result;
				}

				public new String ToString()
				{
					return this.ToDateTime().ToString();
				}
			}

			public struct TextWithLanguage
			{
				public String Language;
				public String Text;

				public TextWithLanguage(byte[] data)
				{
					var languageLength = (int)data[0];
					this.Language = System.Text.Encoding.UTF8.GetString(data, 1, languageLength);
					var textLength = (int)data[languageLength + 1];
					this.Text = System.Text.Encoding.UTF8.GetString(data, languageLength + 1, textLength);
				}

				public new String ToString()
				{
					var result = new StringWriter();

					result.Write("[{0}, {1}]", this.Language, this.Text);

					return result.ToString();
				}
			}
		}

		public static String Language = "ja-jp";
		public static string Encoding = "utf-8";

		public IPP()
		{
			this.AttrGroup = new Dictionary<AttrGroups, List<Attribute>>();

			this.AttrGroup.Add(AttrGroups.OperationAttributesTag,
			                   new System.Collections.Generic.List<Attribute>());

			this.AttrGroup[AttrGroups.OperationAttributesTag].Add(
				new IPP.Attribute(Tag.Charset, IPP.Encoding)
			);

			this.AttrGroup[AttrGroups.OperationAttributesTag].Add(
				new IPP.Attribute(Tag.NaturalLanguage, IPP.Language)
			);
		}

		public IPP(Byte[] bytes)
		{
			var stream = new MemoryStream(bytes);

			this.Read(new EndianBinaryReader(EndianBitConverter.Big, stream));
		}

		public byte[] ToByte()
		{
			var stream = new MemoryStream();

			this.Write(new EndianBinaryWriter(EndianBitConverter.Big, stream));

			return stream.ToArray();

		}

		public void Write(EndianBinaryWriter writer)
		{
			// Version 2 bytes Required
			writer.Write((Byte)(this.Version / 10));
			writer.Write((Byte)(this.Version % 10));

			// OperationID 2 bytes Required
			writer.Write((UInt16)this.OperationId);

			// RequestId
			writer.Write(this.RequestId);

			// Attr Groups
			foreach (KeyValuePair<AttrGroups,
					List<Attribute>> pair in this.AttrGroup)
			{
				// Attr Group Name 1 byte
				writer.Write((Byte)pair.Key);

				// Values
				foreach (Attribute attr in pair.Value)
				{
					// Tag Name 1 byte
					writer.Write((Byte)attr.Tag);
					if (attr.Tag == Tag.Extension)
					{
						writer.Write((int)0); // TODO: write something
					}

					// Name
					writer.Write(attr.Name, false);

					// Value Length 2 byte
					writer.Write((UInt16)attr.Value.Length);

					// Value
					writer.Write(attr.Value);
				}
			}

			writer.Write((Byte)AttrGroups.EndOfAttributesTag);

			if (this.Data != null && this.Data.Length > 0)
				writer.Write(this.Data);
		}

		public void Read(EndianBinaryReader reader)
		{
			// Version 2 bytes Required
			this.Version = (UInt16)((UInt16)reader.ReadByte() * 10);
			this.Version += (UInt16)reader.ReadByte();

			// StatusCode 2 bytes Required
			this.StatusCode = (StatusCodes)reader.ReadUInt16();

			// RequestId
			this.RequestId = reader.ReadUInt32();

			this.AttrGroup = new Dictionary<AttrGroups, List<Attribute>>();

			// Attr Group
			while (reader.BaseStream.Position < reader.BaseStream.Length &&
				   reader.PeekByte() != (Byte)AttrGroups.EndOfAttributesTag)
			{
				// Attr Group Name 1 byte
				var attrGroupName = (AttrGroups)reader.ReadByte();

				this.AttrGroup[attrGroupName] = new List<Attribute>();

				// Tag
				// TODO: support 00 00
				// TODO: support beg-collection
				var prev_name = "";
				while (reader.PeekByte() >= (Byte)Tag.Unsupported)
				{
					var tagName = (Tag)reader.ReadByte();
					if (tagName == Tag.Extension)
					{
						reader.ReadBytes(4);
					}

					var nameLength = reader.ReadUInt16();
					var name = System.Text.Encoding.UTF8.GetString(reader.ReadBytes(nameLength), 0, nameLength);

					var valueLength = reader.ReadUInt16();
					var value = reader.ReadBytes(valueLength);

					if (tagName == Tag.MemberAttrName)
					{
						name = System.Text.Encoding.UTF8.GetString(value, 0, valueLength);
					}

					if (nameLength == 0)
					{
						name = prev_name;
					}
					else {
						prev_name = name;
					}

					this.AttrGroup[attrGroupName].Add(new Attribute
					{
						Tag = tagName,
						Name = name,
						Value = value,
					});
				}
			}

			if (reader.BaseStream.Position != reader.BaseStream.Length)
			{
				// TODO: support long cast, check length
				this.Data = reader.ReadBytes((int)(reader.BaseStream.Length - reader.BaseStream.Position));
			}

			return;
		}

		public new String ToString()
		{
			var result = new StringWriter();

			result.WriteLine("Version: {0}", this.Version);
			result.WriteLine("OperationID: {0}", OperationIdExt.ToString(this.OperationId));
			result.WriteLine("StatusCode: {0}", this.StatusCode.ToString());

			foreach (KeyValuePair<AttrGroups, List<Attribute>> pair in this.AttrGroup)
			{
				result.WriteLine("{0} [", pair.Key.ToString());

				foreach (Attribute attr in pair.Value)
				{
					result.WriteLine("  {0} [", TagsExt.ToString(attr.Tag));
					result.WriteLine("         Name: {0}", attr.Name);
					result.WriteLine("         Value: {0}", attr.ValueAsObject.ToString());
					result.WriteLine("  ]");
				}

				result.WriteLine("]");
			}

			if (this.Data != null && this.Data.Length > 0)
				result.WriteLine("Data: Size={0}", this.Data.Length);

			return result.ToString();
		}

		public UInt16 Version = 20;
		public OperationId OperationId = OperationId.None;
		public StatusCodes StatusCode = StatusCodes.None;
		public UInt32 RequestId = 0;
		public Byte[] Data = null;

		public Dictionary<AttrGroups,
					List<Attribute>> AttrGroup;

	}
}
