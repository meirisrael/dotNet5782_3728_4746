using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBL
{
	namespace BO
	{
		public class Parcel
		{
			public int Id { get; set; }
			public CustomerInParcel Sender { get; set; }
			public CustomerInParcel Target { get; set; }
			public WeightCategories Weight { get; set; }
			public Priorities Priority { get; set; }
			public DroneInParcel Drone { get; set; }
			public DateTime Requested { get; set; }
			public DateTime Scheduled { get; set; }
			public DateTime PickedUp { get; set; }
			public DateTime Delivered { get; set; }

			public override string ToString()
			{
				return $"Parcel:\n" +
					$" Id: {this.Id}\n" +
					$" Sender: {this.Sender.ToString()}\n" +
					$" Target: {this.Target.ToString()}\n" +
					$" Weight: {this.Weight}\n" +
					$" Priority: {this.Priority}\n" +
					$" Drone: {this.Drone.ToString()}\n" +
					$" Requested: {this.Requested}\n" +
					$" Scheduled: {this.Scheduled}\n" +
					$" Picked Up: {this.PickedUp}\n" +
					$" Delivred: {this.Delivered}";
			}
		}
	}
}