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
                cmd.Parameters.AddWithValue("username", player.GetUsername());
                cmd.Parameters.AddWithValue("email", player.GetEmail());

                await cmd.ExecuteNonQueryAsync();
            }
        }

        Console.WriteLine($"Player with username '{player.GetUsername()}' has been registered.");
    }

    public async Task UpdatePlayerRanking(int playerId)
    {
        // Unique ID for each task for test purposes    
        var taskId = Guid.NewGuid();

        await using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();

        const int maxRetries = 3;
        int currentRetry = 0;

        while (currentRetry < maxRetries)
        {
            await using var transaction = await conn.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);

            try
            {
                Console.WriteLine($"Task {taskId}: Trying to update ranking for player ID {playerId}");
                Console.WriteLine($"Task {taskId}: Lock for player ID {playerId}");

                await using (var cmd = new NpgsqlCommand("CALL update_player_ranking(@playerId)", conn, transaction))
                {
                    cmd.Parameters.AddWithValue("playerId", playerId);
                    await cmd.ExecuteNonQueryAsync();
                }

                await transaction.CommitAsync();
                Console.WriteLine($"Task {taskId}: Ranking updated for player ID {playerId}.");
                return; 

            }
            catch (PostgresException ex) when (ex.SqlState == "40001")
            {
                // If deadlock is detected, retry the transaction
                currentRetry++;
                Console.WriteLine($"Task {taskId}: Deadlock detected while updating ranking for player {playerId}. Retrying {currentRetry} out of {maxRetries}...");

                if (transaction != null && transaction.Connection != null)
                {
                    await transaction.RollbackAsync();
                }

                await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, currentRetry))); 

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Task {taskId}: Error occurred - {ex.Message}");

                if (transaction != null && transaction.Connection != null)
                {
                    await transaction.RollbackAsync();
                }
                break; 
            }
        }

        if (currentRetry == maxRetries)
        {
            Console.WriteLine($"Task {taskId}: Failed to update ranking for player with ID: {playerId} after {maxRetries} retries.");
        }
    }

    public async Task TestUpdatePlayerRankingLogicFail()
    {
        int playerId = 1;

        Task task1 = Task.Run(() => UpdatePlayerRanking(playerId));
        Task task2 = Task.Run(() => UpdatePlayerRanking(playerId));

        await Task.WhenAll(task1, task2);
    }

    public async Task TestUpdatePlayerRankingLogicSuccess()
    {
        int playerId = 1;

        await UpdatePlayerRanking(playerId);
        await UpdatePlayerRanking(playerId);
    }

}

