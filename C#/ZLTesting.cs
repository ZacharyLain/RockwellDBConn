public class ODBCTest
{
    private string connectionString = $"DRIVER={{ODBC Driver 17 for SQL Server}};SERVER={SERVER};DATABASE={DATABASE};Trusted_Connection=yes;"
    private string serialNum;

    static void Main(string[] args)
    {
        while (true)
        {   
            Console.WriteLine("New serial num: ");
            serialNum = Console.ReadLine();

            try
            {
                InsertRow(connectionString, serialNum);
            }
            catch (OdbcException ex) // want to catch exception if the value is already in the table, serialNum is primary key, believe it would be integrityerror
            {
                if (ex.Errors.Count > 0 && ex.Errorsp[0].SQLState == "23000")
                {
                    Console.WriteLine("The value is already in the table.");
                }
                else
                {
                    Constole.WriteLine($"An ODBC exception occurred: {ex.Message}");
                }
            }
            catch (exception ex)
            {
                Console.WriteLine($"Exception occurred: {ex.Message}");
            }
        }
    }

    static private void InsertRow(string connectionString, string serialNum)
    {
        using (OdbcConnection connection = new OdbcConnection(connectionString))
        {
            OdbcCommand command = new OdbcCommand;
        
            // prepared query with placeholder
            string queryString = "INSERT INTO SerialNums (SerialNumber) VALUES (?)";
            
            // set the command text and connection for the command
            command.CommandText = queryString;
            command.Connection = connection;

            // add parameters to command
            command.Parameters.AddWithValue("?", serialNum);

            // open connection, execute query, and close connection
            connection.Open();
            command.ExecuteNonQuery();

            // commit to the db
            CommitInsertion(connectionString);

            // The connection is automatically closed at
            // the end of the Using block.
        }
    }

    static private void CommitInsertion(string connectionString)
    {
        string queryString = "COMMIT";
        OdbcCommand command = new OdbcCommand(queryString);

        using (OdbcConnection connection = new OdbcConnection(connectionString))
        {
            command.Connection = connection;
            connection.Open();
            command.ExecuteNonQuery();

            // The connection is automatically closed at
            // the end of the Using block.
        }
    }
}
