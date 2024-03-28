using MeetingSchedulerConsoleApp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
namespace MeetingSchedulerTests
{
    [TestClass]
    public class SchedulerTests
    {
        [TestMethod]
        public void ScheduleMeeting_WithOverlappingSchedulesWithinSinglePerson_ReturnsCorrectInterval()
        {
            var persons = new List<Person>
            {
                new Person
                {
                    Schedule = new List<Interval>
                    {
                        new Interval { Start = 0, End = 900 }, //Outside of office time
                        new Interval { Start = 900, End = 1030 },
                        new Interval { Start = 1000, End = 1100 } // Overlaps
                    }
                },
                new Person
                {
                    Schedule = new List<Interval>()
                }
            };
            int duration = 30;
            var result = MeetingScheduler.ScheduleMeeting(persons, duration);
            Assert.IsNotNull(result);
            Assert.AreEqual(1100, result?.Start);
            Assert.AreEqual(1130, result?.End);
        }
        [TestMethod]
        public void ScheduleMeeting_WithOverlappingSchedulesAcrossPersons_ReturnsCorrectInterval()
        {
            var persons = new List<Person>
            {
                new Person
                {
                    Schedule = new List<Interval> { new Interval { Start = 900, End = 1000 } }
                },
                new Person
                {
                    Schedule = new List<Interval> { new Interval { Start = 950, End = 1050 } } // Overlaps
                }
            };
            int duration = 45;
            var result = MeetingScheduler.ScheduleMeeting(persons, duration);
            Assert.IsNotNull(result);
            Assert.AreEqual(1050, result?.Start);
            Assert.AreEqual(1095, result?.End);
        }
        [TestMethod]
        public void ScheduleMeeting_NoOverlapping_ReturnsFirstAvailableSlot()
        {
            var persons = new List<Person>
            {
                new Person
                {
                    Schedule = new List<Interval>
                    {
                        new Interval { Start = 800, End = 900 },
                        new Interval { Start = 1000, End = 1100 }
                    }
                },
                new Person
                {
                    Schedule = new List<Interval> { new Interval { Start = 900, End = 950 } }
                }
            };
            int duration = 45;
            var result = MeetingScheduler.ScheduleMeeting(persons, duration);
            Assert.IsNotNull(result);
            Assert.AreEqual(950, result?.Start);
            Assert.AreEqual(995, result?.End);
        }
        [TestMethod]
        public void ScheduleMeeting_LargeDuration_NoAvailableSlot()
        {
            var persons = new List<Person>
            {
                new Person
                {
                    Schedule = new List<Interval> { new Interval { Start = 900, End = 1400 } }
                }
            };
            int duration = 400; // Too large
            var result = MeetingScheduler.ScheduleMeeting(persons, duration);
            Assert.IsNull(result);
        }
        [TestMethod]
        public void ScheduleMeeting_WhenSchedulesAreEmpty_ReturnsFirstPossibleSlot()
        {
            var persons = new List<Person> { new Person { Schedule = new List<Interval>() }, new Person { Schedule = new List<Interval>() } };
            int duration = 60;
            var result = MeetingScheduler.ScheduleMeeting(persons, duration);
            Assert.IsNotNull(result);
            Assert.AreEqual(900, result?.Start); // Assuming start of day
            Assert.AreEqual(960, result?.End);
        }
        [TestMethod]
        public void ScheduleMeeting_WhenOnlyOnePersonHasSchedule_ReturnsCorrectInterval()
        {
            var persons = new List<Person>
            {
                new Person { Schedule = new List<Interval> { new Interval { Start = 900, End = 1000 } } },
                new Person { Schedule = new List<Interval>() }
            };
            int duration = 50;
            var result = MeetingScheduler.ScheduleMeeting(persons, duration);
            Assert.IsNotNull(result);
            Assert.AreEqual(1000, result?.Start);
            Assert.AreEqual(1050, result?.End);
        }
        [TestMethod]
        public void ScheduleMeeting_WithBackToBackMeetings_FindsNextAvailableSlot()
        {
            var persons = new List<Person>
            {
                new Person { Schedule = new List<Interval> { new Interval { Start = 900, End = 1000 }, new Interval { Start = 1000, End = 1100 } } },
                new Person { Schedule = new List<Interval> { new Interval { Start = 1100, End = 1200 } } }
            };
            int duration = 90;
            var result = MeetingScheduler.ScheduleMeeting(persons, duration);
            Assert.IsNotNull(result);
            Assert.AreEqual(1200, result?.Start);
            Assert.AreEqual(1290, result?.End);
        }
        [TestMethod]
        public void ScheduleMeeting_WhenDurationIsTooLarge_ReturnsNull()
        {
            var persons = new List<Person>
            {
                new Person { Schedule = new List<Interval> { new Interval { Start = 1300, End = 1600 }, new Interval { Start = 900, End = 1200 } } }
            };
            int duration = 500; // Very large
            var result = MeetingScheduler.ScheduleMeeting(persons, duration);
            Assert.IsNull(result);
        }
        [TestMethod]
        public void ScheduleMeeting_WhenMeetingFitsAtTheDayStart_ReturnsFirstInterval()
        {
            var persons = new List<Person>
            {
                new Person { Schedule = new List<Interval> { new Interval { Start = 1100, End = 1200 } } }
            };
            int duration = 60;
            var result = MeetingScheduler.ScheduleMeeting(persons, duration);
            Assert.IsNotNull(result);
            Assert.AreEqual(900, result?.Start); // Assuming day starts at 0
            Assert.AreEqual(960, result?.End);
        }
        [TestMethod]
        public void ScheduleMeeting_WhenExactFitBetweenTwoMeetings_ReturnsThatInterval()
        {
            var persons = new List<Person>
            {
                new Person { Schedule = new List<Interval> { new Interval { Start = 900, End = 1050 },
                    new Interval { Start = 1000, End = 1200 }, new Interval { Start = 1250, End = 1350 } } }
            };
            int duration = 50; // Exact gap
            var result = MeetingScheduler.ScheduleMeeting(persons, duration);
            Assert.IsNotNull(result);
            Assert.AreEqual(1200, result?.Start);
            Assert.AreEqual(1250, result?.End);
        }
    }
}