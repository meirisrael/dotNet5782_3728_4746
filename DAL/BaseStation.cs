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
			public double Longitude { get; set; }//אורך
			public double Lattitude { get; set; }//רוחב
			
			public string toString()
			{
				return $"Base Station:" +
					$"Id: {this.Id}," +
					$"Name: { this.Name}," +
					$"Charge Slots: {this.ChargeSlots}," +
					$"Longitude: {(int)(this.Longitude)}°{(int)(this.Longitude-(int)(this.Longitude))*60}' , ,'' S"+
					$"Lattitude: {this.Lattitude}"+
					"\n";
			}
		}
	}
}