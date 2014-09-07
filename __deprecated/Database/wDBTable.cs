using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ItzWarty;
using ItzWarty.Database;

namespace ItzWarty.Database
{
    public class wDBTable
    {
        string rootLocation = "";
        DatabaseClient dbc;
        public wDBTable(string name, DatabaseClient root)
        {
            rootLocation = root.currentFolderLocation;
            dbc = root.Clone();
            dbc.SelectDB("$" + name);
            dbc.SetCurrentDBAsRoot();
            dbc.ReturnToRoot();
        }
        public wDBRowCollection GetRows()
        {
            dbc.ReturnToRoot();
            string[] rowNums = dbc.ListDatabases();
            List<wDBTableRow> rows = new List<wDBTableRow>();
            for (int i = 0; i < rowNums.Length; i++)
            {
                string rowNum = rowNums[i];
                dbc.SelectDB(rowNum);
                rows.Add(
                    new wDBTableRow(dbc)
                );
                dbc.ReturnToRoot();
            }
            return new wDBRowCollection(rows);
        }

        public static void CreateTable(string tableName, DatabaseClient parentDB)
        {
            if (parentDB.ExistDB("$" + tableName) == DatabaseClient.DBResponse.Exists) return;
                parentDB.CreateDB("$" + tableName);

            DatabaseClient dbc = parentDB.Clone();
            dbc.SelectDB("$" + tableName);
            dbc.SetValue(".rowsAdded", "0");
        }
        public void AddRow(string[][] keyValues)
        {
            int rowsAdded = int.Parse(dbc.GetValue(".rowsAdded"));
            dbc.ReturnToRoot();
            dbc.CreateDB(rowsAdded.ToString());
            dbc.SelectDB(rowsAdded.ToString());
            wDBTableRow row = new wDBTableRow(dbc);
            for (int i = 0; i < keyValues.Length; i++)
                row[keyValues[i][0]] = keyValues[i][1];

            dbc.ReturnToRoot();
            dbc.SetValue(".rowsAdded", rowsAdded + 1);
        }
    }
    public class wDBTableRow
    {
        DatabaseClient dbc;
        public wDBTableRow(DatabaseClient dbc)
        {
            this.dbc = dbc.Clone();
        }
        public string this[string columnName]
        {
            get
            {
                return dbc.GetValue(columnName);
            }
            set
            {
                dbc.SetValue(columnName, value);
            }
        }
    }
}
