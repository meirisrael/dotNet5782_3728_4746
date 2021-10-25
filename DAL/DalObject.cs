using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalObject
{
	public class DalObject
	{
		public DalObject() { DataSource.Inutialize(); }

		public static void AddBaseStation()
        {
			IDAL.DO.BaseStation station=new IDAL.DO.BaseStation();
			int a;
			double b;
			Console.WriteLine("Id:");
			int.TryParse(Console.ReadLine(),out a);
			station.Id = a;
			Console.WriteLine("Name:");
			int.TryParse(Console.ReadLine(), out a);
			station.Name = a;
			Console.WriteLine("ChargeSlots:");
			int.TryParse(Console.ReadLine(), out a);
			station.ChargeSlots = a;
			Console.WriteLine("Longitude:");
			double.TryParse(Console.ReadLine(), out b);
			station.Longitude = b;
			Console.WriteLine("Lattitude:");
			double.TryParse(Console.ReadLine(), out b);
			station.Lattitude = b;
			DataSource.baseStation[DataSource.Config.indexBaseStation] = station;
		}
		public static void AddDrone()
		{
			IDAL.DO.Drone drone = new IDAL.DO.Drone();
			int a;
			double b;
			IDAL.DO.WeightCategories c;
			IDAL.DO.DroneStatuses d;
			Console.WriteLine("Id:");
			int.TryParse(Console.ReadLine(), out a);
			drone.Id = a;
			Console.WriteLine("Model:");
			drone.Model = Console.ReadLine();
			Console.WriteLine("MaxWeight:");
			Enum.TryParse<IDAL.DO.WeightCategories>(Console.ReadLine(), out c);
			drone.MaxWeight = c;
			Console.WriteLine("Status:");
			Enum.TryParse< IDAL.DO.DroneStatuses>(Console.ReadLine(), out d);
			drone.status = d;
			Console.WriteLine("Battery:");
			double.TryParse(Console.ReadLine(), out b);
			drone.Battery = b;
			DataSource.drone[DataSource.Config.indexDrones] = drone;
		}
		public static void AddCustomer()
		{
			IDAL.DO.Customer customer = new IDAL.DO.Customer();
			int a;
			double b;
			Console.WriteLine("Id:");
			int.TryParse(Console.ReadLine(), out a);
			customer.Id = a;
			Console.WriteLine("Name:");
			customer.Name = Console.ReadLine();
			Console.WriteLine("Phone:");
			customer.Phone = Console.ReadLine();
			Console.WriteLine("Longitude:");
			double.TryParse(Console.ReadLine(), out b);
			customer.Longitude = b;
			Console.WriteLine("Lattitude:");
			double.TryParse(Console.ReadLine(), out b);
			customer.Lattitude = b;
			DataSource.customers[DataSource.Config.indexCustomer] = customer;
		}
		public static void AddParcel()
		{
			IDAL.DO.Parcel parcel = new IDAL.DO.Parcel();
			int a;
			double b;
			IDAL.DO.WeightCategories c;
			IDAL.DO.Priorities d;
			DateTime e;
			Console.WriteLine("Id:");
			int.TryParse(Console.ReadLine(), out a);
			parcel.Id = a;
			Console.WriteLine("TargetId:");
			int.TryParse(Console.ReadLine(), out a);
			parcel.TargetId = a;
			Console.WriteLine("SenderId:");
			int.TryParse(Console.ReadLine(), out a);
			parcel.SenderId = a;
			Console.WriteLine("Weigh:");
			Enum.TryParse<IDAL.DO.WeightCategories>(Console.ReadLine(), out c);
			parcel.Weight = c;
			Console.WriteLine("Priority:");
			Enum.TryParse<IDAL.DO.Priorities>(Console.ReadLine(), out d);
			parcel.Priority = d;
			Console.WriteLine("DroneId:");
			int.TryParse(Console.ReadLine(), out a);
			parcel.DroneId = a;
			Console.WriteLine("Requested:");
			DateTime.TryParse(Console.ReadLine(), out e);
			parcel.Requested = e;
			Console.WriteLine("Scheduled:");
			DateTime.TryParse(Console.ReadLine(), out e);
			parcel.Scheduled = e;
			Console.WriteLine("PickedUp:");
			DateTime.TryParse(Console.ReadLine(), out e);
			parcel.PickedUp = e;
			Console.WriteLine("Delivered:");
			DateTime.TryParse(Console.ReadLine(), out e);
			parcel.Delivered = e;
			DataSource.parcels[DataSource.Config.indexCustomer] = parcel;
		}
		public static void AssignParcelToDrone()
        {
			int a,i = 0;
			Console.WriteLine("ParcelId:");
			int.TryParse(Console.ReadLine(), out a);
			while (DataSource.parcels[i].Id != a) i++;
			Console.WriteLine("DroneId:");
			int.TryParse(Console.ReadLine(), out a);
			DataSource.parcels[i].DroneId = a;
		}
		public static void ParcelOnDrone()
		{
			int a, i = 0,j=0;
			Console.WriteLine("ParcelId:");
			int.TryParse(Console.ReadLine(), out a);
			while (DataSource.parcels[i].Id != a) i++;
			DataSource.parcels[i].PickedUp = DateTime.Now;
			Console.WriteLine("DroneId:");
			while (DataSource.drone[j].Id != a) j++;
			DataSource.drone[j].status = IDAL.DO.DroneStatuses.Shipping ;
		}
		public static void ParcelDelivered()
        {
			int a, i = 0,j=0;
			Console.WriteLine("ParcelId:");
			int.TryParse(Console.ReadLine(), out a);
			while (DataSource.parcels[i].Id != a) i++;
			DataSource.parcels[i].Delivered = DateTime.Now;
			a = DataSource.parcels[i].DroneId;
			while (DataSource.drone[j].Id != a) j++;
			DataSource.drone[j].status = IDAL.DO.DroneStatuses.free;
		}
		public static void DisplayBaseStation()
        {
			Console.WriteLine("Enter Id:");
			int a,i=0;
			int.TryParse(Console.ReadLine(), out a);
			while (DataSource.baseStation[i].Id != a) i++;
			IDAL.DO.BaseStation station = new IDAL.DO.BaseStation();
			station = DataSource.baseStation[i];
			Console.WriteLine(station.toString());
			//Console.WriteLine("Name: {0} ChargeSlots: {1} Longitude:{2} Lattitude:{3}", station.Name, station.ChargeSlots, station.Longitude, station.Lattitude);
		}
	}
}
