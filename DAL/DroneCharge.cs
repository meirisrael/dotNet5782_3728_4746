using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDAL
{
	namespace DO
	{
		/// <summary>
		/// this struct represant a drone and where he is charging 
		/// </summary>
		public struct DroneCharge
		{
			public int DroneId { get; set; }
			public int StationId { get; set; }

			public override string ToString()
			{
				return $"Drone Charge:\n" +
					$"Drone Id:{this.DroneId}\n" +
					$"Station Id:{this.StationId}" +
					"\n";
			}
		}
	}
}
