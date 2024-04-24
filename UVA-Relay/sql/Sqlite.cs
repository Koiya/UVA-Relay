using System;
using System.Data.SQLite;
/*
 TODO
 - set up sqlite with a test server
 - Display guild settings with embed message
    Name
    option1 : value
    option2 : value
    ...
*/
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
                        //Safest way to insert data without sql injections?
                        var discordIdParameter = cmd.CreateParameter();
                        //var canvasIdParameter = cmd.CreateParameter();
                        var guildIdParameter = cmd.CreateParameter();
                        discordIdParameter.ParameterName = "@discordIdQuery";
                        guildIdParameter.ParameterName = "@guildIdQuery";
                        //canvasIdParameter.ParameterName = "@canvasIdQuery";
                        cmd.Parameters.Add(discordIdParameter);
                        cmd.Parameters.Add(guildIdParameter);
                        //cmd.Parameters.Add(canvasIdParameter);
                        discordIdParameter.Value = discordId;
                        guildIdParameter.Value = guildId;
                        //canvasIdParameter.Value = ;
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
        public void UpdateGuildSettings(long guildId, long value, string option)
        {
            var s = new SQLiteConnection(@_dbFile);
            try
            {
                using (s)
                {
                    using (var cmd = s.CreateCommand())
                    {
                        s.Open();
                        cmd.CommandText = $"UPDATE GUILD SET {option} = @valueQuery WHERE GuildId = @GuildIdQuery";
                        var optionParam = cmd.CreateParameter();
                        var guildIdParameter = cmd.CreateParameter();
                        var valueParameter = cmd.CreateParameter();
                        optionParam.ParameterName = "@optionQuery";
                        guildIdParameter.ParameterName = "@GuildIdQuery";
                        valueParameter.ParameterName = "@valueQuery";
                        cmd.Parameters.Add(optionParam);
                        cmd.Parameters.Add(guildIdParameter);
                        cmd.Parameters.Add(valueParameter);
                        optionParam.Value = option;
                        guildIdParameter.Value = guildId;
                        valueParameter.Value = value;
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
        //Update guild settings to database
        public void UpdateDiscordUserSettings(string discordId, string guildId, long value, string option)
        {
            var s = new SQLiteConnection(@_dbFile);
            try
            {
                using (s)
                {
                    using (var cmd = s.CreateCommand())
                    {
                        s.Open();
                        cmd.CommandText = $"UPDATE USER SET {option} = @valueQuery WHERE GuildId = @GuildIdQuery AND DiscordID = @DiscordIdQuery";
                        var optionParam = cmd.CreateParameter();
                        var guildIdParameter = cmd.CreateParameter();
                        var discordIdParameter = cmd.CreateParameter();
                        var valueParameter = cmd.CreateParameter();
                        optionParam.ParameterName = "@optionQuery";
                        guildIdParameter.ParameterName = "@GuildIdQuery";
                        discordIdParameter.ParameterName = "@DiscordIdQuery";
                        valueParameter.ParameterName = "@valueQuery";
                        cmd.Parameters.Add(optionParam);
                        cmd.Parameters.Add(guildIdParameter);
                        cmd.Parameters.Add(discordIdParameter);
                        cmd.Parameters.Add(valueParameter);
                        optionParam.Value = option;
                        guildIdParameter.Value = guildId;
                        discordIdParameter.Value = discordId;
                        valueParameter.Value = value;
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
