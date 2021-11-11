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
		List<IBL.BO.Drone> droneForList = new List<IBL.BO.Drone>();

		BL()
		{
			GetCharge();
			GiveList();
		}

		private void GetCharge()
		{
			double[] arr = new double[5];
			arr = dal.GetChargingRate();
			_useWhenFree = arr[0];
			_useWhenHeavily = arr[1];
			_useWhenLightly = arr[2];
			_useWhenMedium = arr[3];
			_chargingRate = arr[4];
		}
		private void GiveList()
		{
			foreach (IDAL.DO.Drone item in dal.GetListDrones())
			{
				droneForList.Add(new IBL.BO.Drone { Id = item.Id, Model = item.Model, MaxWeight = (IBL.BO.WeightCategories)item.MaxWeight, Battery = 0, status = IBL.BO.DroneStatuses.free });
			}

			foreach (IDAL.DO.Parcel item in dal.GetListParcels())
			{
				if (item.Delivered < DateTime.Now && item.DroneId != 0)
				{
					for (int i = 0; i < droneForList.Count(); i++)
					{
						if (item.DroneId == droneForList[i].Id)
						{
							droneForList[i].status = IBL.BO.DroneStatuses.Shipping;
							break;
						}
						if (item.Scheduled > DateTime.Now && item.PickedUp < DateTime.Now)
						{
							double latti;
							double longi;
							foreach (IDAL.DO.BaseStation a in dal.GetListBaseStations())
							{
								double dis = DistanceBetweenTwoPoints(SerachLattitudeFromSender(item.SenderId), SerachLongitudeFromSender(item.SenderId), a.Lattitude, a.Longitude);
								if (dis
							}
						}
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

		private double SerachLongitudeFromSender(int id)
		{
			foreach (IDAL.DO.Customer item in dal.GetListCustomers())
			{
				if (item.Id == id)
					return item.Longitude;
			}
			throw;
		}
		private double SerachLattitudeFromSender(int id)
		{
			foreach (IDAL.DO.Customer item in dal.GetListCustomers())
			{
				if (item.Id == id)
					return item.Lattitude;
			}
			throw;
		}
		private IEnumerable<IBL.BO.Drone> GetLocationBaseOfDrone()
		{
			IBL.BO.Drone d;
			for (int i = 0; i <IDAL.DO.BaseStation ; i++)
			{

			}
		}
	}
}
