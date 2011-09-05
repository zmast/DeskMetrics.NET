// **********************************************************************//
//                                                                       //
//     DeskMetrics NET - Watcher.cs                                      //
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
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using System.Net;
using System.Net.Security;
using System.Threading;
using System.Security.Cryptography.X509Certificates;
using DeskMetrics.Json;


namespace DeskMetrics
{
    public class Watcher : IDisposable
    {
        /// <summary>
        /// Thread which sends data when closing the application
        /// </summary>
        private Thread _stopThread;

        /// <summary>
        /// Thread Lock
        /// </summary>
        private object _objectLock;

        /// <summary>
        /// Session GUID
        /// </summary>
        private string _sessionGUID;

        /// <summary>
        /// User GUID
        /// </summary>
        private string _userGUID;

        /// <summary>
        /// Json list of data to send to webservice
        /// </summary>
        private List<string> _json;

        /// <summary>
        /// Application Id
        /// </summary>
        private string _applicationId;

        /// <summary>
        /// Application version
        /// </summary>
        private string _applicationVersion;

        /// <summary>
        /// Flow number
        /// </summary>
        private int _flowNumber;

        /// <summary>
        /// Persistent cache where the component stores the data that could not be sent (i.e. because there was no internet connection)
        /// </summary>
        private Cache _cache;

        /// <summary>
        /// Indicates if the component has ben started.
        /// </summary>
        public bool Started { get; private set; }

        /// <summary>
        /// Indicates if the component is enabled. If set to false, no method can be called.
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// The name of this component
        /// </summary>
        public string ComponentName
        {
            get
            {
                Assembly thisAsm = this.GetType().Assembly;
                object[] attrs = thisAsm.GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                string componentName = ((AssemblyTitleAttribute)attrs[0]).Title;
                return componentName;
            }
        }

        /// <summary>
        /// The version of this component
        /// </summary>
        public string ComponentVersion
        {
            get
            {
                string componentVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                return componentVersion;
            }
        }

        /// <summary>
        /// The service data
        /// </summary>
        public Services Services { get; private set; }


        /// <summary>
        /// Main constructor
        /// </summary>
        public Watcher()
        {
            _objectLock = new object();
            _json = new List<string>();
            Services = new Services();
            Enabled = true;
        }

        /// <summary>
        /// Starts the application tracking.
        /// </summary>
        /// <param name="applicationId">
        /// Your app ID. You can get it at http://analytics.deskmetrics.com/
        /// </param>
        /// <param name="applicationVersion">
        /// Your app version.
        /// </param>
        public void Start(string applicationId, string applicationVersion)
        {
            if (applicationId == null || applicationId.Trim() == string.Empty)
                throw new ArgumentException("You must specify an non-empty application ID");
            if (applicationVersion == null || applicationVersion.Trim() == string.Empty)
                throw new ArgumentException("You must specify an non-empty application version");

            lock (_objectLock)
            {
                if (Started)
                    throw new InvalidOperationException("Start has already been called");

                CheckIfEnabled();

                _applicationId = applicationId;
                _applicationVersion = applicationVersion;

                _sessionGUID = Util.GetNewSessionID();
                _userGUID = Util.GetCurrentUserID();
                _cache = new Cache(_applicationId);
                
                ResetFlowNumber();

                AddCachedData();
                AddStartJson();

                Started = true;
            }
        }

        /// <summary>
        /// Stops the application tracking and send the collected data to DeskMetrics
        /// </summary>
        public void Stop()
        {
            lock (_objectLock)
            {
                if (Started == false)
                    throw new InvalidOperationException("The component has NOT been started");

                CheckIfEnabled();

                _stopThread = new Thread(_StopThreadFunc);
                _stopThread.Name = "StopSender";
                _stopThread.Start();
            }
        }

        /// <summary>
        /// Register an event occurence
        /// </summary>
        /// <param name="eventCategory">EventCategory Category</param>
        /// <param name="eventName">EventCategory Name</param>
        public void TrackEvent(string eventCategory, string eventName)
        {
            lock (_objectLock)
            {
                if (Started)
                {
                    CheckIfEnabled();
                    var json = new EventJson(_sessionGUID, eventCategory, eventName, GetFlowNumber());
                    _json.Add(JsonBuilder.GetJsonFromHashTable(json.GetJsonHashTable()));
                }
            }
        }

        /// <summary>
        /// Tracks an event related to time and intervals
        /// </summary>
        /// <param name="eventCategory">
        /// The event category
        /// </param>
        /// <param name="eventName">
        /// The event name
        /// </param>
        /// <param name="eventTime">
        /// The event duration 
        /// </param>
        /// <param name="completed">
        /// True if the event was completed.
        /// </param>
        public void TrackEventPeriod(string eventCategory, string eventName, int eventTime, bool completed)
        {
            lock (_objectLock)
            {
                if (Started)
                {
                    CheckIfEnabled();
                    var json = new EventPeriodJson(_sessionGUID, eventCategory, eventName, GetFlowNumber(), eventTime, completed);
                    _json.Add(JsonBuilder.GetJsonFromHashTable(json.GetJsonHashTable()));
                }
            }
        }
        
        /// <summary>
        /// Tracks an exception
        /// </summary>
        /// <param name="exception">
        /// The exception object to be tracked
        /// </param>
        public void TrackException(Exception exception)
        {
            if (exception == null)
                throw new ArgumentNullException("exception");

            lock (_objectLock)
            {
                if (Started)
                {
                    CheckIfEnabled();
                    var json = new ExceptionJson(_sessionGUID, exception, GetFlowNumber());
                    _json.Add(JsonBuilder.GetJsonFromHashTable(json.GetJsonHashTable()));
                }
            }
        }

        /// <summary>
        /// Tracks an installation
        /// </summary>
        /// <param name="version">
        /// Your app version
        /// </param>
        /// <param name="appid">
        /// Your app ID. You can get it at http://analytics.deskmetrics.com/
        /// </param>
        public void TrackInstall(string version, string appid)
        {
            lock (_objectLock)
            {
                string installSessionGUID = Util.GetNewSessionID();
                var json = new InstallJson(installSessionGUID, version, GetFlowNumber());
                try
                {
                    Services.PostData(appid, JsonBuilder.GetJsonFromHashTable(json.GetJsonHashTable()));
                }
                catch (WebException)
                {
                    // only hide unhandled exception due no internet connection
                }
            }
        }

        /// <summary>
        /// Tracks an uninstall
        /// </summary>
        /// <param name="version">
        /// Your app version
        /// </param>
        /// <param name="appid">
        /// Your app ID. You can get it at http://analytics.deskmetrics.com/
        /// </param>
        public void TrackUninstall(string version, string appid)
        {
            lock (_objectLock)
            {
                string uninstallSessionGUID = Util.GetNewSessionID();
                var json = new UninstallJson(uninstallSessionGUID, version, GetFlowNumber());
                try
                {
                    Services.PostData(appid, JsonBuilder.GetJsonFromHashTable(json.GetJsonHashTable()));
                }
                catch (WebException)
                {
                    // only hide unhandled exception due no internet connection
                }
            }
        }
        
        /// <summary>
        /// Tracks an event with custom value
        /// </summary>
        /// <param name="eventCategory">
        /// The event category
        /// </param>
        /// <param name="eventName">
        /// The event name
        /// </param>
        /// <param name="eventValue">
        /// The custom value
        /// </param>
        public void TrackEventValue(string eventCategory, string eventName, string eventValue)
        {
            lock (_objectLock)
            {
                if (Started)
                {
                    CheckIfEnabled();
                    var json = new EventValueJson(_sessionGUID, eventCategory, eventName, eventValue, GetFlowNumber());
                    _json.Add(JsonBuilder.GetJsonFromHashTable(json.GetJsonHashTable()));
                }
            }
        }

        /// <summary>
        /// Tracks custom data
        /// </summary>
        /// <param name="customDataName">
        /// The custom data name
        /// </param>
        /// <param name="customDataValue">
        /// The custom data value
        /// </param>
        public void TrackCustomData(string customDataName, string customDataValue)
        {
            lock (_objectLock)
            {
                if (Started)
                {
                    CheckIfEnabled();
                    var json = new CustomDataJson(_sessionGUID, customDataName, customDataValue, GetFlowNumber());
                    _json.Add(JsonBuilder.GetJsonFromHashTable(json.GetJsonHashTable()));
                }
            }
        }

        /// <summary>
        /// Tracks a log
        /// </summary>
        /// <param name="message">
        /// The log message
        /// </param>
        public void TrackLog(string message)
        {
            lock (_objectLock)
            {
                if (Started)
                {
                    CheckIfEnabled();
                    var json = new LogJson(_sessionGUID, message, GetFlowNumber());
                    _json.Add(JsonBuilder.GetJsonFromHashTable(json.GetJsonHashTable()));
                }
            }
        }
        
        /// <summary>
        /// Try to track real time customized data and caches it to send later if any network error occurs.
        /// </summary>
        /// <param name="customDataName">
        /// A <see cref="System.String"/>
        /// </param>
        /// <param name="customDataValue">
        /// A <see cref="System.String"/>
        /// </param>
        /// <returns>
        /// True if it was sended in real time, false otherwise
        /// </returns>
        public bool TrackCachedCustomDataR(string customDataName, string customDataValue)
        {
            lock (_objectLock)
            {
                if (Started)
                {
                    CheckIfEnabled();

                    bool sentInRealTime = TrackCustomDataR(customDataName, customDataValue);
                    if(sentInRealTime == false)
                    {
                        var json = new CustomDataRJson(_sessionGUID, customDataName, customDataValue, GetFlowNumber(), _applicationId, _applicationVersion);
                        _json.Add(JsonBuilder.GetJsonFromHashTable(json.GetJsonHashTable()));
                    }
                    return sentInRealTime;
                }
                else
                    return false;
            }
        }

        /// <summary>
        /// Tracks a custom data without cache support
        /// </summary>
        /// <param name="customDataName">
        /// Self-explanatory ;)
        /// </param>
        /// <param name="customDataValue">
        /// Self-explanatory ;)
        /// </param>
        /// <returns>
        /// True if it was sent in real time, false otherwise
        /// </returns>
        public bool TrackCustomDataR(string customDataName, string customDataValue)
        {
            lock (_objectLock)
            {
                if (Started)
                {
                    CheckIfEnabled();
                    try
                    {
                        var json = new CustomDataRJson(_sessionGUID, customDataName, customDataValue, GetFlowNumber(), _applicationId, _applicationVersion);
                        Services.PostData(_applicationId, JsonBuilder.GetJsonFromHashTable(json.GetJsonHashTable()));
                        return true;
                    }
                    catch (WebException)
                    {
                        return false;
                    }
                }
                else
                    return false;
            }
        }

        /// <summary>
        /// Sends the data collected so far.
        /// If the data could not be sent it is saved to cache.
        /// </summary>
        public void SendDataAsync()
        {
            lock (_objectLock)
            {
                if (Started)
                {
                    CheckIfEnabled();

                    Thread thread = new Thread(() =>
                    {
                        lock (_objectLock)
                        {
                            string jsonText = JsonBuilder.GetJsonFromList(_json);
                            try
                            {
                                Services.PostData(_applicationId, jsonText);
                                _json.Clear();
                                _cache.Delete();
                            }
                            catch (Exception)
                            {
                                _cache.Save(jsonText);
                            }
                        }
                    });
                    thread.Start();
                }
            }
        }

        /// <summary>
        /// Adds the start data to the list
        /// </summary>
        private void AddStartJson()
        {
            var startjson = new StartAppJson(_sessionGUID, _applicationVersion, _userGUID);
            _json.Add(JsonBuilder.GetJsonFromHashTable(startjson.GetJsonHashTable()));
        }

        /// <summary>
        /// Adds the stop data to the list
        /// </summary>
        private void AddStopJson()
        {
            var json = new StopAppJson(_sessionGUID);
            _json.Add(JsonBuilder.GetJsonFromHashTable(json.GetJsonHashTable()));
        }

        /// <summary>
        /// Adds the caches data (if exists) to the list
        /// </summary>
        private void AddCachedData()
        {
            string cachedData = _cache.GetCacheData();
            //Checks if there is no cache file or it is empty
            if (string.IsNullOrEmpty(cachedData))
                return;
            //Checks if the cache file content is valid
            bool success = false;
            Json.Json.JsonDecode("[" + cachedData + "]", ref success);
            if (success == false)
                return;
            //Adds the file content to the json list
            _json.Add(cachedData);
        }

        /// <summary>
        /// Thread which sends the data on application exit
        /// </summary>
        private void _StopThreadFunc()
        {
            lock (_objectLock)
            {
                AddStopJson();

                string jsonText = JsonBuilder.GetJsonFromList(_json);
                try
                {
                    Services.PostData(_applicationId, jsonText);
                    _json.Clear();
                    _cache.Delete();
                }
                catch (Exception)
                {
                    _cache.Save(jsonText);
                }

                Started = false;
            }
        }

        /// <summary>
        /// Checks if the component is enabled, if not raises an exception
        /// </summary>
        private void CheckIfEnabled()
        {
            if (!Enabled)
                throw new InvalidOperationException("The component is not enabled");
        }
        
        /// <summary>
        /// Resets the Flow number
        /// </summary>
        private void ResetFlowNumber()
        {
            _flowNumber = 1;
        }

        /// <summary>
        /// Gets the next value to use as Flow number
        /// </summary>
        /// <returns></returns>
        private int GetFlowNumber()
        {
            int n = _flowNumber;
            _flowNumber++;
            return n;
        }

        /// <summary>
        /// Disposes this component
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            //Do not finalize this object as it is already disposed
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes this component
        /// </summary>
        /// <param name="disposing">True if manually disposing, False if called by the framework</param>
        protected virtual void Dispose(bool disposing)
        {
            if (Started && Enabled)
                Stop();
        }
    }
}
