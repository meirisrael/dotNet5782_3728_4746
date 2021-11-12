using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDAL
{
	namespace DO
	{
		// exceptions for base
		public class BaseIdExist : Exception
		{
			public BaseIdExist() : base("ERROR: THIS BASE-STATION ID ALREADY EXIST\n") { }
		}
		public class BaseIdNotExist : Exception
		{
			public BaseIdNotExist() : base("ERROR: THIS BASE-STATION ID DO NOT EXIST\n") { }
		}
		public class InvalidBaseId : Exception
		{
			public InvalidBaseId() : base("ERROR: ID OF BASE STATION NEED TO BE BIGGER THAN ZERO\n") { }
		}
		public class InvalidChargeSlot : Exception
		{
			public InvalidChargeSlot() : base("ERROR: CHARGE SLOT NEED TO BE BIGGER THAN ZERO OR EQUAL TO ZERO\n") { }
		}
		public class InvalidLongitude : Exception
		{
			public InvalidLongitude() : base("ERROR: LONGITUD NEED TO BE BEHTWEEN -180 TO 180\n") { }
		}
		public class InvalidLatitude : Exception
		{
			public InvalidLatitude() : base("ERROR: LATITUDE NEED TO BE BEHTWEEN -90 TO 90\n") { }
		}

		// exceptions for drone
		public class DorneIdExist : Exception
		{
			public DorneIdExist() : base("ERROR: THIS DRONE ID ALREADY EXIST\n") { }
		}
		public class DroneIdNotExist : Exception
		{
			public DroneIdNotExist() : base("ERROR: THIS DRONE ID DO NOT EXIST\n") { }
		}
		public class InvalidDroneId : Exception
		{
			public InvalidDroneId() : base("ERROR: ID OF DRONE NEED TO BE BIGGER THAN ZERO OR EQUAL\n") { }
		}
		public class InvalidWeight : Exception
		{
			public InvalidWeight() : base("ERROR: THIS OPTION FOR WEIGHT IS NOT EXIST, YOU NEED TO GIVE AN  OPTION BETWEEN 1 TO 3\n") { }
		}
		public class NegativeDorneId : Exception
		{
			public NegativeDorneId() : base("ERROR: THE DRONE ID NEED TO BE POSITIVE OR ZERO->NOT ASSIGNE TO AN SPECIFIC DRONE OR, BIGEER THAN ZERO->ASSIGNE TO AN SPECIFIC DRONE\n") { }
		}

		// exceptions for customer
		public class CustomerIdExist : Exception
		{
			public CustomerIdExist() : base("ERROR: THIS CUSTOMER ID ALREADY EXIST\n") { }
		}
		public class CustomerIdNotExist : Exception
		{
			public CustomerIdNotExist() : base("ERROR: THIS CUSTOMER ID DO NOT EXIST\n") { }
		}
		public class InvalidCustomerId : Exception
		{
			public InvalidCustomerId() : base("ERROR: ID OF CUSTOMER NEED TO BE BIGGER THAN ZERO\n") { }
		}

		// exceptions for parcel
		public class ParcelIdExist : Exception
		{
			public ParcelIdExist() : base("ERROR: THIS PARCEL ID ALREADY EXIST\n") { }
		}
		public class ParcelIdNotExist : Exception
		{
			public ParcelIdNotExist() : base("ERROR: THIS PARCEL ID DO NOT EXIST\n") { }
		}
		public class InvalidParcelId : Exception
		{
			public InvalidParcelId() : base("ERROR: ID OF PARCEL NEED TO BE BIGGER THAN ZERO\n") { }
		}
		public class InvalidPriority : Exception
		{
			public InvalidPriority() : base("ERROR: THIS  OPTION FOR PRIORITIES IS NOT EXIST, YOU NEED TO GIVE AN  OPTION BETWEEN 1 TO 3\n") { }
		}

		// exceptions for sender
		public class SenderIdNotExist : Exception
		{
			public SenderIdNotExist() : base("ERROR: THIS SENDER ID DO NOT EXIST\n") { }
		}
		public class InvalidSenderId : Exception
		{
			public InvalidSenderId() : base("ERROR: ID OF SENDER NEED TO BE BIGGER THAN ZERO\n") { }
		}
		// exceptions for target
		public class TargetIdNotExist : Exception
		{
			public TargetIdNotExist() : base("ERROR: THIS TARGET ID DO NOT EXIST\n") { }
		}
		public class InvalidTargetId : Exception
		{
			public InvalidTargetId() : base("ERROR: ID OF SENDER NEED TO BE BIGGER THAN ZERO\n") { }
		}
	}
}

