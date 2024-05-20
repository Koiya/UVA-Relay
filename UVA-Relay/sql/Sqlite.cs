using System;
using System.Data.SQLite;
namespace UVA_Relay.sql
{
    public class SQL
    {
        //db set as a file instead of server for now
        private string _dbFile = "Data Source=" + Environment.GetEnvironmentVariable("fileDB");

        //Test getting query and returning result
        public string GetQuery(string guildId)
        {
            string result = "";
            var s = new SQLiteConnection(@_dbFile);
            try
            {
                using (s)
                {
                    using (var cmd = s.CreateCommand())
                    {
                        s.Open();
                        cmd.CommandText = $"SELECT id FROM GUILD where guildID = @guildIdQuery";
                        cmd.Parameters.AddWithValue("@guildIdQuery", guildId);
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
        public void AddGuildToDatabase(string guildName, ulong guildId)
        {
            var s = new SQLiteConnection(@_dbFile);
            try
            {
                using (s)
                {
                    using (var cmd = s.CreateCommand())
                    {
                        s.Open();
                        cmd.CommandText = "INSERT INTO GUILD (GuildName, GuildId) VALUES(@nameQuery, @idQuery)";
                        cmd.Parameters.AddWithValue("@nameQuery", guildName);
                        cmd.Parameters.AddWithValue("@idQuery",guildId);
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

        //Adds users in a guild to database
        public void AddUsersToDatabase(ulong discordId, ulong guildId)
        {
            var s = new SQLiteConnection(@_dbFile);
            try
            {
                using (s)
                {
                    using (var cmd = s.CreateCommand())
                    {
                        s.Open();
                        //"INSERT INTO USER (discordId, canvasId) VALUES(@discordIdQuery, @canvasIdQuery)";
                        cmd.CommandText = "INSERT INTO USER (discordId, guildId) VALUES(@discordIdQuery, @guildIdQuery)";
                        cmd.Parameters.AddWithValue( "@discordIdQuery",discordId);
                        cmd.Parameters.AddWithValue( "@guildIdQuery",guildId);
                        //Execute query
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (SQLiteException exc)
            {
                //Console.WriteLine(exc.Message);
                Console.WriteLine("User already in database or error has occured");

            }
        }
        //Update guild settings to database
        //Change optionOne Guild
        //Placeholder name
        public void UpdateGuildSettingsOptionOne(long guildId, long value)
                 {
                     var s = new SQLiteConnection(@_dbFile);
                     try
                     {
                         using (s)
                         {
                             using (var cmd = s.CreateCommand())
                             {
                                 s.Open();
                                 cmd.CommandText = $"UPDATE GUILD SET OptionOne = @valueQuery WHERE GuildId = @GuildIdQuery";
                                 cmd.Parameters.AddWithValue("@GuildIdQuery",guildId);
                                 cmd.Parameters.AddWithValue("@valueQuery",value);
         
                                 //Execute query
                                 cmd.ExecuteNonQuery();
                             }
                         }
                     }
                     catch (SQLiteException exc)
                     {
                         Console.WriteLine(exc.Message);
                         //Console.WriteLine("Error has occurred");
         
                     }
                 }
        //Change OptionTwo Guild
        public void UpdateGuildSettingsOptionTwo(long guildId, long value)
        {
            var s = new SQLiteConnection(@_dbFile);
            try
            {
                using (s)
                {
                    using (var cmd = s.CreateCommand())
                    {
                        s.Open();
                        cmd.CommandText = $"UPDATE GUILD SET OptionTwo = @valueQuery WHERE GuildId = @GuildIdQuery";
                        cmd.Parameters.AddWithValue("@GuildIdQuery",guildId);
                        cmd.Parameters.AddWithValue("@valueQuery",value);

                        //Execute query
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (SQLiteException exc)
            {
                Console.WriteLine(exc.Message);
                //Console.WriteLine("Error has occurred");

            }
        }
        //Update discord user settings to database
        public void UpdateDiscordUserSettingsOptionOne(string discordId, string guildId, long value)
        {
            var s = new SQLiteConnection(@_dbFile);
            try
            {
                using (s)
                {
                    using (var cmd = s.CreateCommand())
                    {
                        s.Open();
                        cmd.CommandText = $"UPDATE USER SET OptionOne = @valueQuery WHERE GuildId = @GuildIdQuery AND DiscordID = @DiscordIdQuery";
                        cmd.Parameters.AddWithValue("@GuildIdQuery",guildId);
                        cmd.Parameters.AddWithValue("@DiscordIdQuery",discordId);
                        cmd.Parameters.AddWithValue("@valueQuery",value);
                        //Execute query
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (SQLiteException exc)
            {
                Console.WriteLine(exc.Message);
                //Console.WriteLine("Error has occurred");

            }
        }
        public void UpdateDiscordUserSettingsOptionTwo(string discordId, string guildId, long value)
        {
            var s = new SQLiteConnection(@_dbFile);
            try
            {
                using (s)
                {
                    using (var cmd = s.CreateCommand())
                    {
                        s.Open();
                        cmd.CommandText = $"UPDATE USER SET OptionTwo = @valueQuery WHERE GuildId = @GuildIdQuery AND DiscordID = @DiscordIdQuery";
                        cmd.Parameters.AddWithValue("@GuildIdQuery",guildId);
                        cmd.Parameters.AddWithValue("@DiscordIdQuery",discordId);
                        cmd.Parameters.AddWithValue("@valueQuery",value);
                        //Execute query
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (SQLiteException exc)
            {
                Console.WriteLine(exc.Message);
                //Console.WriteLine("Error has occurred");

            }
        }
    }
}
