using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;
using ClrUtils;
using Microsoft.Diagnostics.Runtime;
using Microsoft.Samples.Debugging.CorDebug;

namespace EnumAppDomains
{
	class Program
	{
		static void Main(string[] args)
		{
			int whatPid = 0;
			bool showGac= false;
			if (args.Length >= 2)
			{
				whatPid = Convert.ToInt32(args[1]);

				showGac = (args.Length == 3 && (args[2].ToUpper() == "-GAC"));

			}
			else { ShowHelp();}

			if (whatPid != 0)
			{
				try
				{
					bool currProc64 = CLRUtility.Is64BitProcess();
					bool remoteProc64 = CLRUtility.Is64BitProcess(whatPid);

					if (currProc64 != remoteProc64)
					{
						Console.WriteLine("Process Bitness must match. This process is {0}, {1} is {2}", currProc64 ? "64-bit" : "32-bit",
							whatPid, remoteProc64 ? "64-bit" : "32-bit");
						return;

					}
				
					//using (ManagedProcess mp = ClrUtils.ManagedProcess.GetManagedProcessByID(whatPid))
					//{
					//	foreach (CorAppDomain appDomain in mp.AppDomains)
					//	{
					//		Console.WriteLine("AppDomain Id={0}, Name={1}",
					//			appDomain.Id,
					//			appDomain.Name);

					//		foreach(CorAssembly assy in appDomain.Assemblies)
					//		{
					//			string thisName = assy.Name;
					//			if((!thisName.ToUpper().Contains("GAC") && (!thisName.ToUpper().Contains("<UNKNOWN>"))) || showGac)
					//			{
					//				Console.WriteLine($"   Assembly={assy.Name}");
					//			}

								
					//		}

					//	}

						
						

					//}


					using (DataTarget d = DataTarget.AttachToProcess(whatPid, 5000, AttachFlag.NonInvasive))
					{
						ClrRuntime runtime = d.ClrVersions.First().CreateRuntime();
						Console.WriteLine("PID: {0} GC: {1}", whatPid,runtime.ServerGC?"Server":"Workstation");
						foreach (ClrAppDomain domain in runtime.AppDomains)
						{
							Console.WriteLine("Name:    {0}:{1}:{2}", domain.Name,domain.Id,domain.Address);

					
							IEnumerable<ClrThread> threadList = runtime.Threads.Where(x => (x.AppDomain ==domain.Address));
							Console.WriteLine("  Threads: {0}", threadList.Count());
							
							foreach (ClrThread clrt in threadList)
							{

								Console.WriteLine("  MT: {0} NT: {1} GC: {2} {3}", clrt.ManagedThreadId.ToString("D3"), clrt.OSThreadId.ToString("D10"), clrt.IsGC, clrt.IsAlive ? "" : "DEAD");
							}
							Console.WriteLine("");

						}

					}

				}catch (ArgumentException ae)
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

		static void ShowHelp()
		{
			Console.WriteLine("EnumAppDomains.exe:  Shows what appdomains are running in a process");
			Console.WriteLine("	(c) 2018 Bill Loytty");
			Console.WriteLine("	USAGE:  EnumAppDomains -PID xxxx");
			Console.WriteLine(" Shows all appdomains running in process xxxx");
		}
	}
}
