using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBL
{
	namespace BO
	{
		public class Location
		{
			public double Longitude { get; set; }//אורך
			public double Latitude { get; set; }//רוחב

			public override string ToString()
			{
				return $"\n" +
					$"	Longitude: { Math.Abs((int)(this.Longitude))}°{ Math.Abs((int)(((this.Longitude) - (int)(this.Longitude)) * 60))}'{Math.Abs(Math.Round(((((this.Longitude) - (int)(this.Longitude)) * 60) - (int)(((this.Longitude) - (int)(this.Longitude)) * 60)) * 60, 3))}''S\n" +
					$"	Latitude: {Math.Abs((int)(this.Latitude))}°{Math.Abs((int)(((this.Latitude) - (int)(this.Latitude)) * 60))}'{Math.Abs(Math.Round(((((this.Latitude) - (int)(this.Latitude)) * 60) - (int)(((this.Latitude) - (int)(this.Latitude)) * 60)) * 60, 3))}''E"
					;
			}
		}
	}
}
