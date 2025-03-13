
using System.Collections.Concurrent;
using System.Diagnostics;
using Npgsql; 
 
public class OptimistMapper(string connectionString)
{
    //  stresstes
    int successfulTransactions = 0;
    int rollbacks = 0;
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
                            Interlocked.Increment(ref successfulTransactions);
                            return true;
                        }
                        else
                        {
                            await tx.RollbackAsync();
                            Interlocked.Increment(ref rollbacks);
                            return false;
                        }
                    }
                }
                catch (Exception)
                {
                    await tx.RollbackAsync();
                    Interlocked.Increment(ref rollbacks);
                    throw;
                }
            }
        }
    }

    public async Task SetStartDateStressTest(int numThreads)
    {
        this.successfulTransactions = 0;
        this.rollbacks = 0;
        
        Console.WriteLine($"Starting stress test with {numThreads} threads...");
        Stopwatch stopwatch = Stopwatch.StartNew();

        var tasks = new ConcurrentBag<Task>();
        var rnd = new Random();

        for (int i = 0; i < numThreads; i++)
        {
            int day = rnd.Next(1, 29);
            int month = rnd.Next(1, 13);
            int year = rnd.Next(2025, 2101);
            tasks.Add(Task.Run(() => SetStartDate(1, DateOnly.Parse($"{day}-{month}-{year}"))));
        }

        await Task.WhenAll(tasks);
        stopwatch.Stop();

        Console.WriteLine("\n--- Stress Test Results ---");
        Console.WriteLine($"Successful Transactions: {successfulTransactions}");
        Console.WriteLine($"Rollbacks: {rollbacks}");
        Console.WriteLine($"Time elapsed {stopwatch.Elapsed}");
        Console.WriteLine("---------------------------");
    }

    public async Task<bool> RegisterPlayerToTournamentOptimistic(int playerId, int tournamentId)
    {
        await using (var conn = new NpgsqlConnection(_connectionString))
        {
            await conn.OpenAsync();
            await using (NpgsqlTransaction tx = await conn.BeginTransactionAsync())
            {
                try
                {
                    await using (var insertBatch = new NpgsqlBatch(conn, tx))
                    {
                        insertBatch.BatchCommands.Add(new NpgsqlBatchCommand("LOCK TABLE tournament_registrations IN ACCESS EXCLUSIVE MODE"));

                        var insertCmd = new NpgsqlBatchCommand(
                            $"INSERT INTO tournament_registrations (tournament_id, player_id) VALUES (@tournament_id, @player_id)");
                        insertCmd.Parameters.AddWithValue("tournament_id", tournamentId);
                        insertCmd.Parameters.AddWithValue("player_id", playerId);

                        insertBatch.BatchCommands.Add(insertCmd);

                        await insertBatch.ExecuteNonQueryAsync();
                    }

                    int max_players;
                    await using (var selectCmd = new NpgsqlCommand("SELECT max_players FROM tournaments WHERE tournament_id = @tournament_id", conn, tx))
                    {
                        selectCmd.Parameters.AddWithValue("tournament_id", tournamentId);
                        using var mpReader = await selectCmd.ExecuteReaderAsync();
                        if (!await mpReader.ReadAsync())
                        {
                            throw new Exception($"No tournament with id: {tournamentId}");
                        }
                        max_players = mpReader.GetInt32(0);
                    }

                    int count;
                    await using (var countCmd = new NpgsqlCommand("SELECT COUNT(*) FROM tournament_registrations WHERE tournament_id = @tournament_id", conn, tx))
                    {
                        countCmd.Parameters.AddWithValue("tournament_id", tournamentId);
                        using var cReader = await countCmd.ExecuteReaderAsync();
                        if (!await cReader.ReadAsync())
                        {
                            throw new Exception($"No Registrations found with tournament: {tournamentId}");
                        }

                        count = cReader.GetInt32(0);
                    }

                    if (count <= max_players)
                    {
                        await tx.CommitAsync();
                        Interlocked.Increment(ref successfulTransactions);
                        Console.WriteLine($"Player {playerId} registered at tournament {tournamentId}");
                        return true;
                    }

                    await tx.RollbackAsync();
                    Console.WriteLine($"Max players reached in tournament: {tournamentId}");
                    Interlocked.Increment(ref rollbacks);
                    return false;
                }
                catch (Exception e)
                {
                    await tx.RollbackAsync();
                    Interlocked.Increment(ref rollbacks);
                    Console.WriteLine(e.Message);
                    return false;
                }
            }
        }
    }

    public async Task RegisterPlayerTournamentStressTest(int numThreads)
    {
        this.successfulTransactions = 0;
        this.rollbacks = 0;
        
        Console.WriteLine($"Starting stress test with {numThreads} threads...");
        Stopwatch stopwatch = Stopwatch.StartNew();

        var tasks = new ConcurrentBag<Task>();
        var rnd = new Random();

        for (int i = 0; i < numThreads; i++)
        {
            int playerId = rnd.Next(1, 10);
            tasks.Add(Task.Run(() => RegisterPlayerToTournamentOptimistic(playerId, 5)));
        }

        await Task.WhenAll(tasks);
        stopwatch.Stop();

        Console.WriteLine("\n--- Stress Test Results ---");
        Console.WriteLine($"Successful Transactions: {successfulTransactions}");
        Console.WriteLine($"Rollbacks: {rollbacks}");
        Console.WriteLine($"Time elapsed {stopwatch.Elapsed}");
        Console.WriteLine("---------------------------");
    }
}