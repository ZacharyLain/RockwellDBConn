using System;
using System.Data.Odbc;
using System.Net.Security;
using DotNetEnv;


namespace dotnetdb
{
    class Program
    {
        static void Main(string[] args)
        {
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // START ENV DEBUG SECTION

            // Specify the path to the .env file
            string envFilePath = @"../../../../../.env";

            Env.Load(envFilePath);

            // Retrieve the environment variables
            string server = Environment.GetEnvironmentVariable("SERVER");
            string database = Environment.GetEnvironmentVariable("DATABASE");
            string serialTag = Environment.GetEnvironmentVariable("SERIAL_TAG");
            string table = Environment.GetEnvironmentVariable("TABLE");
            string serialCol = Environment.GetEnvironmentVariable("SERIAL_COL");
            string ipAddress = Environment.GetEnvironmentVariable("IP_ADDRESS");

            // Use the environment variables
            Console.WriteLine($"Server: {server}");
            Console.WriteLine($"Database: {database}");
            Console.WriteLine($"Serial Tag: {serialTag}");
            Console.WriteLine($"Table: {table}");
            Console.WriteLine($"Serial Column: {serialCol}");
            Console.WriteLine($"IP Address: {ipAddress}");

            // END ENV DEBUG SECTION
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            // Set up string for connection
            string connectionString = $"DRIVER={{ODBC Driver 17 for SQL Server}};SERVER={server};DATABASE={database};Trusted_Connection=yes;";
            string serialNum = "";

            // just loop this thing forever
            while (true)
            {
                // This will become a read from the PLC serial number tag
                Console.WriteLine("New serial num: ");
                serialNum = Console.ReadLine();
                
                // try to insert the row into the db
                // this allows us to just insert whatever and will flag whenever
                // there is an exception
                try
                {
                    InsertRow(connectionString, serialNum);
                    Console.WriteLine("Serial Number added to the DB");
                }

                // catch OdbcException
                // this will catch anything related to the database
                catch (OdbcException ex)
                {
                    // exception related to data already being in the db
                    if (ex.Errors.Count > 0 && ex.Errors[0].SQLState == "23000")
                    {
                        // This will become a flag to show that there is a duplicate serial number
                        Console.WriteLine("The value is already in the table.");
                    }

                    // all other db exceptions
                    else
                    {
                        Console.WriteLine($"An ODBC exception occurred: {ex.Message}");
                    }
                }
                // catch all other exceptions
                // none of these should occur, but just in case they do
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception occurred: {ex.Message}");
                }
            }
        }

        static private void InsertRow(string connectionString, string serialNum)
        {
            // create a new ODBC connection and command
            using OdbcConnection connection = new OdbcConnection(connectionString);
            OdbcCommand command = new OdbcCommand();

            // prepared query with placeholder
            string queryString = "INSERT INTO Parts (SerialNumber) VALUES (?)";

            // set the command text and connection for the command
            command.CommandText = queryString;
            command.Connection = connection;

            // add parameters to command
            command.Parameters.AddWithValue("?", serialNum);

            // open connection, execute query, and close connection
            connection.Open();
            command.ExecuteNonQuery();

            // The connection is automatically closed at
            // the end of the Using block.
        }
    }
}