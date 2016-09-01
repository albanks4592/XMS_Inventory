using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using MigrationTool.Models;

namespace MigrationTool.ViewModels
{
    public class SourceData
    {
        public string Name { get; set; }
        [Display(Name = "Status")]
        public bool Succcess { get; set; }
        [Display(Name = "Date of Last Import")]
        public DateTime CreateDate { get; set; }
        [Display(Name = "Duration")]
        public TimeSpan Duration { get; set; }
    }

    public class HomeViewModel
    {
        public HomeViewModel(MigrationToolEntities db)
        {
            this.DataSources = db.DataSources
                .Where(x => !x.Inactive && x.DataSourcesStatus.Count() > 0)
                .Select(x => x.DataSourcesStatus
                    .Where(y => y.CreateDate == x.DataSourcesStatus.Max(z => z.CreateDate)).FirstOrDefault())
                .OrderBy(x => x.DataSource.Name).ToList()
                .Select(x => new SourceData
                {
                    Name = x.DataSource.Name,
                    Succcess = x.Success,
                    CreateDate = x.CreateDate,
                    Duration = new TimeSpan(0,0,0,0,x.Duration)
                })
                .ToList();
        }

        public IEnumerable<SourceData> DataSources { get; set; }
 
    }
}