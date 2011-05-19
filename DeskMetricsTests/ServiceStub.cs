using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeskMetrics;

namespace DeskMetricsTest
{
    class ServiceStub:IServices
    {
        string IServices.PostData(string PostMode, string json)
        {
            return "{\"status_code\":1}";
        }

        bool IServices.SendDataAsync(string json)
        {
            return true;
        }

        string IServices.ProxyHost
        {
            get
            {
                return "";
            }
            set
            {
                
            }
        }

        string IServices.ProxyUserName
        {
            get
            {
                return "";
            }
            set
            {
                
            }
        }

        string IServices.ProxyPassword
        {
            get
            {
                return "";
            }
            set
            {
                
            }
        }

        int IServices.ProxyPort
        {
            get
            {
                return 80;
            }
            set
            {
                
            }
        }
    }
}
