// **********************************************************************//
//                                                                       //
//     DeskMetrics NET - Json/EventValueJson.cs                          //
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
	class EventValueJson : EventJson
    {
        public string Value { get; private set; }

        public EventValueJson(string session, string category, string name, string value, int flow)
            : base(session, category, name, flow)
        {
            Type = EventType.EventValue;
            Value = value;
        }

        public override Hashtable GetJsonHashTable()
        {
            var json = base.GetJsonHashTable();
            json.Add("vl", Value);
            return json;
        }
    }

}

