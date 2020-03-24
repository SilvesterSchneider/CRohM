using System;
using System.Collections.Generic;
using System.Text;

namespace ModelLayer.DataTransferObjects
{
    public class EducationalOpportunityDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public float Ects { get; set; }
    }
}