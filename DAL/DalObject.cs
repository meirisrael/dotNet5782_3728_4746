using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalObject
{
	public class DalObject : IDAL.IDal
	{
		public DalObject() 
		{ DataSource.Initialize(); }

		/// <summary>
		/// add a station in baseStation[] created in DataSource with details given by user
		/// </summary>
		public void AddBaseStation(int id,int name,int chargeSlots,double longe,double lati)
        {
			foreach (IDAL.DO.BaseStation item in DataSource.baseStation)
			{if (item.Id == id) throw new IDAL.DO.InvalidIdBase();	}
			DataSource.baseStation.Add(new IDAL.DO.BaseStation()
			{
				Id = id,
				Name = name,
				ChargeSlots = chargeSlots,
				Lattitude = lati,
				Longitude = longe
			}) ;
		}
		/// <summary>
		/// /// add a drone in drone[] created in DataSource with details given by user
		/// </summary>
		public void AddDrone(int id,string model,IDAL.DO.WeightCategories weight)
		{
			foreach (IDAL.DO.Drone item in DataSource.drone)
			{ if (item.Id == id) throw new IDAL.DO.InvalidIdDrone(); }
			DataSource.drone.Add(new IDAL.DO.Drone()
			{
				Id = id,
				Model = model,
				MaxWeight = weight
			});
		}
		/// <summary>
		/// add a customer in customer[] created in DataSource with details given by user
		/// </summary>
		public void AddCustomer(int id,string name,string phone,double longi,double lati)
		{
			foreach (IDAL.DO.Customer item in DataSource.customers)
			{ if (item.Id == id) throw new IDAL.DO.InvalidIdCustomer(); }
			DataSource.customers.Add(new IDAL.DO.Customer()
			{
				Id = id,
				Name = name,
				Phone = phone,
				Longitude = longi,
				Lattitude = lati
			});
		}
		/// <summary>
		/// add a parcel in parcels[] created in DataSource with details given by user
		/// </summary>
		public void AddParcel(int id,int senderId,int targetId,IDAL.DO.WeightCategories weight,IDAL.DO.Priorities priorities,int droneId,DateTime requested, 
			DateTime scheduled, DateTime pickedUp, DateTime delivered)
		{
			foreach (IDAL.DO.Parcel item in DataSource.parcels)
			{
				if (item.Id == id) throw new IDAL.DO.InvalidIdParcel();
				if (item.SenderId== senderId) throw new IDAL.DO.InvalidIdSender();
				if (item.TargetId== targetId) throw new IDAL.DO.InvalidIdTarget();
				if (item.DroneId== droneId) throw new IDAL.DO.InvalidIdDroneExist();
			}
			DataSource.parcels.Add(new IDAL.DO.Parcel()
			{
				Id =id,
				SenderId = senderId,
				TargetId = targetId,
				Weight = weight,
				Priority = priorities,
				DroneId = droneId,
				Requested = requested,
				Scheduled = scheduled,
				PickedUp = pickedUp,
				Delivered = delivered
			});
		}
		/// <summary>
		/// add a drone Id in droneId field from a parcel given by the user
		/// </summary>
		public void AssignParcelToDrone(int parcelId,int droneId)
        {
			foreach (IDAL.DO.Parcel item in DataSource.parcels)
			{
				if (item.Id == parcelId) throw new IDAL.DO.InvalidIdParcelExist();
				if (item.DroneId == droneId) throw new IDAL.DO.InvalidIdDroneExist();
			}
			for (int i = 0; i < DataSource.parcels.Count; i++)
			{
				if (DataSource.parcels[i].Id == parcelId)
				{
					IDAL.DO.Parcel x = DataSource.parcels[i];
					x.DroneId = droneId;
					DataSource.parcels[i] = x;
					break;
				}
			}
		}
		/// <summary>
		/// change the PickedUp date of a given parcel by the present time and change the status of the related drone to "shipping"
		/// </summary>
		public void ParcelOnDrone(int parcelId)
		{
			foreach (IDAL.DO.Parcel item in DataSource.parcels)
			{if (item.Id == parcelId) throw new IDAL.DO.InvalidIdParcelExist();		}
			for (int i = 0; i < DataSource.parcels.Count; i++)
			{
				if (DataSource.parcels[i].Id == parcelId)
				{
					IDAL.DO.Parcel x = DataSource.parcels[i];
					x.PickedUp=DateTime.Now;
					DataSource.parcels[i] = x;
					break;
				}
			}
		}
		/// <summary>
		/// change the "Delivered" date of a given parcel by the present time and change the status of the related drone to "free"
		/// </summary>
		public void ParcelDelivered(int parcelId)
		{
			foreach (IDAL.DO.Parcel item in DataSource.parcels)
			{ if (item.Id == parcelId) throw new IDAL.DO.InvalidIdParcelExist(); }
			for (int i = 0; i < DataSource.parcels.Count; i++)
			{
				if (DataSource.parcels[i].Id == parcelId)
				{
					IDAL.DO.Parcel x = DataSource.parcels[i];
					x.Delivered = DateTime.Now;
					DataSource.parcels[i] = x;
					break;
				}
			}
		}
		/// <summary>
		/// change the status of a given drone to "maintenance" and give the user to choose a base station for the drone to charge (details saved in dronecharge[])
		/// </summary>
		public void AssignDroneToBaseStation(int droneId,int baseId)
		{
			foreach (IDAL.DO.Drone item in DataSource.drone)
			{ if (item.Id == droneId) throw new IDAL.DO.InvalidIdDroneExist(); }
			foreach (IDAL.DO.BaseStation item in DataSource.baseStation)
			{ if (item.Id == baseId) throw new IDAL.DO.InvalidIdBaseExist(); }
			for (int i = 0; i < DataSource.baseStation.Count; i++)
			{
				if (DataSource.baseStation[i].Id == baseId)
				{
					IDAL.DO.BaseStation x = DataSource.baseStation[i];
					x.ChargeSlots--;
					DataSource.baseStation[i] = x;
					break;
				}
			}
		}
		/// <summary>
		/// change the status of a given drone to "free" and update the freed charge slot in the related basestation
		/// </summary>
		public void DroneLeaveChargeStation(int droneId, int baseId)
        {
			foreach (IDAL.DO.Drone item in DataSource.drone)
			{ if (item.Id == droneId) throw new IDAL.DO.InvalidIdDroneExist(); }
			for (int i = 0; i < DataSource.baseStation.Count; i++)
			{
				if (DataSource.baseStation[i].Id == baseId)
				{
					IDAL.DO.BaseStation x = DataSource.baseStation[i];
					x.ChargeSlots++;
					DataSource.baseStation[i] = x;
					break;
				}
			}
		}
		/// <summary>
		/// for a given base station Id, display it details
		/// </summary>
		public IDAL.DO.BaseStation DisplayBaseStation(int baseId)
        {
			foreach(IDAL.DO.BaseStation item in DataSource.baseStation)
            {
				if (item.Id == baseId) 
					return item;
            }
			throw "no such base";
		}
		/// <summary>
		/// for a given drone Id, display it details
		/// </summary>
		public IDAL.DO.Drone DisplayDrone(int droneId)
		{
			foreach (IDAL.DO.Drone item in DataSource.drone)
			{
				if (item.Id == droneId) 
					return item;
			}
			throw "no such base";
		}
		/// <summary>
		/// for a given customer Id, display it details
		/// </summary>
		public IDAL.DO.Customer DisplayCustomer(int customerId)
		{
			foreach (IDAL.DO.Customer item in DataSource.customers)
			{
				if (item.Id == customerId) 
					return item;
			}
			throw "no such base";
		}
		/// <summary>
		/// for a given parcel Id, display it details
		/// </summary>
		public IDAL.DO.Parcel DisplayParcel(int parcelId)
		{
			foreach (IDAL.DO.Parcel item in DataSource.parcels)
			{
				if (item.Id == parcelId) 
					return item;
			}
			throw "no such base";

		}
		/// <summary>
		/// display the details of all base stations
		/// </summary>
		public IEnumerable<IDAL.DO.BaseStation> DisplayListBaseStations()
		{
			return DataSource.baseStation;
		}
		/// <summary>
		/// display the details of all drones
		/// </summary>
		public IEnumerable<IDAL.DO.Drone> DisplayListDrones()
		{
			return DataSource.drone;
		}
		/// <summary>
		/// display the details of all customers
		/// </summary>
		public IEnumerable<IDAL.DO.Customer> DisplayListCustomers()
		{
			return DataSource.customers;
		}
		/// <summary>
		/// display the details of all parcels
		/// </summary>
		public IEnumerable<IDAL.DO.Parcel> DisplayListParcels()
		{
			return DataSource.parcels;
		}
		/// <summary>
		/// display the details of all parcels not assigned to a drone yet
		/// </summary>
		public IEnumerable<IDAL.DO.Parcel> DisplayParcelsNotAssignedToDrone()
		{
			List<IDAL.DO.Parcel> x = new List<IDAL.DO.Parcel>();
			foreach (IDAL.DO.Parcel item in DataSource.parcels)
            {
				if (item.DroneId == 0) 
					x.Add(item);
            }
			return x;
		}
		/// <summary>
		/// display the details of all base stations with available(s) charge slots
		/// </summary>
		public IEnumerable<IDAL.DO.BaseStation> DisplayListBaseStationsCanCharge()
		{
			List<IDAL.DO.BaseStation> x = new List<IDAL.DO.BaseStation>();
			foreach (IDAL.DO.BaseStation item in DataSource.baseStation)
			{
				if (item.ChargeSlots > 0)
					x.Add(item);
			}
			return x;
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
