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
		private double _useWhenHeavily;
		private double _useWhenLightly;
		private double _useWhenMedium;
		private double _chargingRate;
		List<IBL.BO.DroneToList> droneToList = new List<IBL.BO.DroneToList>();

		BL()
		{
			GetDataCharge();
			ReqListOfDrone();
			StatusOfDrone();
		}

		private void GetDataCharge()
		{
			double[] arr = new double[5];
			arr = dal.GetChargingRate();
			_useWhenFree = arr[0];
			_useWhenHeavily = arr[1];
			_useWhenLightly = arr[2];
			_useWhenMedium = arr[3];
			_chargingRate = arr[4];
		}

		private void ReqListOfDrone()
		{
			foreach (IDAL.DO.Drone item in dal.GetListDrones())
			{
				droneToList.Add(new IBL.BO.DroneToList { Id = item.Id, Model = item.Model, MaxWeight = (IBL.BO.WeightCategories)item.MaxWeight, Battery = 0,
					Status = IBL.BO.DroneStatuses.free,Lattitude=0,Longitude=0,IdOfParcel=0});
			}
		}
		private void StatusOfDrone()
		{
			List<IBL.BO.ParcelToList> parcelToList = new List<IBL.BO.ParcelToList>();
			List<IBL.BO.BaseStation> baseS = new List<IBL.BO.BaseStation>();
			foreach (IDAL.DO.Parcel item in dal.GetListParcels())
			{
				parcelToList.Add(new IBL.BO.ParcelToList { Id = item.Id, Weight = (IBL.BO.WeightCategories)item.Weight, 
					Priority = (IBL.BO.Priorities)item.Priority, Status = IBL.BO.ParcelStatues.Defined });
			}
			for(int i=0;i<parcelToList.Count();i++)
			{
				foreach (IDAL.DO.Parcel item in dal.GetListParcels())
				{
					if (parcelToList[i].Status != IBL.BO.ParcelStatues.Delivered && item.DroneId > 0)
					{
						droneToList[SearchDrone(item.DroneId)].Status = IBL.BO.DroneStatuses.Shipping;
						if (parcelToList[i].Status == IBL.BO.ParcelStatues.Associated)
						{
							//IBL.BO.Location loc = new IBL.BO.Location();
							//foreach (IDAL.DO.BaseStation b in dal.GetListBaseStations())
							//{
							//	CloserBase(DistanceBetweenTwoPoints(b.Lattitude,b.Longitude,SenderLati(parcelToList[i].NameSender),SenderLong(parcelToList[i].NameSender)),DistanceBetweenTwoPoints())
							//}
						}
						else if (parcelToList[i].Status == IBL.BO.ParcelStatues.Collected)
						{
							droneToList[SearchDrone(item.DroneId)].Loc.Lattitude = SenderLati(parcelToList[i].NameSender);
							droneToList[SearchDrone(item.DroneId)].Loc.Longitude = SenderLong(parcelToList[i].NameSender);
						}
					}
					if (droneToList[SearchDrone(item.DroneId)].Status != IBL.BO.DroneStatuses.Shipping)
					{
						int r = new Random().Next(1, 2);
						if (r == 1)
							droneToList[SearchDrone(item.DroneId)].Status = IBL.BO.DroneStatuses.free;
						else
							droneToList[SearchDrone(item.DroneId)].Status = IBL.BO.DroneStatuses.Maintenance;
					}
					if (droneToList[SearchDrone(item.DroneId)].Status == IBL.BO.DroneStatuses.Maintenance)
					{
						foreach (IDAL.DO.BaseStation b in dal.GetListBaseStations())
						{
							baseS.Add(new IBL.BO.BaseStation { Lattitude = b.Lattitude, Longitude = b.Longitude });
						}
						int r = new Random().Next(0, baseS.Count());
						droneToList[SearchDrone(item.DroneId)].Longitude = baseS[r].Longitude;
						droneToList[SearchDrone(item.DroneId)].Lattitude = baseS[r].Lattitude;
						r = new Random().Next(0, 21);
						droneToList[SearchDrone(item.DroneId)].Battery = r;
						break;
					}
					else if (droneToList[SearchDrone(item.DroneId)].Status == IBL.BO.DroneStatuses.free)
					{

					}
				}
				
			}
		}
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

		private double SenderLong(string name)
		{
			foreach (IDAL.DO.Customer item in dal.GetListCustomers())
			{
				if (item.Name == name)
					return item.Longitude;
			}
			throw;
		}
		private double SenderLati(string name)
		{
			foreach (IDAL.DO.Customer item in dal.GetListCustomers())
			{
				if (item.Name == name)
					return item.Lattitude;
			}
			throw;
		}
		private int CloserBase(double baseA,double baseB)
		{

		}
		private IEnumerable<IBL.BO.Drone> GetLocationBaseOfDrone()
		{
			IBL.BO.Drone d;
			for (int i = 0; i <IDAL.DO.BaseStation ; i++)
			{

			}
		}

		private int SearchDrone(int id)
		{
			for (int i = 0; i < droneToList.Count(); i++)
			{
				if (droneToList[i].Id == id)
					return i;
			}
			throw;
		}
	}
}
