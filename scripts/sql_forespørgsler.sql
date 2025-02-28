-- 1 : Hent alle turneringer, der starter inden for de næste 30 dage.

SELECT * FROM tournaments
WHERE start_date BETWEEN CURRENT_DATE AND CURRENT_DATE + INTERVAL '30 days';

-- 2 : Find det antal turneringer, en spiller har deltaget i.

SELECT player_id, COUNT(tournament_id) AS total_tournaments
FROM tournament_registrations
GROUP BY player_id
ORDER BY total_tournaments DESC;

-- 3 : Vis en liste over spillere registreret i en bestemt turnering.

SELECT p.player_id, p.username, tr.registered_at, tr.tournament_id
FROM players p
JOIN tournament_registrations tr ON p.player_id = tr.player_id
WHERE tr.tournament_id = 1;

-- 4 : Find spillere med flest sejre i en bestemt turnering.

SELECT p.player_id, p.username, COUNT(m.winner_id) AS most_wins
FROM players p
JOIN matches m ON p.player_id = m.winner_id
WHERE m.tournament_id = 1
GROUP BY p.player_id, p.username
ORDER BY most_wins;

-- 5 : Hent alle kampe, hvor en bestemt spiller har deltaget.

SELECT * FROM matches
WHERE player1_id = 2
OR player2_id = 2;

-- 6 : Hent en spillers tilmeldte turneringer.

SELECT t.tournament_id, t.t_name, t.game, t.start_date
FROM tournaments t
JOIN tournament_registrations tr ON t.tournament_id = tr.tournament_id
WHERE tr.player_id = 2

-- 7 : Find de 5 bedst rangerede spillere.

SELECT player_id, username, ranking
FROM players
ORDER BY ranking DESC;

-- 8 : Beregn gennemsnitlig ranking for alle spillere.


-- 9 : Vis turneringer med mindst 5 deltagere.


-- 10 : Find det samlede antal spillere i systemet.


-- 11 : Find alle kampe, der mangler en vinder.


-- 12 : Vis de mest populære spil baseret på turneringsantal.


-- 13 : Find de 5 nyeste oprettede turneringer.


-- 14 : Find spillere, der har registreret sig i flere end 3 turneringer.


-- 15 : Hent alle kampe i en turnering sorteret efter dato.

