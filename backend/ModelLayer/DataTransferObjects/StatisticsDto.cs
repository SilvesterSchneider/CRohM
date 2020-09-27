using System;
using System.Collections.Generic;
using System.Text;

namespace ModelLayer.DataTransferObjects
{
    public class StatisticsDto
    {
        public static string SERIES_INVITED_CONTACTS = "Eingeladen";
        public static string SERIES_PARTICIPATED_CONTACS = "Teilgenommen";

        public enum STATISTICS_VALUES
        {
            INVITED_AND_PARTICIPATED_EVENT_PERSONS = 0
        }

        public class VerticalGroupedBarDto
        {
            public string Name { get; set; }
            public List<VerticalGroupedBarDataSet> Series { get; set; }
        }

        public class VerticalGroupedBarDataSet
        {
            public string Name { get; set; }
            public int Value { get; set; }
        }
    }
}
