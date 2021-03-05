using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Globalization;
using System.Data.SQLite;
using System.Windows.Forms;

namespace AloudBible.Bible
{
    public class SQLiteDatabase
    {
        String DBConnection;

        private readonly SQLiteTransaction _sqLiteTransaction;

        private readonly SQLiteConnection _sqLiteConnection;

        private readonly bool _transaction;

        /// <summary>
        ///     Default Constructor for SQLiteDatabase Class.
        /// </summary>
        /// <param name="transaction">Allow programmers to insert, update and delete values in one transaction</param>
        public SQLiteDatabase(bool transaction = false)
        {
            _transaction = transaction;
            DBConnection = "Data Source=recipes.s3db";
            if (transaction)
            {
                _sqLiteConnection = new SQLiteConnection(DBConnection);
                _sqLiteConnection.Open();
                _sqLiteTransaction = _sqLiteConnection.BeginTransaction();
            }
        }

        /// <summary>
        ///     Single Param Constructor for specifying the DB file.
        /// </summary>
        /// <param name="inputFile">The File containing the DB</param>
        public SQLiteDatabase(String inputFile)
        {
            DBConnection = String.Format("Data Source={0}", inputFile);
        }

        /// <summary>
        ///     Commit transaction to the database.
        /// </summary>
        public void CommitTransaction()
        {
            _sqLiteTransaction.Commit();
            _sqLiteTransaction.Dispose();
            _sqLiteConnection.Close();
            _sqLiteConnection.Dispose();
        }

        /// <summary>
        ///     Single Param Constructor for specifying advanced connection options.
        /// </summary>
        /// <param name="connectionOpts">A dictionary containing all desired options and their values</param>
        public SQLiteDatabase(Dictionary<String, String> connectionOpts)
        {
            String str = connectionOpts.Aggregate("", (current, row) => current + String.Format("{0}={1}; ", row.Key, row.Value));
            str = str.Trim().Substring(0, str.Length - 1);
            DBConnection = str;
        }

        /// <summary>
        ///     Allows the programmer to create new database file.
        /// </summary>
        /// <param name="filePath">Full path of a new database file.</param>
        /// <returns>true or false to represent success or failure.</returns>
        public static bool CreateDB(string filePath)
        {
            try
            {
                SQLiteConnection.CreateFile(filePath);
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, e.GetType().ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        ///     Allows the programmer to run a query against the Database.
        /// </summary>
        /// <param name="sql">The SQL to run</param>
        /// <param name="allowDBNullColumns">Allow null value for columns in this collection.</param>
        /// <returns>A DataTable containing the result set.</returns>
        public DataTable GetDataTable(string sql, IEnumerable<string> allowDBNullColumns = null)
        {
            var dt = new DataTable();
            if (allowDBNullColumns != null)
                foreach (var s in allowDBNullColumns)
                {
                    dt.Columns.Add(s);
                    dt.Columns[s].AllowDBNull = true;
                }
            try
            {
                var cnn = new SQLiteConnection(DBConnection);
                cnn.Open();
                var mycommand = new SQLiteCommand(cnn) { CommandText = sql };
                var reader = mycommand.ExecuteReader();
                dt.Load(reader);
                reader.Close();
                cnn.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return dt;
        }

        public string RetrieveOriginal(string value)
        {
            return
                value.Replace("&amp;", "&").Replace("&lt;", "<").Replace("&gt;", "<").Replace("&quot;", "\"").Replace(
                    "&apos;", "'");
        }

        /// <summary>
        ///     Allows the programmer to interact with the database for purposes other than a query.
        /// </summary>
        /// <param name="sql">The SQL to be run.</param>
        /// <returns>An Integer containing the number of rows updated.</returns>
        public int ExecuteNonQuery(string sql)
        {
            if (!_transaction)
            {
                var cnn = new SQLiteConnection(DBConnection);
                cnn.Open();
                var mycommand = new SQLiteCommand(cnn) { CommandText = sql };
                var rowsUpdated = mycommand.ExecuteNonQuery();
                cnn.Close();
                return rowsUpdated;
            }
            else
            {
                var mycommand = new SQLiteCommand(_sqLiteConnection) { CommandText = sql };
                return mycommand.ExecuteNonQuery();
            }
        }

        /// <summary>
        ///     Allows the programmer to retrieve single items from the DB.
        /// </summary>
        /// <param name="sql">The query to run.</param>
        /// <returns>A string.</returns>
        public string ExecuteScalar(string sql)
        {
            if (!_transaction)
            {
                var cnn = new SQLiteConnection(DBConnection);
                cnn.Open();
                var mycommand = new SQLiteCommand(cnn) { CommandText = sql };
                var value = mycommand.ExecuteScalar();
                cnn.Close();
                return value != null ? value.ToString() : "";
            }
            else
            {
                var sqLiteCommand = new SQLiteCommand(_sqLiteConnection) { CommandText = sql };
                var value = sqLiteCommand.ExecuteScalar();
                return value != null ? value.ToString() : "";
            }
        }

        /// <summary>
        ///     Allows the programmer to easily update rows in the DB.
        /// </summary>
        /// <param name="tableName">The table to update.</param>
        /// <param name="data">A dictionary containing Column names and their new values.</param>
        /// <param name="where">The where clause for the update statement.</param>
        /// <returns>A boolean true or false to signify success or failure.</returns>
        public bool Update(String tableName, Dictionary<String, String> data, String where)
        {
            String vals = "";
            Boolean returnCode = true;
            if (data.Count >= 1)
            {
                vals = data.Aggregate(vals, (current, val) => current + String.Format(" {0} = '{1}',", val.Key.ToString(CultureInfo.InvariantCulture), val.Value.ToString(CultureInfo.InvariantCulture)));
                vals = vals.Substring(0, vals.Length - 1);
            }
            try
            {
                ExecuteNonQuery(String.Format("update {0} set {1} where {2};", tableName, vals, where));
            }
            catch
            {
                returnCode = false;
            }
            return returnCode;
        }

        /// <summary>
        ///     Allows the programmer to easily delete rows from the DB.
        /// </summary>
        /// <param name="tableName">The table from which to delete.</param>
        /// <param name="where">The where clause for the delete.</param>
        /// <returns>A boolean true or false to signify success or failure.</returns>
        public bool Delete(String tableName, String where)
        {
            Boolean returnCode = true;
            try
            {
                ExecuteNonQuery(String.Format("delete from {0} where {1};", tableName, where));
            }
            catch (Exception fail)
            {
                MessageBox.Show(fail.Message, fail.GetType().ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                returnCode = false;
            }
            return returnCode;
        }

        /// <summary>
        ///     Allows the programmer to easily insert into the DB
        /// </summary>
        /// <param name="tableName">The table into which we insert the data.</param>
        /// <param name="data">A dictionary containing the column names and data for the insert.</param>
        /// <returns>returns last inserted row id if it's value is zero than it means failure.</returns>
        public long Insert(String tableName, Dictionary<String, String> data)
        {
            String columns = "";
            String values = "";
            String value;
            foreach (KeyValuePair<String, String> val in data)
            {
                columns += String.Format(" {0},", val.Key.ToString(CultureInfo.InvariantCulture));
                values += String.Format(" '{0}',", val.Value);
            }
            columns = columns.Substring(0, columns.Length - 1);
            values = values.Substring(0, values.Length - 1);
            try
            {
                if (!_transaction)
                {
                    var cnn = new SQLiteConnection(DBConnection);
                    cnn.Open();
                    var sqLiteCommand = new SQLiteCommand(cnn)
                    {
                        CommandText =
                            String.Format("insert into {0}({1}) values({2});", tableName, columns,
                                          values)
                    };
                    sqLiteCommand.ExecuteNonQuery();
                    sqLiteCommand = new SQLiteCommand(cnn) { CommandText = "SELECT last_insert_rowid()" };
                    value = sqLiteCommand.ExecuteScalar().ToString();
                }
                else
                {
                    ExecuteNonQuery(String.Format("insert into {0}({1}) values({2});", tableName, columns, values));
                    value = ExecuteScalar("SELECT last_insert_rowid()");
                }
            }
            catch (Exception fail)
            {
                MessageBox.Show(fail.Message, fail.GetType().ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
            return long.Parse(value);
        }

        /// <summary>
        ///     Allows the programmer to easily delete all data from the DB.
        /// </summary>
        /// <returns>A boolean true or false to signify success or failure.</returns>
        public bool ClearDB()
        {
            try
            {
                var tables = GetDataTable("select NAME from SQLITE_MASTER where type='table' order by NAME;");
                foreach (DataRow table in tables.Rows)
                {
                    ClearTable(table["NAME"].ToString());
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        ///     Allows the user to easily clear all data from a specific table.
        /// </summary>
        /// <param name="table">The name of the table to clear.</param>
        /// <returns>A boolean true or false to signify success or failure.</returns>
        public bool ClearTable(String table)
        {
            try
            {
                ExecuteNonQuery(String.Format("delete from {0};", table));
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        ///     Allows the user to easily reduce size of database.
        /// </summary>
        /// <returns>A boolean true or false to signify success or failure.</returns>
        public bool CompactDB()
        {
            try
            {
                ExecuteNonQuery("Vacuum;");
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static String InsertCommandTextOf(String tableName, Dictionary<String, String> parameters)
        {
            StringBuilder columnsBuilder = new StringBuilder();
            StringBuilder valuesBuilder = new StringBuilder(); ;
            String result;
            foreach (KeyValuePair<String, String> val in parameters)
            {
                columnsBuilder.AppendFormat(" {0},", val.Key);
                valuesBuilder.AppendFormat(" '{0}',", val.Value);
            }

            result = String.Format("INSERT INTO {0}({1}) VALUES({2});", 
                tableName, columnsBuilder.ToString(0, columnsBuilder.Length-1), valuesBuilder.ToString(0, valuesBuilder.Length-1));
            return result;
        }

        public static String UpdateCommandTextOf(String tableName, Dictionary<String, String> parameters, string where)
        {
            StringBuilder sb = new StringBuilder();

            if (parameters == null || parameters.Count == 0 || where == null)
                return "";

            foreach (KeyValuePair<String, String> val in parameters)
            {
                sb.AppendFormat(" {0} = '{1}',", val.Key.ToString(), val.Value.ToString());
            }

            return String.Format("UPDATE {0} SET {1} WHERE {2};", tableName, sb.ToString(0, sb.Length-1), where);
        }

        public static int Insert(SQLiteCommand insertCommand)
        {
            if (insertCommand.Connection == null)
                return -1;

            insertCommand.ExecuteNonQuery();
            return LastInsertId(insertCommand);
        }

        public static int LastInsertId(SQLiteCommand lastInsert)
        {
            SQLiteCommand sqLiteCommand = new SQLiteCommand(lastInsert.Connection) { CommandText = "SELECT last_insert_rowid()" };
            var value = sqLiteCommand.ExecuteScalar();
            int result = -1;
            int.TryParse(value.ToString(), out result);

            return result;
        }
    }
    
    /*/
    public class SQLiteDatabase
    {
        String dbConnection;

        /// <summary>
        ///     Single Param Constructor for specifying the DB file.
        /// </summary>
        /// <param name="inputFile">The File containing the DB</param>
        public SQLiteDatabase(String inputFile)
        {
            dbConnection = String.Format("Data Source={0}", inputFile);
        }

        /// <summary>
        ///     Single Param Constructor for specifying advanced connection options.
        /// </summary>
        /// <param name="connectionOpts">A dictionary containing all desired options and their values</param>
        public SQLiteDatabase(Dictionary<String, String> connectionOpts)
        {
            String str = "";
            foreach (KeyValuePair<String, String> row in connectionOpts)
            {
                str += String.Format("{0}={1}; ", row.Key, row.Value);
            }
            str = str.Trim().Substring(0, str.Length - 1);
            dbConnection = str;
        }

        /// <summary>
        ///     Allows the programmer to run a query against the Database.
        /// </summary>
        /// <param name="sql">The SQL to run</param>
        /// <returns>A DataTable containing the result set.</returns>
        public DataTable GetDataTable(string sql)
        {
            DataTable dt = new DataTable();
            try
            {
                SQLiteConnection cnn = new SQLiteConnection(dbConnection);
                cnn.Open();
                SQLiteCommand mycommand = new SQLiteCommand(cnn);
                mycommand.CommandText = sql;
                SQLiteDataReader reader = mycommand.ExecuteReader();
                dt.Load(reader);
                reader.Close();
                cnn.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return dt;
        }

        /// <summary>
        ///     Allows the programmer to interact with the database for purposes other than a query.
        /// </summary>
        /// <param name="sql">The SQL to be run.</param>
        /// <returns>An Integer containing the number of rows updated.</returns>
        public int ExecuteNonQuery(string sql)
        {
            SQLiteConnection cnn = new SQLiteConnection(dbConnection);
            cnn.Open();
            SQLiteCommand mycommand = new SQLiteCommand(cnn);
            mycommand.CommandText = sql;
            int rowsUpdated = mycommand.ExecuteNonQuery();
            cnn.Close();
            return rowsUpdated;
        }

        /// <summary>
        ///     Allows the programmer to retrieve single items from the DB.
        /// </summary>
        /// <param name="sql">The query to run.</param>
        /// <returns>A string.</returns>
        public string ExecuteScalar(string sql)
        {
            SQLiteConnection cnn = new SQLiteConnection(dbConnection);
            cnn.Open();
            SQLiteCommand mycommand = new SQLiteCommand(cnn);
            mycommand.CommandText = sql;
            object value = mycommand.ExecuteScalar();
            cnn.Close();
            if (value != null)
            {
                return value.ToString();
            }
            return "";
        }

        /// <summary>
        ///     Allows the programmer to easily update rows in the DB.
        /// </summary>
        /// <param name="tableName">The table to update.</param>
        /// <param name="data">A dictionary containing Column names and their new values.</param>
        /// <param name="where">The where clause for the update statement.</param>
        /// <returns>A boolean true or false to signify success or failure.</returns>
        public bool Update(String tableName, Dictionary<String, String> data, String where)
        {
            String vals = "";
            Boolean returnCode = true;
            if (data.Count >= 1)
            {
                foreach (KeyValuePair<String, String> val in data)
                {
                    vals += String.Format(" {0} = '{1}',", val.Key.ToString(), val.Value.ToString());
                }
                vals = vals.Substring(0, vals.Length - 1);
            }
            try
            {
                this.ExecuteNonQuery(String.Format("update {0} set {1} where {2};", tableName, vals, where));
            }
            catch
            {
                returnCode = false;
            }
            return returnCode;
        }

        /// <summary>
        ///     Allows the programmer to easily delete rows from the DB.
        /// </summary>
        /// <param name="tableName">The table from which to delete.</param>
        /// <param name="where">The where clause for the delete.</param>
        /// <returns>A boolean true or false to signify success or failure.</returns>
        public bool Delete(String tableName, String where)
        {
            Boolean returnCode = true;
            try
            {
                this.ExecuteNonQuery(String.Format("delete from {0} where {1};", tableName, where));
            }
            catch (Exception fail)
            {
                MessageBox.Show(fail.Message);
                returnCode = false;
            }
            return returnCode;
        }

        /// <summary>
        ///     Allows the programmer to easily insert into the DB
        /// </summary>
        /// <param name="tableName">The table into which we insert the data.</param>
        /// <param name="data">A dictionary containing the column names and data for the insert.</param>
        /// <returns>A boolean true or false to signify success or failure.</returns>
        public bool Insert(String tableName, Dictionary<String, String> data)
        {
            String columns = "";
            String values = "";
            Boolean returnCode = true;
            foreach (KeyValuePair<String, String> val in data)
            {
                columns += String.Format(" {0},", val.Key.ToString());
                values += String.Format(" '{0}',", val.Value);
            }
            columns = columns.Substring(0, columns.Length - 1);
            values = values.Substring(0, values.Length - 1);
            try
            {
                this.ExecuteNonQuery(String.Format("insert into {0}({1}) values({2});", tableName, columns, values));
            }
            catch (Exception fail)
            {
                MessageBox.Show(fail.Message);
                returnCode = false;
            }
            return returnCode;
        }

        /// <summary>
        ///     Allows the programmer to easily delete all data from the DB.
        /// </summary>
        /// <returns>A boolean true or false to signify success or failure.</returns>
        public bool ClearDB()
        {
            DataTable tables;
            try
            {
                tables = this.GetDataTable("select NAME from SQLITE_MASTER where type='table' order by NAME;");
                foreach (DataRow table in tables.Rows)
                {
                    this.ClearTable(table["NAME"].ToString());
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        ///     Allows the user to easily clear all data from a specific table.
        /// </summary>
        /// <param name="table">The name of the table to clear.</param>
        /// <returns>A boolean true or false to signify success or failure.</returns>
        public bool ClearTable(String table)
        {
            try
            {

                this.ExecuteNonQuery(String.Format("delete from {0};", table));
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
    //*/
}
