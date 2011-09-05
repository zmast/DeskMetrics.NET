// **********************************************************************//
//                                                                       //
//     DeskMetrics NET - Cache.cs                                        //
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
using System.IO;
using System.Collections.Generic;
using DeskMetrics.Json;

namespace DeskMetrics
{
    class Cache
    {
        private object _objectLock = new object();
        private string _filename;

        public Cache(string applicationId)
        {
            _filename = Path.GetTempPath() + applicationId + ".dsmk";
        }
        
        public bool Delete()
        {
            lock (_objectLock)
            {
                if (File.Exists(_filename))
                {
                    File.Delete(_filename);
                    return true;
                }
                return false;
            }
        }

        public string GetCacheData()
        {
            lock (_objectLock)
            {
                if (File.Exists(_filename) == false)
                    return null;

                string fileContents;
                try
                {
                    fileContents = File.ReadAllText(_filename);
                }
                catch
                {
                    fileContents = null;
                }

                return fileContents;
            }
        }

        public void Save(string data)
        {
            lock (_objectLock)
            {
                try
                {
                    File.WriteAllText(_filename, data);
                    File.SetAttributes(_filename, FileAttributes.Hidden);
                }
                catch { }
            }
        }
    }
}

