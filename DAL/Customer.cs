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
		/// this struct represant a customer and all of properties
		/// </summary>
		public struct Customer
		{
			public int Id { get; set; }
			public string Name { get; set; }
			public string Phone { get; set; }
			public double Longitude { get; set; }
			public double Lattitude { get; set; }

			public string toString()
			{
				return $"Customer:\n" +
					$"Id: {this.Id}\n" +
					$"Name: {this.Name}\n" +
					$"Phone: {this.Phone}\n" +
					$"Longitude: {Math.Abs((int)(this.Longitude))}°{Math.Abs((int)(((this.Longitude) - (int)(this.Longitude)) * 60))}'{Math.Abs(Math.Round(((((this.Longitude) - (int)(this.Longitude)) * 60) - (int)(((this.Longitude) - (int)(this.Longitude)) * 60)) * 60, 3))}''S\n" +
					$"Lattitude: {(int)(this.Lattitude)}°{(int)(((this.Lattitude) - (int)(this.Lattitude)) * 60)}'{Math.Round(((((this.Lattitude) - (int)(this.Lattitude)) * 60) - (int)(((this.Lattitude) - (int)(this.Lattitude)) * 60)) * 60, 3)}''E" +
					"\n";
			}
		}
	}
}
