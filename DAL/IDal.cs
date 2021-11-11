using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDAL
{
	public interface IDal
	{
		public void AddBaseStation(int id,int name, int chargeSlots, double longe, double lati);
		public void AddDrone(int id,string model, IDAL.DO.WeightCategories weight);
		public void AddCustomer(int id,string name, string phone, double longi, double lati);
		public void AddParcel(int id, int senderId, int targetId, IDAL.DO.WeightCategories weight, IDAL.DO.Priorities priorities);//, int droneId, DateTime requested,
			//DateTime scheduled, DateTime pickedUp, DateTime delivered);

		public void AssignParcelToDrone(int parcelId, int droneId);
		public void ParcelOnDrone(int parcelId);
		public void ParcelDelivered(int parcelId);
		public void AssignDroneToBaseStation(int droneId, int baseId);
		public void DroneLeaveChargeStation(int droneId, int baseId);


		public IDAL.DO.BaseStation GetBaseStation(int baseId);
		public IDAL.DO.Drone GetDrone(int droneId);
		public IDAL.DO.Customer GetCustomer(int customer);
		public IDAL.DO.Parcel GetParcel(int parecel);


		public IEnumerable<IDAL.DO.Parcel> GetListOfParcelsNotAssignedToDrone();


		public IEnumerable<IDAL.DO.BaseStation> GetListBaseStations();
		public IEnumerable<IDAL.DO.Drone> GetListDrones();
		public IEnumerable<IDAL.DO.Customer> GetListCustomers();
		public IEnumerable<IDAL.DO.Parcel> GetListParcels();
		public IEnumerable<IDAL.DO.BaseStation> GetListBaseStationsCanCharge();

		public double[] GetChargingRate();
	}
}
