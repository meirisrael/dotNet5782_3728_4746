﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalObject
{
	public class DalObject
	{
		public DalObject()
		{
			DataSource.Inutialize();
		}

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
			DataSource.Config.indexBaseStation++;
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
			DataSource.Config.indexDrones++;
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
			DataSource.Config.indexCustomer++;
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
			Console.WriteLine("Requested:(Exemple: Wed 30, 2015");
			DateTime.TryParse(Console.ReadLine(), out e);
			parcel.Requested = e;
			Console.WriteLine("Scheduled:(Exemple: Wed 30, 2015");
			DateTime.TryParse(Console.ReadLine(), out e);
			parcel.Scheduled = e;
			Console.WriteLine("PickedUp:(Exemple: Wed 30, 2015");
			DateTime.TryParse(Console.ReadLine(), out e);
			parcel.PickedUp = e;
			Console.WriteLine("Delivered:(Exemple: Wed 30, 2015");
			DateTime.TryParse(Console.ReadLine(), out e);
			parcel.Delivered = e;
			DataSource.parcels[DataSource.Config.indexCustomer] = parcel;
			DataSource.Config.indexCustomer++;
		}

		public static void AssignParcelToDrone()
        {
			int a,i = 0;
			Console.WriteLine("ParcelId:");
			int.TryParse(Console.ReadLine(), out a);
			while (DataSource.parcels[i].Id != a) 
				i++;
			Console.WriteLine("DroneId:");
			int.TryParse(Console.ReadLine(), out a);
			DataSource.parcels[i].DroneId = a;
		}

		public static void ParcelOnDrone()
		{
			int a, i = 0,j=0;
			Console.WriteLine("ParcelId:");
			int.TryParse(Console.ReadLine(), out a);
			while (DataSource.parcels[i].Id != a) // find the right parcel
				i++;					
			DataSource.parcels[i].PickedUp = DateTime.Now;
			Console.WriteLine("DroneId:");
			while (DataSource.drone[j].Id != a) // find the right drone
				j++;					
			DataSource.drone[j].status = IDAL.DO.DroneStatuses.Shipping ;// drone status updated
		}

		public static void ParcelDelivered()
        {
			int a, i = 0,j=0;
			Console.WriteLine("ParcelId:");
			int.TryParse(Console.ReadLine(), out a);
			while (DataSource.parcels[i].Id != a) 
				i++;
			DataSource.parcels[i].Delivered = DateTime.Now;
			a = DataSource.parcels[i].DroneId;
			while (DataSource.drone[j].Id != a) 
				j++;
			DataSource.drone[j].status = IDAL.DO.DroneStatuses.free;
		}

		public static void AssignDroneToBaseStation()
		{
			int a, i = 0, j = 0, k = 0;
			Console.WriteLine("Droneid:");
			int.TryParse(Console.ReadLine(), out a);
			while (DataSource.drone[i].Id != a) 
				i++;
			DataSource.drone[i].Battery = 0;								// drone battery updated
			DataSource.drone[i].status = IDAL.DO.DroneStatuses.Maintenance;// drone status updated
			Console.WriteLine("Choose a Base Station for the drone to charge by writing the Base station's Id:");
			DisplayListBaseStationsCanCharge();
			int.TryParse(Console.ReadLine(), out a);
			while (DataSource.baseStation[j].Id != a) 
				j++;
			DataSource.baseStation[j].ChargeSlots--;
			DataSource.droneCharge[DataSource.Config.indexDroneCharge].DroneId = DataSource.drone[i].Id;          // drone id saved in DroneCharge with the station id where he charges
			DataSource.droneCharge[DataSource.Config.indexDroneCharge].StationId = DataSource.baseStation[j].Id;
			DataSource.Config.indexDroneCharge++;
		}

		public static void DroneLeaveChargeStation()
        {
			int a, i = 0,j=0,k=0;
			Console.WriteLine("DroneId:");
			int.TryParse(Console.ReadLine(), out a);
			while (DataSource.drone[i].Id != a) 
				i++;
			DataSource.drone[i].status = IDAL.DO.DroneStatuses.free;
			DataSource.drone[i].Battery = 100;
			while (DataSource.droneCharge[j].DroneId != a) // find the right drone in ChargeStation
				j++;								
			while (DataSource.baseStation[k].Id != DataSource.droneCharge[j].StationId) // find the base station where the drone was charging
				k++;
			DataSource.baseStation[k].ChargeSlots++;                                        //charging slot now available
			for (int l = k; l < (DataSource.Config.indexDroneCharge-1); l++) 
				DataSource.droneCharge[l] = DataSource.droneCharge[l + 1]; // erase the charge slot that is now available
			DataSource.Config.indexDroneCharge--;
		}

		public static void DisplayBaseStation()
        {
			Console.WriteLine("Enter Id:");
			int a,i=0;
			int.TryParse(Console.ReadLine(), out a);
			while (DataSource.baseStation[i].Id != a && i < DataSource.Config.indexBaseStation) 
				i++;
			if (i == DataSource.Config.indexBaseStation) 
			{ 
				Console.WriteLine("No such Base station"); 
				return; 
			}
			Console.WriteLine(DataSource.baseStation[i].toString());
		}

		public static void DisplayDrone()
		{
			Console.WriteLine("Enter Id:");
			int a, i = 0;
			int.TryParse(Console.ReadLine(), out a);
			while (DataSource.drone[i].Id != a && i< DataSource.Config.indexDrones) 
				i++;
			if (i == DataSource.Config.indexDrones)
			{ 
				Console.WriteLine("No such drone"); 
				return; 
			}
			Console.WriteLine(DataSource.drone[i].toString());
		}

		public static void DisplayCustomer()
		{
			Console.WriteLine("Enter Id:");
			int a, i = 0;
			int.TryParse(Console.ReadLine(), out a);
			while (DataSource.customers[i].Id != a && i < DataSource.Config.indexCustomer) 
				i++;
			if (i == DataSource.Config.indexCustomer) 
			{ 
				Console.WriteLine("No such customer"); 
				return; 
			} 
			Console.WriteLine(DataSource.customers[i].toString());
		}

		public static void DisplayParcel()
		{
			Console.WriteLine("Enter Id:");
			int a, i = 0;
			int.TryParse(Console.ReadLine(), out a);
			while (DataSource.parcels[i].Id != a && i < DataSource.Config.indexParcel) 
				i++;
			if (i == DataSource.Config.indexParcel) 
			{ 
				Console.WriteLine("No such parcel");
				return; 
			}
			Console.WriteLine(DataSource.parcels[i].toString());
		}

		public static void DisplayListBaseStations()
		{
            for (int i = 0; i< DataSource.Config.indexBaseStation; i++)
            {
				Console.WriteLine(DataSource.baseStation[i].toString());
			}
		}

		public static void DisplayListDrones()
		{
			for (int i = 0; i < DataSource.Config.indexDrones; i++)
			{
				Console.WriteLine(DataSource.drone[i].toString());
			}
		}

		public static void DisplayListCustomers()
		{
			for (int i = 0; i < DataSource.Config.indexCustomer; i++)
			{
				Console.WriteLine(DataSource.customers[i].toString());
			}
		}

		public static void DisplayListParcels()
		{
			for (int i = 0; i < DataSource.Config.indexParcel; i++)
			{
				Console.WriteLine(DataSource.parcels[i].toString());
			}
		}

		public static void DisplayParcelsNotAssignedToDrone()
		{
			for (int i = 0; i < DataSource.Config.indexParcel; i++)
			{
				if(DataSource.parcels[i].DroneId ==0)
					Console.WriteLine(DataSource.parcels[i].toString());
			}
		}

		public static void DisplayListBaseStationsCanCharge()
		{
			for (int i = 0; i < DataSource.Config.indexBaseStation; i++)
			{
				if (DataSource.baseStation[i].ChargeSlots >0) 
					Console.WriteLine(DataSource.baseStation[i].toString());
			}
		}
	}
}
