/**
using Npgsql;
// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");


var connString = "Host=127.0.0.1;Username=postgres;Password=TEST!!;Database=esports";

await using var conn = new NpgsqlConnection(connString);
await conn.OpenAsync();

// Retrieve all rows
await using (var cmd = new NpgsqlCommand("SELECT username FROM players", conn))
await using (var reader = await cmd.ExecuteReaderAsync())
{
while (await reader.ReadAsync())
    Console.WriteLine(reader.GetString(0));
} */

using System;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        var player = new Player("NatasjaKaroline", "Test email");

        string connectionString = "Host=localhost;Username=postgres;Password=TEST;Database=esports";

        var playerMapper = new PlayerMapper(connectionString);

        await playerMapper.RegisterPlayer(player);
;
    }
}

