using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBL
{
	namespace BO
	{
		class DroneInParcel
		{
			public int Id { get; set; }
			public double Battery { get; set; }
			public Location Loc { get; set; }

			public override string ToString()
			{
				return $"Drone In Parcel:\n" +
					$" Id: {this.Id}\n" +
					$" Battery percent: {this.Battery}%\n" +
					$" Location: {this.Loc.ToString()}"
			}
		}
	}

}
