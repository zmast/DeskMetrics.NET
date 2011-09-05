// **********************************************************************//
//                                                                       //
//     DeskMetrics NET - Json/LogJson.cs                                 //
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
	class LogJson : BaseJson
    {
        public string Message { get; private set; }
        public int Flow { get; private set; }

        public LogJson(string session, string msg, int flow)
            : base(session)
        {
            Type = EventType.Log;
            Message = msg;
            Flow = flow;
        }

        public override Hashtable GetJsonHashTable()
        {
            var json = base.GetJsonHashTable();
            json.Add("ms", Message);
            json.Add("fl", Flow);
            return json;
        }
    }
}

