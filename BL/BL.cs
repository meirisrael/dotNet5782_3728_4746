using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
	class BL : IBL.IBL
	{
		private IDAL.IDal p;
		//private = new DalObject.DalObject();
		
		private double _useWhenFree;
		private double _useWhenHeavily;
		private double _useWhenLightly;
		private double _useWhenMedium;
		private double _chargingRate;
		List<IBL.BO.Drone> droneForList = new List<IBL.BO.Drone>();
		BL() 
		{
			charge();
			getList();
		}
		private void charge()
		{
			double[] arr = new double[5];
			arr = p.GetChargingRate();
			_useWhenFree = arr[0];
			_useWhenHeavily = arr[1];
			_useWhenLightly = arr[2];
			_useWhenMedium = arr[3];
			_chargingRate = arr[4];
		}
		private void getList()
		{
			foreach (IDAL.DO.Drone item in p.DisplayListDrones())
			{
				drone.Add(new IBL.BO.Drone {Id=item.Id,Model=item.Model,MaxWeight= (IBL.BO.WeightCategories)item.MaxWeight,Battery=0,status=IBL.BO.DroneStatuses.free });
			}

			foreach (IDAL.DO.Parcel item in p.DisplayListParcels())
			{
				bool flag = false;
				if (item.PickedUp ==null/*>DateTime.Now*/ && item.DroneId != 0)
				{
					for (int i = 0; i < drone.Count(); i++)
					{
						if (item.DroneId==drone[i].Id)
						{
							drone[i].status = IBL.BO.DroneStatuses.Shipping;
							flag = true;
							break;
						}
					}
				}
				if (flag)
					break;
			}
		}



		private IDAL.DO.Drone searchDrone(int id)
		{
			foreach (IDAL.DO.Drone item in p.DisplayListDrones())
			{
				if (item.Id == id)
					return item;
			}
			throw ;
		}
		
	}
}
