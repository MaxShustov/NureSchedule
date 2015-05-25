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
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create ( "http://cist.nure.ua/ias/app/tt/" + requestString );
            HttpWebResponse response = ( await request.GetResponseAsync () ) as HttpWebResponse;
            StreamReader readStream = new StreamReader ( response.GetResponseStream (), Encoding.GetEncoding ( "windows-1251" ) );
            return await readStream.ReadToEndAsync ();
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
                Select(type => new Type((int)type["id"], type["short_name"].ToString())).ToList();
            var test = (int)JObject.Parse(result)["events"][0]["teachers"][0];
            var list = (from timeTableEvent in JObject.Parse (result) ["events"].Values <JToken> ()
                         let teacher = teachers.Single ( t => t.Item1 == (int)timeTableEvent ["teachers"] [0] )
                         let subject = subjects.Single ( s => s.Item1 == (int)timeTableEvent ["subject_id"] )
                         let type = types.Single ( t => t.Item1 == (int)timeTableEvent ["type"])
                         let startTime = FromUnixTime ( timeTableEvent ["start_time"].ToString () )
                         let endTime = FromUnixTime ( timeTableEvent ["end_time"].ToString () )
                         select new TimeTableEvent ( startTime, endTime, teacher, subject, timeTableEvent["auditory"].ToString (), (int)timeTableEvent["number_pair"], type )).AsParallel ().ToList ();
            return list;
        }

        public static DateTime FromUnixTime ( string unixTime )
        {
            var epoch = new DateTime ( 1970, 1, 1, 0, 0, 0, DateTimeKind.Utc );
            return epoch.AddSeconds ( Convert.ToInt64 ( unixTime ) ).AddHours ( 3 );
        }

        public static string ToUnixTime ( DateTime date )
        {
            var epoch = new DateTime ( 1970, 1, 1, 0, 0, 0, DateTimeKind.Utc );
            return Convert.ToInt64 ( ( date - epoch ).TotalSeconds ).ToString ();
        }
    }
}
