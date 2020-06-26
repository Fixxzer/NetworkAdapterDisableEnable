using System;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading;

namespace NetworkAdapterDisableEnable
{
    internal static class Program
    {
        private static void Main()
        {
            var adapter = NetworkInterface.GetAllNetworkInterfaces().FirstOrDefault(x => x.Name == "Wi-Fi");

            if (adapter != null)
            {
                Console.WriteLine("Disabling Network Adapter");
                Toggle(adapter.Name, InterfaceStatus.Disable);

                Console.WriteLine("10 second timer...");
                Thread.Sleep(10000);

                Console.WriteLine("Enabling Network Adapter");
                Toggle(adapter.Name, InterfaceStatus.Enable);

                while (adapter.OperationalStatus != OperationalStatus.Up)
                {
                    Console.WriteLine($"Status: {adapter.OperationalStatus}");
                    Thread.Sleep(500);
                }

                Console.WriteLine($"Operational Status: {adapter.OperationalStatus}");
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        private static void Toggle(string interfaceName, InterfaceStatus interfaceStatus)
        {
            ProcessStartInfo psi = new ProcessStartInfo("netsh", $"interface set interface name={interfaceName} admin={interfaceStatus.ToString().ToUpper()}")
            {
                UseShellExecute = false, 
                RedirectStandardOutput = true, 
                RedirectStandardError = true
            };

            Process p = new Process {StartInfo = psi};
            p.OutputDataReceived += (s, e) => Console.WriteLine(e.Data);
            p.ErrorDataReceived += (s, e) => Console.WriteLine(e.Data);

            p.Start();
        }

        private enum InterfaceStatus
        {
            Enable, Disable
        }
    }
}
