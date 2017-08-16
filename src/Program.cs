using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace S3Console
{
	class Program
	{
		private static string _assemblyName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
		internal static string AssemblyName { get  { return _assemblyName; } }

		static void Main(string[] args)
		{
			try
			{
				//set up console
				if (Console.BufferHeight < 1000) Console.BufferHeight = 1000;
				if (Console.BufferWidth < 120) Console.BufferWidth = 120;
				if (Console.WindowHeight < 30) Console.WindowHeight = 30;
				if (Console.WindowWidth < 120) Console.WindowWidth = 120;

				bool exit = false;
				
				Executor ex = new Executor();
				
				Console.WriteLine("Welcome to the " + AssemblyName + " utility. See help for parameters (h or help).");
				do
				{
                    Console.WriteLine();
					Console.Write(">");
					string line = Console.ReadLine();
					//check for exit
					if (line == "quit" || line == "q" || line == "exit") break;
						
					//not quiting.  process.
					args = line.Split(' ');
					
					if (args.Length == 1 && args[0].Length == 0) continue;
					else if (args.Length == 1 && (args[0] == "help" || args[0] == "h")) ShowHelp(string.Empty);
					else if (args.Length == 1) 
					{
						try
						{
							ex.Run(args[0]);
						}
						catch (Exception runEx)
						{
							WriteErr(runEx.ToString());
						}
					}
					else if (args.Length == 2 && args[1] == "?") ShowHelp(args[0]);
					else if (args.Length >= 2)
					{
						try
						{
							if (args.Length == 2)
                                ex.Run(args[0], string.Empty, args[1]);
                            else
                            {
                                if (args.Length > 3)
                                {
                                    string[] otherArgs = new string[args.Length - 3];
                                    Array.Copy(args, 3, otherArgs, 0, args.Length - 3 );

                                    // pass other args
                                    ex.Run(args[0], args[1], args[2], otherArgs);
                                }
                                else
                                {
                                    ex.Run(args[0], args[1], args[2]);
                                }
                            }
						}
						catch (Exception runEx)
						{
							WriteErr(runEx.ToString());
						}
					}
					else Console.WriteLine("Not enough arguments.");
					
				}
				while (!exit);
				
			}
			catch (Exception ex)
			{
				//write the exception out
				WriteErr(ex.ToString());
			}
		}

        
        //TODO: Add ability to set region for the session
        //TODO: Make access key and secret key configurable
        //TODO: download and upload functionality
		private static void ShowHelp(string action)
		{
			if (string.IsNullOrWhiteSpace(action))
			{
				Console.WriteLine("Available actions are: \r\n" + 
					"\t" + Actions.ListBuckets + "\r\n" +
					"\t" + Actions.List + "\r\n" +
					"\t" + Actions.Delete + "\r\n" +
                    "\t" + Actions.SetBucket + "\r\n" +
                    "\t" + Actions.GenerateDownloadUrl + "\r\n" +
                    "\t" + Actions.PageSize + "\r\n" +
					"Get action specific help with \"[action] ?\"");
			}
			else
			{
				switch (action)
				{
                    case Actions.ListBuckets:
						Console.WriteLine("Usage: \"list-buckets\"");
						return;
                    case Actions.SetBucket:
                        Console.WriteLine("Usage: \"set-bucket [bucket]\"");
                        return;
					case Actions.List: 
						Console.WriteLine("Usage: \"list [bucket] [key prefix]\" \r\n" +
								"\t" + "Ex: \"list mybucket.bucket.com xml/967871 \"");
						return;
					case Actions.Delete:
						Console.WriteLine("Usage: \"delete [bucket] [key]\" \r\n" +
								"\t" + "Ex: \"delete mybucket.bucket.com xml/967871/blah.xml\"");
						return;
                    case Actions.GenerateDownloadUrl:
                        Console.WriteLine("Usage: \"gen-download-url [bucket] [key]\" \r\n" +
                                "\t" + "Ex: \"gen-download-url mybucket.bucket.com xml/967871/blah.xml \"");
                        return;
				}
			}
		}

		private static void WriteErr(string err)
		{
			ConsoleColor usedToBe = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine(err);
			Console.ForegroundColor = usedToBe;
		}
	}	//c
}	//n
