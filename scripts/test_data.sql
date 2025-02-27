INSERT INTO players (username, email, ranking)
VALUES
	('Lasse', 'lasse@email.dk', 1000),
	('Natasja', 'natasja@email.dk', 1500),
	('Hans', 'hans@email.dk', 1200)

INSERT INTO tournaments (t_name, game, max_players, start_date)
VALUES ('PONG WORLD CHAMPIONSHIP', 'Pong', 500, '2025-6-10'),
    	('Mario Kart Cup', 'Mario Kart', 100, '2025-2-28'),
    	('Sims Architecture', 'The Sims 4', 50, '2025-4-20');

INSERT INTO matches (tournament_id, player1_id, player2_id, winner_id, match_date)
VALUES
	(1, 1, 3, 3, '2025-06-11'),  
	(2, 1, 2, 2, '2025-03-01'),  
	(3, 2, 3, 3, '2025-04-20'); 
