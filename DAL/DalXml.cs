using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using Dal;

namespace DalXml
{
	class DalXml : DalApi.IDal
	{
		/// <summary>
		/// lazy initialization
		/// </summary>
		internal static readonly Lazy<DalApi.IDal> _instance = new Lazy<DalApi.IDal>(() => new DalXml());
		/// <summary>
		/// return instance value
		/// </summary>
		public static DalApi.IDal GetInstance { get { return _instance.Value; } }

		string baseStationPath = @"BaseStation.xml";//the pathe to the xml file
		string customerPath = @"Customer.xml";//the pathe to the xml file
		string dronePath = @"drone.xml";//the pathe to the xml file
		string droneChargePath = @"DroneCharge.xml";//the pathe to the xml file
		string parcelPath = @"Parcel.xml";//the pathe to the xml file

		//public DalXml()
		//{ DataSourceXml.Initialize(); }

		#region add method
		//add method
		/// <summary>
		/// add a new base station 
		/// </summary>
		/// <param name="id"> id of base </param>
		/// <param name="name"> name of base </param>
		/// <param name="chargeSlots"></param>
		/// <param name="longe"> longitude </param>
		/// <param name="lati"> latitude </param>
		[MethodImpl(MethodImplOptions.Synchronized)]
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
		/// <summary>
		/// add new drone
		/// </summary>
		/// <param name="id"> id of drone </param>
		/// <param name="model"> mode of the drone </param>
		/// <param name="weight"> the max weight that the drone can take </param>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void AddDrone(int id, string model, DO.WeightCategories weight)
		{
			XElement drones = XmlTools.LoadListFromXMLElement(dronePath);

			if (id < 0 || id == 0) throw new DO.InvalidId("DRONE");
			if ((int)weight > 3 || (int)weight < 1) throw new DO.InvalidCategory("WEIGHT");

			XElement drone1 = (from d in drones.Elements()
							   where int.Parse(d.Element("Id").Value) == id
							   select d).FirstOrDefault();
			if (drone1 != null)
				throw new DO.IdExist("DRONE");

			XElement drone = new XElement("Drone",
				new XElement("Id", id),
				new XElement("Model", model),
				new XElement("MaxWeight", weight));
			drones.Add(drone);
			XmlTools.SaveListToXMLElement(drones, dronePath);
		}
		/// <summary>
		/// add new customer 
		/// </summary>
		/// <param name="id"> id of customer </param>
		/// <param name="name"> his name </param>
		/// <param name="phone"> his number phone </param>
		/// <param name="longi"> longitude </param>
		/// <param name="lati"> latitude </param>
		[MethodImpl(MethodImplOptions.Synchronized)]
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
		/// <summary>
		/// add new parcel 
		/// </summary>
		/// <param name="id"> id of parcel </param>
		/// <param name="senderId"> the sender id </param>
		/// <param name="targetId"> the target id </param>
		/// <param name="droneId"> the drone id </param>
		/// <param name="weight"> the weight of the parcel </param>
		/// <param name="priorities"> the priority of delivering </param>
		[MethodImpl(MethodImplOptions.Synchronized)]
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
		#endregion

		#region method - action on drone and parcel
		//method - action on drone and parcel
		/// <summary>
		/// affect a parcel to a drone 
		/// </summary>
		/// <param name="parcelId"> the parcel id </param>
		/// <param name="droneId"> the drone id </param>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void AssignParcelToDrone(int parcelId, int droneId)
		{
			XElement drones = XmlTools.LoadListFromXMLElement(dronePath);
			XElement parcels = XmlTools.LoadListFromXMLElement(parcelPath);

			XElement droneElement = (from dr in drones.Elements()
									 where int.Parse(dr.Element("Id").Value) == droneId
									 select dr).FirstOrDefault();
			if (droneElement == null)
				throw new DO.IdNotExist("DRONE");
			XElement parcelElement = (from par in drones.Elements()
									  where int.Parse(par.Element("Id").Value) == parcelId
									  select par).FirstOrDefault();
			if (parcelElement == null)
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
		/// <summary>
		/// delete a "new" parcel 
		/// </summary>
		/// <param name="parcelId"> the parcel id </param>
		[MethodImpl(MethodImplOptions.Synchronized)]
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
		/// <summary>
		/// collect parcel 
		/// </summary>
		/// <param name="parcelId"> the parcel id </param>
		[MethodImpl(MethodImplOptions.Synchronized)]
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
		/// <summary>
		/// deliverd parcel
		/// </summary>
		/// <param name="parcelId"> the parcel id</param>
		[MethodImpl(MethodImplOptions.Synchronized)]
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
		#endregion

		#region method- drone to charge,leav charge
		//method- drone to charge,leav charge
		/// <summary>
		/// sent drone to charge
		/// </summary>
		/// <param name="droneId"> the drone id </param>
		/// <param name="baseId">  the base id where the ddrone go </param>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void AssignDroneToBaseStation(int droneId, int baseId)
		{
			XElement drones = XmlTools.LoadListFromXMLElement(dronePath);
			XElement baseStations = XmlTools.LoadListFromXMLElement(baseStationPath);
			XElement droneCharges = XmlTools.LoadListFromXMLElement(droneChargePath);

			XElement droneElement = (from dr in drones.Elements()
									 let drCurId = int.Parse(dr.Element("Id").Value)
									 where drCurId == droneId
									 select dr).FirstOrDefault();
			if (droneElement == null)
				throw new DO.IdNotExist("DRONE");
			XElement baseElement = (from bas in baseStations.Elements()
									let basCurId = int.Parse(bas.Element("Id").Value)
									where basCurId == baseId
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
		/// <summary>
		/// release drone from charge
		/// </summary>
		/// <param name="droneId"> the drone id </param>
		/// <param name="baseId"> the base id wher the drone charged </param>
		[MethodImpl(MethodImplOptions.Synchronized)]
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
			XElement baseElement = (from bas in baseStations.Elements()
									where int.Parse(bas.Element("Id").Value) == baseId
									select bas).FirstOrDefault();
			if (baseElement == null)
				throw new DO.IdNotExist("BASE-STATION");
			XElement droneCharge = (from cha in droneCharges.Elements()
									where int.Parse(cha.Element("DroneId").Value) == droneId
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
		#endregion

		#region method - update data of an object
		//method - update data of an object
		/// <summary>
		/// update data of drone 
		/// </summary>
		/// <param name="drone"> a drone </param>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void UpdateDrone(DO.Drone drone)
		{
			XElement drones = XmlTools.LoadListFromXMLElement(dronePath);

			XElement droneElemente = (from d in drones.Elements()
									  where int.Parse(d.Element("Id").Value) == drone.Id
									  select d).FirstOrDefault();
			droneElemente.Element("Model").Value = drone.Model;

			XmlTools.SaveListToXMLElement(drones, dronePath);
		}
		/// <summary>
		/// update data of base station 
		/// </summary>
		/// <param name="baseStation"> a base station </param>
		[MethodImpl(MethodImplOptions.Synchronized)]
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
		/// <summary>
		/// update data of customer 
		/// </summary>
		/// <param name="customer"> a customer </param>
		[MethodImpl(MethodImplOptions.Synchronized)]
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
		/// <summary>
		/// update data of parcel
		/// </summary>
		/// <param name="parcel"></param>
		[MethodImpl(MethodImplOptions.Synchronized)]
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
		#endregion

		#region method- get a specific object by his Id
		//method- get a specific object by his Id
		/// <summary>
		/// return a base station 
		/// </summary>
		/// <param name="baseId"> the base id</param>
		/// <returns> base station </returns>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public DO.BaseStation GetBaseStation(int baseId)
		{
			List<DO.BaseStation> baseS = XmlTools.LoadListFromXMLSerializer<DO.BaseStation>(baseStationPath);

			DO.BaseStation b = baseS.FirstOrDefault(b => b.Id == baseId);
			if (b.Id == 0)
				throw new DO.IdNotExist("BASE-STATION");
			else
				return b;
		}
		/// <summary>
		/// return a drone 
		/// </summary>
		/// <param name="droneId"> the drone id </param>
		/// <returns> drone </returns>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public DO.Drone GetDrone(int droneId)
		{
			XElement drones = XmlTools.LoadListFromXMLElement(dronePath);
			DO.Drone drone = new DO.Drone();

			drone = (from dr in drones.Elements()
					 let id = int.Parse(dr.Element("Id").Value)
					 where id == droneId
					 select new DO.Drone()
					 {
						 Id = int.Parse(dr.Element("Id").Value),
						 Model = dr.Element("Model").Value,
						 MaxWeight = Enum.Parse<DO.WeightCategories>(dr.Element("MaxWeight").Value)
					 }).FirstOrDefault();

			if (drone.Id == 0)
				throw new DO.IdNotExist("DRONE");
			else
				return drone;
		}
		/// <summary>
		/// return a customer 
		/// </summary>
		/// <param name="customerId"> the customer id </param>
		/// <returns> customer </returns>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public DO.Customer GetCustomer(int customerId)
		{
			List<DO.Customer> customers = XmlTools.LoadListFromXMLSerializer<DO.Customer>(customerPath);

			DO.Customer customer = customers.FirstOrDefault(c => c.Id == customerId);
			if (customer.Id == 0)
				throw new DO.IdNotExist("CUSTOMER");
			else
				return customer;
		}
		/// <summary>
		/// return a parcel 
		/// </summary>
		/// <param name="parcelId"> the parcel id </param>
		/// <returns> parcel </returns>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public DO.Parcel GetParcel(int parcelId)
		{
			List<DO.Parcel> parcels = XmlTools.LoadListFromXMLSerializer<DO.Parcel>(parcelPath);

			DO.Parcel parcel = parcels.FirstOrDefault(p => p.Id == parcelId);
			if (parcel.Id == 0)
				throw new DO.IdNotExist("PARCEL");
			else
				return parcel;
		}
		#endregion

		#region method - return a IEnumerable list of object
		//method - return a IEnumerable list of object
		/// <summary>
		/// return IEnumerable list of base station 
		/// </summary>
		/// <param name="f"> predicat</param>
		/// <returns> base stations </returns>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public IEnumerable<DO.BaseStation> GetListBaseStations(Predicate<DO.BaseStation> f)
		{
			return XmlTools.LoadListFromXMLSerializer<DO.BaseStation>(baseStationPath).FindAll(f);
		}
		/// <summary>
		/// return IEnumerable list of drones
		/// </summary>
		/// <param name="f"> predicat </param>
		/// <returns> drones </returns>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public IEnumerable<DO.Drone> GetListDrones(Predicate<DO.Drone> f)
		{
			XElement drones = XmlTools.LoadListFromXMLElement(dronePath);

			return from dr in drones.Elements()
				   let drCur = new DO.Drone()
				   {
					   Id = int.Parse(dr.Element("Id").Value),
					   Model = dr.Element("Model").Value,
					   MaxWeight = Enum.Parse<DO.WeightCategories>(dr.Element("MaxWeight").Value)
				   }
				   where f(drCur)
				   select drCur;
		}
		/// <summary>
		/// return IEnumerable list of customers 
		/// </summary>
		/// <returns> customers </returns>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public IEnumerable<DO.Customer> GetListCustomers()
		{
			return XmlTools.LoadListFromXMLSerializer<DO.Customer>(customerPath);
		}
		/// <summary>
		/// return IEnumerable list of parcels
		/// </summary>
		/// <param name="f"> predicat </param>
		/// <returns> parcels </returns>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public IEnumerable<DO.Parcel> GetListParcels(Predicate<DO.Parcel> f)
		{
			return XmlTools.LoadListFromXMLSerializer<DO.Parcel>(parcelPath).FindAll(f);
		}
		/// <summary>
		/// display the details of all drone that in charge (by predicat)
		/// </summary>
		/// <param name="f"> predicat </param>
		/// <returns> list of drone charge </returns>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public IEnumerable<DO.DroneCharge> GetListDroneCharge(Predicate<DO.DroneCharge> f)
		{
			XElement droneCharges = XmlTools.LoadListFromXMLElement(droneChargePath);

			return from dr in droneCharges.Elements()
				   let drCur = new DO.DroneCharge()
				   {
					   DroneId = int.Parse(dr.Element("DroneId").Value),
					   StationId=int.Parse(dr.Element("StationId").Value)
				   }
				   where f(drCur)
				   select drCur;
		}
		#endregion


		//methot return a data about the charge and the electricity use
		/// <summary>
		/// the func go to config to take the value of the charging rate and the electricity use
		/// </summary>
		/// <returns> return the arr with the value</returns>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public double[] GetChargingRate()
		{
			DataSourceXml.Config c = new DataSourceXml.Config();
			double[] arr = new double[5];
			arr[0] = DataSource.Config.useWhenFree;
			arr[1] = DataSource.Config.useWhenLightly;
			arr[2] = DataSource.Config.useWhenMedium;
			arr[3] = DataSource.Config.useWhenHeavily;
			arr[4] = c.chargingRate;
			return arr;
		}
	}
}
