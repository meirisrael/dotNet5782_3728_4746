using System;
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
			DataSource.Initialize();
			XmlTools.SaveListToXMLSerializer<DO.BaseStation>(DataSource.baseStations,baseStationPath);
			XmlTools.SaveListToXMLSerializer<DO.Drone>(DataSource.drones, dronePath);
			XmlTools.SaveListToXMLSerializer<DO.DroneCharge>(DataSource.droneCharges, droneChargePath);
			XmlTools.SaveListToXMLSerializer<DO.Customer>(DataSource.customers, customerPath);
			XmlTools.SaveListToXMLSerializer<DO.Parcel>(DataSource.parcels, parcelPath);
		}
	}
}
