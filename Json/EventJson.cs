// **********************************************************************//
//                                                                       //
//     DeskMetrics NET - Json/EventJson.cs                               //
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
	class EventJson : BaseJson
    {
        public string Category { get; private set; }
        public string Name { get; private set; }
        public int Flow { get; private set; }

        public EventJson(string session, string category, string name, int flow)
            : base(session)
        {
            Type = EventType.Event;
            Category = category;
            Name = name;
            Flow = flow;
        }

        public override Hashtable GetJsonHashTable()
        {
            var json = base.GetJsonHashTable();
            json.Add("ca", Category);
            json.Add("nm", Name);
            json.Add("fl", Flow);

            return json;
        }
    }
}

