using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBL
{
	namespace BO
	{
		class Customer
		{
			public int Id { set; get; }
			public string Name { get; set; }
			public string Phone { get; set; }
			public double Longitude { get; set; }
			public double Lattitude { get; set; }

			public string ToString()
			{
				return $"Customer:\n" +
					$"Id: {this.Id}\n" +
					$"Name: {this.Name}\n" +
					$"Phone: {this.Phone}\n" +
					$"Longitude: {Math.Abs((int)(this.Longitude))}°{Math.Abs((int)(((this.Longitude) - (int)(this.Longitude)) * 60))}'{Math.Abs(Math.Round(((((this.Longitude) - (int)(this.Longitude)) * 60) - (int)(((this.Longitude) - (int)(this.Longitude)) * 60)) * 60, 3))}''S\n" +
					$"Lattitude: {Math.Abs((int)(this.Lattitude))}°{Math.Abs((int)(((this.Lattitude) - (int)(this.Lattitude)) * 60))}'{Math.Abs(Math.Round(((((this.Lattitude) - (int)(this.Lattitude)) * 60) - (int)(((this.Lattitude) - (int)(this.Lattitude)) * 60)) * 60, 3))}''E" +
					"\n";
			}
		}
	}