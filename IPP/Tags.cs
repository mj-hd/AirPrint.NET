using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;


namespace IPP
{
	namespace Tags
	{
		public enum Tag : Byte
		{
			[Display(Name = "unsupported")]
			Unsupported = 0x10,
			[Display(Name = "default")]
			Default,
			[Display(Name = "unknown")]
			Unknown,
			[Display(Name = "no-value")]
			NoValue,
			[Display(Name = "no-settable")]
			NotSettable = 0x15,
			[Display(Name = "delete-attributes")]
			DeleteAttributes,
			[Display(Name = "admin-define")]
			AdminDefine,
			[Display(Name = "integer")]
			Integer = 0x21,
			[Display(Name = "boolean")]
			Boolean,
			[Display(Name = "enum")]
			Enum,
			[Display(Name = "octetString")]
			OctetString = 0x30,
			[Display(Name = "datetime")]
			DateTime,
			[Display(Name = "resolution")]
			Resolution,
			[Display(Name = "rangeOfInteger")]
			RangeOfInteger,
			[Display(Name = "begCollection")]
			BegCollection,
			[Display(Name = "textWithLanguage")]
			TextWithLanguage,
			[Display(Name = "nameWithLanguage")]
			NameWithLanguage,
			[Display(Name = "endEollection")]
			EndCollection,
			[Display(Name = "textWithoutLanguage")]
			TextWithoutLanguage = 0x41,
			[Display(Name = "nameWithoutLanguage")]
			NameWithoutLanguage,
			[Display(Name = "keyword")]
			Keyword = 0x44,
			[Display(Name = "printer-uri")]
			URI,
			[Display(Name = "uriScheme")]
			URIScheme,
			[Display(Name = "attributes-charset")]
			Charset,
			[Display(Name = "attributes-natural-language")]
			NaturalLanguage,
			[Display(Name = "mimeMediaType")]
			MimeMediaType,
			[Display(Name = "memberAttributeName")]
			MemberAttrName,
			[Display(Name = "extension")]
			Extension = 0x7F,
			[Display(Name = "none")]
			None = 0xFF
		}

		public static class TagsExt
		{
			public static String ToString(this Tag value)
			{
				var fieldInfo = value.GetType().GetRuntimeField(value.ToString());
				var descriptionAttributes = fieldInfo.GetCustomAttributes(
					typeof(DisplayAttribute), false) as DisplayAttribute[];
				if (descriptionAttributes != null && descriptionAttributes.Length > 0)
				{
					return descriptionAttributes[0].Name;
				}
				else {
					return value.ToString();
				}
			}
		}
	}
}
