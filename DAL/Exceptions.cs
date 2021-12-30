using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DO
{

	// exceptions - problem with id
	public class IdExist : Exception//id of drone, base.... exist
	{
		public IdExist(string type_) : base($"DAL-ERROR: THIS {type_} ID ALREADY EXIST\n") { this._type = type_; }
		public string _type;
	}
	public class IdNotExist : Exception//id of drone, base.... not exist
	{
		public IdNotExist(string type_) : base($"DAL-ERROR: THIS {type_} ID DO NOT EXIST\n") { this._type = type_; }
		public string _type;
	}
	public class InvalidId : Exception// the id of base,drone.... is negative or equal to zero
	{
		public InvalidId(string type_) : base($"DAL-ERROR: ID OF {type_} TO BE BIGGER THAN ZERO\n") { this._type = type_; }
		public string _type;
	}

	//the range of longi or lati is not correct 
	public class InvalidLoc : Exception
	{
		public InvalidLoc(string type_, string range_) : base($"DAL-ERROR: {type_} NEED TO BE BEHTWEEN {range_}\n") { this._type = type_;this.range = range_; }
		public string _type;
		public string range;
	}

	// exceptions - problem with Categories
	public class InvalidCategory : Exception//WEIGHT PRIORITIES
	{
		public InvalidCategory(string type_) : base($"DAL-ERROR: THIS OPTION FOR {type_} IS NOT EXIST, YOU NEED TO GIVE AN OPTION BETWEEN 1 TO 3\n") { this._type = type_; }
		public string _type;
	}

	// if user inser negativ value for charge slots
	public class InvalidChargeSlot : Exception
	{
		public InvalidChargeSlot() : base("DAL-ERROR: CHARGE SLOT NEED TO BE BIGGER THAN ZERO OR EQUAL TO ZERO\n") { }
	}

	// exceptions specific for drone
	public class NegativeDroneId : Exception//if the user give an negative value when he need to give a drone id for parcel
	{
		public NegativeDroneId() : base("DAL-ERROR: THE DRONE ID NEED TO BE POSITIVE OR ZERO->NOT ASSIGNE TO AN SPECIFIC DRONE OR, BIGEER THAN ZERO->ASSIGNE TO AN SPECIFIC DRONE\n") { }
	}
	public class DroneNotInCharge : Exception// if the drone is not charging right now
	{
		public DroneNotInCharge() : base("DAL-ERROR: ERROR IN THE DATA BASE THE DRONE IS NOT CHARGE\n") { }
	}

	// exceptions for Weight that drone can't take
	public class ParcelTooHeavy : Exception//if the parcel is too heavy and the drone cant take
	{
		public ParcelTooHeavy() : base("DAL-ERROR: THE CATEGORIE OF PARCEL IS TOO HEAVY AND THE DRONE CAN'T TAKE THEM\n") { }
	}

	// exceptions specific for parcel
	public class SenderTargetIdEqual : Exception
	{
		public SenderTargetIdEqual() : base("DAL-ERROR: THE TARGET AND THE SENDER IS THE SAME PERSON\n") { }
	}
	// the parcel cant be removed 
	public class CantRemove : Exception
	{
		public CantRemove() : base("DAL ERROR: THE PARCEL CAN'T BE REMOVED BECAUSE IS ALREADY SCHEDULED TO A DRONE\n") { }
	}

	// if not request the right factory 
	public class FactoryError : Exception
	{
		public FactoryError() : base("DAL-ERROR: ONLY TWO FACTORY ARE EXISTS\n") { }
	}

	public class XMLFileLoadCreate : Exception
	{
		string type;
		public XMLFileLoadCreate(string type_, string msg) : base($"DAL-ERROR: {msg}\n") { type = type_; }
	}
}
