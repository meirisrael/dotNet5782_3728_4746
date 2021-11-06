using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBL
{
	namespace BO
	{
		class Drone
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
					$"MaxWeight: {this.MaxWeight }" +
					$"\n";
			}
	}
}
