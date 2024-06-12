﻿namespace EventPlanner.Models
{
	public class EventType
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public ICollection<Event> Events { get; set; }
	}
}
