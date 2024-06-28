using System;
using System.Data.Odbc;

public class TestingCSharp
{
    private static string connectionString = "DSN=TestingCode;";
    private static string SerialNumber;
    

    static void Main(string[] args)
    {
        int PartID = 100;

        while (true)
        {   
            Console.WriteLine("New serial num: ");
            SerialNumber = Console.ReadLine();
            PartID++;

            try
            {
                InsertRow(connectionString, PartID, SerialNumber);
            }
            catch (OdbcException ex)
            {
                if (ex.Errors.Count > 0 && ex.Errors[0].SQLState == "23000")
                {
                    Console.WriteLine($"The value is already in the table.\n{ex}");
                }
                else
                {
                    Console.WriteLine($"An ODBC exception occurred: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occurred: {ex.Message}");
            }
        }
    }

    static private void InsertRow(string connectionString, int PartID, string SerialNumber)
    {
        using (OdbcConnection connection = new OdbcConnection(connectionString))
        {
            // Prepare the command
            string queryString = "INSERT INTO PartsInfo (PartID, SerialNumber) VALUES (?, ?)";
            OdbcCommand command = new OdbcCommand(queryString, connection);
            
            // Add parameter
            command.Parameters.AddWithValue("?", PartID);
            command.Parameters.AddWithValue("?", SerialNumber);

            try
            {
                // Open connection, execute query
                connection.Open();
                command.ExecuteNonQuery();

                // No need to commit explicitly for INSERT statements in most databases
            }
            catch (OdbcException)
            {
                // Rethrow the exception to be caught in Main
                throw;
            }
        }
    }
}
