using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IDAL;

namespace DalObject
{
	/// <summary>
	/// data source of the program
	/// </summary>
	internal class DataSource
	{
		internal static List<IDAL.DO.Drone> drone = new List<IDAL.DO.Drone>();
		internal static List<IDAL.DO.BaseStation> baseStation = new List<IDAL.DO.BaseStation>();
		internal static List<IDAL.DO.Customer> customers = new List<IDAL.DO.Customer>();
		internal static List<IDAL.DO.Parcel> parcels = new List<IDAL.DO.Parcel>();
		internal static List<IDAL.DO.DroneCharge> droneCharge = new List<IDAL.DO.DroneCharge>();

		public static Random r = new Random();

		internal class Config
		{
			internal static double useWhenFree = 0.0001;//0.0001% per kilometere
			internal static double useWhenLightly = 0.0003;//0.0003% per kilometere
			internal static double useWhenMedium = 0.0004;//0.0004% per kilometere
			internal static double useWhenHeavily =0.0009;//0.0009% per kilometere
			internal double chargingRate = 35; //35% per hour
		}
		/// <summary>
		/// the func initialize all parameter of the program with a default solution for each parametre
		/// </summary>
		public static void Initialize()
		{
			int CounterBaseStation = 2000;
			int CounterDrones = 1000;
			int CounterCustomer = 3000;
			int CounterParcel = 4000;
			string[] name_ = new string[] { "Meir", "Lior", "Hilel", "Mendel", "Chimon", "David", "Loki", "Rephael", "levi", "Nathan" };
			for (int i = 0; i < 2; i++)//for base station
			{
				baseStation.Add(new IDAL.DO.BaseStation()
				{
					Id = CounterBaseStation++,
					Name = i,
					ChargeSlots = r.Next(1,6),
					Latitude = r.Next(-90, 91) * r.NextDouble(),    //(r.Next(-90, 91) * r.NextDouble())%1 +34.5,  shipping only israel
					Longitude = r.Next(-180, 181) * r.NextDouble()  //(r.Next(-180, 181) * r.NextDouble())%3.7 +29.5 shipping only israel
				});
			}
			for (int i = 0; i < 10; i++)//for the drone
			{
				drone.Add(new IDAL.DO.Drone()
				{
					Id = CounterDrones,
					Model = "Fantome-" + i,
					MaxWeight = (IDAL.DO.WeightCategories)3
				});
				if (i < 9)
					CounterDrones++;
			}
			for (int i = 0; i < 10; i++)//for customer
			{
				customers.Add(new IDAL.DO.Customer()
				{
					Id = CounterCustomer,
					Name = name_[i],
					Phone = (3761 + i).ToString(),
					Latitude = r.Next(-90, 91) * r.NextDouble(), //(r.Next(-90, 91) * r.NextDouble())%1 +34.5,  shipping only israel
					Longitude = r.Next(-180, 181) * r.NextDouble() //(r.Next(-180, 181) * r.NextDouble())%3.7 +29.5 shipping only israel
				});
				if (i < 9)
					CounterCustomer++;
			}
			for (int i = 1; i <= 10; i++)//for parcel
			{
				DateTime? x,z;
				if (i % 3 == 0) { x = DateTime.Now.AddHours(2); z =x.Value.AddHours(1) ; }
				else { x = null; z = null; }
				parcels.Add(new IDAL.DO.Parcel()
				{
					Id = CounterParcel++,
					SenderId = i+3000-1,
					TargetId = CounterCustomer--,
					Weight = (IDAL.DO.WeightCategories)(r.Next(1, 4)),
					Priority = (IDAL.DO.Priorities)(r.Next(1, 4)),
					DroneId = CounterDrones,
					Requested = DateTime.Now,
					Scheduled = DateTime.Now.AddSeconds(5),
					PickedUp = x,
					Delivered = z
				});
					CounterDrones--;
				
			}
		}
	}
}
