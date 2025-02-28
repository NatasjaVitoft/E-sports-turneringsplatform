using Npgsql;

var connString = "Host=127.0.0.1;Username=postgres;Password=postgres;Database=esports";

await using var conn = new NpgsqlConnection(connString);
await conn.OpenAsync();

// Retrieve all rows
/* await using (var cmd = new NpgsqlCommand("SELECT username FROM players", conn))
await using (var reader = await cmd.ExecuteReaderAsync())
{
    while (await reader.ReadAsync())
        Console.WriteLine(reader.GetString(0));
}

conn.Close(); */

while (true)
{
    Console.WriteLine("1. Register Player");
    Console.WriteLine("q. Quit");

    switch (Console.ReadLine())
    {
        case "1":
            Console.WriteLine("Enter username:");
            var username = Console.ReadLine();
            Console.WriteLine("Enter email:");
            var email = Console.ReadLine();

            Player newPlayer = new(username, email);
            Console.WriteLine(newPlayer.GetUsername(), newPlayer.GetEmail());
            // PLaceholder for inserting into the database
            Console.WriteLine("Player registered!");

            break;
        case "q":
            conn.Close();
            return;
    }
}


