using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginObject
{
	class LoginObject:LoginApi.ILogin
	{
		public LoginObject()
		{ DataLogin.Initialize(); }
		private static BlApi.IBL ibl;

		public void AddNewUser(int id,string name)
		{
			string code = name + id;
			DataLogin.Logins.Add(new PO.Login { userId = id, password = code, Status = PO.PersonStatus.Customer });
		}
		public void AddNewManager(int id, string code)
		{
			DataLogin.Logins.Add(new PO.Login { userId = id, password = code, Status = PO.PersonStatus.Manager });
		}


		public void UpdatePassword(int id,string code)
		{
			DataLogin.Logins.Find(l => l.userId == id).password = code;
		}
		public bool CheckLogin(int id,string code)
		{
			foreach (PO.Login item in DataLogin.Logins)
			{
				if (item.userId == id && item.password == code)
					return true;
			}
			return false;
		}
	}
}
