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

		}
	}
}
