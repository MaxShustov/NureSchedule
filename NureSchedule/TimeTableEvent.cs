using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NureSchedule
{
    using Teacher = System.Tuple<int, string, string>;
    using Subject = System.Tuple<int, string, string, System.Tuple<int, int>>;

    public enum Type
    { 
        Lecture, Practice, Laboratory, Consultation, Test, Exam
    }

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
        public static readonly Dictionary<Type, string> NameOfType = new Dictionary<Type, string>() { {Type.Lecture, "Лк"}, {Type.Laboratory, "Лб"}, {Type.Practice, "Пз"},
                                                                                              {Type.Exam, "Екз"}, {Type.Test, "Тест"}, {Type.Consultation, "Конс"}};

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
