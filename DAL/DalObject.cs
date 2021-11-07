using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalObject
{
	public class DalObject : IDal.IDal
	{
		public DalObject() 
		{ 
			DataSource.Initialize(); 
		}
		/// <summary>
		/// add a station in baseStation[] created in DataSource with details given by user
		/// </summary>
		public void AddBaseStation()
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
			DataSource.baseStation[DataSource.Config.CounterBaseStation] = station;
			DataSource.Config.CounterBaseStation++;
		}
		/// <summary>
		/// /// add a drone in drone[] created in DataSource with details given by user
		/// </summary>
		public void AddDrone()
		{
			if (DataSource.Config.CounterDrones < 10)
			{
				IDAL.DO.Drone drone = new IDAL.DO.Drone();
				int a;
				double b;
				IDAL.DO.WeightCategories c;
				//IDAL.DO.DroneStatuses d;
				Console.WriteLine("Id:");
				int.TryParse(Console.ReadLine(), out a);
				drone.Id = a;
				Console.WriteLine("Model:");
				drone.Model = Console.ReadLine();
				Console.WriteLine("MaxWeight:");
				Enum.TryParse<IDAL.DO.WeightCategories>(Console.ReadLine(), out c);
				drone.MaxWeight = c;
				Console.WriteLine("Status:");
				Enum.TryParse<IDAL.DO.DroneStatuses>(Console.ReadLine(), out d);
				//drone.status = d;
				Console.WriteLine("Battery:");
				double.TryParse(Console.ReadLine(), out b);
				//drone.Battery = b;
				DataSource.drone[DataSource.Config.CounterDrones] = drone;
				DataSource.Config.CounterDrones++;
			}
			else Console.WriteLine("no more space for a new drone");
		}
		/// <summary>
		/// add a customer in customer[] created in DataSource with details given by user
		/// </summary>
		public void AddCustomer()
		{
			if (DataSource.Config.CounterCustomer < 100)
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
				DataSource.customers[DataSource.Config.CounterCustomer] = customer;
				DataSource.Config.CounterCustomer++;
			}
			else Console.WriteLine("no more space for a new drone");
		}
		/// <summary>
		/// add a parcel in parcels[] created in DataSource with details given by user
		/// </summary>
		public void AddParcel()
		{
			if (DataSource.Config.CounterParcel < 1000)
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
				DataSource.parcels[DataSource.Config.CounterParcel] = parcel;
				DataSource.Config.CounterParcel++;
			}
			else Console.WriteLine("no more space for a new drone");
		}
		/// <summary>
		/// add a drone Id in droneId field from a parcel given by the user
		/// </summary>
		public void AssignParcelToDrone()
        {
			int a,i = 0;
			Console.WriteLine("ParcelId:");
			int.TryParse(Console.ReadLine(), out a);
			while (DataSource.parcels[i].Id != a) 
				i++;
			Console.WriteLine("DroneId:");
			int.TryParse(Console.ReadLine(), out a);
			IDAL.DO.Parcel parcel = DataSource.parcels[i];
			parcel.DroneId = a;
			DataSource.parcels[i]= parcel;
			//DataSource.parcels[i].DroneId = a;
			//DataSource.parcels.Insert(i,)
		}
		/// <summary>
		/// change the PickedUp date of a given parcel by the present time and change the status of the related drone to "shipping"
		/// </summary>
		public void ParcelOnDrone()
		{
			int a, i = 0,j=0;
			Console.WriteLine("ParcelId:");
			int.TryParse(Console.ReadLine(), out a);
			while (DataSource.parcels[i].Id != a) // find the right parcel
				i++;					
			//DataSource.parcels[i].PickedUp = DateTime.Now;
			IDAL.DO.Parcel parcel = DataSource.parcels[i];
			parcel.PickedUp = DateTime.Now;
			DataSource.parcels[i] = parcel;
			//while (DataSource.drone[j].Id != DataSource.parcels[i].DroneId) j++;					// find the right drone
			//DataSource.drone[j].status = IDAL.DO.DroneStatuses.Shipping ;// drone status updated
		}
		/// <summary>
		/// change the "Delivered" date of a given parcel by the present time and change the status of the related drone to "free"
		/// </summary>
		public void ParcelDelivered()
        {
			int a, i = 0,j=0;
			Console.WriteLine("ParcelId:");
			int.TryParse(Console.ReadLine(), out a);
			while (DataSource.parcels[i].Id != a) 
				i++;

			//DataSource.parcels[i].Delivered = DateTime.Now;
			IDAL.DO.Parcel parcel = DataSource.parcels[i];
			parcel.Delivered = DateTime.Now;
			DataSource.parcels[i] = parcel;
			//a = DataSource.parcels[i].DroneId;
			//while (DataSource.drone[j].Id != a) 
			//	j++;
			//DataSource.drone[j].status = IDAL.DO.DroneStatuses.free;
		}/// <summary>
		/// change the status of a given drone to "maintenance" and give the user to choose a base station for the drone to charge (details saved in dronecharge[])
		/// </summary>
		public void AssignDroneToBaseStation()
		{
			int a, i = 0, j = 0, k = 0;
			Console.WriteLine("Droneid:");
			int.TryParse(Console.ReadLine(), out a);
			//while (DataSource.drone[i].Id != a) 
			//	i++;
			//DataSource.drone[i].Battery = 0;								// drone battery updated
			//DataSource.drone[i].status = IDAL.DO.DroneStatuses.Maintenance;// drone status updated
			Console.WriteLine("Choose a Base Station for the drone to charge by writing the Base station's Id:");
			DisplayListBaseStationsCanCharge();
			int.TryParse(Console.ReadLine(), out a);
			while (DataSource.baseStation[j].Id != a) 
				j++;
			//DataSource.baseStation[j].ChargeSlots--;
			IDAL.DO.BaseStation baseStation = DataSource.baseStation[j];
			baseStation.ChargeSlots--;
			DataSource.baseStation[j] = baseStation;
			DataSource.droneCharge[DataSource.Config.indexDroneCharge].DroneId = DataSource.drone[i].Id;          // drone id saved in DroneCharge with the station id where he charges
			DataSource.droneCharge[DataSource.Config.indexDroneCharge].StationId = DataSource.baseStation[j].Id;
			DataSource.Config.indexDroneCharge++;
		}
		/// <summary>
		/// change the status of a given drone to "free" and update the freed charge slot in the related basestation
		/// </summary>
		public void DroneLeaveChargeStation()
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
			//DataSource.baseStation[k].ChargeSlots++;
			IDAL.DO.BaseStation baseStation = DataSource.baseStation[j];
			baseStation.ChargeSlots++;
			DataSource.baseStation[j] = baseStation;//charging slot now available
			for (int l = k; l < (DataSource.Config.indexDroneCharge-1); l++) 
				DataSource.droneCharge[l] = DataSource.droneCharge[l + 1]; // erase the charge slot that is now available
			DataSource.Config.indexDroneCharge--;
		}
		/// <summary>
		/// for a given base station Id, display it details
		/// </summary>
		public void DisplayBaseStation()
        {
			Console.WriteLine("Enter Id:");
			int a,i=0;
			int.TryParse(Console.ReadLine(), out a);
			while (DataSource.baseStation[i].Id != a && i < DataSource.Config.CounterBaseStation) 
				i++;
			if (i == DataSource.Config.CounterBaseStation) 
			{ 
				Console.WriteLine("No such Base station"); 
				return; 
			}
			Console.WriteLine(DataSource.baseStation[i].toString());
		}
		/// <summary>
		/// for a given drone Id, display it details
		/// </summary>
		public void DisplayDrone()
		{
			Console.WriteLine("Enter Id:");
			int a, i = 0;
			int.TryParse(Console.ReadLine(), out a);
			while (DataSource.drone[i].Id != a && i< DataSource.Config.CounterDrones) 
				i++;
			if (i == DataSource.Config.CounterDrones)
			{ 
				Console.WriteLine("No such drone"); 
				return; 
			}
			Console.WriteLine(DataSource.drone[i].toString());
		}
		/// <summary>
		/// for a given customer Id, display it details
		/// </summary>
		public void DisplayCustomer()
		{
			Console.WriteLine("Enter Id:");
			int a, i = 0;
			int.TryParse(Console.ReadLine(), out a);
			while (DataSource.customers[i].Id != a && i < DataSource.Config.CounterCustomer) 
				i++;
			if (i == DataSource.Config.CounterCustomer) 
			{ 
				Console.WriteLine("No such customer"); 
				return; 
			} 
			Console.WriteLine(DataSource.customers[i].toString());
		}
		/// <summary>
		/// for a given parcel Id, display it details
		/// </summary>
		public void DisplayParcel()
		{
			Console.WriteLine("Enter Id:");
			int a, i = 0;
			int.TryParse(Console.ReadLine(), out a);
			while (DataSource.parcels[i].Id != a && i < DataSource.Config.CounterParcel) 
				i++;
			if (i == DataSource.Config.CounterParcel) 
			{ 
				Console.WriteLine("No such parcel");
				return; 
			}
			Console.WriteLine(DataSource.parcels[i].toString());
		}
		/// <summary>
		/// display the details of all base stations
		/// </summary>
		public void DisplayListBaseStations()
		{
            for (int i = 0; i< DataSource.Config.CounterBaseStation; i++)
            {
				Console.WriteLine(DataSource.baseStation[i].toString());
			}
		}
		/// <summary>
		/// display the details of all drones
		/// </summary>
		public void DisplayListDrones()
		{
			for (int i = 0; i < DataSource.Config.CounterDrones; i++)
			{
				Console.WriteLine(DataSource.drone[i].toString());
			}
		}
		/// <summary>
		/// display the details of all customers
		/// </summary>
		public void DisplayListCustomers()
		{
			for (int i = 0; i < DataSource.Config.CounterCustomer; i++)
			{
				Console.WriteLine(DataSource.customers[i].toString());
			}
		}
		/// <summary>
		/// display the details of all parcels
		/// </summary>
		public void DisplayListParcels()
		{
			for (int i = 0; i < DataSource.Config.CounterParcel; i++)
			{
				Console.WriteLine(DataSource.parcels[i].toString());
			}
		}
		/// <summary>
		/// display the details of all parcels not assigned to a drone yet
		/// </summary>
		public void DisplayParcelsNotAssignedToDrone()
		{
			for (int i = 0; i < DataSource.Config.CounterParcel; i++)
			{
				if(DataSource.parcels[i].DroneId ==0)
					Console.WriteLine(DataSource.parcels[i].toString());
			}
		}
		/// <summary>
		/// display the details of all base stations with available(s) charge slots
		/// </summary>
		public void DisplayListBaseStationsCanCharge()
		{
			for (int i = 0; i < DataSource.Config.CounterBaseStation; i++)
			{
				if (DataSource.baseStation[i].ChargeSlots >0) 
					Console.WriteLine(DataSource.baseStation[i].toString());
			}
		}
		public double[] GetChargingRate()
		{
			DataSource.Config c = new DataSource.Config();
			double[] arr = new double[5];
			arr[0] = DataSource.Config.useWhenFree;
			arr[1] = DataSource.Config.useWhenHeavily;
			arr[2] = DataSource.Config.useWhenLightly;
			arr[3] = DataSource.Config.useWhenMedium;
			arr[4] = c.chargingRate;
			return arr;
		}
	}
}
