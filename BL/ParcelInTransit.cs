using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
	public class ParcelInTransit
	{
		public int Id { get; set; }
		public bool Status { get; set; }
		public Priorities Priority { get; set; }
		public WeightCategories Weight { get; set; }
		public CustomerInParcel Sender { get; set; }
		public CustomerInParcel Target { get; set; }
		public Location LocPickedUp { get; set; }
		public Location LocDelivered { get; set; }
		public double DistanceDelivery { get; set; }

		public ParcelInTransit() : base() { Sender = new(); Target = new(); LocPickedUp = new(); LocDelivered = new(); }
		public override string ToString()
		{
			string status = "";
			if (this.Status == true)
				status = "on the way";
			else
				status = "waiting for collection";
			return $"\n	Parcel In Transit -\n" +
				$"		Id: {this.Id}\n" +
				$"		Parcel Statut: {status}\n" +
				$"		Weight: {this.Weight}\n" +
				$"		Priority: {this.Priority}\n" +
				$"		Sender:		{this.Sender.ToString()}\n" +
				$"		Target:		{this.Target.ToString()}\n" +
				$"		Location of PickedUp:	{this.LocPickedUp.ToString()}\n" +
				$"		Location of Delivered:	{this.LocDelivered.ToString()}\n" +
				$"		The distance of delivery: {Math.Round(this.DistanceDelivery,3)} KM"
				;
		}
	}
}