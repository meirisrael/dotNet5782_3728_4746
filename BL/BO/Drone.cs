﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
	public class Drone
	{
		public int Id { get; set; }
		public string Model { get; set; }
		public WeightCategories MaxWeight { get; set; }
		public DroneStatuses Status { get; set; }
		public double Battery { get; set; }
		public Location Loc { get; set; }
		public ParcelInTransit? InTransit { get; set; }

		public Drone() : base() { Loc = new(); InTransit = new(); }
		public override string ToString()
		{
			string transit = "";
			if (InTransit.Id==0)
				transit = "none";
			else
				transit = this.InTransit.ToString();
			return $"Drone:\n" +
					 $" Id: {this.Id}\n" +
					 $" Model: {this.Model}\n" +
					 $" MaxWeight: {this.MaxWeight }\n" +
				     $" Drone Statut: {this.Status}\n" +
					 $" Battery percent: {this.Battery}%\n" +
					 $" Location: {this.Loc.ToString()}\n" +
					 $" Parcel in transit now: {transit}"
					 ;
		}
	}
}