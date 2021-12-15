using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
	public class DalFactory
	{
		private static readonly object LockObj = new object();
		public static DalApi.IDal GetDal(string typeDL)
		{
			switch (typeDL)
			{
				case "List":
					lock (LockObj)
					{
						return DalObject.DalObject.GetInstance;
					}
					break;
				case "XML":
				//	return;
				//	break;
				default:
					throw new DO.FactoryError();
			}
		}
	}
}
