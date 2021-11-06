using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBL
{
	namespace BO
	{
		class DroneCharge
		{
			public int DroneId { get; set; }
			public int StationId { get; set; }

			public string toString()
			{
				return $"Drone Charge:\n" +
					$"Drone Id:{this.DroneId}\n" +
					$"Station Id:{this.StationId}" +
					"\n";
			}
	}
}
