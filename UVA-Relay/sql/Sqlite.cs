using System;
using System.Data.SQLite;
/*
 TODO
 - Design a table for user/server settings 
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
        private string dbFile = "Data Source=C:\\Users\\daniel.nguyen\\OneDrive - University View Academy\\Documents\\GitHub\\UVA-Relay\\UVA-Relay\\sql\\test.db";
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
