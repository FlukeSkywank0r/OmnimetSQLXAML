using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Excel = Microsoft.Office.Interop.Excel;
using System.Data.SqlClient;
using Microsoft.Office.Core;
using System.Runtime.InteropServices;
using System.Data;
using System.Collections;
using System.Linq;
using System.Windows.Input;
using System.Drawing;

namespace OmnimetSQLXAML
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        private SqlDbConnect con;

        public MainWindow()
        {
            InitializeComponent();

            txtLicense.Text = "";
            cmbSoft.Text = "";
            txtVersion.Text = "";
            cmbSType.Text = "";
            txtCustomer.Text = "";
            cmbSeller.Text = "";
            txtNotes.Text = "";

            try
            {
                con = new SqlDbConnect();

                con.SqlQuery("SELECT * FROM LICENSES");
                con.QueryEx();
                DataTable dt = con.QueryEx();
                dataGrid.ItemsSource = dt.DefaultView;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.ToString(), "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }

        }

        private DateTime _selectedDate;
        public string theDate;

        public DateTime SelectedDate
        {
            get
            {
                return _selectedDate;
            }
            set
            {
                _selectedDate = value;
                theDate = _selectedDate.ToShortDateString();
            }
        }

        public IEnumerable<DataGridRow> GetDataGridRows(DataGrid grid)
        {
            var itemsSource = grid.ItemsSource as IEnumerable;
            if (null == itemsSource) yield return null;
            foreach (var item in itemsSource)
            {
                var row = grid.ItemContainerGenerator.ContainerFromItem(item) as DataGridRow;
                if (null != row) yield return row;
            }
        }

        private void SelectedRow(object sender, SelectionChangedEventArgs e)
        {
            string s = "";
            for (int i = 0; i < dataGrid.SelectedItems.Count; i++)
                if (dataGrid.SelectedItems[i] is DataRowView)
                {
                    DataRowView drv = dataGrid.SelectedItems[i] as DataRowView;
                    s += drv.Row["ID"] + ", " + drv.Row["License"] + ", " + drv.Row["Software"] + ", " + drv.Row["Version"] + ", " + drv.Row["SType"] + ", " + drv.Row["Dongle"] + ", " + drv.Row["PDate"] + ", " + drv.Row["Customer"] + ", " + drv.Row["Seller"] + ", " + drv.Row["Notes"] + "\n";

                    txtID.Text = drv.Row["ID"].ToString();
                    txtLicense.Text = drv.Row["License"].ToString();
                    cmbSoft.Text = drv.Row["Software"].ToString();
                    txtVersion.Text = drv.Row["Version"].ToString();
                    cmbSType.Text = drv.Row["SType"].ToString();
                    string DongleX = drv.Row["Dongle"].ToString();
                    txtDongle.Text = DongleX;

                    if (DongleX == "True")
                    {
                        chkDongle.IsChecked = true;
                    }
                    else
                    {
                        chkDongle.IsChecked = false;
                    }

                    dPick.Text = drv.Row["PDate"].ToString();
                    var inMyString = dPick.SelectedDate.Value.ToShortDateString();
                    dPick.SelectedDate = DateTime.Parse(inMyString);
                    txtCustomer.Text = drv.Row["Customer"].ToString();
                    cmbSeller.Text = drv.Row["Seller"].ToString();
                    txtNotes.Text = drv.Row["Notes"].ToString();
                }

            if (s != "")
            {
                System.Windows.MessageBox.Show(s);
            }
        }

        private void DataGrid_Details_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var row_list = GetDataGridRows(dataGrid);
                foreach (DataGridRow single_row in row_list)
                {
                    if (single_row.IsSelected == true)
                    {
                        System.Windows.MessageBox.Show("the row no." + single_row.GetIndex().ToString() + " is selected!");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnInsert_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtLicense.Text))
            {
                System.Windows.MessageBox.Show("License, Software and Customer fields need to be filled", "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
            if (string.IsNullOrWhiteSpace(cmbSoft.Text))
            {
                MessageBox.Show("License, Software and Customer fields need to be filled", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            if (string.IsNullOrWhiteSpace(txtCustomer.Text))
            {
                MessageBox.Show("License, Software and Customer fields need to be filled", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            else {
                try
                {
                    string theDate = dPick.Text.ToString();

                    SqlConnection con = new SqlConnection(@"Data Source =BOSCHERM-W764N\SQLEXPRESS; Initial Catalog = Omnimet;");
                    SqlConnection myConnection = new SqlConnection("user id=itwbuehler;" +
                                               "password=05370537;server=localhost;" +
                                               "Trusted_Connection=yes;" +
                                               "database=Omnimet; " +
                                               "connection timeout=30");

                    string query =
                        "Insert into Licenses (License, Software, Version, SType, Dongle, PDate, Customer, Seller, Notes)"
                        + "values(@License, @Software, @Version, @SType, @Dongle, @PDate, @Customer, @Seller, @Notes )";
                    SqlCommand Command = new SqlCommand(query, myConnection);

                    Command.Parameters.AddWithValue("@License", txtLicense.Text);
                    Command.Parameters.AddWithValue("@Software", cmbSoft.Text);
                    Command.Parameters.AddWithValue("@Version", int.Parse(txtVersion.Text));
                    Command.Parameters.AddWithValue("@SType", cmbSType.Text);
                    Command.Parameters.AddWithValue("@Dongle", chkDongle.IsChecked);
                    Command.Parameters.AddWithValue("@PDate", theDate);
                    Command.Parameters.AddWithValue("@Customer", txtCustomer.Text);
                    Command.Parameters.AddWithValue("@Seller", cmbSeller.Text);
                    Command.Parameters.AddWithValue("@Notes", txtNotes.Text);

                    SqlDataAdapter dataAdp = new SqlDataAdapter(Command);
                    System.Data.DataTable dt = new System.Data.DataTable("Licenses");
                    dataAdp.Fill(dt);
                    dataGrid.ItemsSource = dt.DefaultView;

                    System.Windows.MessageBox.Show("The data has been added successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    myConnection.Close();

                    chkDongle.IsChecked = !chkDongle.IsChecked;
                }
                catch (System.Exception excep)
                {
                    MessageBox.Show(excep.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnAdmin_Click(object sender, RoutedEventArgs e)
        {
            grpPassword.Visibility = Visibility.Visible;
            txtPass.Visibility = Visibility.Visible;
            btnEnter.Visibility = Visibility.Visible;
        }

        private void btnEnter_Click(object sender, RoutedEventArgs e)
        {
            if (txtPass.Text == ("itwbuehler"))
            {
                //System.Windows.Application.Current.MainWindow.Height = 650;
                txttheDate.Visibility = Visibility.Visible;
                txtID.Visibility = Visibility.Visible;
                lblID.Visibility = Visibility.Visible;
                btnInsert.Visibility = Visibility.Visible;
                btnUpdate.Visibility = Visibility.Visible;
                btnView.Visibility = Visibility.Visible;
                btnDelete.Visibility = Visibility.Visible;
                txtAxlLines.Visibility = Visibility.Visible;
                btnCloseAdmin.Visibility = Visibility.Visible;
            }
            else
            {
                System.Windows.MessageBox.Show("Please try again", "Wrong Password!", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnCloseAdmin_Click(object sender, RoutedEventArgs e)
        {
            //System.Windows.Application.Current.MainWindow.Height = 590;
            grpPassword.Visibility = Visibility.Hidden;
            txtPass.Visibility = Visibility.Hidden;
            btnEnter.Visibility = Visibility.Hidden;
            txttheDate.Visibility = Visibility.Hidden;
            txtID.Visibility = Visibility.Hidden;
            lblID.Visibility = Visibility.Hidden;
            btnInsert.Visibility = Visibility.Hidden;
            btnUpdate.Visibility = Visibility.Hidden;
            btnView.Visibility = Visibility.Hidden;
            btnDelete.Visibility = Visibility.Hidden;
            txtAxlLines.Visibility = Visibility.Hidden;
            btnCloseAdmin.Visibility = Visibility.Hidden;
            txtPass.Text = "";
        }

        private void cmbSBox_Loaded(object sender, RoutedEventArgs e)
        {
            // ... A List.
            List<string> data = new List<string>();
            data.Add("License");
            data.Add("Software");
            data.Add("Seller");
            data.Add("Customer");
            data.Add("Notes");

            // ... Get the ComboBox reference.
            var cmbSBox = sender as ComboBox;

            // ... Assign the ItemsSource to the List.
            cmbSBox.ItemsSource = data;

            // ... Make the first item selected.
            cmbSBox.SelectedIndex = 0;
        }

        private void cmbSBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // ... Get the ComboBox.
            var cmbSoft = sender as ComboBox;

            /* ... Set SelectedItem as Window Title.
            string value = cmbSBox.SelectedItem as string;
            this.Title = "Selected: " + value;*/
        }

        private void cmbSoft_Loaded(object sender, RoutedEventArgs e)
        {
            // ... A List.
            List<string> data = new List<string>();
            data.Add("DiaMet");
            data.Add("MHT");
            data.Add("Minuteman");
            data.Add("Omnimet");
            data.Add("WinControl");

            // ... Get the ComboBox reference.
            var cmbSoft = sender as ComboBox;

            // ... Assign the ItemsSource to the List.
            cmbSoft.ItemsSource = data;

            // ... Make the first item selected.
            cmbSoft.SelectedIndex = 0;
        }

        private void cmbSoft_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // ... Get the ComboBox.
            var cmbSBox = sender as ComboBox;

            /* ... Set SelectedItem as Window Title.
            string value = cmbSBox.SelectedItem as string;
            this.Title = "Selected: " + value;*/
        }

        private void cmbSType_Loaded(object sender, RoutedEventArgs e)
        {
            // ... A List.
            List<string> data = new List<string>();
            data.Add("Basic");
            data.Add("Demo");
            data.Add("Enterprise");
            data.Add("Capture & Measure");
            data.Add("Omnimet");
            data.Add("WinControl");

            // ... Get the ComboBox reference.
            var cmbSType = sender as ComboBox;

            // ... Assign the ItemsSource to the List.
            cmbSType.ItemsSource = data;

            // ... Make the first item selected.
            cmbSType.SelectedIndex = 0;
        }

        private void cmbSType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // ... Get the ComboBox.
            var cmbSType = sender as ComboBox;

            /* ... Set SelectedItem as Window Title.
            string value = cmbSBox.SelectedItem as string;
            this.Title = "Selected: " + value;*/
        }

        private void cmbSeller_Loaded(object sender, RoutedEventArgs e)
        {
            // ... A List.
            List<string> data = new List<string>();
            data.Add("Kozole");
            data.Add("Pascher");
            data.Add("Service");
            data.Add("Straub");
            data.Add("Ziegenhagen");

            // ... Get the ComboBox reference.
            var cmbSBox = sender as ComboBox;

            // ... Assign the ItemsSource to the List.
            cmbSBox.ItemsSource = data;

            // ... Make the first item selected.
            cmbSBox.SelectedIndex = 0;
        }

        private void cmbSeller_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // ... Get the ComboBox.
            var cmbSeller = sender as ComboBox;

            /* ... Set SelectedItem as Window Title.
            string value = cmbSBox.SelectedItem as string;
            this.Title = "Selected: " + value;*/
        }

        private void cmbSearch_Loaded(object sender, RoutedEventArgs e)
        {
            // ... A List.
            List<string> data = new List<string>();
            data.Add("License");
            data.Add("Software");
            data.Add("Version");
            data.Add("Software Type");
            data.Add("Customer");
            data.Add("Seller");

            // ... Get the ComboBox reference.
            var cmbSBox = sender as ComboBox;

            // ... Assign the ItemsSource to the List.
            cmbSBox.ItemsSource = data;

            // ... Make the first item selected.
            cmbSBox.SelectedIndex = 0;
        }

        private void cmbSearch_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // ... Get the ComboBox.
            var cmbSeller = sender as ComboBox;

            /* ... Set SelectedItem as Window Title.
            string value = cmbSBox.SelectedItem as string;
            this.Title = "Selected: " + value;*/
        }

        private void chkDongle_Checked(object sender, RoutedEventArgs e)
        {
            Handle(sender as System.Windows.Controls.CheckBox);
        }

        private void chkDongle_Unchecked(object sender, RoutedEventArgs e)
        {
            Handle(sender as System.Windows.Controls.CheckBox);
        }

        void Handle(System.Windows.Controls.CheckBox chkDongle)
        {
            // Use IsChecked.
            bool flag = chkDongle.IsChecked.Value;

            // Assign Window Title.
            this.Title = "IsChecked = " + flag.ToString();
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bool Checkerle = false;

                if (chkDongle.IsChecked.Value == true)
                {
                    Checkerle = true;
                }

                con = new SqlDbConnect();

                con.SqlQuery("UPDATE Licenses SET License='" + txtLicense.Text + "', Software='" + cmbSoft.Text + "', Version='" + txtVersion.Text + "', SType='" + cmbSType.Text + "', Dongle='" + Checkerle + "', PDate='" + dPick.Text + "', Customer='" + txtCustomer.Text + "', Seller='" + cmbSeller.Text + "', Notes='" + txtNotes.Text + "' WHERE ID ='" + txtID.Text + "'");
                con.QueryEx();
                DataTable dt = con.QueryEx();
                dataGrid.ItemsSource = dt.DefaultView;

                string y = "";
                y = theDate + "UPDATE Licenses SET License='" + txtLicense.Text + "', Software='" + cmbSoft.Text + "', Version='" + txtVersion.Text + "', SType='" + cmbSType.Text + "', Dongle='" + Checkerle + "', PDate='" + dPick.Text + "', Customer='" + txtCustomer.Text + "', Seller='" + cmbSeller.Text + "', Notes='" + txtNotes.Text + "' WHERE ID ='" + txtID.Text + "'";

                System.Windows.MessageBox.Show("The data has been updated successfully", "Success", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);

                chkDongle.IsChecked = !chkDongle.IsChecked;

                if (y != "")
                {
                    System.Windows.MessageBox.Show(y);
                }
            }
            catch (System.Exception excep)
            {
                System.Windows.MessageBox.Show(excep.Message, "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            //  if (cmbSBox.Text == "License")
            {
                con = new SqlDbConnect();

                con.SqlQuery("SELECT ID, License, Software, Version, SType, Dongle, PDate, Customer, Seller, Notes From Licenses where License like '%" + txtSearch.Text + "%'");
                con.QueryEx();
                DataTable dt = con.QueryEx();
                dataGrid.ItemsSource = dt.DefaultView;
            }
            if (cmbSearch.Text == "Software")
            {
                con = new SqlDbConnect();

                con.SqlQuery("SELECT ID, License, Software, Version, SType, Dongle, PDate, Customer, Seller, Notes From Licenses where Software like '%" + txtSearch.Text + "%'");
                con.QueryEx();
                DataTable dt = con.QueryEx();
                dataGrid.ItemsSource = dt.DefaultView;
            }
            if (cmbSearch.Text == "Version")
            {
                con = new SqlDbConnect();

                con.SqlQuery("SELECT ID, License, Software, Version, SType, Dongle, PDate, Customer, Seller, Notes From Licenses where Version like '%" + txtSearch.Text + "%'");
                con.QueryEx();
                DataTable dt = con.QueryEx();
                dataGrid.ItemsSource = dt.DefaultView;
            }
            if (cmbSearch.Text == "Software Type")
            {
                con = new SqlDbConnect();

                con.SqlQuery("SELECT ID, License, Software, Version, SType, Dongle, PDate, Customer, Seller, Notes From Licenses where SType like '" + txtSearch.Text + "%'");
                con.QueryEx();
                DataTable dt = con.QueryEx();
                dataGrid.ItemsSource = dt.DefaultView;
            }
        }

        private void btnView_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                con = new SqlDbConnect();

                con.SqlQuery("SELECT * FROM Licenses");
                con.QueryEx();
                DataTable dt = con.QueryEx();
                dataGrid.ItemsSource = dt.DefaultView;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.ToString(), "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }
        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                con = new SqlDbConnect();

                con.SqlQuery("Delete From Licenses where ID = " + txtID.Text);
                con.QueryEx();
                System.Windows.MessageBox.Show("The data has been deleted successfully", "Success", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            }
            catch (System.Exception excep)
            {
                MessageBox.Show(excep.Message, "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }
        private void btnSearchDate_Click(object sender, EventArgs e)
        {
            con = new SqlDbConnect();

            con.SqlQuery("SELECT ID, License, Software, Version, SType, Dongle, PDate, Customer, Seller, Notes From Licenses where PDate between'" + dPickStart.Text + "'and'" + dPickEnd.Text + "'");
            con.QueryEx();
            DataTable dt = con.QueryEx();
            dataGrid.ItemsSource = dt.DefaultView;
        }

        private void btnExportExcel_Click(object sender, EventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "LicenseExport"; // Default file name

            string sDesktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\";
            //string sDesktop = @"T:\Excel\";
            dlg.InitialDirectory = sDesktop;
            dlg.FileName = "LicenseExport";
            dlg.Title = "Save as an Excel File";
            dlg.DefaultExt = ".xlsx"; // Default file extension
            dlg.Filter = "Excel Files(2007)|*.xlsx"; // Filter files by extension

            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                // Save document
                string filename = dlg.FileName;
            }

            try
            {
                var AxlEx = new Excel.Application();
                Excel.Workbook xlWorkBook = AxlEx.Workbooks.Add();
                Excel.Worksheet xlWorkSheet = xlWorkBook.Sheets[1];

                //Sizing the columns
                AxlEx.Columns[1].ColumnWidth = 5;
                AxlEx.Columns[3].ColumnWidth = 12;
                AxlEx.Columns[4].ColumnWidth = 10;
                AxlEx.Columns[5].ColumnWidth = 15;
                AxlEx.Columns[6].ColumnWidth = 10;
                AxlEx.Columns[7].ColumnWidth = 9;
                AxlEx.Columns[8].ColumnWidth = 30;
                AxlEx.Columns[9].ColumnWidth = 12;
                AxlEx.Columns[10].ColumnWidth = 40;
                AxlEx.get_Range("a8", "j8").Merge(true);

                //Coloring and Font adjustments
                Excel.Range formatRange;
                formatRange = AxlEx.get_Range("a8", "e8");
                formatRange.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.SkyBlue);
                formatRange.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                formatRange.Font.Size = 20;
                formatRange.FormulaR1C1 = "Omnimet License Export";
                formatRange.HorizontalAlignment = 3;
                formatRange.VerticalAlignment = 3;
                //OmnimetSQLXAML.Properties.Resources.logo
                string[] all = System.Reflection.Assembly.GetEntryAssembly().GetManifestResourceNames();

                foreach (string one in all)
                {
                    MessageBox.Show(one);
                }
                //string logo123 = Properties.Resources.logo.ToString();
                //Bitmap bmp = new Bitmap(System.Reflection.Assembly.GetEntryAssembly().GetManifestResourceStream(logo123));

                string AppPath = AppDomain.CurrentDomain.BaseDirectory;
                //System.Windows.MessageBox.Show(AppPath + "pic.jpg", "Path", MessageBoxButton.OK, MessageBoxImage.Information);
                xlWorkSheet.Shapes.AddPicture(AppPath + "pic.jpg", MsoTriState.msoFalse, MsoTriState.msoCTrue, 250, 0, 300, 90);

                dataGrid.Columns[0].Header = "ID";
                dataGrid.Columns[1].Header = "License";
                dataGrid.Columns[2].Header = "Software";
                dataGrid.Columns[3].Header = "Version";
                dataGrid.Columns[4].Header = "SType";
                dataGrid.Columns[5].Header = "Dongle";
                dataGrid.Columns[6].Header = "Purchase Date";
                dataGrid.Columns[7].Header = "Customer";
                dataGrid.Columns[8].Header = "Seller";
                dataGrid.Columns[9].Header = "Notes";

                //Putting in of Column Header 

                for (int i = 1; i < dataGrid.Columns.Count + 1; i++)
                {
                    Excel.Range BackgroundColor;
                    BackgroundColor = xlWorkSheet.get_Range("a9", "j9");
                    BackgroundColor.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.RoyalBlue);

                    AxlEx.Cells[9, i] = dataGrid.Columns[i - 1].Header;
                }

                //storing rows and columns with values to excel cells
                for (int i = 0; i < dataGrid.Items.Count-1; i++)
                {
                    DataRowView aux = (DataRowView)dataGrid.Items[i];

                    for (int j = 0; j < aux.Row.ItemArray.Length; j++)
                    {
                        //Console.WriteLine(string.Format("{0}-{1}", j, aux.Row.ItemArray[j]));
                        AxlEx.Cells[i + 10, j + 1] = aux.Row.ItemArray[j];
                    }   
                }
                
                xlWorkSheet.Columns.AutoFit();
                //Expanding table for graphic input
                int Ranger = dataGrid.Items.Count + 10;
                txtAxlLines.Text = Ranger.ToString();
                //Boarder around Table
                Excel.Range boarderRange;
                boarderRange = xlWorkSheet.get_Range("a1", "j" + Ranger);
                boarderRange.BorderAround(Excel.XlLineStyle.xlContinuous,
                    Excel.XlBorderWeight.xlMedium,
                    Excel.XlColorIndex.xlColorIndexAutomatic,
                    Excel.XlColorIndex.xlColorIndexAutomatic);

                AxlEx.ActiveWorkbook.SaveCopyAs(dlg.FileName);
                AxlEx.ActiveWorkbook.Saved = true;
                xlWorkBook.Close(true);
                AxlEx.Quit();

                Marshal.ReleaseComObject(AxlEx);
                MessageBox.Show("File created !", "Excel Export", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error!", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }
    }
}
/*{ 
        dataGrid.SelectAllCells();
        dataGrid.ClipboardCopyMode = DataGridClipboardCopyMode.IncludeHeader;
        ApplicationCommands.Copy.Execute(null, dataGrid);
        String resultat = (string)Clipboard.GetData(DataFormats.CommaSeparatedValue);
        String result = (string)Clipboard.GetData(DataFormats.Text);
        dataGrid.UnselectAllCells();
        System.IO.StreamWriter file = new System.IO.StreamWriter(@"T:\Excel\test.xlsx");
        file.WriteLine(result.Replace(',', ' '));
        file.Close();

        System.Windows.MessageBox.Show("Exporting DataGrid data to Excel file created");



    for (int i = 0; i < YourDataTable.Rows.Count; i++)
{
    // to do: format datetime values before printing
    for (int j = 0; j < YourDataTable.Columns.Count; j++)
    {
        AxlEx.Cells[(i + 2), (j + 1)] = YourDataTable.Rows[i][j];
    }
}
}*/
