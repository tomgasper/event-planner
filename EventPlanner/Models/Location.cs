﻿using System.ComponentModel.DataAnnotations;

namespace EventPlanner.Models
{
    public class Location
    {
        public int Id { get; set; }

        [DataType(DataType.PostalCode)]
        public string PostalCode { get; set; }
        public string BuildingNumber { get; set; }

        public virtual Street Street { get; set; }

        public virtual ICollection<Event> Events { get;}
    }
}