// **********************************************************************//
//                                                                       //
//     DeskMetrics NET - Json/CustomDataJson.cs                          //
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
    class CustomDataJson : BaseJson
    {
        public string Name { get; private set; }
        public string Value { get; private set; }
        public int Flow { get; private set; }

        public CustomDataJson(string session, string name, string value, int flow)
            : base(session)
        {
            Type = EventType.CustomData;
            Name = name;
            Value = value;
            Flow = flow;
        }

        public override Hashtable GetJsonHashTable()
        {
            var json = base.GetJsonHashTable();
            json.Add("nm", Name);
            json.Add("vl", Value);
            json.Add("fl", Flow);
            return json;
        }
    }
}

