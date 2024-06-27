from pylogix import PLC
import pyodbc

# Define PLC IP and tag to read
IP_ADDRESS = '10.0.0.1'
SERIAL_TAG = 'Tag'

# Define server and db names
SERVER = 'ZLAIN-Strix\SQLEXPRESS'
DATABASE = 'PartsDB'
TABLE = 'Parts'
SERIAL_COL = 'SerialNum'

# Set up connection string
conn_str = (f'DRIVER={{ODBC Driver 17 for SQL Server}};SERVER={SERVER};DATABASE={DATABASE};Trusted_Connection=yes;')

def selectAll(cursor, tableName):
    cursor.execute(f"SELECT * FROM {tableName}")

    for row in cursor.fetchall():
        print(row)

# Check if serial num already exists in the DB
def serial_num_exists(cursor, tableName, columnName, value):
    query = f'SELECT 1 FROM ? WHERE ? = ?'
    cursor.execute(query, tableName, columnName, value)
    return cursor.fetchone() is not None

def add_serial_num(cursor, tableName, value):
    query = f'INSERT INTO {tableName} (SerialNumber) VALUES (?)'
    cursor.execute(query, value)

try:
    # Start communication with DB
    plc = PLC()
    plc.IPAddress = IP_ADDRESS
    tag = plc.Read(SERIAL_TAG) # this should be probably be used with a curr_tag vs old_tag so we can check when there is a change

    # Connect to DB
    conn = pyodbc.connect(conn_str)
    print('Connection successful')

    # Make a cursor to use cursor methods
    cursor = conn.cursor()

    selectAll(cursor, TABLE)

    while True:
        testVal = input('New serial num: ')

        try:
            add_serial_num(cursor, TABLE, testVal)
        except Exception as e:
            if '23000' in e.args[0]:
                print('This value already exists in the database')
            else:
                print('The error is not related to the value being in the table')

        # if (serial_num_exists(cursor, TABLE, SERIAL_COL, tag.Value)):
        #     print('ERROR: Serial number has already been used.')
        # else:
        #     if (tag.Value != '0'):
        #         add_serial_num(cursor, TABLE, tag.Value)
        #     else:
        #         break

    # Close PLC communication and DB connection
    plc.Close()
    conn.close()

# Catch exception
except Exception as e:
    print('Error while connection: ', e)

