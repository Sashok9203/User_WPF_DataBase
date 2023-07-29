using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }


        private bool flagfix = true;
        private void salesGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (flagfix)
            {
                DataGrid? dataGrid = sender as DataGrid;
                DataRowView? row = e.Row.Item as DataRowView;
                string? message = null;
                flagfix = false;
                if (e.EditAction == DataGridEditAction.Cancel) dataGrid?.CancelEdit();
                else
                {
                    for (int i = 1; i <= 5; i++)
                    {
                        if (string.IsNullOrWhiteSpace(row?.Row[i].ToString()))
                        {
                            message =  $"{row?.Row.Table.Columns[i].ColumnName} not be empty !!!";
                            break;
                        }
                        if (i == 1 && !loginCheck(dataGrid, row?.Row[i].ToString()))
                        {
                            message = "This login allready exists !!!";
                            break;
                        }
                    }
                }

                if (message != null)
                {
                    dataGrid?.CancelEdit();
                    dataGrid?.Items.Refresh();
                }
                else
                {
                    dataGrid?.CommitEdit();
                    message = "The changes have been added to the database !!!";
                }
                MessageBox.Show(message);
                flagfix = true;
            }
        }

        private bool loginCheck(DataGrid? data,string? login)
        {
            int count = 0;
            foreach (var item in data.Items)
            {
                DataRowView? row = item as DataRowView;
                if (row?.Row["Login"].ToString() == login) count++;
                if (count > 1) return false;
            }
            return true; 
        }

        

       
    }
}
