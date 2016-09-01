using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MigrationTool.Models;

namespace MigrationTool.ViewModels.Invoicing
{
    public class IndexViewModel
    {
        public int BillableServerCount { get; set; }
        public Dictionary<VirtualMachineType, int> BillableServersByVmType { get; set; }
        public Dictionary<XenAppServerRole, int> BillableServersByRole { get; set; }
        public Dictionary<XenAppFarmVersion, int> BillableServersByFarmVersion { get; set; }
        public List<BrokerDesktopGroupData> BillableBrokerDesktopCounts { get; set; }

        public int UnbillableServerCount { get; set; }
        public Dictionary<VirtualMachineType, int> UnbillableServersByVmType { get; set; }
        public Dictionary<XenAppServerRole, int> UnbillableServersByRole { get; set; }
        public Dictionary<XenAppFarmVersion, int> UnbillableServersByFarmVersion { get; set; }
        public List<BrokerDesktopGroupData> UnbillableBrokerDesktopCounts { get; set; }
    }
}