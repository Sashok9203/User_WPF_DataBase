using Azure;
using Microsoft.Data.SqlClient;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WpfApp2
{
    [AddINotifyPropertyChangedInterface]
    internal class DataBase 
    {
        private bool flag = true;
        private readonly string phonePattern = @"\(?\d{3}\)?-? *\d{3}-? *-?\d{4}";
        private readonly string loginPattern = @"^[a-zA-Z][a-zA-Z0-9]{3,9}$";
        private readonly string passwordPattern = @"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$";
        private SqlDataAdapter? adapter, deleteAdapter;
        private DataSet? dataSet;
        private readonly string cmd = "select* from Users;select* from Positions;";
        private readonly RelayCommand delete;
        private bool Updated { get; set; }

        private void dataChanged(object sender,DataRowChangeEventArgs e)
        {
            if (e.Action == DataRowAction.Delete)
            {
                MessageBoxResult result = MessageBox.Show($"Are you sure you want to delete ?", "Delete", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.No) return;
            }
            else if ((e.Action == DataRowAction.Add || e.Action == DataRowAction.Change) && dataSet != null && dataSet.HasChanges())
               adapter?.Update(dataSet);
        }
        


        private void DeletePositions()
        {
            if (deleteAdapter == null)
            {
              deleteAdapter =   new("delete_positions", ConfigurationManager.ConnectionStrings["connStr"].ConnectionString);
              deleteAdapter.SelectCommand.CommandType = CommandType.StoredProcedure;
            }
            deleteAdapter.SelectCommand.Parameters.Clear();
            deleteAdapter.SelectCommand.Parameters.AddWithValue("@positionId", PosId);
            IEnumerable<DataRow>? delindexes = dataSet?.Tables[0].AsEnumerable().Where(n =>(int) n[4] == PosId).ToArray();
            if (dataSet != null && delindexes!=null && delindexes.Any())
            {
                MessageBoxResult result =  MessageBox.Show($"Are you sure you want to delete {delindexes.Count()} {Positions.ElementAt(PosId)} ?","Delete",MessageBoxButton.YesNo) ;
                if (result == MessageBoxResult.No) return;
                dataSet.Tables[0].RowDeleting -= dataChanged;
                deleteAdapter?.Fill(dataSet);
                foreach (var rowIndex in delindexes)
                    dataSet?.Tables[0]?.Rows.Remove(rowIndex);
                Updated = true;
                dataSet.Tables[0].RowDeleting += dataChanged;
            }
        }

        private void Load()
        {
          
            dataSet ??= new DataSet();
            dataSet.Clear();
            if (adapter == null)
            {
                adapter = new SqlDataAdapter(cmd, ConfigurationManager.ConnectionStrings["connStr"].ConnectionString);
                _ = new SqlCommandBuilder(adapter);
            }
            _ = adapter?.Fill(dataSet);
            dataSet.Tables[0].RowChanged += new(dataChanged);
            dataSet.Tables[0].RowDeleting += new(dataChanged);
           Updated = true;
            SelectedIndex = -1;
        }

        public bool IsAdmin { get; set; }
        public int PosId { get; set; } = 2;
        public int SelectedIndex { get; set; } = -1;
        public DataBase()
        {
            Load();
            delete = new((o) =>  DeletePositions());
        }

        public IEnumerable<string?> Positions
        {
            get
            {
                DataRowCollection? dr = dataSet?.Tables[1]?.Rows;
                for (int i = 0; i < dr?.Count; i++)
                     yield return dr[i].ItemArray[1]?.ToString();
            }
        }

        [DependsOn( "Updated","IsAdmin")]
        public DataView? Source
        {
            get
            {
                Updated = false;
                DataView? view =  dataSet?.Tables[0].DefaultView;
                if(view!= null) view.RowFilter = IsAdmin ? "PositionId = 0" : "";
                return view;
            }
        }

        public ICommand Delete => delete;
    }
}
