using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalObject
{
	class DalObject : DalApi.IDal
	{
		internal static readonly Lazy<DalApi.IDal> _instance = new Lazy<DalApi.IDal>(() => new DalObject());
		public static DalApi.IDal GetInstance
		{
			get { return _instance.Value; }
		}

		//internal static DalObject _instance = null;
		//public static DalObject GetInstance() => _instance ?? (_instance = new DalObject());

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
			if (id < 0 || id == 0) throw new DO.InvalidId("BASE-STATION");
			if (name < 0 || name == 0) throw new DO.EmptyValue("BASE-STATION BIGGER THAN 0");
			if (chargeSlots < 0) throw new DO.InvalidChargeSlot();
			if (latti > 90 || latti < -90) throw new DO.InvalidLoc("LATITUDE","-90 TO 90");
			if (longi > 180 || longi < -180) throw new DO.InvalidLoc("LONGITUDE","-180 TO 180");

			if(DataSource.baseStations.Exists(item => item.Id == id))
				throw new DO.IdExist("BASE-STATION");

			DataSource.baseStations.Add(new DO.BaseStation()
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
		public void AddDrone(int id, string model, DO.WeightCategories weight)
		{
			if (id < 0 || id == 0) throw new DO.InvalidId("DRONE");
			if ((int)weight > 3 || (int)weight < 1) throw new DO.InvalidCategory("WEIGHT");
			if (model == "") throw new DO.EmptyValue("MODEL");

			if(DataSource.drones.Exists(item=>item.Id==id))
				throw new DO.IdExist("DRONE");

			DataSource.drones.Add(new DO.Drone()
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
			if (id < 0 || id == 0) throw new DO.InvalidId("CUSTOMER");
			if (name == "") throw new DO.EmptyValue("NAME");
			if (phone == "") throw new DO.EmptyValue("PHONE");
			if (latti > 90 || latti < -90) throw new DO.InvalidLoc("LATITUDE", "-90 TO 90");
			if (longi > 180 || longi < -180) throw new DO.InvalidLoc("LONGITUDE", "-180 TO 180");

			if (DataSource.customers.Exists(item=>item.Id==id))
				throw new DO.IdExist("CUSTOMER");

			DataSource.customers.Add(new DO.Customer()
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
		public void AddParcel(int id, int senderId, int targetId, int droneId, DO.WeightCategories weight, DO.Priorities priorities)
		{
			if (id < 0 || id == 0) throw new DO.InvalidId("PARCEL");
			if (senderId == 0 || senderId < 0) throw new DO.InvalidId("SENDER");
			if (targetId == 0 || targetId < 0) throw new DO.InvalidId("TARGET");
			if (targetId == senderId) throw new DO.SenderTargetIdEqual();
			if (droneId < 0) throw new DO.NegativeDroneId();
			if ((int)weight > 3 || (int)weight < 1) throw new DO.InvalidCategory("WEIGHT");
			if ((int)priorities > 3 || (int)priorities < 1) throw new DO.InvalidCategory("PRIORITIES");

			if (DataSource.parcels.Exists(item=>item.Id==id))
				throw new DO.IdExist("PARCEL");
			if(!DataSource.customers.Exists(item=>item.Id==senderId))
				throw new DO.IdNotExist("SENDER");
			if (!DataSource.customers.Exists(item => item.Id == targetId))
				throw new DO.IdNotExist("TARGET");

			DataSource.parcels.Add(new DO.Parcel()
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
			DO.Parcel p = new DO.Parcel();
			DO.Drone d = new DO.Drone();
			if(!DataSource.parcels.Exists(item => item.Id == parcelId))
				throw new DO.IdNotExist("PARCEL");
			if(!DataSource.drones.Exists(item => item.Id == droneId))
				throw new DO.IdNotExist("DRONE");
				
			foreach (DO.Parcel item in DataSource.parcels)
			{
				if (item.Id == parcelId)
				{
					p = item;
					break;
				}
			}
			foreach (DO.Drone item in DataSource.drones)
			{
				if (item.Id == droneId)
				{
					d = item;
					break;
				}
			}
			if (d.MaxWeight != p.Weight)
			{
				if (d.MaxWeight == DO.WeightCategories.Light && (p.Weight == DO.WeightCategories.Medium || p.Weight == DO.WeightCategories.Medium))
					throw new DO.ParcelTooHeavy();
				else if (d.MaxWeight == DO.WeightCategories.Medium && p.Weight == DO.WeightCategories.Medium)
					throw new DO.ParcelTooHeavy();
			}
			for (int i = 0; i < DataSource.parcels.Count(); i++)
			{
				if (DataSource.parcels[i].Id == parcelId)
				{
					DO.Parcel x = DataSource.parcels[i];
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
				throw new DO.IdNotExist("PARCEL");

			for (int i = 0; i < DataSource.parcels.Count; i++)
			{
				if (DataSource.parcels[i].Id == parcelId)
				{
					DO.Parcel x = DataSource.parcels[i];
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
				throw new DO.IdNotExist("PARCEL");

			for (int i = 0; i < DataSource.parcels.Count; i++)
			{
				if (DataSource.parcels[i].Id == parcelId)
				{
					DO.Parcel x = DataSource.parcels[i];
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
			if(!DataSource.drones.Exists(item => item.Id == droneId)) 
				throw new DO.IdNotExist("DRONE");
			if(!DataSource.baseStations.Exists(item => item.Id == baseId)) 
				throw new DO.IdNotExist("BASE-STATION");

			for (int i = 0; i < DataSource.baseStations.Count; i++)
			{
				if (DataSource.baseStations[i].Id == baseId)
				{
					DO.BaseStation x = DataSource.baseStations[i];
					x.ChargeSlots--;
					DataSource.baseStations[i] = x;
					break;
				}
			}
			DataSource.droneCharges.Add(new DO.DroneCharge { DroneId = droneId, StationId = baseId });
		}
		/// <summary>
		/// change the status of a given drone to "free" and update the freed charge slot in the related basestation
		/// </summary>
		/// <param name="droneId"></param>
		/// <param name="baseId"></param>
		public void DroneLeaveChargeStation(int droneId, int baseId)
		{
			if(!DataSource.drones.Exists(item => item.Id == droneId))
				throw new DO.IdNotExist("DRONE");
			if(!DataSource.baseStations.Exists(item => item.Id == baseId))
				throw new DO.IdNotExist("BASE-STATION");

			for (int i = 0; i < DataSource.baseStations.Count; i++)
			{
				if (DataSource.baseStations[i].Id == baseId)
				{
					DO.BaseStation x = DataSource.baseStations[i];
					x.ChargeSlots++;
					DataSource.baseStations[i] = x;
					break;
				}
			}
			for (int i  = 0; i < DataSource.droneCharges.Count(); i++)
			{
				if (DataSource.droneCharges[i].DroneId == droneId)
				{
					DataSource.droneCharges.RemoveAt(i);
				}
			}

		}

		//---------------------------------------------------------------------------------------------------AN SPECIFIC OBJECT-------------------------------------------------------------------------------------------
		/// <summary>
		/// for a given base station Id, display it details
		/// </summary>
		/// <param name="baseId"></param>
		/// <returns> an base station </returns>
		public DO.BaseStation GetBaseStation(int baseId)
		{
			foreach (DO.BaseStation item in DataSource.baseStations)
			{
				if (item.Id == baseId)
					return item;
			}
			throw new DO.IdNotExist("BASE-STATION");
		}
		/// <summary>
		/// for a given drone Id, display it details
		/// </summary>
		/// <param name="droneId"></param>
		/// <returns> an drone </returns>
		public DO.Drone GetDrone(int droneId)
		{
			foreach (DO.Drone item in DataSource.drones)
			{
				if (item.Id == droneId)
					return item;
			}
			throw new DO.IdNotExist("DRONE");
		}
		/// <summary>
		/// for a given customer Id, display it details
		/// </summary>
		/// <param name="customerId"></param>
		/// <returns> a customer </returns>
		public DO.Customer GetCustomer(int customerId)
		{
			foreach (DO.Customer item in DataSource.customers)
			{
				if (item.Id == customerId)
					return item;
			}
			throw new DO.IdNotExist("CUSTOMER");
		}
		/// <summary>
		/// for a given parcel Id, display it details
		/// </summary>
		/// <param name="parcelId"></param>
		/// <returns> an parcel </returns>
		public DO.Parcel GetParcel(int parcelId)
		{
			foreach (DO.Parcel item in DataSource.parcels)
			{
				if (item.Id == parcelId)
					return item;
			}
			throw new DO.IdNotExist("PARCEL");
		}

		//---------------------------------------------------------------------------------------------------LIST OF AN OBJECT--------------------------------------------------------------------------------------------
		/// <summary>
		/// display the details of all base stations
		/// </summary>
		/// <returns> list of Base-Station</returns>
		public IEnumerable<DO.BaseStation> GetListBaseStations(Predicate<DO.BaseStation> f)
		{
			return DataSource.baseStations.FindAll(f);
		}
		/// <summary>
		/// display the details of all drones
		/// </summary>
		/// <returns> list of drones </returns>
		public IEnumerable<DO.Drone> GetListDrones(Predicate<DO.Drone> f)
		{
			return DataSource.drones.FindAll(f);
		}
		/// <summary>
		/// display the details of all customers
		/// </summary>
		/// <returns> list of custome </returns>
		public IEnumerable<DO.Customer> GetListCustomers()
		{
			return DataSource.customers;
		}
		/// <summary>
		/// display the details of all parcels
		/// </summary>
		/// <returns> List of parcels </returns>
		public IEnumerable<DO.Parcel> GetListParcels(Predicate<DO.Parcel> f)
		{
			return DataSource.parcels.FindAll(f);
		}

		//--------------------------------------------------------------------------------------------------------SETTER-----------------------------------------------------------------------------------------
		/// <summary>
		/// the func change the name of the model drone by his id
		/// </summary>
		/// <param name="drone"></param>
		public void UpdateDrone(DO.Drone drone)
		{
			for (int i = 0; i < DataSource.drones.Count(); i++)
			{
				if (DataSource.drones[i].Id == drone.Id)
				{	
					DataSource.drones[i] = drone; break;
				}
			}
		}
		/// <summary>
		/// the func update some data in the data base
		/// </summary>
		/// <param name="baseStation"></param>
		public void UpdateBaseStation(DO.BaseStation baseStation)
		{
			for (int i = 0; i < DataSource.baseStations.Count(); i++)
			{
				if (DataSource.baseStations[i].Id == baseStation.Id)
				{ 
					DataSource.baseStations[i] = baseStation; break;
				}
			}
		}
		/// <summary>
		/// update data of customer
		/// </summary>
		/// <param name="customer"></param>
		public void UpdateCustomer(DO.Customer customer)
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
		public void UpdateParcel(DO.Parcel parcel)
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