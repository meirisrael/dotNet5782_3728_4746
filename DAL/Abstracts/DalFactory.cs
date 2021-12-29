using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
	/// <summary>
	/// choose which type return dal with list or dal with xml
	/// </summary>
	public class DalFactory
	{
		private static readonly object LockObj = new object();
		
		public static DalApi.IDal GetDal(string typeDL)
		{
			switch (typeDL)
			{
				case "List"://if list type
					lock (LockObj)//thread safe
					{
						return Dal.DalObject.GetInstance;//singeltone
					}
				case "XML"://if XML type

				default:
					throw new DO.FactoryError();
			}
		}
	}
}
