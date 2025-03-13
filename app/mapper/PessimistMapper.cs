using System.Collections.Concurrent;
using System.Diagnostics;
using Npgsql;

public class PessimistMapper(string connectionString)
{
    //  stresstest
    int totalAttempts = 0;
    int successfulTransactions = 0;
    int deadlocks = 0;
    private string _connectionString = connectionString;

    public async Task<bool> SetMatchResult(int matchId, int winnerId)
    {
        int maxRetries = 10;
        int delay = 100;
        await using (var conn = new NpgsqlConnection(_connectionString))
        {
            await conn.OpenAsync();
            await using (NpgsqlTransaction tx = await conn.BeginTransactionAsync())
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
                        await tx.RollbackAsync();
                        Interlocked.Increment(ref deadlocks);
                        Console.WriteLine($"Deadlock detected on attempt {a}, retrying...");

                        await Task.Delay(delay);
                        delay *= 2;
                    }

                    catch (Exception e)
                    {
                        Console.WriteLine($"Transaction failed: {e.Message}");
                        await tx.RollbackAsync();
                        return false;
                    }
                }

                Console.WriteLine("Max retries reached. Aborting.");
                return false;
            }
        }
    }

    public async Task SetMatchResultStressTest()
    {
        int numThreads = 50; // Number of concurrent tasks
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
        Console.WriteLine($"Deadlocks Detected: {deadlocks}");
        Console.WriteLine($"Time elapsed {stopwatch.Elapsed}");
        Console.WriteLine("---------------------------");
    }
}