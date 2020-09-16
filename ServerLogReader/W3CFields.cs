using System;

namespace UrlRanking.ServerLogReader
{
    public class W3CFields
    {
        public W3CFields(DateTime _dateTime, string _cs_uri_stem)
        {
            date = _dateTime;
            time = _dateTime.TimeOfDay;
            cs_uri_stem = _cs_uri_stem;
        }
        public DateTime date { get; }
        public TimeSpan time { get; }
        public string cs_uri_stem { get; }
    }
}