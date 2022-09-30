using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace OrphanHitman
{
    internal class OsUtils
    {
        public static void KillProcessTree(int pid)
        {
            ManagementObjectSearcher procSearcher = new ManagementObjectSearcher("Select * From Win32_Process Where ParentProcessID=" + pid);
            ManagementObjectCollection procCollection = procSearcher.Get();

            try
            {
                Process proc = Process.GetProcessById(pid);

                if (!proc.HasExited)
                    proc.Kill();
            }
            catch (ArgumentException)
            {
                // Process already exited.
            }

            if (procCollection != null)
            {
                foreach (ManagementObject mo in procCollection)
                    KillProcessTree(Convert.ToInt32(mo["ProcessID"])); //kill child processes (also kills children of children etc.)
            }
        }
    }
}
