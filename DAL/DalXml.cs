using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DalXml
{
	class DalXml : DalApi.IDal
	{
		string baseStationPath = @"BaseStation.xml";
		string customerPath = @"Customer.xml";
		string dronePath = @"Drone.xml";
		string droneChargePath = @"DroneCharge.xml";
		string parcelPath = @"Parcel.xml";

		//add method
		public void AddBaseStation(int id, int name, int chargeSlots, double longe, double lati)
		{
			List<DO.BaseStation> baseStations = XmlTools.LoadListFromXMLSerializer<DO.BaseStation>(baseStationPath);


			if (id < 0 || id == 0) throw new DO.InvalidId("BASE-STATION");
			if (chargeSlots < 0) throw new DO.InvalidChargeSlot();
			if (longe > 180 || longe < -180) throw new DO.InvalidLoc("LONGITUDE", "-180 TO 180");
			if (lati > 90 || lati < -90) throw new DO.InvalidLoc("LATITUDE", "-90 TO 90");

			if (baseStations.Exists(b => b.Id == id))
				throw new DO.IdExist("BASE-STATION");

			baseStations.Add(new DO.BaseStation()
			{
				Id = id,
				Name = name,
				ChargeSlots = chargeSlots,
				Latitude = lati,
				Longitude = longe
			});
			XmlTools.SaveListToXMLSerializer<DO.BaseStation>(baseStations, baseStationPath);
		}

		public void AddDrone(int id, string model, DO.WeightCategories weight)
		{
			XElement drones = XmlTools.LoadListFromXMLElement(dronePath);

			if (id < 0 || id == 0) throw new DO.InvalidId("DRONE");
			if ((int)weight > 3 || (int)weight < 1) throw new DO.InvalidCategory("WEIGHT");

			XElement drone1 = (from d in drones.Elements()
							   where int.Parse(d.Element("Id").Value) == id
							   select d).FirstOrDefault();
			if(drone1!=null)
				throw new DO.IdExist("DRONE");

			XElement drone = new XElement("Drone",
				new XElement("Id", id),
				new XElement("Model", model),
				new XElement("Weight", weight));
			drones.Add(drone);
			XmlTools.SaveListToXMLElement(drones, dronePath);
		}

		public void AddCustomer(int id, string name, string phone, double longi, double lati)
		{
			List<DO.Customer> customers = XmlTools.LoadListFromXMLSerializer<DO.Customer>(customerPath);

			if (id < 0 || id == 0) throw new DO.InvalidId("CUSTOMER");
			if (lati > 90 || lati < -90) throw new DO.InvalidLoc("LATITUDE", "-90 TO 90");
			if (longi > 180 || longi < -180) throw new DO.InvalidLoc("LONGITUDE", "-180 TO 180");

			if (customers.Exists(item => item.Id == id))
				throw new DO.IdExist("CUSTOMER");

			customers.Add(new DO.Customer()
			{
				Id = id,
				Name = name,
				Phone = phone,
				Longitude = longi,
				Latitude = lati
			});
			XmlTools.SaveListToXMLSerializer<DO.Customer>(customers, customerPath);
		}

		public void AddParcel(int id, int senderId, int targetId, int droneId, DO.WeightCategories weight, DO.Priorities priorities)
		{
			List<DO.Parcel> parcels = XmlTools.LoadListFromXMLSerializer<DO.Parcel>(parcelPath);
			List<DO.Customer> customers = XmlTools.LoadListFromXMLSerializer<DO.Customer>(customerPath);

			if (id < 0 || id == 0) throw new DO.InvalidId("PARCEL");
			if (senderId == 0 || senderId < 0) throw new DO.InvalidId("SENDER");
			if (targetId == 0 || targetId < 0) throw new DO.InvalidId("TARGET");
			if (targetId == senderId) throw new DO.SenderTargetIdEqual();
			if (droneId < 0) throw new DO.NegativeDroneId();
			if ((int)weight > 3 || (int)weight < 1) throw new DO.InvalidCategory("WEIGHT");
			if ((int)priorities > 3 || (int)priorities < 1) throw new DO.InvalidCategory("PRIORITIES");

			if (parcels.Exists(item => item.Id == id))
				throw new DO.IdExist("PARCEL");
			if (!customers.Exists(item => item.Id == senderId))
				throw new DO.IdNotExist("SENDER");
			if (!customers.Exists(item => item.Id == targetId))
				throw new DO.IdNotExist("TARGET");

			parcels.Add(new DO.Parcel()
			{
				Id = id,
				SenderId = senderId,
				TargetId = targetId,
				Weight = weight,
				Priority = priorities,
				DroneId = droneId,
				Requested = DateTime.Now,
				Scheduled = null,
				PickedUp = null,
				Delivered = null
			});
			XmlTools.SaveListToXMLSerializer<DO.Parcel>(parcels, parcelPath);
		}

		//method - action on drone and parcel
		public void AssignParcelToDrone(int parcelId, int droneId)
		{
			XElement drones = XmlTools.LoadListFromXMLElement(dronePath);
			XElement parcels = XmlTools.LoadListFromXMLElement(parcelPath);

			XElement droneElement = (from dr in drones.Elements()
							   where int.Parse(dr.Element("Id").Value) == droneId
							   select dr).FirstOrDefault();
			if(droneElement == null)
				throw new DO.IdNotExist("DRONE");
			XElement parcelElement = (from par in drones.Elements()
							   where int.Parse(par.Element("Id").Value) == parcelId
							   select par).FirstOrDefault();
			if(parcelElement == null)
				throw new DO.IdNotExist("PARCEL");

			if (droneElement.Element("MaxWeight").Value != parcelElement.Element("Weight").Value)
			{
				if (droneElement.Element("MaxWeight").Value.ToString() == DO.WeightCategories.Light.ToString() && (parcelElement.Element("Weight").Value.ToString() == DO.WeightCategories.Medium.ToString() || parcelElement.Element("Weight").Value.ToString() == DO.WeightCategories.Medium.ToString()))
					throw new DO.ParcelTooHeavy();
				else if (droneElement.Element("MaxWeight").Value.ToString() == DO.WeightCategories.Medium.ToString() && parcelElement.Element("Weight").Value.ToString() == DO.WeightCategories.Medium.ToString())
					throw new DO.ParcelTooHeavy();
			}
			parcelElement.Element("DroneId").Value = droneId.ToString();
			parcelElement.Element("Scheduled").Value = DateTime.Now.ToString();

			XmlTools.SaveListToXMLElement(parcels, parcelPath);
		}

		public void DeleteParcel(int parcelId)
		{
			List<DO.Parcel> parcels = XmlTools.LoadListFromXMLSerializer<DO.Parcel>(parcelPath);

			DO.Parcel p = new DO.Parcel();
			if (parcelId <= 0) throw new DO.InvalidId("PARCEL");
			if (!parcels.Exists(p => p.Id == parcelId)) throw new DO.IdNotExist("PARCEL");
			else p = parcels.Find(p => p.Id == parcelId);

			if (p.Scheduled == null)
				parcels.Remove(p);
			else throw new DO.CantRemove();

			XmlTools.SaveListToXMLSerializer<DO.Parcel>(parcels, parcelPath);
		}

		public void ParcelOnDrone(int parcelId)
		{
			List<DO.Parcel> parcels = XmlTools.LoadListFromXMLSerializer<DO.Parcel>(parcelPath);
			if (parcels.Exists(item => item.Id == parcelId))
				throw new DO.IdNotExist("PARCEL");

			for (int i = 0; i < parcels.Count; i++)
			{
				if (parcels[i].Id == parcelId)
				{
					DO.Parcel x = parcels[i];
					x.PickedUp = DateTime.Now;
					parcels[i] = x;
					break;
				}
			}
			XmlTools.SaveListToXMLSerializer<DO.Parcel>(parcels, parcelPath);
		}

		public void ParcelDelivered(int parcelId)
		{
			List<DO.Parcel> parcels = XmlTools.LoadListFromXMLSerializer<DO.Parcel>(parcelPath);
			if (!parcels.Exists(item => item.Id == parcelId))
				throw new DO.IdNotExist("PARCEL");

			for (int i = 0; i < parcels.Count; i++)
			{
				if (parcels[i].Id == parcelId)
				{
					DO.Parcel x = parcels[i];
					x.Delivered = DateTime.Now;
					parcels[i] = x;
					break;
				}
			}
			XmlTools.SaveListToXMLSerializer<DO.Parcel>(parcels, parcelPath);
		}

		//method- drone to charge,leav charge
		public void AssignDroneToBaseStation(int droneId, int baseId)
		{
			XElement drones = XmlTools.LoadListFromXMLElement(dronePath);
			XElement baseStations = XmlTools.LoadListFromXMLElement(baseStationPath);
			XElement droneCharges = XmlTools.LoadListFromXMLElement(droneChargePath);

			XElement droneElement = (from dr in drones.Elements()
									 where int.Parse(dr.Element("Id").Value) == droneId
									 select dr).FirstOrDefault();
			if (droneElement == null)
				throw new DO.IdNotExist("DRONE");
			XElement baseElement = (from bas in drones.Elements()
									  where int.Parse(bas.Element("Id").Value) == baseId
									  select bas).FirstOrDefault();
			if (baseElement == null)
				throw new DO.IdNotExist("BASE-STATION");

			int x = int.Parse(baseElement.Element("ChargeSlots").Value);
			x--;
			baseElement.Element("ChargeSlots").Value = x.ToString();

			XElement drone = new XElement("Drone",
				new XElement("DroneId", droneId),
				new XElement("StationId", baseId));
			droneCharges.Add(drone);

			XmlTools.SaveListToXMLElement(baseStations, baseStationPath);
			XmlTools.SaveListToXMLElement(droneCharges, droneChargePath);
		}

		public void DroneLeaveChargeStation(int droneId, int baseId)
		{
			XElement drones = XmlTools.LoadListFromXMLElement(dronePath);
			XElement baseStations = XmlTools.LoadListFromXMLElement(baseStationPath);
			XElement droneCharges = XmlTools.LoadListFromXMLElement(droneChargePath);

			XElement droneElement = (from dr in drones.Elements()
									 where int.Parse(dr.Element("Id").Value) == droneId
									 select dr).FirstOrDefault();
			if (droneElement == null)
				throw new DO.IdNotExist("DRONE");
			XElement baseElement = (from bas in drones.Elements()
									where int.Parse(bas.Element("Id").Value) == baseId
									select bas).FirstOrDefault();
			if (baseElement == null)
				throw new DO.IdNotExist("BASE-STATION");
			XElement droneCharge= (from cha in drones.Elements()
								   where int.Parse(cha.Element("Id").Value) == droneId
								   select cha).FirstOrDefault();
			if (droneCharge == null)
				throw new DO.DroneNotInCharge();

			int x = int.Parse(baseElement.Element("ChargeSlots").Value);
			x++;
			baseElement.Element("ChargeSlots").Value = x.ToString();

			droneCharge.Remove();

			XmlTools.SaveListToXMLElement(baseStations, baseStationPath);
			XmlTools.SaveListToXMLElement(droneCharges, droneChargePath);
		}

		//method- get a specific object by his Id
		public DO.BaseStation GetBaseStation(int baseId)
		{
			foreach (DO.BaseStation item in XmlTools.LoadListFromXMLSerializer<DO.BaseStation>(baseStationPath))
			{
				if (item.Id == baseId)
					return item;
			}
			throw new DO.IdNotExist("BASE-STATION");
		}

		public DO.Drone GetDrone(int droneId)
		{
			XElement drones = XmlTools.LoadListFromXMLElement(dronePath);
			DO.Drone drone = new DO.Drone();
			try
			{
				drone = (from dr in drones.Elements()
						 where int.Parse(dr.Element("Id").Value) == droneId
						 select new DO.Drone()
						 {
							 Id = int.Parse(dr.Element("Id").Value),
							 Model = dr.Element("Model").Value,
							 MaxWeight = (DO.WeightCategories)int.Parse(dr.Element("MaxWeight").Value)
						 }).FirstOrDefault();
			}
			catch
			{
				throw new DO.IdNotExist("DRONE");
			}
			return drone;
		}

		public DO.Customer GetCustomer(int customerId)
		{
			foreach (DO.Customer item in XmlTools.LoadListFromXMLSerializer<DO.Customer>(customerPath))
			{
				if (item.Id == customerId)
					return item;
			}
			throw new DO.IdNotExist("CUSTOMER");
		}

		public DO.Parcel GetParcel(int parcelId)
		{
			foreach (DO.Parcel item in XmlTools.LoadListFromXMLSerializer<DO.Parcel>(parcelPath))
			{
				if (item.Id == parcelId)
					return item;
			}
			throw new DO.IdNotExist("PARCEL");
		}

		//method - return a IEnumerable list of object
		public IEnumerable<DO.BaseStation> GetListBaseStations(Predicate<DO.BaseStation> f)
		{
			return XmlTools.LoadListFromXMLSerializer<DO.BaseStation>(baseStationPath).FindAll(f);
		}

		public IEnumerable<DO.Drone> GetListDrones(Predicate<DO.Drone> f)
		{
			XElement drones = XmlTools.LoadListFromXMLElement(dronePath);
			List<DO.Drone> dronesList = new List<DO.Drone>();

			try
			{
				dronesList = (from dr in drones.Elements()
							  select new DO.Drone()
							  {
								  Id = int.Parse(dr.Element("Id").Value),
								  Model = dr.Element("Model").Value,
								  MaxWeight = (DO.WeightCategories)int.Parse(dr.Element("MaxWeight").Value)
							  }).ToList();
			}
			catch
			{
				dronesList = null;
			}
			return dronesList;
		}

		public IEnumerable<DO.Customer> GetListCustomers()
		{
			return XmlTools.LoadListFromXMLSerializer<DO.Customer>(customerPath);
		}

		public IEnumerable<DO.Parcel> GetListParcels(Predicate<DO.Parcel> f)
		{
			return XmlTools.LoadListFromXMLSerializer<DO.Parcel>(parcelPath).FindAll(f);
		}

		//method - update data of an object
		public void UpdateDrone(DO.Drone drone)
		{
			XElement drones = XmlTools.LoadListFromXMLElement(dronePath);

			XElement droneElemente = (from d in drones.Elements()
								  where int.Parse(d.Element("Id").Value) == drone.Id
								  select d).FirstOrDefault();
			droneElemente.Element("Model").Value = drone.Model;

			XmlTools.SaveListToXMLElement(drones, dronePath);
		}

		public void UpdateBaseStation(DO.BaseStation baseStation)
		{
			List<DO.BaseStation> baseStations = XmlTools.LoadListFromXMLSerializer<DO.BaseStation>(baseStationPath);
			for (int i = 0; i < baseStations.Count(); i++)
			{
				if (baseStations[i].Id == baseStation.Id)
				{
					baseStations[i] = baseStation; break;
				}
			}
			XmlTools.SaveListToXMLSerializer<DO.BaseStation>(baseStations, baseStationPath);
		}

		public void UpdateCustomer(DO.Customer customer)
		{
			List<DO.Customer> customers = XmlTools.LoadListFromXMLSerializer<DO.Customer>(customerPath);
			for (int i = 0; i < customers.Count(); i++)
			{
				if (customers[i].Id == customer.Id)
				{
					customers[i] = customer; break;
				}
			}
			XmlTools.SaveListToXMLSerializer<DO.Customer>(customers, customerPath);
		}

		public void UpdateParcel(DO.Parcel parcel)
		{
			List<DO.Parcel> parcels = XmlTools.LoadListFromXMLSerializer<DO.Parcel>(parcelPath);
			for (int i = 0; i < parcels.Count(); i++)
			{
				if (parcels[i].Id == parcel.Id)
				{
					parcels[i] = parcel; break;
				}
			}
			XmlTools.SaveListToXMLSerializer<DO.Parcel>(parcels, parcelPath);
		}
		//methot return a data about the charge and the electricity use
		public double[] GetChargingRate()
		{
			//to do
			return null;
		}
	}
}
