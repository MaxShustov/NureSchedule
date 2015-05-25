using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace NureTimeTable
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

        public async Task<IEnumerable<JToken>> GetSpecialityGroups( string specialituID, string facultyID, int year )
        {
            string result = await GetResult("P_API_GROUP_JSON");
            var faculty = JObject.Parse(result)["university"]["faculties"].Values<JToken>().
                First(fac => fac["id"].ToString() == facultyID);
            var specialityGroups = (from dir in faculty["directions"]
                                let specialities = dir["specialities"].Values<JToken>()
                                let speciality = specialities.SingleOrDefault(spec => spec["id"].ToString() == specialituID)
                                where speciality != null
                                let groups = speciality ["groups"]
                                from gr in groups
                                    where gr["name"].ToString().Contains(speciality["short_name"].ToString() + "с")
                                || gr["name"].ToString().Contains(speciality["short_name"].ToString() + "м") 
                                || gr ["name"].ToString ().Contains ( dir ["full_name"].ToString () + "-" + GetCodeYear ( year ) )
                                select gr).ToList ();
            return specialityGroups;
        }

        public async void GetSchedule(string groupID, string timeFrom = null, string timeTo = null)
        {
            var result = await GetResult("P_API_EVENTS_GROUP_JSON?p_id_group=" + groupID + "&time_from=" + ToUnixTime ( DateTime.Now ) );
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
