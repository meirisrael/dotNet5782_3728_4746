using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBL
{
	public interface IBL
	{
		//
		public void AddBaseStation(int id, int name, int chargeSlots, BO.Location location);
		public void AddDrone(int id, string model, IDAL.DO.WeightCategories weight, int firstBaseStation);
		public void AddCustomer(int id, string name, string phone, BO.Location location);
		public void AddParcel(int id, int senderId, int targetId, IDAL.DO.WeightCategories weight, IDAL.DO.Priorities priorities);
		//
		public void UpdateDrone(int droneId, string model);
		public void UpdateBaseStation(int baseId, string name = null, string chargeslots = null);
		public void UpdateCustomer(int customerId, string name = null, string phone = null);
		public void DroneToCharge(int droneId);
		public void DroneLeaveCharge(int droneId, int time);
		public void AffectParcelToDrone(int droneId);
		public void ParcelCollection(int droneId);
		public void ParcelDeliverd(int droneId);
		//
		public BO.BaseStation GetBaseStation(int baseId);
		public BO.Drone GetDrone(int droneId);
		public BO.Customer GetCustomer(int customerId);
		public BO.Parcel GetParcel(int parcelId);
		//
		public IEnumerable<List<BO.BaseToList>> GetListOfBaseStations();
		public IEnumerable<List<BO.DroneToList>> GetListOfDrone();
		public IEnumerable<List<BO.CustomerToList>> GetListOfCustomer();
		public IEnumerable<List<BO.ParcelToList>> GetListOfParcel();
		public IEnumerable<List<BO.ParcelToList>> GetListParcelNotAssignToDrone();
		public IEnumerable<List<BO.BaseToList>> GetListBaseWithChargeSlot();
	}
}