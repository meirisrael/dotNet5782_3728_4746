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
			internal static double useWhenFree = 0.0001;//0.5% per kilometere
			internal static double useWhenLightly = 0.0003;//2% per kilometere
			internal static double useWhenMedium = 0.0004;//3% per kilometere
			internal static double useWhenHeavily =0.0007;//5% per kilometere
			internal double chargingRate = 40; //40% per hour
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
			for (int i = 0; i < 2; i++)//for base station
			{
				baseStation.Add(new IDAL.DO.BaseStation()
				{
					Id = CounterBaseStation++,
					Name = i,
					ChargeSlots = r.Next() % 5,
					Latitude = r.Next(-90, 91) * r.NextDouble(),
					Longitude = r.Next(-180, 181) * r.NextDouble()
				});
			}
			for (int i = 0; i < 5; i++)//for the drone
			{
				drone.Add(new IDAL.DO.Drone()
				{
					Id = CounterDrones,
					Model = "Fantome-" + i,
					MaxWeight = (IDAL.DO.WeightCategories)(r.Next(1, 4))
				});
				if (i < 4)
					CounterDrones++;
			}
			for (int i = 0; i < 10; i++)//for customer
			{
				customers.Add(new IDAL.DO.Customer()
				{
					Id = CounterCustomer,
					Name = ("a" + i),
					Phone = (3761 + i).ToString(),
					Latitude = r.Next(-90, 91) * r.NextDouble(),
					Longitude = r.Next(-180, 181) * r.NextDouble()
				});
				if (i < 9)
					CounterCustomer++;
			}
			for (int i = 1; i <= 10; i++)//for parcel
			{
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
					PickedUp = DateTime.MinValue,
					Delivered = DateTime.MinValue
				});
				if (i % 2 == 0)
					CounterDrones--;
			}
		}
	}
}
