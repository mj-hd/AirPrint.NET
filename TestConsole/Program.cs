using System;
using System.IO;
using System.Text;
using IPP;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using Zeroconf;

namespace TestConsole
{
	class MainClass
	{
		public static void Main(string[] args)
		{
            Console.WriteLine("Find Printers...");
			var printers_task = ZeroconfResolver.ResolveAsync("_printer._tcp.local.", TimeSpan.FromSeconds(30.0), 5);

			printers_task.Wait();

			var printers = printers_task.Result;

			if (printers.Count <= 0)
				throw new Exception("Printer not found");

			// list found printers
			foreach (var pt in printers)
				Console.WriteLine(pt.DisplayName);

            Console.WriteLine("Please input bmp file path:");

            // open bmp file
            var bmpPath = Console.ReadLine();
			var bmp = new UNIRAST.BMP(File.ReadAllBytes(bmpPath));

			var conved = UNIRAST.Converter.BMP2UNIRAST(bmp);
			conved.Pages[0].DPI = 72;

			var printerUrl = "http://" + printers[0].IPAddress + "/";
			Console.WriteLine("Connect With " + printerUrl);


			// initialize printer
			var printer = new Printer(printerUrl);
			Console.WriteLine("Send Data:");

			var job = Job.PrintJob("user", "testprint", "image/urf", conved.ToBytes());

			job.MediaSize = "iso_a4_210x297mm"; // TODO: hard-coded
			job.Orientation = IPP.Enums.OrientationRequested.ReversePortrait; // TODO: hard-coded

			// send data
			printer.Send(job);

			Console.WriteLine("Job Information Loop:");

			while (job.State != IPP.Enums.DocumentState.Completed)
			{
				printer.UpdateJobState(job);

				Thread.Sleep(1000);

				Console.WriteLine(job.State);
			}

			Console.WriteLine("Finished!");

		}
	}
}
