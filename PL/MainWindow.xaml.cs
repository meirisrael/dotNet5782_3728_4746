using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PL
{
	/// <summary>
	/// Interaction logic for MainWindow1.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		BlApi.IBL ibl = BL.BlFactory.GetBl();
		manager meir = new manager(2610, "Ma2610");// project partner
		manager lior = new manager(4746, "Li4746");// project partner
		public MainWindow()
		{
			InitializeComponent();
		}

		/// <summary>
		/// user entere data
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"> warp </param>
		private void userIdBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			userIdBox.Background = Brushes.LightGreen;
			if (userIdBox.Text != "" && passwordBox.Password != "")
				connect_Button.IsEnabled = true;
			else connect_Button.IsEnabled = false;
		}
		/// <summary>
		/// user entere data
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"> warp </param>
		private void passwordBox_PasswordChanged(object sender, RoutedEventArgs e)
		{
			passwordBox.Background = Brushes.LightGreen;
			if (userIdBox.Text != "" && passwordBox.Password != "")
				connect_Button.IsEnabled = true;
			else connect_Button.IsEnabled = false;
		}
		/// <summary>
		/// if the user enter all data for conection
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"> click </param>
		private void Connect_Click(object sender, RoutedEventArgs e)
		{
			bool flag = false;
			foreach (BO.CustomerToList item in ibl.GetListOfCustomers())
			{
				if (userIdBox.Text == item.Id.ToString() && passwordBox.Password == (item.Name + item.Id.ToString()))
				{ new customerWindow(ibl, item).ShowDialog(); flag = true; }
			}
			if (!flag)
			{
				MessageBox.Show("the userId or password is incorect", "ERROR");
				passwordBox.Background = Brushes.Salmon;
				userIdBox.Background = Brushes.Salmon;
			}
		}
		/// <summary>
		/// if the user is not a customer that alredy exist add want to register
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void AddNewCustomer_Click(object sender, RoutedEventArgs e)
		{
			new customerWindow(ibl).ShowDialog();
		}


		/// <summary>
		///  user entere data
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"> warp </param>
		private void AdminIdBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			AdminIdBox.Background = Brushes.LightGreen;
			if (AdminIdBox.Text != "" && AdminpasswordBox.Password != "")
				AdminConnect_Button.IsEnabled = true;
			else AdminConnect_Button.IsEnabled = false;
		}
		/// <summary>
		///  user entere data
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"> warp </param>
		private void AdminpasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
		{
			AdminpasswordBox.Background = Brushes.LightGreen;
			if (AdminIdBox.Text != "" && AdminpasswordBox.Password != "")
				AdminConnect_Button.IsEnabled = true;
			else AdminConnect_Button.IsEnabled = false;
		}
		/// <summary>
		/// if the user enter all data for conection as a manager
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"> click </param>
		private void AdminConnect_Click(object sender, RoutedEventArgs e)
		{
			if ((AdminIdBox.Text == meir.userId.ToString() && AdminpasswordBox.Password == meir.Password) || (AdminIdBox.Text == lior.userId.ToString() && AdminpasswordBox.Password == lior.Password))
				new AdminWindow(ibl).ShowDialog();
			else MessageBox.Show("the userId or password is incorect", "ERROR");
		}

		/// <summary>
		/// this class represent a manager whith thier deatils for conection 
		/// </summary>
		class manager
		{
			internal int userId { get; }
			internal string Password { get; }
			public manager(int id,string code)
			{
				userId = id;
				Password = code;
			}
		}
	}
}
