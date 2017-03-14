using System;
using System.Net;
using System.Net.Http;
using System.Collections.Generic;
using IPP.Enums;
using IPP.Tags;

namespace IPP
{
	public class Printer
	{
		public Printer(String uri)
		{
			this.URI = new Uri(uri);
			this._Client = new HttpClient();

			var ippUri = new UriBuilder(this.URI);
			ippUri.Scheme = "ipp";

			this.IPP_URI = ippUri.Uri;

			var attributes = this._GetPrinterAttributes();

			this._SupportedMimeTypes = new List<String>();
			this._SupportedMedia = new List<String>();
			this._SupportedOrientations = new List<string>();

			foreach(IPP.Attribute attr in attributes.AttrGroup[AttrGroups.PrinterAttributesTag])
			{
				if (attr.Tag == Tag.MimeMediaType && attr.Name.CompareTo("document-format-supported") == 0) {
					this._SupportedMimeTypes.Add((String)attr.ValueAsObject);
				}

				if (attr.Tag == Tag.Keyword && attr.Name.CompareTo("media-supported") == 0)
				{
					this._SupportedMedia.Add((String)attr.ValueAsObject);
				}

				if (attr.Tag == Tag.Keyword && attr.Name.CompareTo("orientation-requested-supported") == 0)
				{
					this._SupportedOrientations.Add((String)attr.ValueAsObject);
				}
			}
		}

		public Uri URI;
		protected Uri IPP_URI;
		protected HttpClient _Client;
		protected uint _RequestCounter = 0;
		protected List<String> _SupportedMimeTypes;
		protected List<String> _SupportedMedia;
		protected List<String> _SupportedOrientations;

		public IPP Send(IPP request)
		{
			request.RequestId = ++this._RequestCounter;

			request.AttrGroup[AttrGroups.OperationAttributesTag].Add(new IPP.Attribute(Tag.URI, this.IPP_URI.ToString()));

			var content = new ByteArrayContent(request.ToByte());
			content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/ipp");

			var task = this._Client.PostAsync(this.URI, content);

			task.Wait();

			var res = task.Result;

			if (res.StatusCode != HttpStatusCode.OK)
			{
				throw new Exception(res.ReasonPhrase);
			}

			var conTask = res.Content.ReadAsByteArrayAsync();
			conTask.Wait();

			var response = new IPP(conTask.Result);

			return response;
		}

		public IPP Send(Job job)
		{
			if (!this._SupportedMimeTypes.Contains(job.MimeType)) {
				job.MimeType = "application/octet-stream";
			}


			var binary = job.Request.Data;
			job.Request.Data = null;

			job.Request.OperationId = OperationId.ValidateJob;

			var ipp = this.Send(job.Request);

			if (ipp.StatusCode > StatusCodes.SuccessfulOKConflictingAttributes)
				throw new Exception("Failed To Validate");
			

			job.Request.Data = binary;

			job.Request.OperationId = OperationId.PrintJob;

			ipp = this.Send(job.Request);

			if (ipp.StatusCode > StatusCodes.SuccessfulOKConflictingAttributes)
			{
				throw new Exception("(" + ipp.StatusCode.ToString() + ")");
			}

			job.RecieveResponse(ipp);

			return ipp;
		}

		public void UpdateJobState(Job job)
		{
			var ipp = new IPP();
			ipp.OperationId = OperationId.GetJobAttributes;
			ipp.AttrGroup[AttrGroups.OperationAttributesTag].Add(new IPP.Attribute(Tag.URI, "job-uri", job.JobURI));

			job.RecieveResponse(this.Send(ipp));
		}

		protected IPP _GetPrinterAttributes()
		{
			var ipp = new IPP();

			ipp.OperationId = OperationId.GetPrinterAttributes;

			ipp.AttrGroup[AttrGroups.OperationAttributesTag].Add(
				new IPP.Attribute(Tag.Keyword, "requested-attributes", "document-format-supported")
			);

			ipp.AttrGroup[AttrGroups.OperationAttributesTag].Add(
				new IPP.Attribute(Tag.Keyword, "", "media-supported")
			);

			ipp.AttrGroup[AttrGroups.OperationAttributesTag].Add(
				new IPP.Attribute(Tag.Keyword, "", "orientation-requested-supported")
			);

			ipp.AttrGroup[AttrGroups.OperationAttributesTag].Add(
				new IPP.Attribute(Tag.Keyword, "", "print-quality-supported")
			);

			return this.Send(ipp);
		}
	}
}
