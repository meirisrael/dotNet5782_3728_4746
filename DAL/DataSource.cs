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
			static double useWhenFree;
			static double useWhenLightly;
			static double useWhenMedium;
			static double useWhenHeavily;
			double chargingRate;

			internal static int CounterDrones = 1000;
			internal static int CounterBaseStation = 2000;
			internal static int CounterCustomer = 3000;
			internal static int CounterParcel = 4000;
		}
		/// <summary>
		/// the func initialize all parameter of the program with a default solution for each parametre
		/// </summary>
		public static void Inutialize()
		{
			for (int i = 0; i < 2; i++)//for base station
			{
				baseStation.Add(new IDAL.DO.BaseStation() 
				{
					Id = Config.CounterBaseStation++,
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
					Id = Config.CounterDrones++,
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
					Id = Config.CounterCustomer++,
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
					Id = Config.CounterParcel++,
					SenderId = r.Next(2000, 3000),
					TargetId = Config.CounterCustomer--,
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
