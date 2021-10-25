using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDAL
{
	namespace DO
	{
		public struct Parcel
		{
			public int Id { get; set; }
			public int SenderId { get; set; }
			public int TargetId { get; set; }
			public WeightCategories Weight { get; set; }
			public Priorities Priority { get; set; }
			public int DroneId { get; set; }
			public DateTime Requested { get; set; }
			public DateTime Scheduled { get; set; }
			public DateTime PickedUp { get; set; }
			public DateTime Delivered { get; set; }

			public string toString()
			{
				return $"Parcel: " +
					$"Id: {this.Id}," +
					$"Sender Id: {this.SenderId}," +
					$"Target Id: {this.TargetId}," +
					$"Weight: {this.Weight}," +
					$"Priority: {this.Priority}," +
					$"Drone Id: {this.DroneId}," +
					$"Requested: {this.Requested}," +
					$"Scheduled: {this.Scheduled}," +
					$"Picked Up: {this.PickedUp}," +
					$"Delivred: {this.Delivered}," +
					"\n";
			}
		}
	}
}