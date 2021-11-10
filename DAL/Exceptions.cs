using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDAL
{
	namespace DO
	{
		public class InvalidIdBase : Exception
		{
			public InvalidIdBase() : base("ERROR: THIS BASE-STATION ID ALREADY EXIST\n") { }
		}
		public class InvalidIdBaseExist : Exception
		{
			public InvalidIdBaseExist() : base("ERROR: THIS BASE-STATION ID DO NOT EXIST\n") { }
		}

		public class InvalidIdDrone : Exception
		{
			public InvalidIdDrone() : base("ERROR: THIS DRONE ID ALREADY EXIST\n") { }
		}

		public class InvalidIdDroneExist : Exception
		{
			public InvalidIdDroneExist() : base("ERROR: THIS DRONE ID DO NOT EXIST\n") { }
		}

		public class InvalidIdCustomer : Exception
		{
			public InvalidIdCustomer() : base("ERROR: THIS CUSTOMER ID ALREADY EXIST\n") { }
		}

		public class InvalidIdParcel : Exception
		{
			public InvalidIdParcel() : base("ERROR: THIS PARCEL ID ALREADY EXIST\n") { }
		}

		public class InvalidIdParcelExist : Exception
		{
			public InvalidIdParcelExist() : base("ERROR: THIS PARCEL ID DO NOT EXIST\n") { }
		}

		public class InvalidIdSender : Exception
		{
			public InvalidIdSender() : base("ERROR: THIS SENDER ID DO NOT EXIST\n") { }
		}

		public class InvalidIdTarget : Exception
		{
			public InvalidIdTarget() : base("ERROR: THIS TARGET ID DO NOT EXIST\n") { }
		}
		public class InvalidIdCustomerExist : Exception
		{
			public InvalidIdCustomerExist() : base("ERROR: THIS CUSTOMER ID DO NOT EXIST\n") { }
		}
		public class NegativeId : Exception
		{
			public NegativeId() : base("ERROR: ID CAN'T BE NEGATIVE\n") { }
		}
		public class IdEqualToZero : Exception
		{
			public IdEqualToZero() : base("ERROR: ID CAN'T BE EQAUL TO ZERO\n") { }
		}
		public class NegativeChargeSlot : Exception
		{
			public NegativeChargeSlot() : base("ERROR: CHARGE SLOT CAN'T BE NEGATIVE\n") { }
		}
		public class InvalidLongitude : Exception
		{
			public InvalidLongitude() : base("ERROR: LONGITUD NEED TO BE BEHTWEEN -180 TO 180\n") { }
		}
		public class InvalidLatitude : Exception
		{
			public InvalidLatitude() : base("ERROR: LATITUDE NEED TO BE BEHTWEEN -90 TO 90\n") { }

		}
	}
}

