using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeskMetrics
{
    public interface IServices
    {
        string PostData(string PostMode, string json);
        bool SendDataAsync(string json);
        string ProxyHost { get; set; }
        string ProxyUserName { get; set; }
        string ProxyPassword { get; set; }
        Int32 ProxyPort { get; set; }
    }
}
