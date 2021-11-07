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
			public int DroneId { get; set; }
			public WeightCategories Weight { get; set; }
			public Priorities Priority { get; set; }
			public DateTime Requested { get; set; }
			public DateTime Scheduled { get; set; }
			public DateTime PickedUp { get; set; }
			public DateTime Delivered { get; set; }

			public string toString()
			{
				return $"Parcel:\n " +
					$"Id: {this.Id}\n" +
					$"Sender Id: {this.SenderId}\n" +
					$"Target Id: {this.TargetId}\n" +
					$"Weight: {this.Weight}\n" +
					$"Priority: {this.Priority}\n" +
					$"Drone Id: {this.DroneId}\n" +
					$"Requested: {this.Requested}\n" +
					$"Scheduled: {this.Scheduled}\n" +
					$"Picked Up: {this.PickedUp}\n" +
					$"Delivred: {this.Delivered}\n" +
					"\n";
			}
		}
	}
}