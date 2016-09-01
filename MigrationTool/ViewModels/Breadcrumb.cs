using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MigrationTool.ViewModels
{
    public class Breadcrumb
    {
        public Breadcrumb(string lastItemName, params BreadcrumbItem[] items)
        {
            this.LastItemName = lastItemName;
            this.Items = items;
        }

        public string LastItemName { get; set; }
        public BreadcrumbItem[] Items { get; set; }
    }
}