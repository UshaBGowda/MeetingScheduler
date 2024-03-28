using System;
using System.Collections.Generic;
using System.Linq;
namespace MeetingSchedulerConsoleApp
{
    // Defines a time interval
    public struct Interval
    {
        public int Start; // Start time
        public int End;   // End time
    }
    // Represents a person with a schedule
    public class Person
    {
        public List<Interval> Schedule { get; set; } // A person’s schedule
    }
    // Class to schedule meetings
    public class MeetingScheduler
    {
        // FOR SIMPLICITY Keeping times in integers
        private static readonly int DayStart = 900;
        private static readonly int DayEnd = 1700;
        // Schedules a meeting to fit all provided schedules
        public static Interval? ScheduleMeeting(List<Person> persons, int duration)
        {
            List<Interval> mergedSchedules = new List<Interval>();
            // Merging schedules for all persons
            foreach (var person in persons)
            {
                mergedSchedules.AddRange(person.Schedule);
            }
            // Consolidate the merged schedules to handle overlaps
            List<Interval> consolidatedSchedules = ConsolidateSchedule(mergedSchedules);
            // Start the search from the beginning of the workday
            int potentialStart = DayStart;
            // First, check if there’s room at the start of the day before any meetings
            if (consolidatedSchedules.Any() && consolidatedSchedules[0].Start - potentialStart >= duration)
            {
                return new Interval { Start = potentialStart, End = potentialStart + duration };
            }
            // Then check between scheduled meetings
            foreach (var slot in consolidatedSchedules)
            {
                if (slot.Start - potentialStart >= duration)
                {
                    return new Interval { Start = potentialStart, End = potentialStart + duration };
                }
                potentialStart = Math.Max(potentialStart, slot.End);
            }
            // Finally, check after the last meeting ends, but before the end of the workday
            if (DayEnd - potentialStart >= duration)
            {
                return new Interval { Start = potentialStart, End = potentialStart + duration };
            }
            // If no suitable slot is found within work hours
            return null;
        }
        // Consolidates overlapping intervals
        private static List<Interval> ConsolidateSchedule(List<Interval> schedule)
        {
            if (!schedule.Any()) return schedule;
            // Sort intervals by start time
            schedule.Sort((a, b) => a.Start.CompareTo(b.Start));
            List<Interval> consolidated = new List<Interval>();
            Interval current = schedule[0];
            foreach (var next in schedule.Skip(1))
            {
                if (current.End >= next.Start)
                {
                    // Overlapping intervals, merge them
                    current.End = Math.Max(current.End, next.End);
                }
                else
                {
                    // No overlap, add current to the consolidated list, and move to the next
                    consolidated.Add(current);
                    current = next;
                }
            }
            // Add the last interval
            consolidated.Add(current);
            return consolidated;
        }
    }
    public class Program
    {
        public static void Main()
        {
            List<Person> persons = new List<Person>
        {
            new Person { Schedule = new List<Interval> { new Interval { Start = 900, End = 1030 }, new Interval { Start = 1200, End = 1300 } } },
            new Person { Schedule = new List<Interval> { new Interval { Start = 1050, End = 1150 }, new Interval { Start = 1400, End = 1500 } } }
        };
            int duration = 45; // Example duration
            var meetingSlot = MeetingScheduler.ScheduleMeeting(persons, duration);
            if (meetingSlot.HasValue)
            {
                Console.WriteLine($"Meeting can be scheduled from { meetingSlot.Value.Start} to { meetingSlot.Value.End}.");
            }
            else
            {
                Console.WriteLine("No suitable meeting time found.");
            }
        }
    }
}