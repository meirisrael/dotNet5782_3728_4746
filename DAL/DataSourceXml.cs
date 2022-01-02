﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dal;

namespace Dal
{
	class DataSourceXml
	{
		public static Random r = new Random();

		static string baseStationPath = @"BaseStation.xml";//the path to the xml file
		static string customerPath = @"Customer.xml";//the path to the xml file
		static string dronePath = @"Drone.xml";//the path to the xml file
		static string parcelPath = @"Parcel.xml";//the path to the xml file
		static string droneChargePath = @"DroneCharge.xml";//the path to the xml file

		internal class Config
		{
			internal static double useWhenFree = 0.0001;//0.0001% per kilometere
			internal static double useWhenLightly = 0.0003;//0.0003% per kilometere
			internal static double useWhenMedium = 0.0004;//0.0004% per kilometere
			internal static double useWhenHeavily = 0.0009;//0.0009% per kilometere
			internal double chargingRate = 35; //35% per hour
		}
		/// <summary>
		/// the func initialize all parameter of the program with a default solution for each parametre
		/// </summary>
		public static void Initialize()
		{
			int CounterBaseStation = 2000;
			int CounterDrones = 1000;
			int CounterCustomer = 3000;
			int CounterParcel = 4000;
			string[] name_ = new string[] { "Meir", "Lior", "Hilel", "Mendel", "Chimon", "David", "Loki", "Rephael", "levi", "Nathan" };

			
			for (int i = 0; i < 2; i++)//for base station
			{
				List<DO.BaseStation> baseStations = XmlTools.LoadListFromXMLSerializer<DO.BaseStation>(baseStationPath);
				baseStations.Add(new DO.BaseStation()
				{
					Id = CounterBaseStation++,
					Name = i,
					ChargeSlots = r.Next(1, 6),
					Latitude = r.Next(-90, 91) * r.NextDouble(),    //(r.Next(-90, 91) * r.NextDouble())%1 +34.5,  shipping only israel
					Longitude = r.Next(-180, 181) * r.NextDouble()  //(r.Next(-180, 181) * r.NextDouble())%3.7 +29.5 shipping only israel
				});
				XmlTools.SaveListToXMLSerializer<DO.BaseStation>(baseStations, baseStationPath);
			}
			for (int i = 0; i < 10; i++)//for drone
			{
				List<DO.Drone> drones = XmlTools.LoadListFromXMLSerializer<DO.Drone>(dronePath);
				drones.Add(new DO.Drone()
				{
					Id = CounterDrones,
					Model = "Fantome-" + r.Next(4, 6),
					MaxWeight = (DO.WeightCategories)3
				});
				if (i < 9)
					CounterDrones++;
				XmlTools.SaveListToXMLSerializer<DO.Drone>(drones, dronePath);
			}
			for (int i = 0; i < 10; i++)//for customer
			{
				List<DO.Customer> customers = XmlTools.LoadListFromXMLSerializer<DO.Customer>(customerPath);
				customers.Add(new DO.Customer()
				{
					Id = CounterCustomer,
					Name = name_[i],
					Phone = (3761 + i).ToString(),
					Latitude = r.Next(-90, 91) * r.NextDouble(), //(r.Next(-90, 91) * r.NextDouble())%1 +34.5,  shipping only israel
					Longitude = r.Next(-180, 181) * r.NextDouble() //(r.Next(-180, 181) * r.NextDouble())%3.7 +29.5 shipping only israel
				});
				if (i < 9)
					CounterCustomer++;
				XmlTools.SaveListToXMLSerializer<DO.Customer>(customers, customerPath);
			}
			for (int i = 1; i <= 10; i++)//for parcel
			{
				List<DO.Parcel> parcels = XmlTools.LoadListFromXMLSerializer<DO.Parcel>(parcelPath);
				DateTime? x, z;
				if (i % 3 == 0) { x = DateTime.Now.AddHours(2); z = x.Value.AddHours(1); }
				else { x = null; z = null; }
				parcels.Add(new DO.Parcel()
				{
					Id = CounterParcel++,
					SenderId = i + 3000 - 1,
					TargetId = CounterCustomer--,
					Weight = (DO.WeightCategories)(r.Next(1, 4)),
					Priority = (DO.Priorities)(r.Next(1, 4)),
					DroneId = CounterDrones,
					Requested = DateTime.Now,
					Scheduled = DateTime.Now.AddSeconds(5),
					PickedUp = x,
					Delivered = z
				});
				CounterDrones--;
				XmlTools.SaveListToXMLSerializer<DO.Parcel>(parcels, parcelPath);
			}
			List<DO.DroneCharge> droneCharge = XmlTools.LoadListFromXMLSerializer<DO.DroneCharge>(droneChargePath);
			XmlTools.SaveListToXMLSerializer<DO.DroneCharge>(droneCharge,droneChargePath);
			
		}
	}
}
