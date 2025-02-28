using Npgsql;
// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");


var connString = "Host=127.0.0.1;Username=postgres;Password=postgres;Database=esport";

await using var conn = new NpgsqlConnection(connString);
await conn.OpenAsync();

// Retrieve all rows
await using (var cmd = new NpgsqlCommand("SELECT username FROM players", conn))
await using (var reader = await cmd.ExecuteReaderAsync())
{
while (await reader.ReadAsync())
    Console.WriteLine(reader.GetString(0));
}
