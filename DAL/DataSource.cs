using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IDAL;

namespace DalObject
{
	/// <summary>
	/// 
	/// </summary>
	internal class DataSource
	{
		internal static IDAL.DO.Drone[] drone = new IDAL.DO.Drone[10];
		internal static IDAL.DO.BaseStation[] baseStation = new IDAL.DO.BaseStation[5];
		internal static IDAL.DO.Customer[] customers = new IDAL.DO.Customer[100];
		internal static IDAL.DO.Parcel[] parcels = new IDAL.DO.Parcel[1000];
		internal static IDAL.DO.DroneCharge[] droneCharge = new IDAL.DO.DroneCharge[10];

		public static Random r = new Random();

		internal class Config
		{
			internal static int indexDrones = 0;
			internal static int indexBaseStation = 0;
			internal static int indexCustomer = 0;
			internal static int indexParcel = 0;
			internal static int indexDroneCharge = 0;

			internal static int CounterDrones = 1000;
			internal static int CounterBaseStation = 2000;
			internal static int CounterCustomer = 3000;
			internal static int CounterParcel = 4000;
		}

		public static void Inutialize()
		{
			for (int i = 0; i < 2; i++)//for base station
			{
				baseStation[i] = new IDAL.DO.BaseStation()
				{
					Id = Config.CounterBaseStation++,
					Name = i,
					ChargeSlots = r.Next() % 5,
					Lattitude = (r.NextDouble() * 180) - 90,
					Longitude = (r.NextDouble() * 180) - 90
				};
				Config.indexBaseStation++;
			}
			for (int i = 0; i < 5; i++)//for the drone
			{
				drone[i] = new IDAL.DO.Drone()
				{
					Id = Config.CounterDrones++,
					Model = "Fantome-"+i,
					MaxWeight = (IDAL.DO.WeightCategories)(r.Next(1, 4)),
					status = (IDAL.DO.DroneStatuses)(r.Next(1, 4)),
					Battery = 100
				};
				Config.indexDrones++;
				Config.CounterDrones++;
			}
			for (int i = 0; i < 10; i++)//for customer
			{
				customers[i] = new IDAL.DO.Customer()
				{
					Id = Config.CounterCustomer++,
					Name = ("a"+i),
					Phone = (3761+i).ToString(),
					Longitude = (r.NextDouble() * 180) - 90,
					Lattitude = (r.NextDouble() * 180) - 90
				};
				Config.indexCustomer++;
			}
			for (int i = 0; i < 10; i++)//for parcel
			{
				DateTime today = DateTime.Now;
				parcels[i] = new IDAL.DO.Parcel()
				{
					Id=Config.CounterParcel++,
					SenderId= r.Next(2000,2500),
					TargetId= Config.CounterCustomer--,
					Weight= (IDAL.DO.WeightCategories)(r.Next(1, 4)),
					Priority=(IDAL.DO.Priorities)(r.Next(1, 4)),
					DroneId=r.Next(1000,1005),
					Requested=today,
					Scheduled =today.AddDays(2),
					PickedUp = today.AddDays(5),
					Delivered = today.AddDays(7)
				};
				Config.indexParcel++;
			}
		}
	}
}
