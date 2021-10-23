using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IDAL;

namespace DalObject
{
	internal class DataSource
	{
		internal static IDAL.DO.Drone[] drone = new IDAL.DO.Drone[10];
		internal static IDAL.DO.BaseStation[] baseStation = new IDAL.DO.BaseStation[5];
		internal static IDAL.DO.Customer[] customers = new IDAL.DO.Customer[100];
		internal static IDAL.DO.Parcel[] parcels = new IDAL.DO.Parcel[1000];

		public static Random r = new Random();

		internal class Config
		{
			internal static int indexDrones = 0;
			internal static int indexBaseStation = 0;
			internal static int indexCustomer = 0;
			internal static int indexParcel = 0;
		}

		public static void Inutialize()
		{
			for (int i = 0; i < 2; i++)//for base station
			{
				baseStation[i] = new IDAL.DO.BaseStation()
				{
					Id = i,
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
					Id = i,
					Model = "x",
					MaxWeight = r.Next(1, 4),
					status = r.Next(1, 4),
					Battery = 100
				};
				Config.indexDrones++;
			}
			for (int i = 0; i < 10; i++)//for customer
			{
				customers[i] = new IDAL.DO.Customer()
				{
					Id = i,
					Name = "x",
					Phone = "x",
					Longitude = (i + 1) * 10,
					Lattitude = (i + 1) * 10
				};
				Config.indexCustomer++;
			}
			for (int i = 0; i < 10; i++)//for parcel
			{
				parcels[i] = new IDAL.DO.Parcel()
				{
					Id=i,
					SenderId=i,
					TargetId=i,
					Weight= r.Next(1, 4),
					Priority= r.Next(1, 4),
					DroneId=i%5,
					Requested=DateTime.FromFileTime(),
					Scheduled = DateTime.FromFileTime(),
					PickedUp = DateTime.FromFileTime(),
					Delivered = DateTime.FromFileTime(),

				};
				Config.indexParcel++;
			}
		}
	}
}
