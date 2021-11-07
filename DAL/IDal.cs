using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDAL
{
	interface IDal
	{
		public void AddBaseStation(int name, int chargeSlots, double longe, double lati);
		public void AddDrone(string model, IDAL.DO.WeightCategories weight);
		public void AddCustomer(string name, string phone, double longi, double lati);
		public void AddParcel(int senderId, int targetId, IDAL.DO.WeightCategories weight, IDAL.DO.Priorities priorities, int droneId, DateTime requested,
			DateTime scheduled, DateTime pickedUp, DateTime delivered);

		public void AssignParcelToDrone(int parcelId, int droneId);
		public void ParcelOnDrone(int parcelId);
		public void ParcelDelivered(int parcelId);
		public void AssignDroneToBaseStation(int droneId, int baseId);
		public void DroneLeaveChargeStation(int droneId, int baseId);


		public void DisplayBaseStation(int baseId);
		public void DisplayDrone(int droneId);
		public void DisplayCustomer(int customer);
		public void DisplayParcel(int parecel);


		public void DisplayParcelsNotAssignedToDrone();


		public void DisplayListBaseStations();
		public void DisplayListDrones();
		public void DisplayListCustomers();
		public void DisplayListParcels();
		public void DisplayListBaseStationsCanCharge();

		public double[] GetChargingRate();
	}
}
