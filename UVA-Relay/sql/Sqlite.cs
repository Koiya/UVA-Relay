using System;
using System.Data.SQLite;
/*
 TODO
 - set up sqlite with a test server
 - Implement a way to save or store settings 
 - Display guild settings with embed message
    Name
    option1 : value
    option2 : value
    ...
 - Test to see if discord commands saves to settings
 
*/
namespace UVA_Relay.sql
{
    public class SQL
    {
        //db set as a file instead of server for now
        private string _dbFile = "Data Source=" + Environment.GetEnvironmentVariable("fileDB");
        //Test getting query and returning result
        public string GetQuery(string guildId){ 
            string result = "";
            var s = new SQLiteConnection(@_dbFile);
            try
            {
                using (s)
                {
                    using (var cmd = s.CreateCommand())
                    {
                        s.Open();
                        cmd.CommandText = $"SELECT id FROM GUILD where guildID = {guildId}";
                        result = cmd.ExecuteScalar().ToString();
                    }
                }
            }
            catch (SQLiteException exc)
            {
                Console.WriteLine(exc.Message);
            }
            return result;

        }

       //Adds guild ID to database
        public void AddGuildToDatabase(string guildName, ulong guildId){ 
            var s = new SQLiteConnection(@_dbFile);
            try
            {
                using (s)
                {
                    using (var cmd = s.CreateCommand())
                    {
                        s.Open();
                        cmd.CommandText = "INSERT INTO GUILD (GuildName, GuildId) VALUES(@nameQuery, @idQuery)";
                        //Safest way to insert data without sql injections?
                        var nameParameter = cmd.CreateParameter();
                        var idParameter = cmd.CreateParameter();
                        nameParameter.ParameterName = "@nameQuery";
                        idParameter.ParameterName = "@idQuery";
                        cmd.Parameters.Add(nameParameter);
                        cmd.Parameters.Add(idParameter);
                        nameParameter.Value = guildName;
                        idParameter.Value = guildId;
                        //Execute query
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (SQLiteException exc)
            {
                //Console.WriteLine(exc.Message);
                Console.WriteLine("Guild already in database or error has occured");

            }
        }
    }
}
