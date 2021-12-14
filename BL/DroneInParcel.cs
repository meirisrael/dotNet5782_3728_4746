using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
	public class DroneInParcel
	{
		public int Id { get; set; }
		public double Battery { get; set; }
		public Location Loc { get; set; }

		public DroneInParcel() : base(){ Loc = new(); }
		public override string ToString()
		{
			return $"\n	Drone In Parcel:\n" +
				$"	Id: {this.Id}\n" +
				$"	Battery percent: {this.Battery}%\n" +
				$"	Location: {this.Loc.ToString()}"
				;
		}
	}
}