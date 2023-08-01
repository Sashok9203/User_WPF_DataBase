using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool flag = true;
        private readonly string phonePattern = @"\(?\d{3}\)?-? *\d{3}-? *-?\d{4}";
        private readonly string loginPattern = @"^[a-zA-Z][a-zA-Z0-9]{3,9}$";
        private readonly string passwordPattern = @"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$";
        public MainWindow()
        {
            InitializeComponent();
        }

        private void salesGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (flag)
            {
                DataGrid? dataGrid = sender as DataGrid;
                DataRow? row = (e.Row.Item as DataRowView)?.Row;
                string? message = null;
                flag = false;
                if (e.EditAction == DataGridEditAction.Cancel) dataGrid?.CancelEdit();
                else
                {
                    for (int i = 1; i <= 5; i++)
                    {
                        if (string.IsNullOrWhiteSpace(row?[i].ToString()))
                        {
                            message = $"{row?.Table.Columns[i].ColumnName} not be empty !!!";
                            break;
                        }
                        switch (i)
                        {
                            case 1:
                                if (!Regex.IsMatch(row?[i]?.ToString() ?? string.Empty, loginPattern))
                                    message = $"Invalid login \"{row?[i]?.ToString()}\" !!!\nAt least one letter or number\nEvery character from the start to the end is a letter or number\nLogin is not allowed to start with digits\nMin/max length restrictions: 3 - 9";
                                else if (!loginCheck(dataGrid, row?[i].ToString()))
                                    message = "This login allready exists !!!";
                                break;
                            case 2:
                                if (!Regex.IsMatch(row?[i]?.ToString() ?? string.Empty, passwordPattern))
                                    message = "Invalid password \"{row?[i]?.ToString()}\" !!!\nAt least one upper case\nAt least one lower case letter\nAt least one digit\nAt least one special character\nMinimum eight in length 8";
                                break;
                            case 3:
                                if (!Regex.IsMatch(row?[i]?.ToString() ?? string.Empty, phonePattern))
                                    message = "Invalid phone \"{row?[i]?.ToString()}\"!!!\n(xxx)xxxxxxx\r\n(xxx) xxxxxxx\r\n(xxx)xxx-xxxx\r\n(xxx) xxx-xxxx\r\nxxxxxxxxxx\r\nxxx-xxx-xxxxx"; break;
                        }
                    }
                }

                if (message != null)
                {
                    dataGrid?.CancelEdit();
                    dataGrid?.Items.Refresh();
                    MessageBox.Show(message);
                }
                else dataGrid?.CommitEdit();
                flag = true;
            }
        }

        private bool loginCheck(DataGrid? data, string? login)
        {
            int count = 0;
            foreach (var item in data.Items)
            {
                DataRow? row = (item as DataRowView)?.Row;
                if (row?["Login"].ToString() == login) count++;
                if (count > 1) return false;
            }
            return true;
        }




    }
}
