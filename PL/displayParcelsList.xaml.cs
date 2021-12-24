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
	/// Interaction logic for displayParcelsList.xaml
	/// </summary>
	public partial class displayParcelsList : Window
	{
		private BlApi.IBL bl;
		private IEnumerable<BO.ParcelToList> parcels;
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
		public displayParcelsList(BlApi.IBL ibl)
		{
			InitializeComponent();
			bl = ibl;
			StatusSelector.ItemsSource = Enum.GetValues(typeof(BO.ParcelStatues));
			ParcelListView.Visibility = Visibility.Visible;
			parcels = bl.GetListOfParcels(p => true);
			ParcelListView.ItemsSource = parcels;
		}


		/// <summary>
		/// if the user want to add an new parcel 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"> click </param>
		private void Add_Click(object sender, RoutedEventArgs e)
		{
			new ParcelWindowAdmin(bl).ShowDialog();
			parcels = bl.GetListOfParcels(p => true);
			ParcelListView.ItemsSource = parcels;
		}
		/// <summary>
		/// if the user want to group the list of parcel by the sender of parcel or return to default view
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"> click </param>
		private void groupButton_Click(object sender, RoutedEventArgs e)
		{
			if (groupButton.Content.ToString() == "Group the List")
			{
				ParcelListViewGrouping.ItemsSource = parcels;
				CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(ParcelListViewGrouping.ItemsSource);
				PropertyGroupDescription groupDescription = new PropertyGroupDescription("NameSender");
				view.GroupDescriptions.Add(groupDescription);
				ParcelListViewGrouping.Visibility = Visibility.Visible;
				ParcelListView.Visibility = Visibility.Hidden;
				groupButton.Content = "Default display";
				StatusSelector.IsEnabled = false;
			}
			else if (groupButton.Content.ToString() == "Default display")
			{
				ParcelListViewGrouping.Visibility = Visibility.Hidden;
				ParcelListView.Visibility = Visibility.Visible;
				groupButton.Content = "Group the List";
				parcels = bl.GetListOfParcels(d => true);
				ParcelListView.ItemsSource = parcels;
				StatusSelector.IsEnabled = true;
			}
		}
		/// <summary>
		/// clear selection and retur to default view 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"> click </param>
		private void Clear_Click(object sender, RoutedEventArgs e)
		{
			StatusSelector.SelectedItem = null;
			parcels = bl.GetListOfParcels(p => true);
			ParcelListView.ItemsSource = parcels;
		}

		/// <summary>
		/// if the user change selction to see different parcel by her status of delivery
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void StatusSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{ filterByStatus(); }


		/// <summary>
		/// if the user want to see details or update data of specific parcel
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"> Double Click </param>
		private void ParcelListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if ((BO.ParcelToList)ParcelListView.SelectedItem == null)
				MessageBox.Show("Choose a drone !!", "ERROR");
			else
			{
				new ParcelWindowAdmin(bl, (BO.ParcelToList)ParcelListView.SelectedItem).ShowDialog();
				parcels = bl.GetListOfParcels(p => true);
				ParcelListView.ItemsSource = parcels;
				filterByStatus();
			}
		}
		/// <summary>
		/// if the user want to see details or update data of specific parcel
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"> Double Click </param>
		private void ParcelListViewGrouping_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if ((BO.ParcelToList)ParcelListViewGrouping.SelectedItem == null)
				MessageBox.Show("Choose a drone !!", "ERROR");
			else
			{
				new ParcelWindowAdmin(bl, (BO.ParcelToList)ParcelListViewGrouping.SelectedItem).ShowDialog();
				parcels = bl.GetListOfParcels(p => true);
				ParcelListViewGrouping.ItemsSource = parcels;
				filterByStatus();
			}
		}

		/// <summary>
		/// filter the list view by the status of parcel
		/// </summary>
		private void filterByStatus()
		{
			if (StatusSelector.SelectedItem == null)
				return;
			else if (StatusSelector.SelectedItem != null)
			{
				parcels = bl.GetListOfParcels(p => true);
				parcels = parcels.ToList().FindAll(p => p.Status == (BO.ParcelStatues)StatusSelector.SelectedItem);
			}
			else
				parcels = bl.GetListOfParcels(d => true);
			ParcelListView.ItemsSource = parcels;
		}

		/// <summary>
		/// if the user want to close the page
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"> click </param>
		private void Close_Click(object sender, RoutedEventArgs e) => Close();
	}
}
