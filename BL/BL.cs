using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{

	public class BL : IBL.IBL
	{
		private IDAL.IDal dal;
		private static BL _instance = null;
		public static BL GetInstance() => _instance ?? (_instance = new BL());

		private double _useWhenFree;
		private double _useWhenLightly;
		private double _useWhenMedium;
		private double _useWhenHeavily;
		private double _chargingRate;
		List<IBL.BO.DroneToList> droneToList = new List<IBL.BO.DroneToList>();
		//ctor
		private BL()
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
			//add the data location of base station in the list
			foreach (IDAL.DO.BaseStation b in dal.GetListBaseStations())
			{
				IBL.BO.Location l = new IBL.BO.Location();
				l.Latitude = b.Latitude; l.Longitude = b.Longitude;
				baseS.Add(new IBL.BO.BaseStation { Loc = l });
			}

			for (int i = 0; i < droneToList.Count(); i++)
			{
				if (droneToList[i].Status != IBL.BO.DroneStatuses.Shipping)//if the drone is not in shipping choos random status
				{
					int r = new Random().Next(1, 3);
					if (r == 1)
						droneToList[i].Status = IBL.BO.DroneStatuses.free;
					else
						droneToList[i].Status = IBL.BO.DroneStatuses.Maintenance;
				}
				if (droneToList[i].Status == IBL.BO.DroneStatuses.Maintenance)//according to the previous "if" now-if the random choose that the drone is in Maintenance
				{
					int r = new Random().Next(0, baseS.Count());//choose randome location 
					droneToList[i].Loc.Longitude = baseS[r].Loc.Longitude;
					droneToList[i].Loc.Latitude = baseS[r].Loc.Latitude;
					r = new Random().Next(0, 21);//choose random percent of battery between 0to20%
					droneToList[i].Battery = r;
					break;
				}
				else if (droneToList[i].Status == IBL.BO.DroneStatuses.free)//according to the previous "if" now-if the random choose that the drone is free
				{
					List<IBL.BO.Customer> customerList = new List<IBL.BO.Customer>();
					foreach (IDAL.DO.Parcel item in dal.GetListParcels())//add data of parcel that was deliver
					{
						if (item.Delivered != DateTime.MinValue)
								customerList.Add(getCustomer(item.TargetId));	
					}
					int r = new Random().Next(0, customerList.Count());//choose a random loc of parcel 
					droneToList[i].Loc.Longitude = customerList[r].Loc.Longitude;
					droneToList[i].Loc.Latitude = customerList[r].Loc.Latitude;

					int percent = calcBatteryToCloserBase(droneToList[i].Loc.Longitude, droneToList[i].Loc.Latitude);//calculate the min percent battery to go back to the base station
					int finalPercent = new Random().Next(percent, 101);//random choose percent between "percent" to 100 
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
					Status = getStatusOfParcel(item)
				});
			}
			for (int i = 0; i < parcelToList.Count(); i++)
			{
				int droneIdAssigneToParcel = searchDroneIdAssigneToParcel(parcelToList[i]);
				if (parcelToList[i].Status != IBL.BO.ParcelStatues.Delivered && droneIdAssigneToParcel > 0)//if the parcel was not deliver and the parcel he has a drone need to the shipping
				{
					droneToList[searchDrone(droneIdAssigneToParcel)].Status = IBL.BO.DroneStatuses.Shipping;

					locOfDroneByParcel(parcelToList[i], droneIdAssigneToParcel);

					int percent = (int)Math.Ceiling(calcBatteryToShipping(droneToList[searchDrone(droneIdAssigneToParcel)], parcelToList[i]));//calculate the percent of battery the drone need to do the shipping 
					int finalPercent = new Random().Next(percent, 101);//choose random the percent between "percent" to 100
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
		private double calcBatteryToShipping(IBL.BO.DroneToList d, IBL.BO.ParcelToList p)
		{
			double disToSenderFromBase = distanceBetweenTwoPoints(d.Loc.Latitude, d.Loc.Longitude, senderLati(p.NameSender), senderLong(p.NameSender)); //one way to the sender from the closer base station possible is equal to "0"
			double disToTarget = distanceBetweenTwoPoints(d.Loc.Latitude, d.Loc.Longitude, targetLati(p.NameTarget), targetLong(p.NameTarget));// + one way
			double disToBaseFromTarget = disToCloserBase(targetLati(p.NameTarget), targetLong(p.NameTarget));// + second way

			double batteryToBaseFromTarget = _useWhenFree * disToBaseFromTarget;//percent of battery the drone need to go back to the base station
			double batteryToSenderFromBase = _useWhenFree * disToSenderFromBase;//percent of battery the drone need to arrive at sender
			double batteryToTargetFromSender;// percent of battery need to go from the sender to the target
			if (p.Weight == IBL.BO.WeightCategories.Light)
				batteryToTargetFromSender = _useWhenLightly * disToTarget;
			else if (p.Weight == IBL.BO.WeightCategories.Medium)
				batteryToTargetFromSender = _useWhenMedium * disToTarget;
			else
				batteryToTargetFromSender = _useWhenHeavily * disToTarget;
			double finalBattery = batteryToBaseFromTarget + batteryToTargetFromSender + batteryToSenderFromBase;
			if (finalBattery > 100) throw new IBL.BO.DistanceTooBigger();
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
			throw new IBL.BO.IdNotExist("DRONE");
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
				if (item.Id == p.Id)//if the drone is assciated to the parcel
					return item.DroneId;
			}
			return 0;
		}
		/// <summary>
		/// the func locate the drone according to his status 
		/// </summary>
		/// <param name="p"></param>
		/// <param name="droneId"></param>
		private void locOfDroneByParcel(IBL.BO.ParcelToList p, int droneId)
		{
			if (p.Status ==IBL.BO.ParcelStatues.Associated)//if the parcel was Associated so the loc of the drone is in the close base station
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
			else if (p.Status == IBL.BO.ParcelStatues.Collected)//if the parcel was collected by the drone so the loc of drone is where the sender is 
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
			try
			{
				IBL.BO.Customer customer = new IBL.BO.Customer();
				IDAL.DO.Customer c = new IDAL.DO.Customer();
				c = dal.GetCustomer(id);
				customer.Id = c.Id;
				customer.Name = c.Name;
				customer.Phone = c.Phone;
				customer.Loc.Latitude = c.Latitude;
				customer.Loc.Longitude = c.Longitude;
				return customer;
			}
			catch(IDAL.DO.CustomerIdNotExist ex) 
			{
				throw new IBL.BO.IdNotExist(ex.Message,"CUSTOMER");
			}	
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
			{ throw new IBL.BO.InvalidId(ex.Message,"BASE"); }

			catch (IDAL.DO.InvalidChargeSlot ex)
			{ throw new IBL.BO.InvalidChargeSlot(ex.Message); }

			catch (IDAL.DO.InvalidLatitude ex)
			{ throw new IBL.BO.InvalidLoc(ex.Message,"LATITUDE", "-90 TO 90"); }

			catch (IDAL.DO.InvalidLongitude ex)
			{ throw new IBL.BO.InvalidLoc(ex.Message,"LONGITUDE","-180 TO 180"); }

			catch (IDAL.DO.BaseIdExist ex)
			{ throw new IBL.BO.IdExist(ex.Message,"BASE"); }
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
			{ throw new IBL.BO.InvalidId(ex.Message,"DRONE"); }

			catch (IDAL.DO.InvalidWeight ex)
			{ throw new IBL.BO.InvalidCategory(ex.Message, "WEIGHT"); }

			catch (IDAL.DO.DroneIdExist ex)
			{ throw new IBL.BO.IdExist(ex.Message,"DRONE"); }

			try
			{ dal.AssignDroneToBaseStation(id, firstBaseStation); }
			catch (IDAL.DO.DroneIdNotExist ex)
			{ throw new IBL.BO.IdNotExist(ex.Message,"DRONE"); }
			catch (IDAL.DO.BaseIdNotExist ex)
			{ throw new IBL.BO.IdNotExist(ex.Message,"BASE"); }

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
			{ throw new IBL.BO.IdNotExist("DRONE"); }
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
			{ throw new IBL.BO.InvalidId(ex.Message,"CUSTOMER"); }

			catch (IDAL.DO.InvalidLatitude ex)
			{ throw new IBL.BO.InvalidLoc(ex.Message,"LATITUDE","-90 TO 90"); }

			catch (IDAL.DO.InvalidLongitude ex)
			{ throw new IBL.BO.InvalidLoc(ex.Message,"LONGITUDE","-180 TO 180"); }

			catch (IDAL.DO.CustomerIdExist ex)
			{ throw new IBL.BO.IdExist(ex.Message,"CUSTOMER"); }
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
			{ throw new IBL.BO.InvalidId(ex.Message,"PARCEL"); }

			catch (IDAL.DO.InvalidSenderId ex)
			{ throw new IBL.BO.InvalidId(ex.Message,"SENDER");
			}

			catch (IDAL.DO.InvalidTargetId ex)
			{ throw new IBL.BO.InvalidId(ex.Message,"TARGET");
			}

			catch (IDAL.DO.NegativeDroneId ex)
			{ throw new IBL.BO.InvalidId(ex.Message,"DRONE");
			}

			catch (IDAL.DO.InvalidWeight ex)
			{ throw new IBL.BO.InvalidCategory(ex.Message, "WEIGHT"); }

			catch (IDAL.DO.InvalidPriority ex)
			{ throw new IBL.BO.InvalidCategory(ex.Message, "PRIORITIES");
			}

			catch (IDAL.DO.ParcelIdExist ex)
			{ throw new IBL.BO.IdExist(ex.Message,"PARCEL");
			}

			catch (IDAL.DO.SenderIdNotExist ex)
			{ throw new IBL.BO.IdNotExist(ex.Message,"SENDER"); }

			catch (IDAL.DO.TargetIdNotExist ex)
			{ throw new IBL.BO.IdNotExist(ex.Message,"TARGET"); }
		}

		//-----------------------------------------------------------------------------------------------------UPDATE - OPTION-------------------------------------------------------------------------------------------------------
		/// <summary>
		/// the func update the name of model drone
		/// </summary>
		/// <param name="droneId"></param>
		/// <param name="model"></param>
		public void UpdateDrone(int droneId, string model)
		{
			try
			{
				IDAL.DO.Drone drone = new IDAL.DO.Drone();
				drone = dal.GetDrone(droneId);
				drone.Model = model;
				dal.UpdateDrone(drone);
			}
			catch (IDAL.DO.DroneIdNotExist ex)
			{ throw new IBL.BO.IdNotExist(ex.Message,"DRONE"); }
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
			try
			{ baseStation = dal.GetBaseStation(baseId); }
			catch (IDAL.DO.DroneIdNotExist ex)
			{ throw new IBL.BO.IdNotExist(ex.Message,"BASE"); }

			int name_int, charge_slots;
			if (name != "")
			{
				name_int = int.Parse(name);
				baseStation = dal.GetBaseStation(baseId);
				baseStation.Name = name_int;
				dal.UpdateBaseStation(baseStation);
			}
			if (chargeslots != "")
			{
				charge_slots = int.Parse(chargeslots);
				baseStation.ChargeSlots = charge_slots;
				dal.UpdateBaseStation(baseStation);
			}
		}
		/// <summary>
		/// the func update the data of customer
		/// </summary>
		/// <param name="customerId"></param>
		/// <param name="name"></param>
		/// <param name="phone"></param>
		public void UpdateCustomer(int customerId, string name = null, string phone = null)
		{
			IDAL.DO.Customer customer = new IDAL.DO.Customer();
			try
			{ customer = dal.GetCustomer(customerId); }
			catch(IDAL.DO.CustomerIdNotExist ex)
			{ throw new IBL.BO.IdExist(ex.Message,"CUSTOMER"); }
			if (name != "")
			{
				
				customer = dal.GetCustomer(customerId);
				customer.Name = name;
				dal.UpdateCustomer(customer);
			}
			if (phone != "")
			{
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
			if (drone.Status != IBL.BO.DroneStatuses.free)//the drone is not free so he cant go to charge
				throw new IBL.BO.DroneNotFree();
			if (drone.Battery < calcBatteryToCloserBase(drone.Loc.Longitude, drone.Loc.Latitude))//no enough battery to go to charge station
				throw new IBL.BO.NotEnoughBattery();
			drone.Battery -= calcBatteryToCloserBase(drone.Loc.Longitude, drone.Loc.Latitude);

			IDAL.DO.BaseStation baseStation = new IDAL.DO.BaseStation();
			baseStation = CloserBase(drone.Loc.Longitude, drone.Loc.Latitude);//search the closer base 
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
			if (drone.Status != IBL.BO.DroneStatuses.Maintenance)//if the drone id of drone that the user gave is not in Maintenance so need to throw that
				throw new IBL.BO.DroneNotInCharge();
			drone.Status = IBL.BO.DroneStatuses.free;
			drone.Battery += (time / 60) * _chargingRate;
			if (drone.Battery > 100)
				drone.Battery = 100;
			IDAL.DO.BaseStation baseStation = new IDAL.DO.BaseStation();
			baseStation = currentBase(drone.Loc.Longitude, drone.Loc.Latitude);
			dal.DroneLeaveChargeStation(droneId, baseStation.Id);//base station- charge slot++ and remove the drone from the list "dron charge"
		}
		/// <summary>
		/// the func associte the parcel to a drone
		/// </summary>
		/// <param name="droneId"></param>
		public int AffectParcelToDrone(int droneId)
		{
			int parcelId;
			IBL.BO.DroneToList drone = new IBL.BO.DroneToList();
			drone = droneToList[searchDrone(droneId)];//O(n)
			if (drone.Status != IBL.BO.DroneStatuses.free)
				throw new IBL.BO.DroneNotFree();

			IDAL.DO.Parcel p = new IDAL.DO.Parcel();
			List<IDAL.DO.Parcel> parcel = new List<IDAL.DO.Parcel>();
			foreach (IDAL.DO.Parcel item in dal.GetListOfParcelsNotAssignedToDrone())//O(n)
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
			if (parcel.PickedUp != DateTime.MinValue)//if parcel pick-up
				throw new IBL.BO.AlreadyPickedUp();
			if (parcel.Scheduled == DateTime.MinValue)// if parcel not associate to a drone 
				throw new IBL.BO.NotScheduledYet();
			parcel.PickedUp = DateTime.Now;
			dal.UpdateParcel(parcel);
			drone.Battery -= calcBatteryUsedWhenEmpty(distanceBetweenTwoPoints(drone.Loc.Latitude, drone.Loc.Longitude, getCustomerLocation(parcel.SenderId).Latitude, getCustomerLocation(parcel.SenderId).Longitude));
			drone.Loc.Longitude = getCustomerLocation(parcel.SenderId).Longitude;//update the location to the sender
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
			IDAL.DO.Parcel parcel = new IDAL.DO.Parcel();
			foreach (IDAL.DO.Parcel item in dal.GetListParcels())
			{
				if (item.DroneId == drone.Id)
				{
					parcel = item;
				}
			}
			if (parcel.DroneId != droneId) 
				throw new IBL.BO.NoParcelId();
			if (parcel.PickedUp == DateTime.MinValue) //if parcel not pick-up
				throw new IBL.BO.NotPickedUpYet();
			if (parcel.Delivered != DateTime.MinValue) //if parcel was deliver
				throw new IBL.BO.AlreadyDelivered();
			parcel.Delivered = DateTime.Now;
			dal.UpdateParcel(parcel);
			drone.Battery -= calcBatteryUsedWhenShipping(parcel.Weight, distanceBetweenTwoPoints(drone.Loc.Latitude, drone.Loc.Longitude, getCustomerLocation(parcel.TargetId).Latitude, getCustomerLocation(parcel.TargetId).Longitude));
			drone.Loc.Longitude = getCustomerLocation(parcel.TargetId).Longitude;//update the location to the target
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
			try
			{ b = dal.GetBaseStation(baseId); }
			catch (IDAL.DO.BaseIdNotExist ex)
			{ throw new IBL.BO.IdNotExist(ex.Message,"BASE"); }

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
			try
			{ dal.GetDrone(droneId); }
			catch (IDAL.DO.DroneIdNotExist ex)
			{throw new IBL.BO.IdNotExist(ex.Message,"DRONE"); }
			for (int i = 0; i < droneToList.Count(); i++)//get all drone
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
			if (drone.Status == IBL.BO.DroneStatuses.Shipping)//if drone is in shipping searche who is the parcel
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
				if (item.SenderId == customerId)//if the customer is the sender
					customer.ParcelFromCustomer.Add(new IBL.BO.ParcelAtCustomer
					{ Id = item.Id, Weight = (IBL.BO.WeightCategories)item.Weight, Priority = (IBL.BO.Priorities)item.Priority,
						status = getStatusOfParcel(item), SenderOrTraget = target, });
			}
			foreach (IDAL.DO.Parcel item in dal.GetListParcels())
			{
				sender.Id = item.SenderId;
				target.Name = getCustomer(item.SenderId).Name;
				if (item.TargetId == customerId)//if the customer is the target
					customer.ParcelToCustomer.Add(new IBL.BO.ParcelAtCustomer
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
			try
			{ p = dal.GetParcel(parcelId); }
			catch (IDAL.DO.ParcelIdNotExist ex) 
			{ throw new IBL.BO.IdNotExist(ex.Message,"PARCEL"); }

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
			throw new IBL.BO.IdNotExist("CUSTOMER");
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

			foreach(IDAL.DO.Customer item in dal.GetListCustomers())//search who is the sender and who is the target
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
		/// <summary>
		/// remove all parcel that their priority status is not equal to param="pri"
		/// </summary>
		/// <param name="p"></param>
		/// <param name="pri"></param>
		/// <returns> an list </returns>
		private List<IDAL.DO.Parcel> removeByPriority(List<IDAL.DO.Parcel> p,IDAL.DO.Priorities pri)
		{
			List<IDAL.DO.Parcel> parcel = p;
			for (int i = 0; i < parcel.Count(); i++)
			{
				if (parcel[i].Priority == pri)
					parcel.RemoveAt(i);
			}
			return parcel;
		}
		/// <summary>
		/// remove all parcel that the drone can't take
		/// </summary>
		/// <param name="p"></param>
		/// <param name="w"></param>
		/// <returns> an lisrt </returns>
		private List<IDAL.DO.Parcel> removeByWeight(List<IDAL.DO.Parcel> p, IDAL.DO.WeightCategories w)
		{
			List<IDAL.DO.Parcel> parcel = p;
			if (w == IDAL.DO.WeightCategories.Heavy)//if max weight of drone can take is heavy parcel so he can take everything 
				return p;
			else if (w == IDAL.DO.WeightCategories.Medium)//if max weight of drone can take is medium parcel so all parcel that heavy he can't takes
			{
				for (int i = 0; i < parcel.Count(); i++)
				{
					if (parcel[i].Weight == IDAL.DO.WeightCategories.Heavy)
						parcel.RemoveAt(i);
				}
			}
			else//if max weight of drone can take is light parcel so all parcel that heavy and medium he can't takes
			{
				for (int i = 0; i < parcel.Count(); i++)
				{
					if (parcel[i].Weight == IDAL.DO.WeightCategories.Heavy|| parcel[i].Weight == IDAL.DO.WeightCategories.Medium)
						parcel.RemoveAt(i);
				}
			}
			return parcel;
		}
		/// <summary>
		/// the func calculate by priorities and battery whiche parcel the drone can take and if it is possible return the id of parcel
		/// </summary>
		/// <param name="p"></param>
		/// <param name="drone"></param>
		/// <returns> id of parcel </returns>
		private int chooseParcel(List<IDAL.DO.Parcel> p, IBL.BO.DroneToList drone)
		{
			List<IBL.BO.ParcelToList> parcelA = new List<IBL.BO.ParcelToList>();//A,B,C IS PRIORITIES
			List<IBL.BO.ParcelToList> parcelB = new List<IBL.BO.ParcelToList>();
			List<IBL.BO.ParcelToList> parcelC = new List<IBL.BO.ParcelToList>();
			for (int i = 0; i < p.Count(); i++)
			{
				//if the next parcel is equal to maxWeight
				if (p[i].Weight == (IDAL.DO.WeightCategories)drone.MaxWeight)
				{
					parcelA.Add(new IBL.BO.ParcelToList
					{
						Id = p[i].Id,
						NameSender = GetCustomer(p[i].SenderId).Name,
						NameTarget = GetCustomer(p[i].TargetId).Name,
						Weight = (IBL.BO.WeightCategories)p[i].Weight
					});
				}
				//if the next parcel is equal to maxWeight -1 but not 0 (according the enum)
				else if ((p[i].Weight == (IDAL.DO.WeightCategories)((int)drone.MaxWeight) - 1) && ((int)(drone.MaxWeight - 1) != 0))
				{
					parcelB.Add(new IBL.BO.ParcelToList
					{
						Id = p[i].Id,
						NameSender = GetCustomer(p[i].SenderId).Name,
						NameTarget = GetCustomer(p[i].TargetId).Name,
						Weight = (IBL.BO.WeightCategories)p[i].Weight
					});
				}
				// if the next parcel is equal to maxWeight - 2 but not 0(according the enum)
				else if ((p[i].Weight == (IDAL.DO.WeightCategories)((int)drone.MaxWeight) - 2) && ((int)(drone.MaxWeight - 2) != 0))
				{
					parcelC.Add(new IBL.BO.ParcelToList
					{
						Id = p[i].Id,
						NameSender = GetCustomer(p[i].SenderId).Name,
						NameTarget = GetCustomer(p[i].TargetId).Name,
						Weight = (IBL.BO.WeightCategories)p[i].Weight
					});
				}

				//IBL.BO.Location loc = new IBL.BO.Location { Longitude = GetCustomer(p[i].SenderId).Loc.Longitude, Latitude = GetCustomer(p[i].SenderId).Loc.Latitude };
				//double x = distanceBetweenTwoPoints(drone.Loc.Latitude, drone.Loc.Longitude,loc.Latitude ,loc.Longitude );//the distance between drone and the parcel

					//if (parcel.Weight == drone.MaxWeight && (x < distance || distance == -1))//
					//	distance = x;
					//else if ((parcel.Weight == (IBL.BO.WeightCategories)((int)drone.MaxWeight) - 1) && ((int)drone.MaxWeight != 0))//if the next parcel is equal to maxWeight -1 but not 0 (according the enum)
					//{
					//	int battery = 
					//}
					//else if ((parcel.Weight == (IBL.BO.WeightCategories)((int)drone.MaxWeight) - 2) && ((int)drone.MaxWeight != 0))//if the next parcel is equal to maxWeight -2 but not 0 (according the enum)
					//{

					//}
			}
			int idA = closerParcel(parcelA, drone);
			if (calcBatteryToShipping(drone, parcelA.Find(item => (item.Id == idA))) >= drone.Battery)//if the closer parcel in list A the drone can do the shipping
				return idA;
			int idB = closerParcel(parcelB, drone);
			if (calcBatteryToShipping(drone, parcelA.Find(item => (item.Id == idB))) >= drone.Battery)//if the closer parcel in list B the drone can do the shipping
				return idB;
			int idC = closerParcel(parcelC, drone);
			if (calcBatteryToShipping(drone, parcelA.Find(item => (item.Id == idC))) >= drone.Battery)//if the closer parcel in list C the drone can do the shipping
				return idC;
			return -1;//if the drone can take any parcel return -1
		}
		/// <summary>
		/// check the weight of parcel and return the battery use per kilometer for thei parcel
		/// </summary>
		/// <param name="parcel"></param>
		/// <returns> battery use </returns>
		private double whichUse(IDAL.DO.Parcel parcel)
		{
			if (parcel.Weight == IDAL.DO.WeightCategories.Heavy)
				return _useWhenHeavily;
			else if (parcel.Weight == IDAL.DO.WeightCategories.Light)
				return _useWhenMedium;
			else
				return _useWhenLightly;
		}
		/// <summary>
		/// calculate the closer parcel from drone
		/// </summary>
		/// <param name="parcel"></param>
		/// <returns> the id of parcel </returns>
		private int closerParcel(List<IBL.BO.ParcelToList> parcel, IBL.BO.DroneToList drone)
		{
			double disToParcel = -1;
			IBL.BO.ParcelToList p = new IBL.BO.ParcelToList();
			foreach (IBL.BO.ParcelToList item in parcel)
			{
				IBL.BO.Location locParcel = new IBL.BO.Location { Longitude =senderLong(item.NameSender) , Latitude = senderLati(item.NameSender) };
				double dis = distanceBetweenTwoPoints(locParcel.Latitude, locParcel.Longitude, drone.Loc.Latitude,drone.Loc.Longitude);
				if (disToParcel == -1 || dis < disToParcel)//if "dis" that is calculate in the previous line is smaller than the "disToParcel"= the smaller distanse right now so "disToParcel = dis"
				{ disToParcel = dis; p = item; }
			}
			return p.Id;
		}
	}
}