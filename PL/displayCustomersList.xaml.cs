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
using System.Runtime.InteropServices;
using System.Windows.Interop;


namespace PL
{
	/// <summary>
	/// Interaction logic for displayCustomersList.xaml
	/// </summary>
	public partial class displayCustomersList : Window
	{
		private BlApi.IBL bl;
		IEnumerable<BO.CustomerToList> customers = new List<BO.CustomerToList>();
		//-------------------------------------------------------------- FUNC AND CONST VARIABL -------------------------------------------------------------------------------------------------
		private const Int32 GWL_STYLE = -16;
		private const uint MF_BYCOMMAND = 0x00000000;
		private const uint MF_BYPOSITION = 0x00000400;
		private const uint SC_CLOSE = 0xF060;

		[DllImport("user32.dll")]
		static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

		[DllImport("user32.dll")]
		static extern uint RemoveMenu(IntPtr hMenu, uint nPosition, uint wFlags);

		protected override void OnSourceInitialized(EventArgs e)
		{
			base.OnSourceInitialized(e);

			WindowInteropHelper wih = new WindowInteropHelper(this);
			IntPtr hWnd = wih.Handle;
			IntPtr hMenu = GetSystemMenu(hWnd, false);

			// CloseButton disabled
			RemoveMenu(hMenu, SC_CLOSE, MF_BYCOMMAND);
		}
		//--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="ibl"></param>
		public displayCustomersList(BlApi.IBL ibl)
		{
			InitializeComponent();
			bl = ibl;
			customers = bl.GetListOfCustomers();
			CustomerlistView.ItemsSource = customers;
		}
		/// <summary>
		/// Close button
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
        private void Close_Click(object sender, RoutedEventArgs e) => Close();
		/// <summary>
		/// double click on a customer from the list to view his details and to update his name and phone number
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
        private void CustomerlistView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
			if ((BO.CustomerToList)CustomerlistView.SelectedItem == null)
				MessageBox.Show("Choose a drone !!", "ERROR");
			else
			{
				new CustomerWindowAdmin(bl, (BO.CustomerToList)CustomerlistView.SelectedItem).ShowDialog();
				customers = bl.GetListOfCustomers();
				CustomerlistView.ItemsSource = customers;
			}
		}
		// button add to add a new customer
        private void Add_Click(object sender, RoutedEventArgs e)
        {
			new CustomerWindowAdmin(bl).ShowDialog();
			customers = bl.GetListOfCustomers();
			CustomerlistView.ItemsSource = customers;
		}
    }
}
