using Npgsql;


var connString = "Host=127.0.0.1;Username=postgres;Password=postgres;Database=esports";


/* What mapper to use */

// PlayerMapper pm = new(connString);
SQLMapper pm = new(connString);

while (true)
{
    Console.WriteLine("1. Register Player");
    Console.WriteLine("2. Join Tournament");
    Console.WriteLine("3. Submit Match Result");
    Console.WriteLine("q. Quit");

    switch (Console.ReadLine())
    {
        case "1":
            Console.WriteLine("Enter username:");
            var username = Console.ReadLine();
            Console.WriteLine("Enter email:");
            var email = Console.ReadLine();

            Player newPlayer = new(username, email);

            try {await pm.RegisterPlayer(newPlayer);}
            catch (NpgsqlException e) {Console.WriteLine(e.Message);}
            break;
        
        case "2":
            // Join tournament
            Console.WriteLine("Enter Tournament ID:");
            if (int.TryParse(Console.ReadLine(), out int t_id))
            Console.WriteLine("Enter player ID:");

            if (int.TryParse(Console.ReadLine(), out int p_id))

            try { await pm.JoinTournament(p_id, t_id); }
            catch (NpgsqlException e) { Console.WriteLine(e.Message); }

            Console.WriteLine("Joined Tournament!");
            break;

        case "3":
            // submit match result
            Console.WriteLine("Enter Match ID:");
            if (int.TryParse(Console.ReadLine(), out int m_id))
            Console.WriteLine("Enter Winner ID:");

            if (int.TryParse(Console.ReadLine(), out int w_id))

            try { await pm.SubmitMatchResult(m_id, w_id); }
            catch (NpgsqlException e) { Console.WriteLine(e.Message); }

            Console.WriteLine("Submitted match result!");
            // ...
            break;

        case "q":
            return;
    }
}

