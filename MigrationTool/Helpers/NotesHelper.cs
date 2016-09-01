using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MigrationTool.Models;
using System.Text;
using System.Web.Mvc;

namespace MigrationTool.Helpers
{
    public class NotesHelper
    {
        public static MvcHtmlString RenderNoteCounts(IEnumerable<Note> notes)
        {
            StringBuilder builder = new StringBuilder();
            var counts = from n in notes group n by n.NoteType into g orderby g.Key select g;

            foreach (var item in counts)
            {
                if (item.Key == 0)
                {
                    builder.AppendFormat("<span class=\"label label-info\">{0}</span> ", item.Count());
                }
                else if (item.Key == 1)
                {
                    builder.AppendFormat("<span class=\"label label-success\">{0}</span> ", item.Count());
                }
                else if (item.Key == 2)
                {
                    builder.AppendFormat("<span class=\"label label-warning\">{0}</span> ", item.Count());
                }
                else if (item.Key == 3)
                {
                    builder.AppendFormat("<span class=\"label label-danger\">{0}</span> ", item.Count());
                }
            }

            return new MvcHtmlString(builder.ToString());
        }
    }
}