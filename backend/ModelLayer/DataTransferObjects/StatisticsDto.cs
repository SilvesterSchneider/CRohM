using System;
using System.Collections.Generic;
using System.Text;

namespace ModelLayer.DataTransferObjects
{
    public class StatisticsDto
    {
        public static string SERIES_INVITED_CONTACTS = "Eingeladen";
        public static string SERIES_PARTICIPATED_CONTACS = "Teilgenommen";
        public static string SERIES_AGREED_CONTACS = "Zugesagt";
        public static string SERIES_CREATED_CONTACTS = "Kontakte";
        public static string SERIES_CREATED_ORGANIZATIONS = "Organisationen";
        public static string SERIES_CREATED_EVENTS = "Veranstaltungen";

        public enum STATISTICS_VALUES
        {
            ALL_CREATED_OBJECTS = 0,
            INVITED_AND_PARTICIPATED_EVENT_PERSONS = 1,
            ALL_TAGS = 2
        }

        public class VerticalGroupedBarDto
        {
            public string Name { get; set; }
            public List<VerticalGroupedBarDataSet> Series { get; set; } = new List<VerticalGroupedBarDataSet>();
        }

        public class VerticalGroupedBarDataSet
        {
            public string Name { get; set; }
            public int Value { get; set; }
        }
    }
}
