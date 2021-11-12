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
		//private = new DalObject.DalObject();

		private double _useWhenFree;
		private double _useWhenLightly;
		private double _useWhenMedium;
		private double _useWhenHeavily;
		private double _chargingRate;
		List<IBL.BO.DroneToList> droneToList = new List<IBL.BO.DroneToList>();
		//ctor
		BL()
		{
			GetDataCharge();
			ReqListOfDrone();
			settingDroneByParcel();
			settingDrone();
		}
		//----------------------------------------------------------------------------METHOD FOR THE CTOR--------------------------------------------------------------------------------------------
		/// <summary>
		/// the func get the data of charge and use battery for each situation(free,light packege...)
		/// </summary>
		private void GetDataCharge()
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
		private void ReqListOfDrone()
		{
			foreach (IDAL.DO.Drone item in dal.GetListDrones())
			{
				droneToList.Add(new IBL.BO.DroneToList { Id = item.Id, Model = item.Model, MaxWeight = (IBL.BO.WeightCategories)item.MaxWeight, Battery = 0,
					Status = IBL.BO.DroneStatuses.free,Latitude=0,Longitude=0,IdOfParcel=0});
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
					int r = new Random().Next(1, 2);
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
					//DistanceBetweenTwoPoints(,,,)
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
				int droneIdAssigneToParcel = SearchDroneIdAssigneToParcel(parcelToList[i]);
				if (parcelToList[i].Status != IBL.BO.ParcelStatues.Delivered && droneIdAssigneToParcel > 0)
				{
					droneToList[SearchDrone(droneIdAssigneToParcel)].Status = IBL.BO.DroneStatuses.Shipping;
					LocOfDrone(parcelToList[i], droneIdAssigneToParcel);
				}
			}
		}
		//--------------------------------------------------------------------FUNC TO HELP THE METHOD OF THE CTOR---------------------------------------------------------------------------
		/// <summary>
		/// the func calculate the distance between two points for examaple: the distance between the target and the closer base station 
		/// </summary>
		/// <param name="latA"></param>
		/// <param name="lonA"></param>
		/// <param name="latB"></param>
		/// <param name="lonB"></param>
		/// <returns> the distance </returns>
		private double DistanceBetweenTwoPoints(double latA, double lonA, double latB, double lonB)
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
		private int calcBattery(IBL.BO.DroneToList d, IBL.BO.ParcelToList p)
		{
			double disToTarget = DistanceBetweenTwoPoints(d.Latitude, d.Longitude, TargetLati(p.NameTarget), TargetLong(p.NameTarget));
			double disToBase =-1;
			foreach (IDAL.DO.BaseStation b in dal.GetListBaseStations())
			{
				double dis = DistanceBetweenTwoPoints(b.Latitude, b.Longitude, TargetLati(p.NameTarget), TargetLong(p.NameTarget));
				if (disToBase == -1 || dis < disToBase)//if "dis" that is calculate in the previous line is smaller than the "disToBase"= the smaller distanse right now so "disToBase = dis"
					disToBase = dis;

			}
			double batteryToBase = _useWhenFree * disToBase;
			double batteryToSender;
			if (p.Weight == IBL.BO.WeightCategories.Light)
				batteryToSender = _useWhenLightly * disToTarget;
			else if(p.Weight == IBL.BO.WeightCategories.Medium)
				batteryToSender = _useWhenMedium * disToTarget;
			else
				batteryToSender = _useWhenHeavily * disToTarget;
			int finalBattery = (int)(Math.Ceiling(batteryToBase) + Math.Ceiling(batteryToSender));
			return finalBattery;
		}
		/// <summary>
		/// the func search the longitude of the sender by her name in the data base
		/// </summary>
		/// <param name="name"></param>
		/// <returns> the longitude </returns>
		private double SenderLong(string name)
		{
			foreach (IDAL.DO.Customer item in dal.GetListCustomers())
			{
				if (item.Name == name)
					return item.Longitude;
			}
			throw;
		}
		/// <summary>
		/// the func search the latitude of the sender by her name in the data base
		/// </summary>
		/// <param name="name"></param>
		/// <returns> the latitude </returns>
		private double SenderLati(string name)
		{
			foreach (IDAL.DO.Customer item in dal.GetListCustomers())
			{
				if (item.Name == name)
					return item.Latitude;
			}
			throw;
		}
		/// <summary>
		/// the func search the longitude of the target by her name in the data base
		/// </summary>
		/// <param name="name"></param>
		/// <returns> the longitude </returns>
		private double TargetLong(string name)
		{
			foreach (IDAL.DO.Customer item in dal.GetListCustomers())
			{
				if (item.Name == name)
					return item.Longitude;
			}
			throw;
		}
		/// <summary>
		/// the func search the latitude of the target by her name in the data base
		/// </summary>
		/// <param name="name"></param>
		/// <returns> the latitude </returns>
		private double TargetLati(string name)
		{
			foreach (IDAL.DO.Customer item in dal.GetListCustomers())
			{
				if (item.Name == name)
					return item.Latitude;
			}
			throw;
		}
		/// <summary>
		/// the func search a drone by her id in the "DroneToList" and return the index of the drone 
		/// </summary>
		/// <param name="id"></param>
		/// <returns> index </returns>
		private int SearchDrone(int id)
		{
			for (int i = 0; i < droneToList.Count(); i++)
			{
				if (droneToList[i].Id == id)
					return i;
			}
			throw;
		}
		/// <summary>
		/// the func search which drone is assigne to this parcel if is defined
		/// </summary>
		/// <param name="p"></param>
		/// <returns> Id </returns>
		private int SearchDroneIdAssigneToParcel(IBL.BO.ParcelToList p)
		{
			foreach (IDAL.DO.Parcel item in dal.GetListParcels())
			{
				if (item.Id == p.Id)
					return item.DroneId;
			}
			throw;
		}
		/// <summary>
		/// the func locate the drone according to his status 
		/// </summary>
		/// <param name="p"></param>
		/// <param name="droneId"></param>
		private void LocOfDrone(IBL.BO.ParcelToList p,int droneId)
		{
			if (p.Status == IBL.BO.ParcelStatues.Associated)
			{
				double finalDis = -1;
				IBL.BO.Location loc = new IBL.BO.Location();
				foreach (IDAL.DO.BaseStation b in dal.GetListBaseStations())
				{
					double dis = DistanceBetweenTwoPoints(b.Latitude, b.Longitude, SenderLati(p.NameSender), SenderLong(p.NameSender));
					if (finalDis == -1 || dis < finalDis)
					{
						droneToList[SearchDrone(droneId)].Loc.Longitude = b.Longitude;
						droneToList[SearchDrone(droneId)].Loc.Latitude = b.Latitude;
						finalDis = dis;
					}

				}
			}
			else if (p.Status == IBL.BO.ParcelStatues.Collected)
			{
				droneToList[SearchDrone(droneId)].Loc.Latitude = SenderLati(p.NameSender);
				droneToList[SearchDrone(droneId)].Loc.Longitude = SenderLong(p.NameSender);
			}
			int r = new Random().Next(1, 2);
			if (r == 1)
				droneToList[SearchDrone(droneId)].Battery = r * 100;
			else
				droneToList[SearchDrone(droneId)].Battery = calcBattery(droneToList[SearchDrone(droneId)], p);
		}
		//-------------------------------------------------------------------------------------------------------------------------------------------------------------
	}
}
