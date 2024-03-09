using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JohnUtilities.Model;
using System.Xml;
using JohnUtilities.Services.Interfaces;
using JohnUtilities.Model.Classes;
using JohnUtilities.Services.Adapters;


namespace JohnUtilities.UnitTests
{
    [TestClass]
    public class EventReportingUnitTests
    {
        [TestMethod]
        public void GetEventReporter_GivenInitilization_ReturnsInstance()
        {

            var GetReportingClass = EventReporting.GetEventReporter();

            Assert.IsNotNull(GetReportingClass);

        }
        [TestMethod]
        public void ReportError_GivenInvokedEvent_EventRaised()
        {
            EventReporting.GetEventReporter().CreateEventType(BaseEventTypes.ErrorNotice);

            bool EventTriggered = false;

            EventReporting.GetEventReporter().SubscribeToEvent(BaseEventTypes.ErrorNotice, (Object sender, NotificationEventArgs e) => EventTriggered = true);

            EventReporting.GetEventReporter().InvokeEvent(BaseEventTypes.ErrorNotice, "This is a test error");

            Assert.IsTrue(EventTriggered);

        }

    }
}
