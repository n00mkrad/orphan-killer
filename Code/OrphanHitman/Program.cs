using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace OrphanHitman
{
    internal class Program
    {
        public static List<string> Args = new List<string>(); // All args

        public static Dictionary<string, string> UserArgs = new Dictionary<string, string>(); // User args (excludes 1st which is the path) as key/value pairs

        static void Main(string[] args)
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

                Task.Run(() => MainLoop(parentPid, childPids));
            }
            catch(Exception ex)
            {
                ErrorClose(ex, 5);
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

        }

        static async Task ErrorClose(Exception ex, int waitSeconds)
        {
            Console.WriteLine($"An error occured. Program will close in {waitSeconds} seconds.\n");
            Console.WriteLine($"Exception: {ex.Message}\nStack Trace:\n{ex.StackTrace}");

            await Task.Delay(waitSeconds * 1000);

            Environment.Exit(0);
        }
    }
}
