using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace cversion
{
	class Program
	{
		static void Main(string[] args)
		{
			PrintHeader();
			if (args.Length > 0 && File.Exists(args[0]))
			{

				try
				{

					string filePath = Path.GetFullPath(args[0]);
					FileVersionInfo ver = FileVersionInfo.GetVersionInfo(filePath);
					if (args.Length == 2 && args[1] == "-FULL")
					{
						Console.WriteLine("");
						Console.WriteLine($"Company Name        :{ver.CompanyName}");
						Console.WriteLine($"Comments            :{ver.Comments}");
						Console.WriteLine($"File Name           :{Path.GetFileName(ver.FileName)}");
						Console.WriteLine($"File Path           :{ver.FileName}");
						Console.WriteLine($"File Description    :{ver.FileDescription}");
						Console.WriteLine($"File Version        :{ver.FileVersion}");
						Console.WriteLine($"File Major Part     :{ver.FileMajorPart}");
						Console.WriteLine($"File Minor Part     :{ver.FileMinorPart}");
						Console.WriteLine($"File Build Part     :{ver.FileBuildPart}");

						Console.WriteLine($"File Private Part   :{ver.FilePrivatePart}");

						Console.WriteLine($"Internal Name       :{ver.InternalName}");
						Console.WriteLine($"Is Debug            :{ver.IsDebug}");
						Console.WriteLine($"Is Patched          :{ver.IsPatched}");
						Console.WriteLine($"Is PreRelease       :{ver.IsPreRelease}");
						Console.WriteLine($"Is Private Build    :{ver.IsPrivateBuild}");
						Console.WriteLine($"Is Special Build    :{ver.IsSpecialBuild}");
						Console.WriteLine($"Language            :{ver.Language}");

						Console.WriteLine("");


					}
					else
					{
						String outputString = ver.FileVersion ?? $"No FileVersion info for {args[0]}";
						Console.WriteLine(outputString);
					}
				}
				catch (ArgumentException arg)
				{
					Console.WriteLine($"Invalid Argument: {arg.Message}");
					return;
				}
				catch (Exception e)
				{
					Console.WriteLine(e.Message);
					return;
				}


			}
			else
			{
				PrintUsage();
			}
		}
		private static void PrintHeader()
		{
			Assembly asm = Assembly.GetExecutingAssembly();
			if (asm != null)
			{
				
				Console.WriteLine($"{asm.FullName.Split(',')[0]}: Displays File Version Info");
				Console.WriteLine($"(C) 1999-{DateTime.Now.Year} Bill Loytty");
			}
		}
		private static void PrintUsage()
		{
			Console.WriteLine("Usage: cversion.exe filename [-FULL]");
		}
	}
}
