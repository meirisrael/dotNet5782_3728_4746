using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
	public class BlFactory
	{
		private static readonly object LockObj = new object();
		public static BlApi.IBL GetBl()
		{
			lock (LockObj)
			{
				return BL.GetInstance;
			}
			//return (IDal)DalObject.DalObject.LazySingleton.Instance;
			//return BL.GetInstance();
		}
	}
}
