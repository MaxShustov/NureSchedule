using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NureSchedule
{
    using Teacher = System.Tuple<int, string, string>;
    using Subject = System.Tuple<int, string, string, System.Tuple<int, int>>;
    using Type = System.Tuple<int, string>;

    public class TimeTableEvent
    {
        public readonly DateTime StartTime;
        public readonly DateTime EndTime;
        //id, short_name, full_name
        public readonly List<Teacher> Teachers;
        //id, brief, title, hours
        public readonly Subject Subject;
        public readonly string Auditory;
        public readonly int NumberOfPair;
        //id, short_name
        public readonly Type Type;

        public TimeTableEvent(DateTime start, DateTime end, IEnumerable<Teacher> teachers, Subject subject, string auditory, int numberOfPair, Type type)
        {
            StartTime = start;
            EndTime = end;
            Teachers = new List<Teacher>();
            if (teachers != null)
                Teachers.AddRange (teachers);
            Subject = subject;
            Auditory = auditory;
            NumberOfPair = numberOfPair;
            Type = type;
        }
    }
}
