// **********************************************************************//
//                                                                       //
//     DeskMetrics NET - Json/InstallJson.cs                             //
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
	class InstallJson: BaseJson
	{
        public string Version { get; private set; }
        public int Flow { get; private set; }
		
		public InstallJson(string session, string version, int flow)
            : base(session)
		{
            Type = EventType.Install;
			Version = version;
            Flow = flow;
		}
		
		public override Hashtable GetJsonHashTable ()
		{
			var json = base.GetJsonHashTable();
			json.Add("fl",Flow);
			json.Add("aver",Version);
			return json;
		}
	}
}

