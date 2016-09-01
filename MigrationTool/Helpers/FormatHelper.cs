using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace MigrationTool.Helpers
{
    public class FormatHelper
    {
        static readonly string[] sizeSuffixes = { "Bytes", "KB", "MB", "GB", "TB", "PB", "EB" }; //Longs run out around EB

        public static HtmlString BytesToString(Int64? bytes, int decimals = 0)
        {
            if (!bytes.HasValue)
            {
                return new HtmlString("");
            }
            else
            {
                double num;
                int suffixIndex;

                if (bytes == 0)
                {
                    num = 0;
                    suffixIndex = 0;
                }
                else
                {
                    Int64 absoluteBytes = Math.Abs(bytes.Value);
                    suffixIndex = Convert.ToInt32(Math.Floor(Math.Log(absoluteBytes, 1024)));
                    num = Math.Round(absoluteBytes / Math.Pow(1024, suffixIndex), decimals);
                }
                return new HtmlString(string.Format(string.Format("{{0:#,#.{0}}} {{1}}", new string('#', decimals)), Math.Sign(bytes.Value) * num, sizeSuffixes[suffixIndex]));
            }
        }

        public static string CapacityConstraintBitsToString(int bits)
        {
            StringBuilder builder = new StringBuilder();

            if ((bits & 2) > 0)
            {
                if (builder.Length > 0)
                {
                    builder.Append(", ");
                }
                builder.Append(Strings.CapacityRule_RuleType_Compute);
            }

            if ((bits & 4) > 0)
            {
                if (builder.Length > 0)
                {
                    builder.Append(", ");
                }
                builder.Append(Strings.CapacityRule_RuleType_Memory);
            }

            if ((bits & 8) > 0)
            {
                if (builder.Length > 0)
                {
                    builder.Append(", ");
                }
                builder.Append(Strings.CapacityRule_RuleType_Storage);
            }

            if ((bits & 1) > 0)
            {
                if (builder.Length > 0)
                {
                    builder.Append(", ");
                }
                builder.Append(Strings.CapacityRule_RuleType_Maximum);
            }

            return builder.ToString();
        }
    }
}