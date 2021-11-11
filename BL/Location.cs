using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBL
{
	namespace BO
	{
		class Location
		{
			public double Longitude { get; set; }//אורך
			public double Lattitude { get; set; }//רוחב

			public override string ToString()
			{
				return $"Lcation\n" +
					$" Longitude: { Math.Abs((int)(this.Longitude))}°{ Math.Abs((int)(((this.Longitude) - (int)(this.Longitude)) * 60))}'{Math.Abs(Math.Round(((((this.Longitude) - (int)(this.Longitude)) * 60) - (int)(((this.Longitude) - (int)(this.Longitude)) * 60)) * 60, 3))}''S\n" +
					$" Lattitude: {Math.Abs((int)(this.Lattitude))}°{Math.Abs((int)(((this.Lattitude) - (int)(this.Lattitude)) * 60))}'{Math.Abs(Math.Round(((((this.Lattitude) - (int)(this.Lattitude)) * 60) - (int)(((this.Lattitude) - (int)(this.Lattitude)) * 60)) * 60, 3))}''E"
	
			}
		}
	}
}
