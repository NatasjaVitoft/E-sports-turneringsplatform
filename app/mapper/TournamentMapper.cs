using Npgsql;

public class TournamentMapper
{
    private string _connectionString;

    public TournamentMapper(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task JoinTournament(int playerId, int tournamentId)
    {
        await using (var conn = new NpgsqlConnection(_connectionString))
        {
            await conn.OpenAsync();

            await using (var cmd = new NpgsqlCommand("CALL  join_tournament(@p_tournament_id, @p_player_id)", conn))
            {
                cmd.Parameters.AddWithValue("p_tournament_id", tournamentId);
                cmd.Parameters.AddWithValue("p_player_id", playerId);

                await cmd.ExecuteNonQueryAsync();
            }
        }

        Console.WriteLine($"Player with ID '{playerId}' has joined tournament with ID '{tournamentId}'");
    }
}
