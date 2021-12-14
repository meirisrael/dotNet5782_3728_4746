using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
	public class DroneToList
	{
		public int Id { get; set; }
		public string Model { get; set; }
		public WeightCategories MaxWeight { get; set; }
		public double Battery { get; set; }
		public DroneStatuses Status { get; set; }
		public Location Loc { get; set; }
		public int? IdOfParcel { get; set; }
		public DateTime whenInCharge { get; set; }

		public DroneToList() : base() { Loc = new(); }
		public override string ToString()
		{
			string idParcel="Not in shipping";
			if (this.Status==DroneStatuses.Shipping)
				idParcel = this.IdOfParcel.ToString();
			return $"Drone\n" +
				$" Id: {this.Id}\n" +
				$" Model: {this.Model}\n" +
				$" MaxWeight: {this.MaxWeight }\n" +
				$" Drone Statut: {this.Status}\n" +
				$" Battery percent: {this.Battery}%\n" +
				$" Location: {this.Loc.ToString()}\n" +
				$" The Id of parcel that are in transit right now: {idParcel}"
				;
		}
	}
}