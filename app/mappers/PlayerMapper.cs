using Npgsql;

public class PlayerMapper
{
    private string _connectionString;

    public PlayerMapper(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task RegisterPlayer(Player player)
    {
        await using (var conn = new NpgsqlConnection(_connectionString))
        {
            await conn.OpenAsync();

            await using (var cmd = new NpgsqlCommand("CALL registerplayer(@username, @email)", conn))
            {
                cmd.Parameters.AddWithValue("username", player.username);
                cmd.Parameters.AddWithValue("email", player.email);

                await cmd.ExecuteNonQueryAsync();
            }
        }

        Console.WriteLine($"Player with username '{player.username}' has been registered.");
    }
}
