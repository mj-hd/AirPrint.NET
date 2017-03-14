using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace IPP
{

	public static class FinishingExt
	{
		public static String ToString(this Enums.Finishing value)
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

	public static class DocumentStateExt
	{
		public static String ToString(this Enums.DocumentState value)
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


	public static class OperationIdExt
	{
		public static String ToString(this Enums.OperationId value)
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


	public static class JobCollationTypeExt
	{
		public static String ToString(this Enums.JobCollationType value)
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


	public static class JobStateExt
	{
		public static String ToString(this Enums.JobState value)
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


	public static class PrinterStateExt
	{
		public static String ToString(this Enums.PrinterState value)
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


	public static class PrintQualityExt
	{
		public static String ToString(this Enums.PrintQuality value)
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

	namespace Enums
	{
		public enum DocumentState : UInt32
		{
			[Display(Name = "pending")]
			Pending = 0x0003,
			[Display(Name = "processing")]
			Processing = 0x0005,
			[Display(Name = "canceled")]
			Canceled = 0x0007,
			[Display(Name = "aborted")]
			Aborted,
			[Display(Name = "completed")]
			Completed,
			[Display(Name = "none")]
			None = 0xFFFF
		}

		public enum Finishing : UInt32
		{
			[Display(Name = "none")]
			None = 0x0003,
			[Display(Name = "Staple")]
			Staple,
			[Display(Name = "punch")]
			Punch,
			[Display(Name = "cover")]
			Cover,
			[Display(Name = "bind")]
			Bind,
			[Display(Name = "saddle-stitch")]
			SaddleStitch,
			[Display(Name = "edge-stitch")]
			EdgeStitch,
			[Display(Name = "fold")]
			Fold,
			[Display(Name = "trim")]
			Trim,
			[Display(Name = "bale")]
			Bale,
			[Display(Name = "booklet-marker")]
			BookletMarker,
			[Display(Name = "jog-offset")]
			JogOffset,
			[Display(Name = "staple-top-left")]
			StapleTopLeft = 0x0014,
			[Display(Name = "staple-bottom-left")]
			StapleBottomLeft,
			[Display(Name = "staple-top-right")]
			StapleTopRight,
			[Display(Name = "staple-bottom-right")]
			StapleBottomRight,
			[Display(Name = "edge-stitch-left")]
			EdgeStitchLeft,
			[Display(Name = "edge-stitch-top")]
			EdgeStitchTop,
			[Display(Name = "edge-stitch-right")]
			EdgeStitchRight,
			[Display(Name = "edge-stitch-bottom")]
			EdgeStitchBottom,
			[Display(Name = "staple-dual-right")]
			StapleDualLeft,
			[Display(Name = "staple-dual-top")]
			StapleDualTop,
			[Display(Name = "staple-dual-right")]
			StapleDualRight,
			[Display(Name = "staple-dual-bottom")]
			StapleDualBottom,
			[Display(Name = "bind-left")]
			BindLeft = 0x0032,
			[Display(Name = "bind-top")]
			BindTop,
			[Display(Name = "bind-right")]
			BindRight,
			[Display(Name = "bind-bottom")]
			BindBottom,
			[Display(Name = "trim-after-pages")]
			TrimAfterPages = 0x003C,
			[Display(Name = "trim-after-documents")]
			TrimAfterDocuments,
			[Display(Name = "trim-after-copies")]
			TrimAfterCopies,
			[Display(Name = "trim-after-job")]
			TrimAfterJob
		}

		public enum OperationId : UInt32
		{
			[Display(Name = "print-job")]
			PrintJob = 0x0002,
			[Display(Name = "print-uri")]
			PrintURI,
			[Display(Name = "validate-job")]
			ValidateJob,
			[Display(Name = "create-job")]
			CreateJob,
			[Display(Name = "send-document")]
			SendDocument,
			[Display(Name = "send-uri")]
			SendURI,
			[Display(Name = "cancel-job")]
			CancelJob,
			[Display(Name = "get-job-attributes")]
			GetJobAttributes,
			[Display(Name = "get-jobs")]
			GetJobs,
			[Display(Name = "get-printer-attributes")]
			GetPrinterAttributes,
			[Display(Name = "hold-job")]
			HoldJob,
			[Display(Name = "release-job")]
			ReleaseJob,
			[Display(Name = "restart-job")]
			RestartJob,
			[Display(Name = "pause-printer")]
			PausePrinter = 0x0010,
			[Display(Name = "resume-printer")]
			ResumePrinter,
			[Display(Name = "purge-jobs")]
			PurgeJobs,
			[Display(Name = "set-printer-attributes")]
			SetPrinterAttributes,
			[Display(Name = "set-job-attributes")]
			SetJobAttributes,
			[Display(Name = "get-printer-supported-values")]
			GetPrinterSupportedValues,
			[Display(Name = "create-subscription-attributes")]
			GetSubscriptionAttributes,
			[Display(Name = "create-subscriptions")]
			GetSubscriptions,
			[Display(Name = "renew-subscriptions")]
			RenewSubscriptions,
			[Display(Name = "cancel-subscription")]
			CancelSubscription,
			[Display(Name = "get-notifications")]
			GetNotifications,
			[Display(Name = "ipp-indp-method")]
			IPPIndpMethod,
			[Display(Name = "get-resource-attributes")]
			GetResourceAttributes,
			[Display(Name = "get-resource-data")]
			GetResourceData,
			[Display(Name = "get-resources")]
			GetResources,
			[Display(Name = "ipp-install")]
			IPPInstall,
			[Display(Name = "enable-printer")]
			EnablePrinter,
			[Display(Name = "disable-printer")]
			DisablePrinter,
			[Display(Name = "pause-printer-after-current-job")]
			PausePrinterAfterCurrentJob,
			[Display(Name = "hold-new-jobs")]
			HoldNewJobs,
			[Display(Name = "release-held-new-jobs")]
			ReleaseHeldNewJobs,
			[Display(Name = "deactivate-printer")]
			DeactivatePrinter,
			[Display(Name = "activate-printer")]
			ActivatePrinter,
			[Display(Name = "restart-printer")]
			RestartPrinter,
			[Display(Name = "shutdown-printer")]
			ShutdownPrinter,
			[Display(Name = "startup-printer")]
			StartupPrinter,
			[Display(Name = "reprocess-job")]
			ReprocessJob,
			[Display(Name = "cancel-current-job")]
			CancelCurrentJob,
			[Display(Name = "suspend-current-job")]
			SuspendCurrentJob,
			[Display(Name = "resume-job")]
			ResumeJob,
			[Display(Name = "promote-job")]
			PromoteJob,
			[Display(Name = "schedule-job-after")]
			ScheduleJobAfter,
			[Display(Name = "cancel-document")]
			CancelDocument = 0x0033,
			[Display(Name = "get-document-attributes")]
			GetDocumentAttributes,
			[Display(Name = "get-documents")]
			GetDocuemnts,
			[Display(Name = "delete-document")]
			DeleteDocument,
			[Display(Name = "set-document-attributes")]
			SetDocumentAttributes,
			[Display(Name = "cancel-jobs")]
			CancelJobs,
			[Display(Name = "cancel-my-job")]
			CancelMyJob,
			[Display(Name = "resubmit-job")]
			ResubmitJob,
			[Display(Name = "close-job")]
			CloseJob,
			[Display(Name = "identify-printer")]
			IdentifyPrinter,
			[Display(Name = "validate-document")]
			ValidateDocument,
			[Display(Name = "none")]
			None = 0xFFFF
		}

		public enum JobCollationType : UInt32
		{
			[Display(Name = "other")]
			Other = 0x0001,
			[Display(Name = "unknown")]
			Unknown,
			[Display(Name = "uncollated-documents")]
			UncollatedDocuments,
			[Display(Name = "collated-documents")]
			CollatedDocuments,
			[Display(Name = "uncollated-documents")]
			UncollatedDocuments2
		}

		public enum JobState : UInt32
		{
			[Display(Name = "pending")]
			Pending = 0x0003,
			[Display(Name = "pending-held")]
			PendingHeld,
			[Display(Name = "processing")]
			Processing,
			[Display(Name = "processing-stopped")]
			ProcessingStopped,
			[Display(Name = "canceled")]
			Canceled,
			[Display(Name = "aborted")]
			Aborted,
			[Display(Name = "completed")]
			Completed
		}

		public enum OrientationRequested : UInt32
		{
			[Display(Name = "portrait")]
			Portrait = 0x0003,
			[Display(Name = "landscape")]
			Landscape,
			[Display(Name = "reverse-landscape")]
			ReverseLandscape,
			[Display(Name = "reverse-portrait")]
			ReversePortrait,
			[Display(Name = "none")]
			None
		}

		public static class OrientationRequestedExt
		{
			public static String ToString(this OrientationRequested value)
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

		public enum PrintQuality : UInt32
		{
			[Display(Name = "draft")]
			Draft = 0x0003,
			[Display(Name = "normal")]
			Normal,
			[Display(Name = "high")]
			High
		}

		public enum PrinterState : UInt32
		{
			[Display(Name = "idle")]
			Idle = 0x0003,
			[Display(Name = "processing")]
			Processing,
			[Display(Name = "stopped")]
			Stopped
		}
	}

}
