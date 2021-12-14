using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
	public class BlFactory
	{
		public static BlApi.IBL GetBl()
		{
			//return (IDal)DalObject.DalObject.LazySingleton.Instance;
			return BL.GetInstance();
		}
	}
}
