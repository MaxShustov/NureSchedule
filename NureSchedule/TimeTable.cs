using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
// a, b, c
using Teacher = System.Tuple<int, string, string>;
using Subject = System.Tuple<int, string, string, System.Tuple<int, int>>;
using Type = System.Tuple<int, string>;

namespace NureSchedule
{
    public class TimeTable
    {
        private async Task<string> GetResult ( string requestString )
        {
            HttpWebRequest request = WebRequest.Create("http://cist.nure.ua/ias/app/tt/" + requestString) as HttpWebRequest;
            using (WebResponse response = await request.GetResponseAsync())
                using (StreamReader readStream = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("windows-1251")))
                    return await readStream.ReadToEndAsync();
        }

        public static string GetCodeYear( int year )
        {
            if (DateTime.Now.Month >= 1 && DateTime.Now.Month <= 8)
                return (DateTime.Now.Year - 1 - 2000 - year + 1).ToString();
            else
                return (DateTime.Now.Year - 2000 - year + 1).ToString();
        }

        public async Task <IEnumerable <JToken>> GetFaculties ()
        {
            string result = await GetResult ( "P_API_FACULTIES_JSON" );
            return JObject.Parse ( result ) ["university"] ["faculties"].Values<JToken> ();
        }

        public async Task<IEnumerable<JToken>> GetDirections( string facultyID )
        {
            string result = await GetResult("P_API_DIRECTIONS_JSON?p_id_faculty=" + facultyID);
            return JObject.Parse(result)["faculty"]["directions"].Values<JToken>();
        }

        public async Task<IEnumerable<JToken>> GetSpecialities( string facultyID )
        {
            List<JToken> specialities = new List<JToken>();
            var directions = await GetDirections(facultyID);
            foreach (var direction in directions)
            {
                var spec = await GetResult("P_API_SPECIALITIES_JSON?p_id_direction=" + direction["id"].ToString() + "&p_id_faculty=" + facultyID);
                specialities.AddRange( JObject.Parse ( spec ) ["faculty"] ["directions"] [0] ["specialities"].Values <JToken> ());
            }
            return specialities;
        }

        public async Task<IEnumerable<JToken>> GetDirectionGroups(string directionID, string facultyID)
        {
            string result = await GetResult("P_API_GRP_OF_DIRECTIONS_JSON?p_id_direction=" + directionID + "&p_id_faculty=" + facultyID);
            return JObject.Parse(result) ["faculty"] ["directions"] [0] ["groups"].Values<JToken>();
        }

        public async Task<IEnumerable<JToken>> GetSpecialityGroups( string specialituID, string facultyID )
        {
            string result = await GetResult("P_API_GRP_OF_SPECIALITIES_JSON?p_id_speciality=" + specialituID + "&p_id_faculty=" + facultyID);
            var list = JObject.Parse(result)["speciality"]["groups"].Values<JToken>().ToList ();
            return list;
        }

        public async Task <IEnumerable <TimeTableEvent>> GetSchedule(string groupID, string timeFrom = null, string timeTo = null)
        {
            var result = await GetResult("P_API_EVENTS_GROUP_JSON?p_id_group=" + groupID );
            var teachers = JObject.Parse(result)["teachers"].Values<JToken>().
                Select(teacher => new Teacher((int)teacher["id"], teacher["short_name"].ToString(), teacher["full_name"].ToString())).ToList ();
            var subjects = JObject.Parse(result)["subjects"].Values<JToken>().
                Select(subject => new Subject ((int)subject["id"], subject["brief"].ToString(), subject ["title"].ToString (),
                    new System.Tuple<int, int>((int)subject["hours"][0]["type"], (int)subject["hours"][0]["val"]))).ToList();
            var types = JObject.Parse(result)["types"].Values<JToken>().
                Select(type => new System.Tuple <int, string> ( (int)type["id"], type ["type"].ToString ())).ToList();
            var test = (int)JObject.Parse(result)["events"][0]["teachers"][0];
            List<TimeTableEvent> list = new List<TimeTableEvent>();
            foreach (var timeTableEvent in JObject.Parse(result)["events"].Values<JToken>())
            {
                var tchs = from teacher in timeTableEvent["teachers"].Values <JToken> ()
                           let fullTeacher = teachers.Single ( t => (int)teacher == t.Item1 )
                           select fullTeacher;
                var subject = subjects.Single(s => s.Item1 == (int)timeTableEvent["subject_id"]);
                Type type = 0;
                switch (types.Single(t => t.Item1 == (int)timeTableEvent["type"]).Item2)
                {
                    case "lecture": type = Type.Lecture; break;
                    case "practice": type = Type.Practice; break;
                    case "laboratory": type = Type.Laboratory; break;
                    case "consultation": type = Type.Consultation; break;
                    case "exam": type = Type.Exam; break;
                    case "test": type = Type.Test; break;
                }
                var startTime = FromUnixTime ( timeTableEvent ["start_time"].ToString () );
                var endTime = FromUnixTime(timeTableEvent["end_time"].ToString());
                list.Add(new TimeTableEvent(startTime, endTime, tchs, subject, timeTableEvent["auditory"].ToString(), (int)timeTableEvent["number_pair"], type));
            }
            return list;
        }

        public static DateTime FromUnixTime ( string unixTime )
        {
            var epoch = new DateTime ( 1970, 1, 1, 0, 0, 0, DateTimeKind.Utc );
            return epoch.AddSeconds ( Convert.ToInt64 ( unixTime ) ).AddHours ( 2 );
        }

        public static string ToUnixTime ( DateTime date )
        {
            var epoch = new DateTime ( 1970, 1, 1, 0, 0, 0, DateTimeKind.Utc );
            return Convert.ToInt64 ( ( date - epoch ).TotalSeconds ).ToString ();
        }
    }
}
