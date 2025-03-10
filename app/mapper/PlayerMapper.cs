using Npgsql;

public class PlayerMapper(string connectionString)
{
    private string _connectionString = connectionString;

    public async Task RegisterPlayer(Player player)
    {
        await using (var conn = new NpgsqlConnection(_connectionString))
        {
            await conn.OpenAsync();

            await using (var cmd = new NpgsqlCommand("CALL registerplayer(@username, @email)", conn))
            {
                cmd.Parameters.AddWithValue("username", player.GetUsername());
                cmd.Parameters.AddWithValue("email", player.GetEmail());

                await cmd.ExecuteNonQueryAsync();
            }
        }  
        Console.WriteLine($"Player with username '{player.GetUsername()}' has been registered.");
    }
}
