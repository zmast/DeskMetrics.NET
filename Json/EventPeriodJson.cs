// **********************************************************************//
//                                                                       //
//     DeskMetrics NET - Json/EventPeriodJson.cs                         //
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
	class EventPeriodJson : EventJson
    {
        public int Time { get; private set; }
        public bool Completed { get; private set; }

        public EventPeriodJson(string session, string category, string name, int flow, int time, bool completed)
            : base(session, category, name, flow)
        {
            Type = EventType.EventPeriod;
            Time = time;
            Completed = completed;
        }

        public override Hashtable GetJsonHashTable()
        {
            var json = base.GetJsonHashTable();
            json.Add("tm", Time);
            json.Add("ec", Completed?"1":"0");
            return json;
        }
    }
}

