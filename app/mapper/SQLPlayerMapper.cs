using Npgsql;
using Npgsql.TypeHandlers.NetworkHandlers;

public class SQLPlayerMapper(string connectionString)
{
    private string _connectionString = connectionString;

    public async Task RegisterPlayer(Player player)
    {
        await using (var conn = new NpgsqlConnection(_connectionString))
        {
            await conn.OpenAsync();

            await using (var cmd = new NpgsqlCommand("INSERT INTO players (username, email) VALUES (@username, @email);", conn))
            {
                cmd.Parameters.AddWithValue("username", player.GetUsername());
                cmd.Parameters.AddWithValue("email", player.GetEmail());

                await cmd.ExecuteNonQueryAsync();
            }
        }
        
        Console.WriteLine($"Player with username '{player.GetUsername()}' has been registered.");
    }

    public async Task JoinTournament(int playerId, int tournamentId)
    {
        await using (var conn = new NpgsqlConnection(_connectionString))
        {
            await conn.OpenAsync();

            await using (var cmd = new NpgsqlCommand("INSERT INTO tournament_registrations (tournament_id, player_id) VALUES (@tournament_id, @player_id);", conn))
            {
                cmd.Parameters.AddWithValue("player_id", playerId);
                cmd.Parameters.AddWithValue("tournament_id", tournamentId);

                await cmd.ExecuteNonQueryAsync();
            }
        }
        
        Console.WriteLine($"Player with id '{playerId}' has been registered and tournament id: {tournamentId}.");
    }

    public async Task SubmitMatchResult(int matchId, int winnerId)
    {
        await using (var conn = new NpgsqlConnection(_connectionString))
        {
            await conn.OpenAsync();

            await using (var cmd = new NpgsqlCommand("""
                UPDATE matches
                SET winner_id = @winner_id
                WHERE match_id = @match_id;
            """, conn))
            {
                cmd.Parameters.AddWithValue("match_id", matchId);
                cmd.Parameters.AddWithValue("winner_id", winnerId);

                await cmd.ExecuteNonQueryAsync();
            }
        }
        
        Console.WriteLine($"Player ('{winnerId}') has been registered as winner on match ('{matchId}').");
    }
}