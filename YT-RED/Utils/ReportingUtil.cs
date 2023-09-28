using System.Linq;
using System.Net.NetworkInformation;

namespace YTR.Utils
{
    public static class ReportingUtil
    {
        public static string GetMac()
        {
            string firstMacAddress = NetworkInterface.GetAllNetworkInterfaces()
                .Where(nic => nic.OperationalStatus == OperationalStatus.Up && nic.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                .Select(nic => nic.GetPhysicalAddress().ToString())
                .FirstOrDefault();
            return firstMacAddress;
        }

    }
}