﻿using System;
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
				droneToList.Add(new IBL.BO.DroneToList
				{
					Id = item.Id,
					Model = item.Model,
					MaxWeight = (IBL.BO.WeightCategories)item.MaxWeight,
					Battery = 0,
					Status = IBL.BO.DroneStatuses.free,
					Latitude = 0,
					Longitude = 0,
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
				baseS.Add(new IBL.BO.BaseStation { Latitude = b.Latitude, Longitude = b.Longitude });
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
					droneToList[i].Longitude = baseS[r].Longitude;
					droneToList[i].Latitude = baseS[r].Latitude;
					r = new Random().Next(0, 21);
					droneToList[i].Battery = r;
					break;
				}
				else if (droneToList[i].Status == IBL.BO.DroneStatuses.free)
				{
					List<IBL.BO.Customer> customerList = new List<IBL.BO.Customer>();
					foreach (IDAL.DO.Parcel item in dal.GetListParcels())
					{
						if (item.Delivered >= DateTime.Now)
						{
							customerList.Add(getCustomer(item.SenderId));
						}
					}
					int r = new Random().Next(0, customerList.Count());
					droneToList[i].Longitude = customerList[r].Longitude;
					droneToList[i].Latitude = customerList[r].Latitude;

					int percent = calcBatteryToCloserBase(droneToList[i].Longitude, droneToList[i].Latitude);
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
			double disToTarget = distanceBetweenTwoPoints(d.Latitude, d.Longitude, targetLati(p.NameTarget), targetLong(p.NameTarget));
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
		/// <returns>r customer </returns>
		private IBL.BO.Customer getCustomer(int id)
		{
			foreach (IDAL.DO.Customer item in dal.GetListCustomers())
			{
				if (item.Id == id)
				{
					IBL.BO.Customer c = new IBL.BO.Customer { Id = item.Id, Name = item.Name, Latitude = item.Latitude, Longitude = item.Longitude };
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
				if ((disToBase == -1 || dis < disToBase) &&	b.ChargeSlots>0)//if "dis" that is calculate in the previous line is smaller than the "disToBase"= the smaller distanse right now so "disToBase = dis"
					disToBase = dis;

			}
			return disToBase;
		}
		//------------------------------------------------------------------------------------------------------ADD OPTION------------------------------------------------------------------------------------------------
		public static Random r = new Random();
		/// <summary>
		/// 
		/// </summary>
		/// <param name="id"></param>
		/// <param name="name"></param>
		/// <param name="chargeSlots"></param>
		/// <param name="location"></param>
		public void AddBaseStation(int id, int name, int chargeSlots, IBL.BO.Location location)
		{
			dal.AddBaseStation(id, name, chargeSlots, location.Longitude, location.Latitude);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="id"></param>
		/// <param name="model"></param>
		/// <param name="weight"></param>
		/// <param name="firstBaseStation"></param>
		public void AddDrone(int id, string model, IDAL.DO.WeightCategories weight, int firstBaseStation)
		{
			dal.AddDrone(id, model, weight);
			dal.AssignDroneToBaseStation(id, firstBaseStation);
			double battery = (r.Next() % 20) + 20;
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
			{ throw new IDAL.DO.DroneIdNotExist(); }
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="id"></param>
		/// <param name="name"></param>
		/// <param name="phone"></param>
		/// <param name="location"></param>
		public void AddCustomer(int id, string name, string phone, IBL.BO.Location location)
		{
			dal.AddCustomer(id, name, phone, location.Longitude, location.Latitude);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="id"></param>
		/// <param name="senderId"></param>
		/// <param name="targetId"></param>
		/// <param name="weight"></param>
		/// <param name="priorities"></param>
		public void AddParcel(int id, int senderId, int targetId, IDAL.DO.WeightCategories weight, IDAL.DO.Priorities priorities)
		{
			dal.AddParcel(id, senderId, targetId, 0, weight, priorities);
			//ב-BL כל הזמנים יאותחלו לזמן אפס למעט תאריך יצירה שיאותחל ל-DateTime.Now
		}
		//-----------------------------------------------------------------------------------------------------------UPDATE OPTION-------------------------------------------------------------------------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// <param name="droneId"></param>
		/// <param name="model"></param>
		public void UpdateDrone(int droneId, string model)
        {
			IDAL.DO.Drone drone = new();
			drone =dal.GetDrone(droneId);
			drone.Model = model;
			dal.UpdateDrone(drone);
		}
		public void UpdateBaseStation(int baseId,string name=null,string chargeslots = null)
        {
			int name_int, charge_slots;
			if (name != "")
			{
				name_int = int.Parse(name);
				IDAL.DO.BaseStation baseStation = new();
				baseStation = dal.GetBaseStation(baseId);
				baseStation.Name = name_int;
				dal.UpdateBaseStation(baseStation);
			}
			if (chargeslots != "")
			{
				charge_slots = int.Parse(chargeslots);
				dal.UpdateChargeSlots(baseId, charge_slots);
			}
		}
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
		public void DroneToCharge(int droneId)
        {
			IBL.BO.DroneToList drone = new();
			drone = droneToList[searchDrone(droneId)];
			if (drone.Status != IBL.BO.DroneStatuses.free) throw new IBL.BO.DroneNotFree();
			if (drone.Battery < calcBatteryToCloserBase(drone.Longitude, drone.Latitude)) throw new IBL.BO.NotEnoughBattery();
			drone.Battery -= calcBatteryToCloserBase(drone.Longitude, drone.Latitude);
			IDAL.DO.BaseStation baseStation = new();
			baseStation =CloserBase(drone.Longitude, drone.Latitude);
			drone.Longitude = baseStation.Longitude;
			drone.Latitude = baseStation.Latitude;
			drone.Status = IBL.BO.DroneStatuses.Maintenance;
			for(int i = 0; i < droneToList.Count(); i++)
            {
				if (drone.Id == droneToList[i].Id) droneToList[i] = drone;
            }
			dal.AssignDroneToBaseStation(droneId, baseStation.Id);
		}

		private IDAL.DO.BaseStation CloserBase(double longi, double lati)
        {
			double disToBase = -1;
			IDAL.DO.BaseStation baseStation=new();
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

		public void DroneLeaveCharge(int droneId,int time)
        {
			IBL.BO.DroneToList drone = new();
			drone = droneToList[searchDrone(droneId)];
			if (drone.Status != IBL.BO.DroneStatuses.Maintenance) throw new IBL.BO.DroneNotMaintenance();
			drone.Status = IBL.BO.DroneStatuses.free;
			drone.Battery += (time / 60) * 40;
			if (drone.Battery > 100) drone.Battery = 100;
			IDAL.DO.BaseStation baseStation = new();
			baseStation = currentBase(drone.Longitude, drone.Latitude);
			dal.DroneLeaveChargeStation(droneId, baseStation.Id);
		}
		private IDAL.DO.BaseStation currentBase(double longi,double lati)
        {
			foreach (IDAL.DO.BaseStation item in dal.GetListBaseStations())
            {
				if (item.Longitude == longi && item.Latitude == lati) return item;
            }
			throw new IBL.BO.DroneNotInBase();
		}
		public void AffectParcelToDrone(int droneId)
        {
			IBL.BO.DroneToList drone = new();
			drone = droneToList[searchDrone(droneId)];
			if (drone.Status != IBL.BO.DroneStatuses.free) throw new IBL.BO.DroneNotFree();
			IDAL.DO.Parcel parcel = new() {Id=0 };
			foreach (IDAL.DO.Parcel item in dal.GetListParcels())
            {
				if(((int)item.Weight<=(int)drone.MaxWeight)&&(item.Priority >= parcel.Priority )&& ((  parcel.Id == 0) ||
					(distanceBetweenTwoPoints(drone.Latitude,drone.Longitude,getSenderLocation(item.SenderId).Latitude, getSenderLocation(item.SenderId).Longitude)
					< distanceBetweenTwoPoints(drone.Latitude, drone.Longitude, getSenderLocation(parcel.SenderId).Latitude, getSenderLocation(parcel.SenderId).Longitude))))
                {					
					parcel = item;
                }
			}
			if (parcel.Id == 0) throw new IBL.BO.NoDroneCanParcel();
			IBL.BO.ParcelToList parcelToList = new() { Id = parcel.Id, NameSender = dal.GetCustomer(parcel.SenderId).Name, NameTarget = dal.GetCustomer(parcel.TargetId).Name, Weight = (IBL.BO.WeightCategories)((int)parcel.Weight) };
			if ((drone.Battery -calcBatteryToShipping(drone,parcelToList))<0)
			drone.IdOfParcel = parcel.Id;
			drone.Status = IBL.BO.DroneStatuses.Shipping;
			updateDroneList(drone);
			parcel.DroneId = drone.Id;
			parcel.Requested = DateTime.Now;
			dal.UpdateParcel(parcel);
		}
		private IBL.BO.Location getSenderLocation(int senderId)
        {
			IBL.BO.Location location = new();
			foreach (IDAL.DO.Customer item in dal.GetListCustomers())
            {
				if(item.Id == senderId)
                {
					location.Longitude = item.Longitude;
					location.Latitude = item.Latitude;
					return location;
                }
            }
			throw new IBL.BO.CustomerIdNotExist();

		}
		private void updateDroneList(IBL.BO.DroneToList drone)
		{
			for (int i = 0; i < droneToList.Count(); i++)
			{
				if (droneToList[i].Id == drone.Id) droneToList[i] = drone;
			}
		}
		public void ParcelCollection(int droneId)
        {
			IBL.BO.DroneToList drone = new();
			drone = droneToList[searchDrone(droneId)];
			if (drone.IdOfParcel == 0) throw new IBL.BO.NoParcelId();
			IDAL.DO.Parcel parcel = new();
			foreach (IDAL.DO.Parcel item in dal.GetListParcels())
            {
                if (item.Id == drone.IdOfParcel)
                {
					parcel = item;
                }
            }
			if (parcel.PickedUp != DateTime.MinValue) throw new IBL.BO.AlreadyPickedUp();
			if (parcel.Requested == DateTime.MinValue) throw new IBL.BO.NotRequestedYet();
			parcel.PickedUp = DateTime.Now;
			dal.UpdateParcel(parcel);
			drone.Battery -= calcBatteryUsedWhenEmpty(distanceBetweenTwoPoints(drone.Latitude, drone.Longitude, getSenderLocation(parcel.SenderId).Latitude, getSenderLocation(parcel.SenderId).Longitude));
			drone.Longitude = getSenderLocation(parcel.SenderId).Longitude;
			drone.Latitude = getSenderLocation(parcel.SenderId).Latitude;
			updateDroneList(drone);
		}
		private int calcBatteryUsedWhenEmpty(double dist)
        {
			double batteryUsed = _useWhenFree * dist;
			return ((int)(Math.Ceiling(batteryUsed)));
		}
	}
}
