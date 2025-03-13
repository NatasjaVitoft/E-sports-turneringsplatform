using Npgsql;



var connString = "Host=127.0.0.1;Username=postgres;Password=postgres;Database=esports";


/* What mapper to use */

// PlayerMapper pm = new(connString);
SQLMapper pm = new(connString);
OptimistMapper om = new(connString);
PessimistMapper pem = new(connString);

while (true)
{
    Console.WriteLine("1. Register Player");
    Console.WriteLine("2. Join Tournament");
    Console.WriteLine("3. Submit Match Result");
    Console.WriteLine("4. Submit Match Result pessimistically");
    Console.WriteLine("5. Update tournament date optimistically");
    Console.WriteLine("6. Pessimistic stress test Match result update");
    Console.WriteLine("7. Optimistic stress test set tournament start date");
    Console.WriteLine("8. Pessimistic Register player to tournament");
    Console.WriteLine("9. Register player to tournament stress test");
    Console.WriteLine("q. Quit");

    switch (Console.ReadLine())
    {
        case "1":
            Console.WriteLine("Enter username:");
            var username = Console.ReadLine();
            Console.WriteLine("Enter email:");
            var email = Console.ReadLine();

            Player newPlayer = new(username, email);

            try { await pm.RegisterPlayer(newPlayer); }
            catch (NpgsqlException e) { Console.WriteLine(e.Message); }
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
        case "4":
            // submit match result pessimistically 

            Console.WriteLine("Enter Match ID:");
            if (int.TryParse(Console.ReadLine(), out int pesMatchId))
                Console.WriteLine("Enter Winner ID:");

            if (int.TryParse(Console.ReadLine(), out int pesWinnerId))

                if (await pem.SetMatchResult(pesMatchId, pesWinnerId))
                {
                    Console.WriteLine("Submitted match result!");
                }

            break;

        case "5":
            Console.WriteLine("Enter Tournament ID:");
            if (int.TryParse(Console.ReadLine(), out t_id))
                Console.WriteLine("Enter Start date:");
            if (DateOnly.TryParse(Console.ReadLine(), out DateOnly date))


                try
                {
                    if (await om.SetStartDate(t_id, date))
                    {
                        Console.WriteLine($"Succesfully set start date to {date}");
                    }

                    else Console.WriteLine("Update failed");
                }
                catch (NpgsqlException e) { Console.WriteLine(e.Message, e.StackTrace); }

            break;
        case "6":
            Console.WriteLine("Number of threads:");
            if (int.TryParse(Console.ReadLine(), out int pnumThreads))
            await pem.SetMatchResultStressTest(pnumThreads);
            break;       
            
        case "7":
            Console.WriteLine("Number of threads:");
            if (int.TryParse(Console.ReadLine(), out int onumThreads))
            await om.SetStartDateStressTest(onumThreads);
            break;

        case "8":
            Console.WriteLine("Enter Tournament ID:");
            if (int.TryParse(Console.ReadLine(), out t_id))
                Console.WriteLine("Enter player ID:");

            if (int.TryParse(Console.ReadLine(), out p_id))

                if (await om.RegisterPlayerToTournamentOptimistic(p_id, t_id))
                {
                    Console.WriteLine("Registered player");
                }
                else Console.WriteLine("Failed to register player");

            break;

        case "9":
            Console.WriteLine("Number of threads:");
            if (int.TryParse(Console.ReadLine(), out int rnumThreads))
            await om.RegisterPlayerTournamentStressTest(rnumThreads);       
            break;    

        case "q":
            return;
    }
}

