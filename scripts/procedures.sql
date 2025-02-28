-- 1 : registerPlayer 

CREATE OR REPLACE PROCEDURE registerplayer(in_username VARCHAR(50), in_email VARCHAR(50))
LANGUAGE SQL
AS $$
INSERT INTO players (username, email)
VALUES (in_username, in_email);
$$;

-- 2 : joinTournament 

CREATE OR REPLACE PROCEDURE join_tournament(p_tournament_id INTEGER, p_player_id INTEGER)
LANGUAGE plpgsql
AS $$
BEGIN
	INSERT INTO tournament_registrations (tournament_id, player_id)
	VALUES (p_tournament_id, p_player_id);
    
	RAISE NOTICE 'Player % joined tournament %.', p_player_id, p_tournament_id;
END;
$$;

--3 : submitMatchResult 

CREATE OR REPLACE PROCEDURE submit_match_result(p_match_id INTEGER, p_winner_id INTEGER)
LANGUAGE plpgsql
AS $$
BEGIN

	IF NOT EXISTS (SELECT 1 FROM matches WHERE match_id = p_match_id) THEN
    	RAISE EXCEPTION 'Match with ID % does not exist.', p_match_id;  
	END IF;

	IF NOT EXISTS (
    	SELECT 1
    	FROM matches
    	WHERE match_id = p_match_id
    	AND (player1_id = p_winner_id OR player2_id = p_winner_id)
	) THEN
    	RAISE EXCEPTION 'Player with ID % is not a part of match with %', p_winner_id, p_match_id;
	END IF;

	UPDATE matches
	SET winner_id = p_winner_id
	WHERE match_id = p_match_id;

	RAISE NOTICE 'Match % result submitted.', p_match_id;
END;
$$;
