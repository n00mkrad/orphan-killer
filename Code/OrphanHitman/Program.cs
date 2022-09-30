using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace OrphanHitman
{
    internal class Program
    {
        public static List<string> Args = new List<string>(); // All args

        public static Dictionary<string, string> UserArgs = new Dictionary<string, string>(); // User args (excludes 1st which is the path) as key/value pairs

        static async Task Main(string[] args)
        {
            try
            {
                HandleArgs(args);

                int parentPid = UserArgs["parent-pid"].GetInt();
                List<int> childPids = new List<int>();

                if (UserArgs.ContainsKey("child-pids"))
                    childPids = UserArgs["child-pids"].Split(',').Select(x => x.GetInt()).ToList();
                else
                    childPids = new List<int>() { UserArgs["child-pid"].GetInt() };

                await MainLoop(parentPid, childPids);
            }
            catch(Exception ex)
            {
                ErrorClose(ex);
            }
        }

        static void HandleArgs (string[] args)
        {
            Args = args.Select(x => x.Trim()).Where(x => !string.IsNullOrWhiteSpace(x)).ToList();

            foreach (string arg in Args)
            {
                if (!(arg.StartsWith("-") && arg.Length > 1 && arg[1] != '-')) // Required arg syntax: Starts with hypen, no double hyphen, more than 1 char
                    continue;

                var split = arg.Substring(1).Split('=').Take(2).ToArray(); // Split into key+value, ignore if more than one '='
                UserArgs.Add(split[0], split[1]); // Add key+value
            }
        }

        static async Task MainLoop (int parentPid, List<int> childPids)
        {
            Console.WriteLine("Main loop start");

            try
            {
                Process parent = Process.GetProcessById(parentPid);
                List<Process> children = childPids.Select(pid => Process.GetProcessById(pid)).ToList();

                while (parent != null && !parent.HasExited)
                {
                    await Task.Delay(1000);
                }

                foreach (Process child in children)
                {
                    if (child == null || child.HasExited)
                    {
                        continue;
                    }

                    try
                    {
                        Console.WriteLine($"Killing child with PID {child.Id}");
                        OsUtils.KillProcessTree(child.Id);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to kill child with PID {child.Id}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorClose(ex);
            }


            Console.WriteLine("We're done here.");
            await Task.Delay(1000);
        }

        static void ErrorClose(Exception ex)
        {
            Console.WriteLine($"An error occured.");
            Console.WriteLine($"Exception: {ex.Message}\nStack Trace:\n{ex.StackTrace}");
        }
    }
}
