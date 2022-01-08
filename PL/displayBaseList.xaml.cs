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
	/// Interaction logic for displayBaseList.xaml
	/// </summary>
	public partial class displayBaseList : Window
	{
		private BlApi.IBL bl;
		IEnumerable<BO.BaseToList> baseStations = new List<BO.BaseToList>();
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
		/// ctor
		/// </summary>
		/// <param name="ibl"></param>
		public displayBaseList(BlApi.IBL ibl)
		{
			InitializeComponent();
			bl = ibl;
			baseStations = bl.GetListOfBaseStations(b => true);
			BaseListView.ItemsSource = baseStations;
		}

		/// <summary>
		/// if the user want to group the list according to avalible charging stations
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void groupButton_Click(object sender, RoutedEventArgs e)
		{
			if (groupButton.Content.ToString() == "Group the List")
			{
				BaseListViewGrouping.ItemsSource = baseStations;
				CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(BaseListViewGrouping.ItemsSource);
				PropertyGroupDescription groupDescription = new PropertyGroupDescription("ChargeSlots");
				view.GroupDescriptions.Add(groupDescription);
				BaseListViewGrouping.Visibility = Visibility.Visible;
				BaseListView.Visibility = Visibility.Hidden;
				groupButton.Content = "Default display";
			}
			else if (groupButton.Content.ToString() == "Default display")
			{
				BaseListViewGrouping.Visibility = Visibility.Hidden;
				BaseListView.Visibility = Visibility.Visible;
				groupButton.Content = "Group the List";
				baseStations = bl.GetListOfBaseStations(d => true);
				BaseListViewGrouping.ItemsSource = baseStations;
			}
		}
		/// <summary>
		/// if the user want to add an new base station
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"> click </param>
		private void Add_Click(object sender, RoutedEventArgs e)
		{
			new baseWindow(bl).ShowDialog();
			baseStations = bl.GetListOfBaseStations(b => true);
			BaseListView.ItemsSource = baseStations;
		}

		/// <summary>
		/// if the user want to update data on specific base station
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"> doubul click </param>
		private void BaseListViewGrouping_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if ((BO.BaseToList)BaseListViewGrouping.SelectedItem == null)
				MessageBox.Show("Choose a base-station !!", "ERROR");
			else
			{
				new baseWindow(bl, (BO.BaseToList)BaseListViewGrouping.SelectedItem).ShowDialog();
				baseStations = bl.GetListOfBaseStations(b => true);
				BaseListViewGrouping.ItemsSource = baseStations;
				CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(BaseListViewGrouping.ItemsSource);
				PropertyGroupDescription groupDescription = new PropertyGroupDescription("ChargeSlots");
				view.GroupDescriptions.Add(groupDescription);
			}
		}
		/// <summary>
		/// if the user want to update data on specific base station
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"> double click </param>
		private void BaseListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if ((BO.BaseToList)BaseListView.SelectedItem == null)
				MessageBox.Show("Choose a base-station !!", "ERROR");
			else
			{
				new baseWindow(bl, (BO.BaseToList)BaseListView.SelectedItem).ShowDialog();
				baseStations = bl.GetListOfBaseStations(b => true);
				BaseListView.ItemsSource = baseStations;
			}
		}

		/// <summary>
		/// if the user want to close or cancel the page
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"> click </param>
		private void Close_Click(object sender, RoutedEventArgs e) => Close();
	}
}
