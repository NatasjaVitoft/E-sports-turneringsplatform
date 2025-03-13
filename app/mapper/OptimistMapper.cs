
using Npgsql; 
 
public class OptimistMapper(string connectionString)
{
    private string _connectionString = connectionString;

    public async Task<bool> SetStartDate(int tournamentId, DateOnly date)
    {
        await using (var conn = new NpgsqlConnection(_connectionString))
        {
            await conn.OpenAsync();
            await using (NpgsqlTransaction tx = await conn.BeginTransactionAsync())
            {
                try
                {
                    // Get initial version
                    int initVersion;
                    await using (var cmd = new NpgsqlCommand("SELECT version FROM tournaments WHERE tournament_id = @tournament_id", conn, tx))
                    {
                        cmd.Parameters.AddWithValue("tournament_id", tournamentId);
                        await using var reader = await cmd.ExecuteReaderAsync();
                        if (!await reader.ReadAsync())
                        {
                            return false;
                        }
                        initVersion = reader.GetInt32(0);
                    }

                    // Update date and version
                    await using (var cmd2 = new NpgsqlCommand(
                        "UPDATE tournaments SET start_date = @date, version = version + 1 WHERE tournament_id = @tournament_id RETURNING version", conn, tx))
                    {
                        cmd2.Parameters.AddWithValue("date", date);
                        cmd2.Parameters.AddWithValue("tournament_id", tournamentId);
                        int commitVersion = (int) await cmd2.ExecuteScalarAsync();

                        // Commit if incremented once
                        // If versions does not fit, another operation has happened during the transaction 
                        if (commitVersion == initVersion + 1)
                        {
                            await tx.CommitAsync();
                            return true;
                        }
                        else
                        {
                            await tx.RollbackAsync();
                            return false;
                        }
                    }
                }
                catch (Exception)
                {
                    await tx.RollbackAsync();
                    throw;
                }
            }
        }
    }
}