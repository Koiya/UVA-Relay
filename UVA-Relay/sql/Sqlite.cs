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
        private string dbFile = "Data Source=" + Environment.GetEnvironmentVariable("fileDB");
        public string GetQuery(string guildID){ 
            
            string result = "";
            var s = new SQLiteConnection(@dbFile);
            try
            {
                using (s)
                {
                    using (var cmd = s.CreateCommand())
                    {
                        s.Open();
                        cmd.CommandText = $"SELECT id FROM GUILD where guildID = {guildID}";
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

       
        public static void saveGuildSetting(){ 
            try
            {
                var s = new SQLiteConnection("test.db");
                using (s)
                {
                    using (var cmd = s.CreateCommand())
                    {
                        s.Open();
                        cmd.CommandText = "SELECT id FROM GUILD";

                    }
                }
            }
            catch (SQLiteException exc)
            {
                Console.WriteLine(exc.Message);
            }
        }
    }
}
