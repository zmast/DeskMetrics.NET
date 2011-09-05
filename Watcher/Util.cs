// **********************************************************************//
//                                                                       //
//     DeskMetrics NET - Util.cs                                         //
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;

namespace DeskMetrics
{
    static class Util
    {
        /// <summary>
        /// Timestamp GMT +0
        /// </summary>
        public static int GetTimeStamp()
        {
            try
            {
                double _timeStamp = 0;
                DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                TimeSpan diff = DateTime.UtcNow - origin;
                _timeStamp = Math.Floor(diff.TotalSeconds);
                return Convert.ToInt32(_timeStamp);
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// Gets a new session ID
        /// </summary>
        /// <returns>A new sesion ID</returns>
        public static string GetNewSessionID()
        {
            return GetNewID();
        }

        /// <summary>
        /// Gets the current user ID
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentUserID()
        {
            string userID;

            using (RegistryKey reg = Registry.CurrentUser.CreateSubKey("Software\\dskMetrics"))
            {
                userID = reg.GetValue("ID") as string;
                if(userID == null)
                {
                    userID = GetNewID();
                    reg.SetValue("ID", userID, RegistryValueKind.String);
                }
            }

            return userID;
        }

        /// <summary>
        /// Gets a new ID which can be used to uniquely identify a session or a user
        /// </summary>
        /// <returns></returns>
        private static string GetNewID()
        {
            return System.Guid.NewGuid().ToString().Replace("-", "").ToUpper();
        }
    }
}
