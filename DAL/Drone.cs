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
		/// this struct represant a drone and all of properties
		/// </summary>
		public struct Drone
		{
			public int Id { get; set; }
			public string Model { get; set; }
			public WeightCategories MaxWeight { get; set; }
			//public DroneStatuses status { get; set; }
			//public double Battery { get; set; }

			public string toString()
			{
				return $"Drone:\n" +
					$"Id: {this.Id}\n" +
					$"Model: {this.Model}\n" +
					$"MaxWeight: {this.MaxWeight }\n";/* +
					$"status: {this.status}\n" +
					$"Battery: {this.Battery}" +
					"\n";*/
			}
		}
	}
}
