using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{

	class BL : IBL.IBL
	{
		private IDAL.IDal dal;

		private double _useWhenFree;
		private double _useWhenLightly;
		private double _useWhenMedium;
		private double _useWhenHeavily;
		private double _chargingRate;
		List<IBL.BO.DroneToList> droneToList = new List<IBL.BO.DroneToList>();
		//ctor
		BL()
		{
			getDataCharge();
			reqListOfDrone();
			settingDroneByParcel();
			settingDrone();
		}
		//------------------------------------------------------------------------------------------------METHOD FOR THE CTOR--------------------------------------------------------------------------------------------
		/// <summary>
		/// the func get the data of charge and use battery for each situation(free,light packege...)
		/// </summary>
		private void getDataCharge()
		{
			double[] arr = new double[5];
			arr = dal.GetChargingRate();
			_useWhenFree = arr[0];
			_useWhenLightly = arr[1];
			_useWhenMedium = arr[2];
			_useWhenHeavily = arr[3];
			_chargingRate = arr[4];
		}
		/// <summary>
		/// the func req from the data base all data about the drone
		/// </summary>
		private void reqListOfDrone()
		{
			foreach (IDAL.DO.Drone item in dal.GetListDrones())
			{
				IBL.BO.Location l = new IBL.BO.Location();
				l.Latitude = 0; l.Longitude = 0;
				droneToList.Add(new IBL.BO.DroneToList
				{
					Id = item.Id,
					Model = item.Model,
					MaxWeight = (IBL.BO.WeightCategories)item.MaxWeight,
					Battery = 0,
					Status = IBL.BO.DroneStatuses.free,
					Loc = l,
					IdOfParcel = 0
				});
			}
		}
		/// <summary>
		/// set for all drone that is not in shipping his battery by his status
		/// </summary>
		private void settingDrone()
		{
			List<IBL.BO.BaseStation> baseS = new List<IBL.BO.BaseStation>();
			//add the data
			foreach (IDAL.DO.BaseStation b in dal.GetListBaseStations())
			{
				IBL.BO.Location l = new IBL.BO.Location();
				l.Latitude = b.Latitude; l.Longitude = b.Longitude;
				baseS.Add(new IBL.BO.BaseStation { Loc = l });
			}

			for (int i = 0; i < droneToList.Count(); i++)
			{
				if (droneToList[i].Status != IBL.BO.DroneStatuses.Shipping)
				{
					int r = new Random().Next(1, 3);
					if (r == 1)
						droneToList[i].Status = IBL.BO.DroneStatuses.free;
					else
						droneToList[i].Status = IBL.BO.DroneStatuses.Maintenance;
				}
				if (droneToList[i].Status == IBL.BO.DroneStatuses.Maintenance)
				{
					int r = new Random().Next(0, baseS.Count());
					droneToList[i].Loc.Longitude = baseS[r].Loc.Longitude;
					droneToList[i].Loc.Latitude = baseS[r].Loc.Latitude;
					r = new Random().Next(0, 21);
					droneToList[i].Battery = r;
					break;
				}
				else if (droneToList[i].Status == IBL.BO.DroneStatuses.free)
				{
					List<IBL.BO.Customer> customerList = new List<IBL.BO.Customer>();
					foreach (IDAL.DO.Parcel item in dal.GetListParcels())
					{
						try
						{
							if (item.Delivered >= DateTime.Now)
								customerList.Add(getCustomer(item.SenderId));
						}
						catch (IDAL.DO.CustomerIdNotExist ex)
						{
							throw new IBL.BO.CustomerIdNotExist();
						}
					}
					int r = new Random().Next(0, customerList.Count());
					droneToList[i].Loc.Longitude = customerList[r].Loc.Longitude;
					droneToList[i].Loc.Latitude = customerList[r].Loc.Latitude;

					int percent = calcBatteryToCloserBase(droneToList[i].Loc.Longitude, droneToList[i].Loc.Latitude);
					int finalPercent = new Random().Next(percent, 101);
					droneToList[i].Battery = finalPercent;
				}
			}
		}
		/// <summary>
		/// the func set the loc and the satus of the drone if is assigne to an parcel
		/// </summary>
		private void settingDroneByParcel()
		{
			List<IBL.BO.ParcelToList> parcelToList = new List<IBL.BO.ParcelToList>();
			//add the data
			foreach (IDAL.DO.Parcel item in dal.GetListParcels())
			{
				parcelToList.Add(new IBL.BO.ParcelToList
				{
					Id = item.Id,
					Weight = (IBL.BO.WeightCategories)item.Weight,
					Priority = (IBL.BO.Priorities)item.Priority,
					Status = IBL.BO.ParcelStatues.Defined
				});
			}
			for (int i = 0; i < parcelToList.Count(); i++)
			{
				int droneIdAssigneToParcel = searchDroneIdAssigneToParcel(parcelToList[i]);
				if (parcelToList[i].Status != IBL.BO.ParcelStatues.Delivered && droneIdAssigneToParcel > 0)
				{
					droneToList[searchDrone(droneIdAssigneToParcel)].Status = IBL.BO.DroneStatuses.Shipping;

					locOfDroneByParcel(parcelToList[i], droneIdAssigneToParcel);

					int percent = calcBatteryToShipping(droneToList[searchDrone(droneIdAssigneToParcel)], parcelToList[i]);
					int finalPercent = new Random().Next(percent, 101);
					droneToList[i].Battery = finalPercent;
				}
			}
		}
		
		//---------------------------------------------------------------------------------------FUNC TO HELP THE METHOD OF THE CTOR-------------------------------------------------------------------------------------
		/// <summary>
		/// the func calculate the distance between two points for examaple: the distance between the target and the closer base station 
		/// </summary>
		/// <param name="latA"></param>
		/// <param name="lonA"></param>
		/// <param name="latB"></param>
		/// <param name="lonB"></param>
		/// <returns> the distance </returns>
		private double distanceBetweenTwoPoints(double latA, double lonA, double latB, double lonB)
		{
			if ((latA == latB) && (lonA == lonB))
			{
				return 0;
			}
			else
			{
				var radlat1 = Math.PI * latA / 180;
				var radlat2 = Math.PI * latB / 180;
				var theta = lonA - lonB;
				var radtheta = Math.PI * theta / 180;
				var dist = Math.Sin(radlat1) * Math.Sin(radlat2) + Math.Cos(radlat1) * Math.Cos(radlat2) * Math.Cos(radtheta);
				if (dist > 1)
				{
					dist = 1;
				}
				dist = Math.Acos(dist);
				dist = dist * 180 / Math.PI;
				dist = dist * 60 * 1.1515;
				dist = dist * 1.609344;
				return dist;
			}
		}
		/// <summary>
		/// the func calculate the percent of battery that the drone need to to the shipping and to return to the base station that the must closer
		/// </summary>
		/// <param name="d"></param>
		/// <param name="p"></param>
		/// <returns> the percent of battery </returns>
		private int calcBatteryToShipping(IBL.BO.DroneToList d, IBL.BO.ParcelToList p)
		{
			double disToTarget = distanceBetweenTwoPoints(d.Loc.Latitude, d.Loc.Longitude, targetLati(p.NameTarget), targetLong(p.NameTarget));
			double disToBase = disToCloserBase(targetLati(p.NameTarget), targetLong(p.NameTarget));

			double batteryToBase = _useWhenFree * disToBase;
			double batteryToSender;
			if (p.Weight == IBL.BO.WeightCategories.Light)
				batteryToSender = _useWhenLightly * disToTarget;
			else if (p.Weight == IBL.BO.WeightCategories.Medium)
				batteryToSender = _useWhenMedium * disToTarget;
			else
				batteryToSender = _useWhenHeavily * disToTarget;
			int finalBattery = (int)(Math.Ceiling(batteryToBase) + Math.Ceiling(batteryToSender));
			return finalBattery;
		}
		/// <summary>
		/// the func calculate the percent of battery that the drone need to go back to the closer base station 
		/// </summary>
		/// <returns></returns>
		private int calcBatteryToCloserBase(double longi, double lati)
		{
			double disToBase = disToCloserBase(longi, lati);
			double batteryToBase = _useWhenFree * disToBase;
			return ((int)(Math.Ceiling(batteryToBase)));
		}
		/// <summary>
		/// the func search the longitude of the sender by her name in the data base
		/// </summary>
		/// <param name="name"></param>
		/// <returns> the longitude </returns>
		private double senderLong(string name)
		{
			foreach (IDAL.DO.Customer item in dal.GetListCustomers())
			{
				if (item.Name == name)
					return item.Longitude;
			}
			throw new IBL.BO.SenderNameNotExist();
		}
		/// <summary>
		/// the func search the latitude of the sender by her name in the data base
		/// </summary>
		/// <param name="name"></param>
		/// <returns> the latitude </returns>
		private double senderLati(string name)
		{
			foreach (IDAL.DO.Customer item in dal.GetListCustomers())
			{
				if (item.Name == name)
					return item.Latitude;
			}
			throw new IBL.BO.SenderNameNotExist();
		}
		/// <summary>
		/// the func search the longitude of the target by her name in the data base
		/// </summary>
		/// <param name="name"></param>
		/// <returns> the longitude </returns>
		private double targetLong(string name)
		{
			foreach (IDAL.DO.Customer item in dal.GetListCustomers())
			{
				if (item.Name == name)
					return item.Longitude;
			}
			throw new IBL.BO.TargetNameNotExist();
		}
		/// <summary>
		/// the func search the latitude of the target by her name in the data base
		/// </summary>
		/// <param name="name"></param>
		/// <returns> the latitude </returns>
		private double targetLati(string name)
		{
			foreach (IDAL.DO.Customer item in dal.GetListCustomers())
			{
				if (item.Name == name)
					return item.Latitude;
			}
			throw new IBL.BO.TargetNameNotExist();
		}
		/// <summary>
		/// the func search a drone by her id in the "DroneToList" and return the index of the drone 
		/// </summary>
		/// <param name="id"></param>
		/// <returns> index </returns>
		private int searchDrone(int id)
		{
			for (int i = 0; i < droneToList.Count(); i++)
			{
				if (droneToList[i].Id == id)
					return i;
			}
			throw new IBL.BO.DroneIdNotExist();
		}
		/// <summary>
		/// the func search which drone is assigne to this parcel if is defined
		/// </summary>
		/// <param name="p"></param>
		/// <returns> Id </returns>
		private int searchDroneIdAssigneToParcel(IBL.BO.ParcelToList p)
		{
			foreach (IDAL.DO.Parcel item in dal.GetListParcels())
			{
				if (item.Id == p.Id)
					return item.DroneId;
			}
			throw new IBL.BO.DroneIdNotExist();
		}
		/// <summary>
		/// the func locate the drone according to his status 
		/// </summary>
		/// <param name="p"></param>
		/// <param name="droneId"></param>
		private void locOfDroneByParcel(IBL.BO.ParcelToList p, int droneId)
		{
			if (p.Status == IBL.BO.ParcelStatues.Associated)
			{
				double finalDis = -1;
				IBL.BO.Location loc = new IBL.BO.Location();
				foreach (IDAL.DO.BaseStation b in dal.GetListBaseStations())
				{
					double dis = distanceBetweenTwoPoints(b.Latitude, b.Longitude, senderLati(p.NameSender), senderLong(p.NameSender));
					if (finalDis == -1 || dis < finalDis)
					{
						droneToList[searchDrone(droneId)].Loc.Longitude = b.Longitude;
						droneToList[searchDrone(droneId)].Loc.Latitude = b.Latitude;
						finalDis = dis;
					}

				}
			}
			else if (p.Status == IBL.BO.ParcelStatues.Collected)
			{
				droneToList[searchDrone(droneId)].Loc.Latitude = senderLati(p.NameSender);
				droneToList[searchDrone(droneId)].Loc.Longitude = senderLong(p.NameSender);
			}
		}
		/// <summary>
		/// the func search who is send the parcel according to the "id"
		/// </summary>
		/// <param name="id"></param>
		/// <returns> an customer </returns>
		private IBL.BO.Customer getCustomer(int id)
		{
			foreach (IDAL.DO.Customer item in dal.GetListCustomers())
			{
				IBL.BO.Location l = new IBL.BO.Location();
				if (item.Id == id)
				{
					l.Latitude = item.Latitude; l.Longitude = item.Longitude;
					IBL.BO.Customer c = new IBL.BO.Customer { Id = item.Id, Name = item.Name, Loc = l };
					return c;
				}
			}
			throw new IBL.BO.CustomerIdNotExist();
		}
		/// <summary>
		/// the func calculate the distance of the closer base station 
		/// </summary>
		/// <returns> the distance </returns>
		private double disToCloserBase(double longi, double lati)
		{
			double disToBase = -1;
			foreach (IDAL.DO.BaseStation b in dal.GetListBaseStations())
			{
				double dis = distanceBetweenTwoPoints(b.Latitude, b.Longitude, lati, longi);
				if ((disToBase == -1 || dis < disToBase) && b.ChargeSlots > 0)//if "dis" that is calculate in the previous line is smaller than the "disToBase"= the smaller distanse right now so "disToBase = dis"
					disToBase = dis;

			}
			return disToBase;
		}

		//------------------------------------------------------------------------------------------------------ADD - OPTION------------------------------------------------------------------------------------------------
		/// <summary>
		/// the func add an new base station to the data base
		/// </summary>
		/// <param name="id"></param>
		/// <param name="name"></param>
		/// <param name="chargeSlots"></param>
		/// <param name="location"></param>
		public void AddBaseStation(int id, int name, int chargeSlots, IBL.BO.Location location)
		{
			try
			{ dal.AddBaseStation(id, name, chargeSlots, location.Longitude, location.Latitude); }
			catch (IDAL.DO.InvalidBaseId ex)
			{ throw new IBL.BO.InvalidBaseId(); }

			catch (IDAL.DO.InvalidChargeSlot ex)
			{ throw new IBL.BO.InvalidChargeSlot(); }

			catch (IDAL.DO.InvalidLatitude ex)
			{ throw new IBL.BO.InvalidLatitude(); }

			catch (IDAL.DO.InvalidLongitude ex)
			{ throw new IBL.BO.InvalidLongitude(); }

			catch (IDAL.DO.BaseIdExist)
			{ throw new IBL.BO.BaseIdExist(); }
		}
		/// <summary>
		/// the func add a new drone to the data base
		/// </summary>
		/// <param name="id"></param>
		/// <param name="model"></param>
		/// <param name="weight"></param>
		/// <param name="firstBaseStation"></param>
		public void AddDrone(int id, string model, IDAL.DO.WeightCategories weight, int firstBaseStation)
		{
			try
			{ dal.AddDrone(id, model, weight); }
			catch (IDAL.DO.InvalidDroneId ex)
			{ throw new IBL.BO.InvalidDroneId(); }

			catch (IDAL.DO.InvalidWeight ex)
			{ throw new IBL.BO.InvalidWeight(); }

			catch (IDAL.DO.DroneIdExist ex)
			{ throw new IBL.BO.DroneIdExist(); }

			try
			{ dal.AssignDroneToBaseStation(id, firstBaseStation); }
			catch (IDAL.DO.DroneIdNotExist ex)
			{ throw new IBL.BO.DroneIdNotExist(); }
			catch (IDAL.DO.BaseIdNotExist ex)
			{ throw new IBL.BO.BaseIdNotExist(); }

			int r = new Random().Next(20, 41);
			double battery = r;

			int counter;
			for (counter = 0; counter < droneToList.Count(); counter++)
			{
				if (droneToList[counter].Id == id)
				{
					droneToList[counter].Status = IBL.BO.DroneStatuses.Maintenance;
					droneToList[counter].Loc.Longitude = dal.GetBaseStation(firstBaseStation).Longitude;
					droneToList[counter].Loc.Latitude = dal.GetBaseStation(firstBaseStation).Latitude;
					break;
				}
			}
			if (counter == droneToList.Count())
			{ throw new IBL.BO.DroneIdNotExist(); }
		}
		/// <summary>
		/// the func add a new customer to the data base
		/// </summary>
		/// <param name="id"></param>
		/// <param name="name"></param>
		/// <param name="phone"></param>
		/// <param name="location"></param>
		public void AddCustomer(int id, string name, string phone, IBL.BO.Location location)
		{
			try
			{
				dal.AddCustomer(id, name, phone, location.Longitude, location.Latitude);
			}
			catch (IDAL.DO.InvalidCustomerId ex)
			{ throw new IBL.BO.InvalidCustomerId(); }

			catch (IDAL.DO.InvalidLatitude ex)
			{ throw new IBL.BO.InvalidLatitude(); }

			catch (IDAL.DO.InvalidLongitude ex)
			{ throw new IBL.BO.InvalidLongitude(); }

			catch (IDAL.DO.CustomerIdExist)
			{ throw new IBL.BO.CustomerIdExist(); }
		}
		/// <summary>
		/// the fun add an new parcel to the data base
		/// </summary>
		/// <param name="id"></param>
		/// <param name="senderId"></param>
		/// <param name="targetId"></param>
		/// <param name="weight"></param>
		/// <param name="priorities"></param>
		public void AddParcel(int id, int senderId, int targetId, IDAL.DO.WeightCategories weight, IDAL.DO.Priorities priorities)
		{
			try
			{ dal.AddParcel(id, senderId, targetId, 0, weight, priorities); }
			//ב-BL כל הזמנים יאותחלו לזמן אפס למעט תאריך יצירה שיאותחל ל-DateTime.Now
			catch (IDAL.DO.InvalidParcelId ex)
			{ throw new IBL.BO.InvalidParcelId(); }

			catch (IDAL.DO.InvalidSenderId ex)
			{ throw new IBL.BO.InvalidSenderId();
			}

			catch (IDAL.DO.InvalidTargetId ex)
			{ throw new IBL.BO.InvalidTargetId();
			}

			catch (IDAL.DO.NegativeDroneId ex)
			{ throw new IBL.BO.InvalidDroneId();
			}

			catch (IDAL.DO.InvalidWeight ex)
			{ throw new IBL.BO.InvalidWeight(); }

			catch (IDAL.DO.InvalidPriority ex)
			{ throw new IBL.BO.InvalidPriority();
			}

			catch (IDAL.DO.ParcelIdExist ex)
			{ throw new IBL.BO.ParcelIdExist();
			}

			catch (IDAL.DO.SenderIdNotExist ex)
			{ throw new IBL.BO.SenderIdNotExist(); }

			catch (IDAL.DO.TargetIdNotExist ex)
			{ throw new IBL.BO.TargetIdNotExist(); }
		}

		//-----------------------------------------------------------------------------------------------------UPDATE - OPTION-------------------------------------------------------------------------------------------------------
		/// <summary>
		/// the func update the name of model drone
		/// </summary>
		/// <param name="droneId"></param>
		/// <param name="model"></param>
		public void UpdateDrone(int droneId, string model)
		{
			IDAL.DO.Drone drone = new IDAL.DO.Drone();
			drone = dal.GetDrone(droneId);
			drone.Model = model;
			dal.UpdateDrone(drone);
		}
		/// <summary>
		/// update the data of data base
		/// </summary>
		/// <param name="baseId"></param>
		/// <param name="name"></param>
		/// <param name="chargeslots"></param>
		public void UpdateBaseStation(int baseId, string name = null, string chargeslots = null)
		{
			IDAL.DO.BaseStation baseStation = new IDAL.DO.BaseStation();
			int name_int, charge_slots;
			if (name != "")
			{
				name_int = int.Parse(name);
				baseStation = dal.GetBaseStation(baseId);
				baseStation.Name = name_int;

			}
			if (chargeslots != "")
			{
				charge_slots = int.Parse(chargeslots);
				baseStation.ChargeSlots = charge_slots;
			}
			dal.UpdateBaseStation(baseStation);
		}
		/// <summary>
		/// the func update the data of customer
		/// </summary>
		/// <param name="customerId"></param>
		/// <param name="name"></param>
		/// <param name="phone"></param>
		public void UpdateCustomer(int customerId, string name = null, string phone = null)
		{
			if (name != "")
			{
				IDAL.DO.Customer customer = new();
				customer = dal.GetCustomer(customerId);
				customer.Name = name;
				dal.UpdateCustomer(customer);
			}
			if (phone != "")
			{
				IDAL.DO.Customer customer = new();
				customer = dal.GetCustomer(customerId);
				customer.Phone = phone;
				dal.UpdateCustomer(customer);
			}
		}
		/// <summary>
		/// the func send the drone to charge
		/// </summary>
		/// <param name="droneId"></param>
		public void DroneToCharge(int droneId)
		{
			IBL.BO.DroneToList drone = new IBL.BO.DroneToList();
			drone = droneToList[searchDrone(droneId)];
			if (drone.Status != IBL.BO.DroneStatuses.free)
				throw new IBL.BO.DroneNotFree();
			if (drone.Battery < calcBatteryToCloserBase(drone.Loc.Longitude, drone.Loc.Latitude))
				throw new IBL.BO.NotEnoughBattery();
			drone.Battery -= calcBatteryToCloserBase(drone.Loc.Longitude, drone.Loc.Latitude);

			IDAL.DO.BaseStation baseStation = new IDAL.DO.BaseStation();
			baseStation = CloserBase(drone.Loc.Longitude, drone.Loc.Latitude);
			drone.Loc.Longitude = baseStation.Longitude;
			drone.Loc.Latitude = baseStation.Latitude;

			drone.Status = IBL.BO.DroneStatuses.Maintenance;

			for (int i = 0; i < droneToList.Count(); i++)
			{
				if (drone.Id == droneToList[i].Id)
					droneToList[i] = drone;
			}

			dal.AssignDroneToBaseStation(droneId, baseStation.Id);
		}
		/// <summary>
		/// the func release the drone from base station charge
		/// </summary>
		/// <param name="droneId"></param>
		/// <param name="time"></param>
		public void DroneLeaveCharge(int droneId, int time)
		{
			IBL.BO.DroneToList drone = new IBL.BO.DroneToList();
			drone = droneToList[searchDrone(droneId)];
			if (drone.Status != IBL.BO.DroneStatuses.Maintenance)
				throw new IBL.BO.DroneNotMaintenance();
			drone.Status = IBL.BO.DroneStatuses.free;
			drone.Battery += (time / 60) * _chargingRate;
			if (drone.Battery > 100)
				drone.Battery = 100;
			IDAL.DO.BaseStation baseStation = new IDAL.DO.BaseStation();
			baseStation = currentBase(drone.Loc.Longitude, drone.Loc.Latitude);
			dal.DroneLeaveChargeStation(droneId, baseStation.Id);
		}
		/// <summary>
		/// the func associte the parcel to a drone
		/// </summary>
		/// <param name="droneId"></param>
		public void AffectParcelToDrone(int droneId)
		{
			IBL.BO.DroneToList drone = new IBL.BO.DroneToList();
			drone = droneToList[searchDrone(droneId)];

			if (drone.Status != IBL.BO.DroneStatuses.free)
				throw new IBL.BO.DroneNotFree();
			IDAL.DO.Parcel parcel = new IDAL.DO.Parcel() { Id = 0 };
			foreach (IDAL.DO.Parcel item in dal.GetListParcels())
			{
				if (((int)item.Weight <= (int)drone.MaxWeight) && (item.Priority >= parcel.Priority) && ((parcel.Id == 0) ||
					(distanceBetweenTwoPoints(drone.Loc.Latitude, drone.Loc.Longitude, getCustomerLocation(item.SenderId).Latitude, getCustomerLocation(item.SenderId).Longitude)
					< distanceBetweenTwoPoints(drone.Loc.Latitude, drone.Loc.Longitude, getCustomerLocation(parcel.SenderId).Latitude, getCustomerLocation(parcel.SenderId).Longitude))))
				{
					parcel = item;
				}
			}
			if (parcel.Id == 0)
				throw new IBL.BO.NoDroneCanParcel();
			//IBL.BO.ParcelToList parcelToList = new() { Id = parcel.Id, NameSender = dal.GetCustomer(parcel.SenderId).Name, NameTarget = dal.GetCustomer(parcel.TargetId).Name, Weight = (IBL.BO.WeightCategories)((int)parcel.Weight) };
			//if ((drone.Battery - calcBatteryToShipping(drone, parcelToList)) < 0)
			//	drone.IdOfParcel = parcel.Id;
			drone.Status = IBL.BO.DroneStatuses.Shipping;
			updateDroneList(drone);
			parcel.DroneId = drone.Id;
			parcel.Requested = DateTime.Now;
			dal.UpdateParcel(parcel);
		}/////////////////////////
		/// <summary>
		/// collect a parcel to deliver
		/// </summary>
		/// <param name="droneId"></param>
		public void ParcelCollection(int droneId)
		{
			IBL.BO.DroneToList drone = new IBL.BO.DroneToList();
			drone = droneToList[searchDrone(droneId)];
			IDAL.DO.Parcel parcel = new IDAL.DO.Parcel();
			foreach (IDAL.DO.Parcel item in dal.GetListParcels())
			{
				if (item.DroneId == drone.Id)
				{
					parcel = item;
				}
			}
			if(parcel.DroneId!=droneId)
				throw new IBL.BO.NoParcelId();
			if (parcel.PickedUp != DateTime.MinValue)
				throw new IBL.BO.AlreadyPickedUp();
			if (parcel.Requested == DateTime.MinValue)
				throw new IBL.BO.NotRequestedYet();
			parcel.PickedUp = DateTime.Now;
			dal.UpdateParcel(parcel);
			drone.Battery -= calcBatteryUsedWhenEmpty(distanceBetweenTwoPoints(drone.Loc.Latitude, drone.Loc.Longitude, getCustomerLocation(parcel.SenderId).Latitude, getCustomerLocation(parcel.SenderId).Longitude));
			drone.Loc.Longitude = getCustomerLocation(parcel.SenderId).Longitude;
			drone.Loc.Latitude = getCustomerLocation(parcel.SenderId).Latitude;
			updateDroneList(drone);
		}
		/// <summary>
		/// the func updaste that the drone was deliver the parcel
		/// </summary>
		/// <param name="droneId"></param>
		public void ParcelDeliverd(int droneId)
		{
			IBL.BO.DroneToList drone = new IBL.BO.DroneToList();
			drone = droneToList[searchDrone(droneId)];
			IDAL.DO.Parcel parcel = new();
			foreach (IDAL.DO.Parcel item in dal.GetListParcels())
			{
				if (item.DroneId == drone.Id)
				{
					parcel = item;
				}
			}
			if (parcel.DroneId != droneId) 
				throw new IBL.BO.NoParcelId();
			if (parcel.PickedUp == DateTime.MinValue) 
				throw new IBL.BO.NotPickedUpYet();
			if (parcel.Delivered != DateTime.MinValue) 
				throw new IBL.BO.AlreadyDelivered();
			parcel.Delivered = DateTime.Now;
			dal.UpdateParcel(parcel);
			drone.Battery -= calcBatteryUsedWhenShipping(parcel.Weight, distanceBetweenTwoPoints(drone.Loc.Latitude, drone.Loc.Longitude, getCustomerLocation(parcel.TargetId).Latitude, getCustomerLocation(parcel.TargetId).Longitude));
			drone.Loc.Longitude = getCustomerLocation(parcel.TargetId).Longitude;
			drone.Loc.Latitude = getCustomerLocation(parcel.TargetId).Latitude;
			drone.Status = IBL.BO.DroneStatuses.free;
			updateDroneList(drone);
		}

		//--------------------------------------------------------------------------------------------DISPLAY SPECIFIC OBJECT - OPTION------------------------------------------------------
		/// <summary>
		/// the func search a base station in the data base 
		/// </summary>
		/// <param name="baseId"></param>
		/// <returns> an base station </returns>
		public IBL.BO.BaseStation GetBaseStation(int baseId)
		{
			IBL.BO.BaseStation baseS = new IBL.BO.BaseStation();
			IDAL.DO.BaseStation b = new IDAL.DO.BaseStation();

			baseS.Id = b.Id;
			baseS.Name = b.Name;
			baseS.ChargeSlots = b.ChargeSlots;
			baseS.Loc.Latitude = b.Latitude;
			baseS.Loc.Longitude = b.Longitude;

			for (int i = 0; i < droneToList.Count(); i++)
			{
				if (droneToList[i].Status == IBL.BO.DroneStatuses.Maintenance && (droneToList[i].Loc.Latitude == baseS.Loc.Latitude && droneToList[i].Loc.Longitude == baseS.Loc.Longitude))
					baseS.DroneInCharge.Add(new IBL.BO.DroneCharge { DroneId = droneToList[i].Id, Battery = droneToList[i].Battery });
			}
			return baseS;
		}
		/// <summary>
		/// the func search an drone in the data base
		/// </summary>
		/// <param name="droneId"></param>
		/// <returns> an drone </returns>
		public IBL.BO.Drone GetDrone(int droneId)
		{
			IBL.BO.Drone drone = new IBL.BO.Drone();
			for (int i = 0; i < droneToList.Count(); i++)
			{
				if (droneToList[i].Id == droneId)
				{
					drone.Id = droneToList[i].Id;
					drone.Model = droneToList[i].Model;
					drone.MaxWeight = droneToList[i].MaxWeight;
					drone.Status = droneToList[i].Status;
					drone.Battery = droneToList[i].Battery;
					drone.Loc.Longitude = droneToList[i].Loc.Longitude;
					drone.Loc.Latitude = droneToList[i].Loc.Latitude;
				}
			}
			if (drone.Status == IBL.BO.DroneStatuses.Shipping)
			{
				foreach (IDAL.DO.Parcel item in dal.GetListParcels())
				{
					if (item.DroneId == drone.Id)
					{ drone.ParcelInTransit = convertParcel(item, droneId); break; }
				}
				drone.ParcelInTransit.Drone.Id = droneId; drone.ParcelInTransit.Drone.Battery = drone.Battery;
				drone.ParcelInTransit.Drone.Loc.Latitude = drone.Loc.Latitude;
				drone.ParcelInTransit.Drone.Loc.Longitude = drone.Loc.Longitude;
			}
			return drone;
		}
		/// <summary>
		/// the func return a customer
		/// </summary>
		/// <param name="customerId"></param>
		/// <returns></returns>
		public IBL.BO.Customer GetCustomer(int customerId)
		{
			IBL.BO.Customer customer = new IBL.BO.Customer();
			IBL.BO.CustomerInParcel target = new IBL.BO.CustomerInParcel();
			IBL.BO.CustomerInParcel sender = new IBL.BO.CustomerInParcel();
			customer = getCustomer(customerId);
			foreach (IDAL.DO.Parcel item in dal.GetListParcels())
			{
				target.Id = item.TargetId;
				target.Name = getCustomer(item.TargetId).Name;
				if (item.SenderId == customerId)
					customer.ParcelFromCustomer.Add(new IBL.BO.ParcelAtCustomer
					{ Id = item.Id, Weight = (IBL.BO.WeightCategories)item.Weight, Priority = (IBL.BO.Priorities)item.Priority,
						status = getStatusOfParcel(item), SenderOrTraget = target, });
			}
			foreach (IDAL.DO.Parcel item in dal.GetListParcels())
			{
				sender.Id = item.SenderId;
				target.Name = getCustomer(item.SenderId).Name;
				if (item.TargetId == customerId)
					customer.ParcelFromCustomer.Add(new IBL.BO.ParcelAtCustomer
					{
						Id = item.Id, Weight = (IBL.BO.WeightCategories)item.Weight, Priority = (IBL.BO.Priorities)item.Priority,
						status = getStatusOfParcel(item), SenderOrTraget = sender,
					});
			}
			return customer;
		}
		/// <summary>
		/// the func search and return a parcel
		/// </summary>
		/// <param name="parcelId"></param>
		/// <returns> an parcel </returns>
		public IBL.BO.Parcel GetParcel(int parcelId)
		{
			IBL.BO.Parcel parcel = new IBL.BO.Parcel();
			IBL.BO.CustomerInParcel sender = new IBL.BO.CustomerInParcel();
			IBL.BO.CustomerInParcel target = new IBL.BO.CustomerInParcel();
			IDAL.DO.Parcel p = new IDAL.DO.Parcel();

			parcel.Id = p.Id;
			parcel.Weight = (IBL.BO.WeightCategories)p.Weight;
			parcel.Priority = (IBL.BO.Priorities)p.Priority;
			parcel.Requested = p.Requested;
			parcel.Scheduled = p.Scheduled;
			parcel.PickedUp = p.PickedUp;
			parcel.Delivered = p.Delivered;

			sender.Id = p.SenderId;
			sender.Name = getCustomer(sender.Id).Name;
			parcel.Sender = sender;

			target.Id = p.TargetId;
			target.Name = getCustomer(target.Id).Name;
			parcel.Target = target;

			return parcel;
		}
		
		//--------------------------------------------------------------------------------------------DISPLAY LIST OF AN OBJECT - OPTION----------------------------------------------------
		/// <summary>
		/// the func return a list of base station
		/// </summary>
		/// <returns> a list of base station </returns>
		public IEnumerable<List<IBL.BO.BaseToList>> GetListOfBaseStations()
		{
			List<IBL.BO.BaseToList> baseS = new List<IBL.BO.BaseToList>();
			foreach (IDAL.DO.BaseStation item in dal.GetListBaseStations())
			{
				IBL.BO.Location l = new IBL.BO.Location();
				baseS.Add(new IBL.BO.BaseToList { Id = item.Id,Name=item.Name,ChargeSlots=item.ChargeSlots,ChargeBusy= howManyCharge(l) 
				});
			}
			return (IEnumerable<List<IBL.BO.BaseToList>>)baseS;
		}
		/// <summary>
		/// the func return an list of drone
		/// </summary>
		/// <returns> list of drone </returns>
		public IEnumerable<List<IBL.BO.DroneToList>> GetListOfDrone()
		{
			return (IEnumerable<List<IBL.BO.DroneToList>>)droneToList;
		}
		/// <summary>
		/// the func return an list of customer
		/// </summary>
		/// <returns> list of customer </returns>
		public IEnumerable<List<IBL.BO.CustomerToList>> GetListOfCustomer()
		{
			List<IBL.BO.CustomerToList> customer = new List<IBL.BO.CustomerToList>();
			foreach (IDAL.DO.Customer item in dal.GetListCustomers())
			{
				customer.Add(new IBL.BO.CustomerToList {Id=item.Id,Name=item.Name,Phone=item.Phone,ParcelDelivred=parcelWasDliver(item.Id),
				ParcelSentNotDelivred=parcelSentNotDeliver(item.Id),ParcelRecived=parcelRecive(item.Id),ParcelInTransit=parcelInTransit(item.Id)
				});
			}
			return (IEnumerable<List<IBL.BO.CustomerToList>>)customer;
		}
		/// <summary>
		/// the func return a list of parcel 
		/// </summary>
		/// <returns> list of parcel </returns>
		public IEnumerable<List<IBL.BO.ParcelToList>> GetListOfParcel()
		{
			List<IBL.BO.ParcelToList> parcel = new List<IBL.BO.ParcelToList>();
			foreach (IDAL.DO.Parcel item in dal.GetListParcels())
			{
				parcel.Add(new IBL.BO.ParcelToList {Id=item.Id,NameSender=dal.GetCustomer(item.SenderId).Name,NameTarget= dal.GetCustomer(item.TargetId).Name,
				Weight=(IBL.BO.WeightCategories)item.Weight,Priority=(IBL.BO.Priorities)item.Priority,Status=getStatusOfParcel(item)
				});
			}
			return(IEnumerable<List<IBL.BO.ParcelToList>>)parcel;
		}
		/// <summary>
		/// the func return a list of parcel yet not associateed
		/// </summary>
		/// <returns> an list of parcel </returns>
		public IEnumerable<List<IBL.BO.ParcelToList>> GetListParcelNotAssignToDrone()
		{
			List<IBL.BO.ParcelToList> parcel = new List<IBL.BO.ParcelToList>();
			foreach (IDAL.DO.Parcel item in dal.GetListParcels())
			{
				if (item.DroneId == 0)
				{
					parcel.Add(new IBL.BO.ParcelToList
					{
						Id = item.Id,NameSender = dal.GetCustomer(item.SenderId).Name,NameTarget = dal.GetCustomer(item.TargetId).Name,
						Weight = (IBL.BO.WeightCategories)item.Weight,Priority = (IBL.BO.Priorities)item.Priority,
						Status = getStatusOfParcel(item)
					});
				}
			}
			return (IEnumerable<List<IBL.BO.ParcelToList>>)parcel;
		}
		/// <summary>
		/// the func return a list of parcel the have a station of charge free
		/// </summary>
		/// <returns> an list of base station </returns>
		public IEnumerable<List<IBL.BO.BaseToList>> GetListBaseWithChargeSlot()
		{
			List<IBL.BO.BaseToList> baseS = new List<IBL.BO.BaseToList>();
			foreach (IDAL.DO.BaseStation item in dal.GetListBaseStations())
			{
				IBL.BO.Location l = new IBL.BO.Location();
				l.Latitude = item.Latitude; l.Longitude = item.Longitude;
				if (item.ChargeSlots > 0)
					baseS.Add(new IBL.BO.BaseToList { Id = item.Id, Name = item.Name, ChargeSlots = item.ChargeSlots,ChargeBusy=howManyCharge(l) 
					});
			}
			return (IEnumerable<List<IBL.BO.BaseToList>>)baseS;
		}

		//---------------------------------------------------------------------------------------------------------HELP FUNC--------------------------------------------------------------------
		/// <summary>
		/// the func calculate the battery use for each parcel and their weight
		/// </summary>
		/// <param name="weight"></param>
		/// <param name="dist"></param>
		/// <returns> battery in percent </returns>
		private int calcBatteryUsedWhenShipping(IDAL.DO.WeightCategories weight,double dist)
		{
			double batteryUsed=0;
			if (weight == IDAL.DO.WeightCategories.Light) batteryUsed = _useWhenLightly * dist;
			if (weight == IDAL.DO.WeightCategories.Medium) batteryUsed = _useWhenMedium * dist;
			if (weight == IDAL.DO.WeightCategories.Heavy) batteryUsed = _useWhenHeavily * dist;
			return ((int)(Math.Ceiling(batteryUsed)));
		}
		/// <summary>
		/// calculate the battery used when the drone no have parcel to deliver
		/// </summary>
		/// <param name="dist"></param>
		/// <returns> the percent of battery </returns>
		private int calcBatteryUsedWhenEmpty(double dist)
		{
			double batteryUsed = _useWhenFree * dist;
			return ((int)(Math.Ceiling(batteryUsed)));
		}
		/// <summary>
		/// the func update the list of drone "drone to list" for the maintenance the project
		/// </summary>
		/// <param name="drone"></param>
		private void updateDroneList(IBL.BO.DroneToList drone)
		{
			for (int i = 0; i < droneToList.Count(); i++)
			{
				if (droneToList[i].Id == drone.Id) droneToList[i] = drone;
			}
		}
		/// <summary>
		/// the func search who is the current base by his location 
		/// </summary>
		/// <param name="longi"></param>
		/// <param name="lati"></param>
		/// <returns> the current base station </returns>
		private IDAL.DO.BaseStation currentBase(double longi, double lati)
		{
			foreach (IDAL.DO.BaseStation item in dal.GetListBaseStations())
			{
				if (item.Longitude == longi && item.Latitude == lati) return item;
			}
			throw new IBL.BO.DroneNotInBase();
		}
		/// <summary>
		/// the func search a customer by his location
		/// </summary>
		/// <param name="customerId"></param>
		/// <returns> an location </returns>
		private IBL.BO.Location getCustomerLocation(int customerId)
		{
			IBL.BO.Location location = new IBL.BO.Location();
			foreach (IDAL.DO.Customer item in dal.GetListCustomers())
			{
				if (item.Id == customerId)
				{
					location.Longitude = item.Longitude;
					location.Latitude = item.Latitude;
					return location;
				}
			}
			throw new IBL.BO.CustomerIdNotExist();
		}
		/// <summary>
		/// the func calculate who is the closer base by his location
		/// </summary>
		/// <param name="longi"></param>
		/// <param name="lati"></param>
		/// <returns> a base station </returns>
		private IDAL.DO.BaseStation CloserBase(double longi, double lati)
		{
			double disToBase = -1;
			IDAL.DO.BaseStation baseStation = new IDAL.DO.BaseStation();
			foreach (IDAL.DO.BaseStation b in dal.GetListBaseStations())
			{
				double dis = distanceBetweenTwoPoints(b.Latitude, b.Longitude, lati, longi);
				if ((disToBase == -1 || dis < disToBase) && b.ChargeSlots > 0)//if "dis" that is calculate in the previous line is smaller than the "disToBase"= the smaller distanse right now so "disToBase = dis"
				{
					disToBase = dis;
					baseStation = b;
				}
			}
			return baseStation;
		}
		/// <summary>
		/// the func convert a parcel from type idal to ibl include add data for the type ibl
		/// </summary>
		/// <param name="p"></param>
		/// <returns> an parcel </returns>
		private IBL.BO.Parcel convertParcel(IDAL.DO.Parcel p,int droneId)
		{
			IBL.BO.Parcel parcel = new IBL.BO.Parcel();
			IBL.BO.CustomerInParcel customerS = new IBL.BO.CustomerInParcel();
			IBL.BO.CustomerInParcel customerT = new IBL.BO.CustomerInParcel();
			parcel.Id = p.Id;
			parcel.Weight = (IBL.BO.WeightCategories)p.Weight;
			parcel.Priority = (IBL.BO.Priorities)p.Priority;
			parcel.Requested = p.Requested;
			parcel.Scheduled = p.Scheduled;
			parcel.PickedUp = p.PickedUp;
			parcel.Delivered = p.Delivered;

			foreach(IDAL.DO.Customer item in dal.GetListCustomers())
			{
				if (p.SenderId == item.Id)
				{ customerS.Id = item.Id; customerS.Name = item.Name; }
				if (p.TargetId == item.Id)
				{ customerT.Id = item.Id; customerT.Name = item.Name; }
			}
			
			return parcel;
		}
		/// <summary>
		/// the func return the status of the parcel
		/// </summary>
		/// <param name="p"></param>
		/// <returns> status </returns>
		private IBL.BO.ParcelStatues getStatusOfParcel(IDAL.DO.Parcel p)
		{
			IBL.BO.ParcelStatues statues = IBL.BO.ParcelStatues.Defined;
			if (p.Scheduled != DateTime.MinValue)
				statues = IBL.BO.ParcelStatues.Associated;
			if (p.PickedUp != DateTime.MinValue)
				statues = IBL.BO.ParcelStatues.Collected;
			if (p.Delivered != DateTime.MinValue)
				statues = IBL.BO.ParcelStatues.Delivered;

			return statues;
		}
		/// <summary>
		/// the func search where the drone is charge 
		/// </summary>
		/// <param name="d"></param>
		/// <returns> return the index in the list in data base of the base station </returns>
		private IBL.BO.Location whereDroneCharge(IBL.BO.DroneToList d)
		{
			IBL.BO.Location l = new IBL.BO.Location();
			foreach (IDAL.DO.BaseStation item in dal.GetListBaseStations())
			{
				if (item.Longitude == d.Loc.Longitude && item.Latitude == d.Loc.Latitude)
				{ l.Latitude = item.Latitude; l.Longitude = item.Longitude; return l; }
			}
			throw new IBL.BO.DroneNotInCharge();
		}
		/// <summary>
		/// the func serche how many parcel the customer sent and their was deliver
		/// </summary>
		/// <param name="customerId"></param>
		/// <returns> number of parcel </returns>
		private int parcelWasDliver(int customerId)
		{
			int counter = 0;
			foreach (IDAL.DO.Parcel item in dal.GetListParcels())
			{
				if (item.SenderId == customerId && item.Delivered != DateTime.MinValue)
					counter++;
			}
			return counter;
		}
		/// <summary>
		/// the func serche how many parcel the customer sent but yet is not delivered
		/// </summary>
		/// <param name="customerId"></param>
		/// <returns> number of parcel </returns>
		private int parcelSentNotDeliver(int customerId)
		{
			int counter = 0;
			foreach (IDAL.DO.Parcel item in dal.GetListParcels())
			{
				if (item.SenderId == customerId && item.Delivered == DateTime.MinValue&&item.PickedUp!=DateTime.MinValue)
					counter++;
			}
			return counter;
		}
		/// <summary>
		/// the func serche how many parcel the customer recive
		/// </summary>
		/// <param name="customerId"></param>
		/// <returns> number of parcel </returns>
		private int parcelRecive(int customerId)
		{
			int counter = 0;
			foreach (IDAL.DO.Parcel item in dal.GetListParcels())
			{
				if (item.TargetId == customerId && item.Delivered != DateTime.MinValue)
					counter++;
			}
			return counter;
		}
		/// <summary>
		/// the func serche how many parcel the customer need to recive in future(parcel is ion tarnsit)
		/// </summary>
		/// <param name="customerId"></param>
		/// <returns></returns>
		private int parcelInTransit(int customerId)
		{
			int counter = 0;
			foreach (IDAL.DO.Parcel item in dal.GetListParcels())
			{
				if (item.TargetId == customerId && item.Delivered == DateTime.MinValue&&item.PickedUp!=DateTime.MinValue)
					counter++;
			}
			return counter;
		}
		/// <summary>
		/// the func calculate how many drone charge in the base station by his location
		/// </summary>
		/// <param name="loc"></param>
		/// <returns> return a number of drone </returns>
		private int howManyCharge(IBL.BO.Location loc)
		{
			int counter = 0;
			foreach (IBL.BO.DroneToList d in droneToList)
			{
				if (d.Status == IBL.BO.DroneStatuses.Maintenance && loc == whereDroneCharge(d))
					counter++;
						
			}
			return counter;
		}
	}
}