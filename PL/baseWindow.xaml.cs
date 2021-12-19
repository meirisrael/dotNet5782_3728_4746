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
	/// Interaction logic for baseWindow.xaml
	/// </summary>
	public partial class baseWindow : Window
	{
		private BlApi.IBL bl;
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
		public baseWindow(BlApi.IBL ibl)
		{
			InitializeComponent();
			bl = ibl;
		}

		private void ChargeBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			ChargeBox.Background = null;
			if (LatitudeBox.Text != "" && LongitudeBox.Text != "" && NameBox.Text != "" && IdBox.Text != "" && ChargeBox.Text != "")
				addButton.IsEnabled = true;
			else addButton.IsEnabled = false;
		}

		private void LatitudeBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			LatitudeBox.Background = null;
			if (ChargeBox.Text != "" && LongitudeBox.Text != "" && NameBox.Text != "" && IdBox.Text != "" && LatitudeBox.Text != "")
				addButton.IsEnabled = true;
			else addButton.IsEnabled = false;
		}

		private void LongitudeBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			LongitudeBox.Background = null;
			if (LatitudeBox.Text != "" && ChargeBox.Text != "" && NameBox.Text != "" && IdBox.Text != "" && LongitudeBox.Text != "")
				addButton.IsEnabled = true;
			else addButton.IsEnabled = false;
		}

		private void NameBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			NameBox.Background = null;
			if (LatitudeBox.Text != "" && LongitudeBox.Text != "" && ChargeBox.Text != "" && IdBox.Text != "" && NameBox.Text != "")
				addButton.IsEnabled = true;
			else addButton.IsEnabled = false;
		}

		private void IdBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			IdBox.Background = null;
			if (LatitudeBox.Text != "" && LongitudeBox.Text != "" && NameBox.Text != "" && ChargeBox.Text != "" && IdBox.Text != "")
				addButton.IsEnabled = true;
			else addButton.IsEnabled = false;
		}

		private void Cancel_Click(object sender, RoutedEventArgs e) => Close();

		private void add_button_Click(object sender, RoutedEventArgs e)
		{
			string id = IdBox.Text;
			string name = NameBox.Text;
			string charge = ChargeBox.Text;
			string longi = LongitudeBox.Text;
			string lati = LatitudeBox.Text;
			int baseId, baseName, chargeSlots;
			double longitude, latitude;

			if (!int.TryParse(id, out baseId))
			{
				MessageBox.Show("Base-Station ID most be an intenger", "ERROR");
				IdBox.Background = Brushes.Salmon;
				return;
			}
			else baseId = int.Parse(id);

			if (!int.TryParse(name, out baseName))
			{
				MessageBox.Show("Base-Station Name most be an intenger", "ERROR");
				IdBox.Background = Brushes.Salmon;
				return;
			}
			else baseName = int.Parse(name);

			if (!int.TryParse(charge, out chargeSlots))
			{
				MessageBox.Show("Base-Station charge-slots most be an intenger", "ERROR");
				IdBox.Background = Brushes.Salmon;
				return;
			}
			else chargeSlots = int.Parse(charge); 
			
			if (!double.TryParse(longi, out longitude))
			{
				MessageBox.Show("Base-Station longitude most be an intenger", "ERROR");
				IdBox.Background = Brushes.Salmon;
				return;
			}
			else longitude = double.Parse(longi); 
			
			if (!double.TryParse(lati, out latitude))
			{
				MessageBox.Show("Base-Station latitude most be an intenger", "ERROR");
				IdBox.Background = Brushes.Salmon;
				return;
			}
			else latitude = double.Parse(lati);
			BO.Location loc = new BO.Location { Longitude = longitude, Latitude = latitude };
			try
			{
				bl.AddBaseStation(baseId, baseName, chargeSlots, loc);
				MessageBox.Show("Successfuly added", "Successfull");
				Close();
			}
			catch (BO.InvalidId)//drone id
			{
				MessageBox.Show("ID need to be bigger than zero", "ERROR");
				IdBox.Background = Brushes.Salmon;
			}
			catch (BO.InvalidChargeSlot)
			{
				MessageBox.Show("Charge-slots need to be bigger than zero", "ERROR");
				IdBox.Background = Brushes.Salmon;
			}
			catch (BO.InvalidLoc ex)
			{
				if (ex.type == "LATITUDE")
				{
					MessageBox.Show($"Latitude need to be between {ex.range}", "ERROR");
					LatitudeBox.Background = Brushes.Salmon;
				}
				else
				{
					MessageBox.Show($"Longitude need to be between {ex.range}", "ERROR");
					LongitudeBox.Background = Brushes.Salmon;
				}
			}
			catch (BO.IdExist)
			{
				MessageBox.Show("ID alredy exist", "ERROR");
				IdBox.Background = Brushes.Salmon;
			}
		}
	}
}