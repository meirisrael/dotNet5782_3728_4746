using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDAL
{
	namespace DO
	{
		public struct BaseStation
		{
			public int Id { get; set; }
			public int Name { get; set; }
			public int ChargeSlots { get; set; }
			public double Longitude { get; set; }
			public double Lattitude { get; set; }
			
			public string toString()
			{
				return $"Base Station:" +
					$"Id: {this.Id}," +
					$"Name: { this.Name}," +
					$"Charge Slotes:{this.ChargeSlots}" +
					$"Longitude: {this.Longitude} " +
					$"Lattitude: {this.Lattitude}\n";
			}
		}
	}
}
//{(int)(this.Longitude)}°{(int)((this.Longitude-(int)(this.