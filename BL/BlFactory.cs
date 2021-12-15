using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
	/// <summary>
	/// call the instance for bl
	/// </summary>
	public class BlFactory
	{
		private static readonly object LockObj = new object();
		public static BlApi.IBL GetBl()
		{
			lock (LockObj)//tread safe
			{
				return BL.GetInstance;//singeltone
			}
			//return (IDal)DalObject.DalObject.LazySingleton.Instance;
			//return BL.GetInstance();
		}
	}
}
