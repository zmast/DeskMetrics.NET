// **********************************************************************//
//                                                                       //
//     DeskMetrics NET - Json/StartAppJson.cs                            //
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
using DeskMetrics.OperatingSystem;
using DeskMetrics.OperatingSystem.Hardware;

namespace DeskMetrics.Json
{
	class StartAppJson : BaseJson
    {
        public string AppVersion { get; private set; }
        public string UserGuid { get; private set; }

        public StartAppJson(string session, string applicationVersion, string userGuid)
            :base(session)
        {
            Type = EventType.StartApplication;
            AppVersion = applicationVersion;
            UserGuid = userGuid;
        }

        public override Hashtable GetJsonHashTable()
        {
            IOperatingSystem osInfo = OperatingSystemFactory.GetOperatingSystem();
            IHardware hardwareInfo = osInfo.Hardware;
            var json = base.GetJsonHashTable();
			
            json.Add("aver", AppVersion);
            json.Add("ID", UserGuid);
            json.Add("osv", osInfo.Version);
            json.Add("ossp", osInfo.ServicePack);
            json.Add("osar", osInfo.Architecture);
            json.Add("osjv", osInfo.JavaVersion);
            json.Add("osnet", osInfo.FrameworkVersion);
            json.Add("osnsp", osInfo.FrameworkServicePack);
            json.Add("oslng", osInfo.Lcid);
            json.Add("osscn", hardwareInfo.ScreenResolution);
            json.Add("cnm", hardwareInfo.ProcessorName);
			json.Add("car", hardwareInfo.ProcessorArchicteture);
            json.Add("cbr", hardwareInfo.ProcessorBrand);
            json.Add("cfr", hardwareInfo.ProcessorFrequency);
            json.Add("ccr", hardwareInfo.ProcessorCores);
            json.Add("mtt", hardwareInfo.MemoryTotal);
            json.Add("mfr", hardwareInfo.MemoryFree);
            json.Add("dtt", "null");
            json.Add("dfr", "null");
            return json;
        }
    }	
}

