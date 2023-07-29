using Azure;
using Microsoft.Data.SqlClient;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Linq;
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
    internal class ViewMD 
    {
        
        string pattern = new(@"^([+]?[\s0-9]+)?(\d{3}|[(]?[0-9]+[)])?([-]?[\s]?[0-9])+$");
        
        private SqlDataAdapter? adapter, deleteAdapter;
        private DataSet? dataSet;
        private readonly string cmd = "select* from Users;select* from Positions;";
        private readonly RelayCommand save,delete;
        private bool Updated { get; set; }
        

        private void DeletePosition()
        {
            
            if (deleteAdapter == null)
            {
              deleteAdapter =   new("delete_positions", ConfigurationManager.ConnectionStrings["connStr"].ConnectionString);
              deleteAdapter.SelectCommand.CommandType = CommandType.StoredProcedure;
            }
            deleteAdapter.SelectCommand.Parameters.Clear();
            deleteAdapter.SelectCommand.Parameters.AddWithValue("@positionId", PosId);
            if (dataSet != null) deleteAdapter?.Fill(dataSet);
            Load();
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
            Updated = true;
        }

        public bool IsAdmin { get; set; }
        public int PosId { get; set; } = 2;
        
        public ViewMD()
        {
            Load();
            save = new((o) => { if (dataSet != null) adapter?.Update(dataSet);  Load(); }, (o) => dataSet?.HasChanges() ?? false);
            delete = new((o) =>  DeletePosition());
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

        public ICommand Update => save;
        public ICommand Delete => delete;
    }
}
