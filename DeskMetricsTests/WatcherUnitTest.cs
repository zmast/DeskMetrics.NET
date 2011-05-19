using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DeskMetrics;

namespace DeskMetricsTest
{
    [TestClass]
    public class WatcherUnitTest
    {
        Watcher watcher;
        [TestInitialize]
        public void setup()
        {
            watcher = new Watcher(new ServiceStub());
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void Start_WhenTheApplicationHasNoID_ItShouldFail()
        {
            watcher.Start(null,"");
            Assert.Fail("It should throw an exception when the app id is not set");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Start_WhenTheApplicationHasEmptyID_ItShouldFail()
        {
            watcher.Start("","");
            Assert.Fail("It should throw an exception when the app id is empty");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Start_WhenTheApplicationIsNotEnabled_ItShouldFail()
        {
            watcher.Enabled = false;
            watcher.Start("some_id", "");
            Assert.Fail("It should throw an exception when the application is not enabled");
        }

        [TestMethod]
        public void TrackEvent_WhenTheApplicationIsStarted_TheJSONListMustHaveTwoItems()
        {
            watcher.Start("some_id", "Some version");
            watcher.TrackEvent("some category", "some name");
            Assert.AreEqual(watcher.JSON.Count, 2);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException),"You must Start the application before call this method")]
        public void TrackEvent_WhenTheApplicationIsNotStarted_ItShouldThrowInvalidOperationException()
        {
            watcher.TrackEvent("some category", "some name");
        }


        [TestMethod]
        public void TrackEventValue_WhenTheApplicationIsStarted_TheJSONListMustHaveTwoItems()
        {
            watcher.Start("some_id", "Some version");
            watcher.TrackEventValue("some category", "some name", "some value");
            Assert.AreEqual(watcher.JSON.Count, 2);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException), "You must Start the application before call this method")]
        public void TrackEventValue_WhenTheApplicationIsNotStarted_ItShouldThrowInvalidOperationException()
        {
            watcher.TrackEventValue("some category", "some name","some value");
        }

        [TestMethod]
        public void TrackEventPeriod_WhenTheApplicationIsStarted_TheJSONListMustHaveTwoItems()
        {
            watcher.Start("some_id", "Some version");
            watcher.TrackEventPeriod("some category", "some name",200,true);
            Assert.AreEqual(watcher.JSON.Count, 2);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException), "You must Start the application before call this method")]
        public void TrackEventPeriod_WhenTheApplicationIsNotStarted_ItShouldThrowInvalidOperationException()
        {
            watcher.TrackEventPeriod("some category", "some name",200,true);
        }

        [TestMethod]
        public void TrackLog_WhenTheApplicationIsStarted_TheJSONListMustHaveTwoItems()
        {
            watcher.Start("some_id", "Some version");
            watcher.TrackLog("some message");
            Assert.AreEqual(watcher.JSON.Count, 2);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException), "You must Start the application before call this method")]
        public void TrackLog_WhenTheApplicationIsNotStarted_ItShouldThrowInvalidOperationException()
        {
            watcher.TrackLog("some message");
        }

        [TestMethod]
        public void TrackCustomData_WhenTheApplicationIsStarted_TheJSONListMustHaveTwoItems()
        {
            watcher.Start("some_id", "Some version");
            watcher.TrackCustomData("some name", "some value");
            Assert.AreEqual(watcher.JSON.Count, 2);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException), "You must Start the application before call this method")]
        public void TrackCustomData_WhenTheApplicationIsNotStarted_ItShouldThrowInvalidOperationException()
        {
            watcher.TrackCustomData("some name","some value");
        }

        [TestMethod]
        public void TrackException_WhenTheApplicationIsStartedAndExceptionIsNotNull_TheJSONListMustHaveTwoItems()
        {
            watcher.Start("some_id", "Some version");
            watcher.TrackException(new Exception("Oh noes!"));
            Assert.AreEqual(watcher.JSON.Count, 2);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException),"The exception cannot be null")]
        public void TrackException_WhenTheApplicationIsStartedAndExceptionIsNull_TheJSONListMustHaveOneItem()
        {
            watcher.Start("some_id", "Some version");
            watcher.TrackException(null);
            Assert.AreEqual(watcher.JSON.Count, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException), "You must Start the application before call this method")]
        public void TrackException_WhenTheApplicationIsNotStarted_ItShouldThrowInvalidOperationException()
        {
            watcher.TrackException(new Exception("Oh noes!"));
        }

        [TestMethod]
        public void SendOrCacheData_IfTheApplicationIsNotSetToRunInRealTime_ItShouldCacheTheJson()
        {
            watcher.RealTime = false;
            watcher.Start("appid","appversion");
            watcher.TrackEvent("some category", "some event");
            Assert.AreEqual(watcher.JSON.Count, 2);
        }

        [TestMethod]
        public void SendOrCacheData_IfTheApplicationIsSetToRunInRealTime_ItShouldNotCacheTheJson()
        {
            watcher.RealTime = true;
            watcher.Start("appid", "appversion");
            watcher.TrackEvent("some category", "some event");
            Assert.AreEqual(watcher.JSON.Count, 0);
        }
    }
}
