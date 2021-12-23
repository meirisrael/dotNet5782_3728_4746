using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginObject
{
	static class DataLogin
	{
		internal static List<PO.Login> Logins;
		private static BlApi.IBL ibl;

		public static void Initialize()
		{
			ibl = BL.BlFactory.GetBl();
			foreach (BO.CustomerToList item in ibl.GetListOfCustomers())
			{
				Logins.Add(new PO.Login
				{
					userId = item.Id,
					password = item.Name + item.Id,
					Status=PO.PersonStatus.Customer
				}) ;
			}
			Logins.Add(new PO.Login
			{
				userId=2610,
				password="Ma26101",
				Status = PO.PersonStatus.Manager
			});
			Logins.Add(new PO.Login
			{
				userId = 4746,
				password = "Ls4746",
				Status = PO.PersonStatus.Manager
			});
		}
	}
}
