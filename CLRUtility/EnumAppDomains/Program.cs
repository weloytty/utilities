using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;
using ClrUtils;
using CommandLine;
using Microsoft.Diagnostics.Runtime;


namespace EnumAppDomains
{
	class Program
	{
		static void Main(string[] args)
		{
			int whatPid = 0;
			bool showGac = false;
			bool showHeap = false;

			var parser = new Parser(with => with.HelpWriter = Console.Out);
			var commandLineOptions =
				parser.ParseArguments<CommandLineOptions>(args).WithParsed(opts=>RunApplication(opts)).WithNotParsed(erropts=>ShowHelp(erropts));

		}

		static void RunApplication(CommandLineOptions opts)
		{


			if (opts.PID != 0)
			{
				try
				{
					bool currProc64 = CLRUtility.Is64BitProcess();
					bool remoteProc64 = CLRUtility.Is64BitProcess(opts.PID);

					if (currProc64 != remoteProc64)
					{
						Console.WriteLine("Process Bitness must match. This process is {0}, {1} is {2}", currProc64 ? "64-bit" : "32-bit",
							opts.PID, remoteProc64 ? "64-bit" : "32-bit");
						return;

					}



					using (DataTarget d = DataTarget.AttachToProcess(opts.PID, 5000, AttachFlag.NonInvasive))
					{
						Console.WriteLine($"Dumping all CLRversions for process {opts.PID}");
						foreach (ClrInfo currInfo in d.ClrVersions)
						{

							ClrRuntime runtime = currInfo.CreateRuntime();
							Console.WriteLine($"CLR: {currInfo.Version} GC: {(runtime.ServerGC ? "Server" : "Workstation")}", "");
							foreach (ClrAppDomain domain in runtime.AppDomains)
							{

								IEnumerable<ClrThread> threadList = runtime.Threads.Where(x => (x.AppDomain == domain.Address));
								string domainInfo = string.Format("App Domain: {0} ID: {1} Threads: {2} Address: {3,12:X8}", domain.Name.PadRight(40),
									domain.Id, threadList.Count(), domain.Address);
								Console.WriteLine(domainInfo);

								Console.WriteLine($"Modules: {domain.Modules.Count.ToString("D3")} (including GAC? {opts.GAC}");
								foreach (ClrModule currMod in domain.Modules)
								{
									bool printMe = (!String.IsNullOrEmpty(currMod.AssemblyName)) && (!currMod.AssemblyName.Contains("\\GAC_") | opts.GAC);
									if (printMe) Console.WriteLine($"{currMod.AssemblyName}");
								}

								Console.WriteLine("");
								Console.WriteLine("Threads:");

								foreach (ClrThread clrt in threadList)
								{
									string threadInfo = string.Format("  Thread: {0,3:G} NT: {1,12:X8} GC: {2} {3}", clrt.ManagedThreadId,
										clrt.OSThreadId, clrt.IsGC, clrt.IsAlive ? "    " : "DEAD");

									Console.WriteLine(threadInfo);
									int frameNum = 0;
									foreach (ClrStackFrame frame in clrt.StackTrace)
									{

										string frameInfo = String.Format("    Frame: {0,2:G} IP: {1,12:X} {2}", frameNum, frame.InstructionPointer,
											frame.DisplayString);
										Console.WriteLine(frameInfo);
										frameNum++;

									}

								}

								Console.WriteLine("");

							}
						}
					}

				}
				catch (ArgumentException ae)
				{
					Console.WriteLine(ae.Message);
				}
				catch (ApplicationException _applicationException)
				{
					Console.WriteLine(_applicationException.Message);
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.Message);
					Console.WriteLine(ex.GetType());
					Console.WriteLine(ex.StackTrace);
					Console.WriteLine("Cannot get the process. "
									  + " Make sure the process exists and it's a managed process");
				}
			}
		}

	



	static void ShowHelp(IEnumerable<CommandLine.Error> opts)
		{
			Console.WriteLine("EnumAppDomains.exe:  Shows what appdomains are running in a process");
			Console.WriteLine("	(c) 2018 Bill Loytty");
			Console.WriteLine("	USAGE:  EnumAppDomains -PID xxxx [-GAC] [-HEAP]");
			Console.WriteLine(" Shows all appdomains running in process xxxx");

			foreach (var error in opts)
			{
				Console.WriteLine(error.ToString());
			}
		}

		class CommandLineOptions
		{
			[Option("PID", Required = true,
				HelpText = "Process ID to enumerate")]
			public int PID { get; set; }


			[Option("GAC", Default = false,
				HelpText = "Display GACd assemblies.")]
			public bool GAC { get; set; }

			[Option("HEAP",Default=false,HelpText = "Display Heap Information.")]
			public bool HEAP{ get; set; }
		}
	}
}
