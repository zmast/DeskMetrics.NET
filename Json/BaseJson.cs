// **********************************************************************//
//                                                                       //
//     DeskMetrics NET - Json/BaseJson.cs                                //
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
	abstract class BaseJson
    {
        protected string Type { get; set; }
        public string Session { get; private set; }
        public int TimeStamp { get; private set; }
        protected Hashtable Json { get; private set; }

        public BaseJson(string session)
        {
            Session = session;
            TimeStamp = Util.GetTimeStamp();
            Json = new Hashtable();
        }

        public virtual Hashtable GetJsonHashTable()
        {
            Json.Add("tp", Type);
            Json.Add("ss", Session);
            Json.Add("ts", TimeStamp);
            return Json;
        }
    }
}

