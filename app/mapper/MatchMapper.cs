using Npgsql;

public class MatchMapper
{
    private string _connectionString;

    public MatchMapper(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task SubmitMatchResult(int matchId, int winnerId)
    {
        await using (var conn = new NpgsqlConnection(_connectionString))
        {
            await conn.OpenAsync();

            await using (var cmd = new NpgsqlCommand("CALL  submit_match_result(@p_match_id, @p_winner_id)", conn))
            {
                cmd.Parameters.AddWithValue("p_match_id", matchId);
                cmd.Parameters.AddWithValue("p_winner_id", winnerId);

                await cmd.ExecuteNonQueryAsync();
            }
        }

        Console.WriteLine($"Player with ID '{winnerId}' has won the match with ID '{matchId}'");
    }
}
