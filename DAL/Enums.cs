using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDAL
{
	namespace DO
	{
		public enum Choice
		{
			ADD,UPDATE,DISPLAY,VIEW_LIST,EXIT
		}
		public enum WeightCategories
		{
			Light=1,Medium,Heavy
		}
		public enum DroneStatuses
		{
			free=1,Maintenance,Shipping
		}
		public enum Priorities
		{
			Normal=1,fast,Emergecey
		}
	}
}
