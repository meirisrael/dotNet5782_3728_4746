using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DO
{
	/// <summary>
	/// this struct represant a customer and all of propertie
	/// </summary>
	public struct Customer
	{
		public int Id { set; get; }
		public string Name { get; set; }
		public string Phone { get; set; }
		public double Longitude { get; set; }
		public double Latitude { get; set; }

		public override string ToString()
		{
			return $"Customer:\n" +
				$" Id: {this.Id}\n" +
				$" Name: {this.Name}\n" +
				$" Phone: {this.Phone}\n" +
				$" Longitude: {Math.Abs((int)(this.Longitude))}°{Math.Abs((int)(((this.Longitude) - (int)(this.Longitude)) * 60))}'{Math.Abs(Math.Round(((((this.Longitude) - (int)(this.Longitude)) * 60) - (int)(((this.Longitude) - (int)(this.Longitude)) * 60)) * 60, 3))}''S\n" +
				$" Latitude: {Math.Abs((int)(this.Latitude))}°{Math.Abs((int)(((this.Latitude) - (int)(this.Latitude)) * 60))}'{Math.Abs(Math.Round(((((this.Latitude) - (int)(this.Latitude)) * 60) - (int)(((this.Latitude) - (int)(this.Latitude)) * 60)) * 60, 3))}''E"
				;
		}
	}
}