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

		//----------------------------------------------------------------------------------------------------------ADDING-------------------------------------------------------------------------------------------------
		/// <summary>
		/// add a station in baseStation[] created in DataSource with details given by user
		/// </summary>
		/// <param name="id"></param>
		/// <param name="name"></param>
		/// <param name="chargeSlots"></param>
		/// <param name="longe"></param>
		/// <param name="lati"></param>
		public void AddBaseStation(int id, int name, int chargeSlots, double longi, double latti)
		{
			if (id < 0 || id == 0) throw new IDAL.DO.InvalidId("BASE-STATION");
			if (chargeSlots < 0) throw new IDAL.DO.InvalidChargeSlot();
			if (latti > 90 || latti < -90) throw new IDAL.DO.InvalidLoc("LATITUDE","-90 TO 90");
			if (longi > 180 || longi < -180) throw new IDAL.DO.InvalidLoc("LONGITUDE","-180 TO 180");

			if(DataSource.baseStation.Exists(item => item.Id == id))
				throw new IDAL.DO.IdExist("BASE-STATION");

			DataSource.baseStation.Add(new IDAL.DO.BaseStation()
			{
				Id = id,
				Name = name,
				ChargeSlots = chargeSlots,
				Latitude = latti,
				Longitude = longi
			});
		}
		/// <summary>
		/// add a drone in drone[] created in DataSource with details given by user
		/// </summary>
		/// <param name="id"></param>
		/// <param name="model"></param>
		/// <param name="weight"></param>
		public void AddDrone(int id, string model, IDAL.DO.WeightCategories weight)
		{
			if (id < 0 || id == 0) throw new IDAL.DO.InvalidId("DRONE");
			if ((int)weight > 3 || (int)weight < 1) throw new IDAL.DO.InvalidCategory("WEIGHT");

			if(DataSource.drone.Exists(item=>item.Id==id))
				throw new IDAL.DO.IdExist("DRONE");

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
		/// <param name="id"></param>
		/// <param name="name"></param>
		/// <param name="phone"></param>
		/// <param name="longi"></param>
		/// <param name="lati"></param>
		public void AddCustomer(int id, string name, string phone, double longi, double latti)
		{
			if (id < 0 || id == 0) throw new IDAL.DO.InvalidId("CUSTOMER");
			if (latti > 90 || latti < -90) throw new IDAL.DO.InvalidLoc("LATITUDE", "-90 TO 90");
			if (longi > 180 || longi < -180) throw new IDAL.DO.InvalidLoc("LONGITUDE", "-180 TO 180");

			if (DataSource.customers.Exists(item=>item.Id==id))
				throw new IDAL.DO.IdExist("CUSTOMER");

			DataSource.customers.Add(new IDAL.DO.Customer()
			{
				Id = id,
				Name = name,
				Phone = phone,
				Longitude = longi,
				Latitude = latti
			});
		}
		/// <summary>
		/// add a parcel in parcels[] created in DataSource with details given by user
		/// </summary>
		/// <param name="id"></param>
		/// <param name="senderId"></param>
		/// <param name="targetId"></param>
		/// <param name="weight"></param>
		/// <param name="priorities"></param>
		/// <param name="droneId"></param>
		/// <param name="requested"></param>
		/// <param name="scheduled"></param>
		/// <param name="pickedUp"></param>
		/// <param name="delivered"></param>
		public void AddParcel(int id, int senderId, int targetId, int droneId, IDAL.DO.WeightCategories weight, IDAL.DO.Priorities priorities)
		{
			if (id < 0 || id == 0) throw new IDAL.DO.InvalidId("PARCEL");
			if (senderId == 0 || senderId < 0) throw new IDAL.DO.InvalidId("SENDER");
			if (targetId == 0 || targetId < 0) throw new IDAL.DO.InvalidId("TARGET");
			if (targetId == senderId) throw new IDAL.DO.SenderTargetIdEqual();
			if (droneId < 0) throw new IDAL.DO.NegativeDroneId();
			if ((int)weight > 3 || (int)weight < 1) throw new IDAL.DO.InvalidCategory("WEIGHT");
			if ((int)priorities > 3 || (int)priorities < 1) throw new IDAL.DO.InvalidCategory("PRIORITIES");

			if (DataSource.parcels.Exists(item=>item.Id==id))
				throw new IDAL.DO.IdExist("PARCEL");
			if(!DataSource.customers.Exists(item=>item.Id==senderId))
				throw new IDAL.DO.IdNotExist("SENDER");
			if (!DataSource.customers.Exists(item => item.Id == targetId))
				throw new IDAL.DO.IdNotExist("TARGET");

			DataSource.parcels.Add(new IDAL.DO.Parcel()
			{
				Id = id,
				SenderId = senderId,
				TargetId = targetId,
				Weight = weight,
				Priority = priorities,
				DroneId = droneId,
				Requested = DateTime.Now,
				Scheduled = DateTime.MinValue,
				PickedUp = DateTime.MinValue,
				Delivered = DateTime.MinValue
			});
		}

		//---------------------------------------------------------------------------------------------------------UPDATE--------------------------------------------------------------------------------------------------
		/// <summary>
		/// add a drone Id in droneId field from a parcel given by the user
		/// </summary>
		/// <param name="parcelId"></param>
		/// <param name="droneId"></param>
		public void AssignParcelToDrone(int parcelId, int droneId)
		{
			IDAL.DO.Parcel p = new IDAL.DO.Parcel();
			IDAL.DO.Drone d = new IDAL.DO.Drone();
			if(!DataSource.parcels.Exists(item => item.Id == parcelId))
				throw new IDAL.DO.IdNotExist("PARCEL");
			if(!DataSource.drone.Exists(item => item.Id == droneId))
				throw new IDAL.DO.IdNotExist("DRONE");
				
			foreach (IDAL.DO.Parcel item in DataSource.parcels)
			{
				if (item.Id == parcelId)
				{
					p = item;
					break;
				}
			}
			foreach (IDAL.DO.Drone item in DataSource.drone)
			{
				if (item.Id == droneId)
				{
					d = item;
					break;
				}
			}
			if (d.MaxWeight != p.Weight)
			{
				if (d.MaxWeight == IDAL.DO.WeightCategories.Light && (p.Weight == IDAL.DO.WeightCategories.Medium || p.Weight == IDAL.DO.WeightCategories.Medium))
					throw new IDAL.DO.ParcelTooHeavy();
				else if (d.MaxWeight == IDAL.DO.WeightCategories.Medium && p.Weight == IDAL.DO.WeightCategories.Medium)
					throw new IDAL.DO.ParcelTooHeavy();
			}
			for (int i = 0; i < DataSource.parcels.Count(); i++)
			{
				if (DataSource.parcels[i].Id == parcelId)
				{
					IDAL.DO.Parcel x = DataSource.parcels[i];
					x.DroneId = droneId;
					x.Scheduled = DateTime.Now;
					DataSource.parcels[i] = x;
					break;
				}
			}
		}
		/// <summary>
		/// change the PickedUp date of a given parcel by the present time and change the status of the related drone to "shipping"
		/// </summary>
		/// <param name="parcelId"></param>
		public void ParcelOnDrone(int parcelId)
		{
			if(!DataSource.parcels.Exists(item => item.Id == parcelId))
				throw new IDAL.DO.IdNotExist("PARCEL");

			for (int i = 0; i < DataSource.parcels.Count; i++)
			{
				if (DataSource.parcels[i].Id == parcelId)
				{
					IDAL.DO.Parcel x = DataSource.parcels[i];
					x.PickedUp = DateTime.Now;
					DataSource.parcels[i] = x;
					break;
				}
			}
		}
		/// <summary>
		/// change the "Delivered" date of a given parcel by the present time and change the status of the related drone to "free"
		/// </summary>
		/// <param name="parcelId"></param>
		public void ParcelDelivered(int parcelId)
		{
			if(!DataSource.parcels.Exists(item => item.Id == parcelId)) 
				throw new IDAL.DO.IdNotExist("PARCEL");

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
		/// <param name="droneId"></param>
		/// <param name="baseId"></param>
		public void AssignDroneToBaseStation(int droneId, int baseId)
		{
			if(!DataSource.drone.Exists(item => item.Id == droneId)) 
				throw new IDAL.DO.IdNotExist("DRONE");
			if(!DataSource.baseStation.Exists(item => item.Id == baseId)) 
				throw new IDAL.DO.IdNotExist("BASE-STATION");

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
			DataSource.droneCharge.Add(new IDAL.DO.DroneCharge { DroneId = droneId, StationId = baseId });
		}
		/// <summary>
		/// change the status of a given drone to "free" and update the freed charge slot in the related basestation
		/// </summary>
		/// <param name="droneId"></param>
		/// <param name="baseId"></param>
		public void DroneLeaveChargeStation(int droneId, int baseId)
		{
			if(!DataSource.drone.Exists(item => item.Id == droneId))
				throw new IDAL.DO.IdNotExist("DRONE");
			if(!DataSource.baseStation.Exists(item => item.Id == baseId))
				throw new IDAL.DO.IdNotExist("BASE-STATION");

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
			for (int i  = 0; i < DataSource.droneCharge.Count(); i++)
			{
				if (DataSource.droneCharge[i].DroneId == droneId)
				{
					DataSource.droneCharge.RemoveAt(i);
				}
			}

		}

		//---------------------------------------------------------------------------------------------------AN SPECIFIC OBJECT-------------------------------------------------------------------------------------------
		/// <summary>
		/// for a given base station Id, display it details
		/// </summary>
		/// <param name="baseId"></param>
		/// <returns> an base station </returns>
		public IDAL.DO.BaseStation GetBaseStation(int baseId)
		{
			foreach (IDAL.DO.BaseStation item in DataSource.baseStation)
			{
				if (item.Id == baseId)
					return item;
			}
			throw new IDAL.DO.IdNotExist("BASE-STATION");
		}
		/// <summary>
		/// for a given drone Id, display it details
		/// </summary>
		/// <param name="droneId"></param>
		/// <returns> an drone </returns>
		public IDAL.DO.Drone GetDrone(int droneId)
		{
			foreach (IDAL.DO.Drone item in DataSource.drone)
			{
				if (item.Id == droneId)
					return item;
			}
			throw new IDAL.DO.IdNotExist("DRONE");
		}
		/// <summary>
		/// for a given customer Id, display it details
		/// </summary>
		/// <param name="customerId"></param>
		/// <returns> a customer </returns>
		public IDAL.DO.Customer GetCustomer(int customerId)
		{
			foreach (IDAL.DO.Customer item in DataSource.customers)
			{
				if (item.Id == customerId)
					return item;
			}
			throw new IDAL.DO.IdNotExist("CUSTOMER");
		}
		/// <summary>
		/// for a given parcel Id, display it details
		/// </summary>
		/// <param name="parcelId"></param>
		/// <returns> an parcel </returns>
		public IDAL.DO.Parcel GetParcel(int parcelId)
		{
			foreach (IDAL.DO.Parcel item in DataSource.parcels)
			{
				if (item.Id == parcelId)
					return item;
			}
			throw new IDAL.DO.IdNotExist("PARCEL");
		}

		//---------------------------------------------------------------------------------------------------LIST OF AN OBJECT--------------------------------------------------------------------------------------------
		/// <summary>
		/// display the details of all base stations
		/// </summary>
		/// <returns> list of Base-Station</returns>
		public IEnumerable<IDAL.DO.BaseStation> GetListBaseStations()
		{
			return DataSource.baseStation;
		}
		/// <summary>
		/// display the details of all drones
		/// </summary>
		/// <returns> list of drones </returns>
		public IEnumerable<IDAL.DO.Drone> GetListDrones()
		{
			return DataSource.drone;
		}
		/// <summary>
		/// display the details of all customers
		/// </summary>
		/// <returns> list of custome </returns>
		public IEnumerable<IDAL.DO.Customer> GetListCustomers()
		{
			return DataSource.customers;
		}
		/// <summary>
		/// display the details of all parcels
		/// </summary>
		/// <returns> List of parcels </returns>
		public IEnumerable<IDAL.DO.Parcel> GetListParcels()
		{
			return DataSource.parcels;
		}
		/// <summary>
		/// display the details of all parcels not assigned to a drone yet
		/// </summary>
		/// <returns> list of parcel that not assign to a drone </returns>
		public IEnumerable<IDAL.DO.Parcel> GetListOfParcelsNotAssignedToDrone()
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
		///<returns> list of a basse station where the drone can go to charger his battery </returns>
		public IEnumerable<IDAL.DO.BaseStation> GetListBaseStationsCanCharge()
		{
			List<IDAL.DO.BaseStation> x = new List<IDAL.DO.BaseStation>();
			foreach (IDAL.DO.BaseStation item in DataSource.baseStation)
			{
				if (item.ChargeSlots > 0)
					x.Add(item);
			}
			return x;
		}

		//--------------------------------------------------------------------------------------------------------SETTER-----------------------------------------------------------------------------------------
		/// <summary>
		/// the func change the name of the model drone by his id
		/// </summary>
		/// <param name="drone"></param>
		public void UpdateDrone(IDAL.DO.Drone drone)
		{
			for (int i = 0; i < DataSource.drone.Count(); i++)
			{
				if (DataSource.drone[i].Id == drone.Id)
				{	
					DataSource.drone[i] = drone; break;
				}
			}
		}
		/// <summary>
		/// the func update some data in the data base
		/// </summary>
		/// <param name="baseStation"></param>
		public void UpdateBaseStation(IDAL.DO.BaseStation baseStation)
		{
			for (int i = 0; i < DataSource.baseStation.Count(); i++)
			{
				if (DataSource.baseStation[i].Id == baseStation.Id)
				{ 
					DataSource.baseStation[i] = baseStation; break;
				}
			}
		}
		/// <summary>
		/// update data of customer
		/// </summary>
		/// <param name="customer"></param>
		public void UpdateCustomer(IDAL.DO.Customer customer)
		{
			for (int i = 0; i < DataSource.customers.Count(); i++)
			{
				if (DataSource.customers[i].Id == customer.Id)
				{ 
					DataSource.customers[i] = customer; break;
				}
			}
		}
		/// <summary>
		/// update date of parcel
		/// </summary>
		/// <param name="parcel"></param>
		public void UpdateParcel(IDAL.DO.Parcel parcel)
		{
			for (int i = 0; i < DataSource.parcels.Count(); i++)
			{
				if (DataSource.parcels[i].Id == parcel.Id)
				{
					DataSource.parcels[i] = parcel; break;
				}
			}
		}
		//-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// the func go to config to take the value of the charging rate and the electricity use
		/// </summary>
		/// <returns> return the arr with the value </returns>
		public double[] GetChargingRate()
		{
			DataSource.Config c = new DataSource.Config();
			double[] arr = new double[5];
			arr[0] = DataSource.Config.useWhenFree;
			arr[1] = DataSource.Config.useWhenLightly;
			arr[2] = DataSource.Config.useWhenMedium;
			arr[3] = DataSource.Config.useWhenHeavily;
			arr[4] = c.chargingRate;
			return arr;
		}
	}
}
