using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDAL
{
	interface IDal
	{
		public void AddBaseStation(int id,int name, int chargeSlots, double longe, double lati);
		public void AddDrone(int id,string model, IDAL.DO.WeightCategories weight);
		public void AddCustomer(int id,string name, string phone, double longi, double lati);
		public void AddParcel(int id,int senderId, int targetId, IDAL.DO.WeightCategories weight, IDAL.DO.Priorities priorities, int droneId, DateTime requested,
			DateTime scheduled, DateTime pickedUp, DateTime delivered);

		public void AssignParcelToDrone(int parcelId, int droneId);
		public void ParcelOnDrone(int parcelId);
		public void ParcelDelivered(int parcelId);
		public void AssignDroneToBaseStation(int droneId, int baseId);
		public void DroneLeaveChargeStation(int droneId, int baseId);


		public IDAL.DO.BaseStation DisplayBaseStation(int baseId);
		public IDAL.DO.Drone DisplayDrone(int droneId);
		public IDAL.DO.Customer DisplayCustomer(int customer);
		public IDAL.DO.Parcel DisplayParcel(int parecel);


		public IEnumerable<IDAL.DO.Parcel> DisplayParcelsNotAssignedToDrone();


		public IEnumerable<IDAL.DO.BaseStation> DisplayListBaseStations();
		public IEnumerable<IDAL.DO.Drone> DisplayListDrones();
		public IEnumerable<IDAL.DO.Customer> DisplayListCustomers();
		public IEnumerable<IDAL.DO.Parcel> DisplayListParcels();
		public IEnumerable<IDAL.DO.BaseStation> DisplayListBaseStationsCanCharge();

		public double[] GetChargingRate();
	}
}
