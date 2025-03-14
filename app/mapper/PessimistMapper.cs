using System.Collections.Concurrent;
using System.Diagnostics;
using Npgsql;

public class PessimistMapper(string connectionString)
{
    //  stresstest
    int totalAttempts = 0;
    int successfulTransactions = 0;
    int deadlocks = 0;
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
                    // Lock Table for update
                    var lockCmd = new NpgsqlCommand("SELECT * FROM tournaments_pes WHERE tournament_id = @tournament_id FOR UPDATE NOWAIT", conn, tx);
                    lockCmd.Parameters.AddWithValue("tournament_id", tournamentId);
                    var lockResult = await lockCmd.ExecuteScalarAsync();
                        
                    if (lockResult != null)
                    {
                        // Update date  
                        await using (var cmd2 = new NpgsqlCommand(
                            "UPDATE tournaments_pes SET start_date = @date WHERE tournament_id = @tournament_id", conn, tx))
                        {
                            cmd2.Parameters.AddWithValue("date", date);
                            cmd2.Parameters.AddWithValue("tournament_id", tournamentId);
                            await cmd2.ExecuteScalarAsync();
                        }
                    }
                    await tx.CommitAsync();
                    Console.WriteLine($"Starte date updated on tournament: {tournamentId}");
                    Interlocked.Increment(ref successfulTransactions);
                    return true;
                }
                catch (Exception e)
                {
                    await tx.RollbackAsync();
                    Interlocked.Increment(ref rollbacks);
                    Console.WriteLine($"Update failed: {e.Message}");
                    return false;
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


    public async Task<bool> SetMatchResult(int matchId, int winnerId)
    {
        int maxRetries = 10;
        int delay = 100;
        await using (var conn = new NpgsqlConnection(_connectionString))
        {
            await conn.OpenAsync();
            using (NpgsqlTransaction tx = await conn.BeginTransactionAsync())
            {
                Interlocked.Increment(ref totalAttempts);
                for (int a = 0; a < maxRetries; a++)
                {
                    try
                    {
                        var lockCmd = new NpgsqlCommand("SELECT * FROM matches WHERE match_id = @match_id FOR UPDATE", conn, tx);
                        lockCmd.Parameters.AddWithValue("match_id", matchId);
                        var lockResult = await lockCmd.ExecuteScalarAsync();

                        if (lockResult != null)
                        {
                            var updateCmd = new NpgsqlCommand("UPDATE matches SET winner_id = @winner_id WHERE match_id = @match_id", conn, tx);
                            updateCmd.Parameters.AddWithValue("match_id", matchId);
                            updateCmd.Parameters.AddWithValue("winner_id", winnerId);

                            await updateCmd.ExecuteNonQueryAsync();
                        }

                        await tx.CommitAsync();
                        Interlocked.Increment(ref successfulTransactions);
                        Console.WriteLine($"match: {matchId} winner was set as player: {winnerId}");
                        return true;
                    }
                    catch (PostgresException ex) when (ex.SqlState == "40001") // Deadlock detected
                    {
                        Console.WriteLine($"Deadlock detected on attempt {a}, retrying...");
                        await tx.RollbackAsync();
                        Interlocked.Increment(ref rollbacks);
                        Interlocked.Increment(ref deadlocks);

                        await Task.Delay(delay);
                        delay *= 2;
                    }

                    catch (Exception e)
                    {
                        Console.WriteLine($"Transaction failed: {e.Message}");
                        Interlocked.Increment(ref rollbacks);
                        await tx.RollbackAsync();
                        return false;
                    }
                }

                Console.WriteLine("Max retries reached. Aborting.");
                return false;
            }
        }
    }

    public async Task SetMatchResultStressTest(int numThreads)
    {
        this.totalAttempts = 0;
        this.successfulTransactions = 0;
        this.deadlocks = 0;
        
        Console.WriteLine($"Starting stress test with {numThreads} threads...");
        Stopwatch stopwatch = Stopwatch.StartNew();

        var tasks = new ConcurrentBag<Task>();
        var rnd = new Random();

        for (int i = 0; i < numThreads; i++)
        {
            tasks.Add(Task.Run(() => SetMatchResult(1, rnd.Next(1, 9))));
        }

        await Task.WhenAll(tasks);
        stopwatch.Stop();

        Console.WriteLine("\n--- Stress Test Results ---");
        Console.WriteLine($"Total Attempts: {totalAttempts}");
        Console.WriteLine($"Successful Transactions: {successfulTransactions}");
        Console.WriteLine($"Rollbacks: {rollbacks}");
        Console.WriteLine($"Deadlocks Detected: {deadlocks}");
        Console.WriteLine($"Time elapsed {stopwatch.Elapsed}");
        Console.WriteLine("---------------------------");
    }
}