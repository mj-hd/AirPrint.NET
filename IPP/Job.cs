using System;
using System.Linq;
using IPP.Tags;
using IPP.Enums;

namespace IPP
{
	public class Job
	{
		public Job()
		{
			this.Request = new IPP();
		}

		public IPP Request;
		public String MimeType;

		public int JobID;
		public String JobURI;
		public DocumentState State = DocumentState.None;
		public String JobName;
		public String UserName;

		public String MediaSize {
			get
			{
				var result = this._GetAttributeValue(AttrGroups.JobAttributesTag, "media");

				if (result == null || result.GetType() != typeof(String)) return "default";

				return (String)result;
			}
			set
			{
				this._SetAttribute(AttrGroups.JobAttributesTag, new IPP.Attribute(Tag.Keyword, "media", value));
			}
		}

		public OrientationRequested Orientation {
			get {
				var result = (OrientationRequested)this._GetAttributeValue(AttrGroups.JobAttributesTag, "orientation-requested");

				if (result.GetType() != typeof(OrientationRequested)) return OrientationRequested.None;

				return result;
			}
			set
			{
				this._SetAttribute(AttrGroups.JobAttributesTag, new IPP.Attribute(Tag.Enum, "orientation-requested", (UInt32)value));
			}
		}

		public PrintQuality Quality {
			get {
				var result = (PrintQuality)this._GetAttributeValue(AttrGroups.JobAttributesTag, "orientation-requested");

				if (result.GetType() != typeof(PrintQuality)) return PrintQuality.Draft;

				return result;
			}
			set
			{
				this._SetAttribute(AttrGroups.JobAttributesTag, new IPP.Attribute(Tag.Enum, "print-quality", (UInt32)value));
			}
		}

		protected dynamic _GetAttributeValue(AttrGroups group, String name)
		{
			if (this.Request.AttrGroup.ContainsKey(group))
			{
				foreach (IPP.Attribute attr in this.Request.AttrGroup[group])
				{
					if (attr.Name.CompareTo(name) == 0)
					{
						return attr.ValueAsObject;
					}
				}
			}

			return null;
		}

		protected void _SetAttribute(AttrGroups group, IPP.Attribute attribute)
		{
			if (this.Request.AttrGroup.ContainsKey(group))
			{
				var index = -1;
				foreach (var attr in this.Request.AttrGroup[group].Select((v, i) => new { v, i }))
				{
					if (attr.v.Name.CompareTo(attribute.Name) == 0)
					{
						index = attr.i;
					}
				}

				if (index < 0)
				{
					this.Request.AttrGroup[group].Add(attribute);
				}
				else {
					this.Request.AttrGroup[group][index] = attribute;
				}
			}
			else {
				this.Request.AttrGroup.Add(group, new System.Collections.Generic.List<IPP.Attribute>());
				this.Request.AttrGroup[group].Add(attribute);
			}
		}

		public static Job PrintJob(string userName, string jobName, string mimeType, byte[] data)
		{
			var ipp = new IPP();

			ipp.OperationId = OperationId.PrintJob;

			ipp.AttrGroup[AttrGroups.OperationAttributesTag].Add(
				new IPP.Attribute(Tag.NameWithoutLanguage, userName)
			);

			ipp.AttrGroup[AttrGroups.OperationAttributesTag].Add(
				new IPP.Attribute(Tag.NameWithoutLanguage, jobName)
			);

			ipp.AttrGroup[AttrGroups.OperationAttributesTag].Add(
				new IPP.Attribute(Tag.MimeMediaType, mimeType)
			);

			ipp.Data = data;

			return new Job()
			{
				Request = ipp,
				MimeType = mimeType,
				JobName = jobName,
				UserName = userName
			};
		}

		public void RecieveResponse(IPP ipp)
		{
			foreach(IPP.Attribute attr in ipp.AttrGroup[AttrGroups.JobAttributesTag])
			{
				if (attr.Tag == Tag.Integer && attr.Name.CompareTo("job-id") == 0)
				{
					this.JobID = (int)attr.ValueAsObject;
				}
				if (attr.Tag == Tag.URI && attr.Name.CompareTo("job-uri") == 0)
				{
					this.JobURI = (String)attr.ValueAsObject;
				}
				if (attr.Tag == Tag.Enum && attr.Name.CompareTo("job-state") == 0)
				{
					this.State = (DocumentState)attr.ValueAsObject;
				}
			}
		}
	}
}
