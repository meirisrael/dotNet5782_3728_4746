using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDAL
{
	interface IDal
	{
		public void AddBaseStation();
		public void AddDrone();
		public void AddCustomer();
		public void AddParcel();
		public void AssignParcelToDrone();
		public void ParcelOnDrone();
		public void ParcelDelivered();
		public void AssignDroneToBaseStation();
		public void DroneLeaveChargeStation();
		public void DisplayBaseStation();
		public void DisplayDrone();
		public void DisplayCustomer();
		public void DisplayParcel();
		public void DisplayListBaseStations();
		public void DisplayListDrones();
		public void DisplayListCustomers();
		public void DisplayListParcels();
		public void DisplayParcelsNotAssignedToDrone();
		public void DisplayListBaseStationsCanCharge();
		public double[] GetChargingRate();
	}
}
