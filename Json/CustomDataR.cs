// **********************************************************************//
//                                                                       //
//     DeskMetrics NET - Json/CustomDataR.cs                             //
//     Copyright (c) 2010-2011 DeskMetrics Limited                       //
//                                                                       //
//     http://deskmetrics.com                                            //
//     http://support.deskmetrics.com                                    //
//                                                                       //
//     support@deskmetrics.com                                           //
//                                                                       //
//     This code is provided under the DeskMetrics Modified BSD License  //
//     A copy of this license has been distributed in a file called      //
//     LICENSE with this source code.                                    //
//                                                                       //
// **********************************************************************//

using System;
using System.Collections;

namespace DeskMetrics.Json
{
	class CustomDataRJson : CustomDataJson
    {
        public string ID { get; private set; }
        public string AppVersion { get; private set; }

        public CustomDataRJson(string session, string name, string value, int flow, string id, string appVersion)
            : base(session, name, value, flow)
        {
            Type = EventType.CustomDataR;
            ID = id;
            AppVersion = appVersion;
        }

        public override Hashtable GetJsonHashTable()
        {
            var json = base.GetJsonHashTable();
            json.Add("aver", AppVersion);
            json.Add("ID", ID);
            return json;
        }
    }
}

