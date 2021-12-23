using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginApi
{
	interface ILogin
	{
		public void AddNewUser(int id, string name);
		public void AddNewManager(int id, string code);
		public void UpdatePassword(int id, string code);
		public bool CheckLogin(int id, string code);
	}
}
