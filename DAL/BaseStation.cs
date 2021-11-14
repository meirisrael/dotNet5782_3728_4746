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
		/// this struct represant a base station of drone and all of properties
		/// </summary>
		public struct BaseStation
		{
			public int Id { get; set; }
			public int Name { get; set; }
			public int ChargeSlots { get; set; }
			public double Longitude { get; set; }//אורך
			public double Latitude { get; set; }//רוחב
			
			public override string ToString()
			{
				return $"Base Station:\n" +
					$" Id: {this.Id}\n" +
					$" Name: { this.Name}\n" +
					$" Charge Slots: {this.ChargeSlots}\n" +
					$" Longitude: {Math.Abs((int)(this.Longitude))}°{Math.Abs((int)(((this.Longitude) - (int)(this.Longitude)) * 60))}'{Math.Abs(Math.Round(((((this.Longitude) - (int)(this.Longitude)) * 60) - (int)(((this.Longitude) - (int)(this.Longitude)) * 60)) * 60, 3))}''S\n" +
					$" Latitude: {Math.Abs((int)(this.Latitude))}°{Math.Abs((int)(((this.Latitude) - (int)(this.Latitude)) * 60))}'{Math.Abs(Math.Round(((((this.Latitude) - (int)(this.Latitude)) * 60) - (int)(((this.Latitude) - (int)(this.Latitude)) * 60)) * 60, 3))}''E"
					;
			}
		}
	}
}