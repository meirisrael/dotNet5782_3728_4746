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
			internal static double useWhenFree = 0;
			internal static double useWhenLightly = 0;
			internal static double useWhenMedium = 0;
			internal static double useWhenHeavily = 0;
			internal double chargingRate = 0; 
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
					Lattitude = (r.NextDouble() * 180) - 90,
					Longitude = (r.NextDouble() * 180) - 90
				});
			}
			for (int i = 0; i < 5; i++)//for the drone
			{
				drone.Add(new IDAL.DO.Drone()
				{
					Id = CounterDrones++,
					Model = "Fantome-" + i,
					MaxWeight = (IDAL.DO.WeightCategories)(r.Next(1, 4)),
					/*status = (IDAL.DO.DroneStatuses)(r.Next(1, 4)),
					Battery = 100*/
				});
			}
			for (int i = 0; i < 10; i++)//for customer
			{
				customers.Add(new IDAL.DO.Customer()
				{
					Id = CounterCustomer++,
					Name = ("a" + i),
					Phone = (3761 + i).ToString(),
					Longitude = (r.NextDouble() * 180) - 90,
					Lattitude = (r.NextDouble() * 180) - 90
				});
			}
			for (int i = 0; i < 10; i++)//for parcel
			{
				DateTime today = DateTime.Now;
				parcels.Add(new IDAL.DO.Parcel()
				{
					Id = CounterParcel++,
					SenderId = r.Next(2000, 3000),
					TargetId = CounterCustomer--,
					Weight = (IDAL.DO.WeightCategories)(r.Next(1, 4)),
					Priority = (IDAL.DO.Priorities)(r.Next(1, 4)),
					DroneId = r.Next(1000, 1005),
					Requested = today,
					Scheduled = today.AddDays(2),
					PickedUp = today.AddDays(5),
					Delivered = today.AddDays(7)
				});
			}
		}
	}
}
