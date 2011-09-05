// **********************************************************************//
//                                                                       //
//     DeskMetrics NET - Json/StopAppJson.cs                             //
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
	class StopAppJson : BaseJson
    {
        public StopAppJson(string session)
            : base(session)
        {
            Type = EventType.StopApplication;
        }

        public override Hashtable GetJsonHashTable()
        {
            return base.GetJsonHashTable(); 
        }
    }
}

