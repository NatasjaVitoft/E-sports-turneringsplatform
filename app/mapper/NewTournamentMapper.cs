using System;
using System.Threading.Tasks;
using Npgsql;

public class NewTournamentMapper
{
    private readonly string _connectionString;

    public NewTournamentMapper(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task JoinTournament(int playerId, int tournamentId)
    {
        await using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();

        await using var transaction = await conn.BeginTransactionAsync();

        try
        {
            int maxPlayers = await GetMaxPlayers(conn, tournamentId, transaction);
            int currentRegistrations = await GetCurrentRegistrations(conn, tournamentId, transaction);

            if (currentRegistrations >= maxPlayers)
            {
                Console.WriteLine($"Tournament {tournamentId} is full. Player {playerId} cannot join.");
                await transaction.RollbackAsync(); 
                return;  
            }

            await RegisterPlayerForTournament(conn, playerId, tournamentId, transaction);

            await transaction.CommitAsync();
            Console.WriteLine($"Player {playerId} successfully joined tournament {tournamentId}.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}\n{ex.StackTrace}");
            await transaction.RollbackAsync(); 
        }
    }

    private async Task<int> GetMaxPlayers(NpgsqlConnection conn, int tournamentId, NpgsqlTransaction transaction)
    {
        await using var cmd = new NpgsqlCommand("SELECT max_players FROM tournaments WHERE tournament_id = @tournamentId", conn, transaction);
        cmd.Parameters.AddWithValue("tournamentId", tournamentId);
        return Convert.ToInt32(await cmd.ExecuteScalarAsync());
    }

    private async Task<int> GetCurrentRegistrations(NpgsqlConnection conn, int tournamentId, NpgsqlTransaction transaction)
    {
        await using var cmd = new NpgsqlCommand("SELECT COUNT(*) FROM tournament_registrations WHERE tournament_id = @tournamentId", conn, transaction);
        cmd.Parameters.AddWithValue("tournamentId", tournamentId);
        return Convert.ToInt32(await cmd.ExecuteScalarAsync());
    }

    private async Task RegisterPlayerForTournament(NpgsqlConnection conn, int playerId, int tournamentId, NpgsqlTransaction transaction)
    {
        await using var cmd = new NpgsqlCommand("CALL join_tournament(@p_tournament_id, @p_player_id)", conn, transaction);
        cmd.Parameters.AddWithValue("p_tournament_id", tournamentId);
        cmd.Parameters.AddWithValue("p_player_id", playerId);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task TestJoinTournamentLogic()
    {
        int testTournamentId = 4; 
        int testPlayerId1 = 1; 
        int testPlayerId2 = 2;
        //int testPlayerId3 = 3; 

        Console.WriteLine("Player 1 is joining the tournament");
        await JoinTournament(testPlayerId1, testTournamentId); 

        Console.WriteLine("Player 2 is joining the tournament");
        await JoinTournament(testPlayerId2, testTournamentId); 

        //Console.WriteLine("Player 3 joining the tournament...");
        //await JoinTournament(testPlayerId3, testTournamentId); 
    }
}
