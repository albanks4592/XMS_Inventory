using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MigrationTool.Models;

namespace MigrationTool.ViewModels.Invoicing
{
    public class BrokerDesktopGroupData
    {
        public XenDesktopCatalogType CatalogType { get; set; }
        public int Count { get; set; }
        public Dictionary<XenDesktopSite, int> Sites { get; set; }
        public bool RandomAllocation { get; set; }
    }
}