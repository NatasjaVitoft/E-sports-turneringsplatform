-- 1 : beforeInsertRegistration 

CREATE OR REPLACE FUNCTION before_insert_registration()
RETURNS TRIGGER AS $$
BEGIN
	IF (SELECT COUNT(*)
    	FROM tournament_registrations
    	WHERE tournament_id = NEW.tournament_id) >=
   	(SELECT max_players
    	FROM tournaments
    	WHERE tournament_id = NEW.tournament_id) THEN
    	RAISE EXCEPTION 'Tournament with ID % has the maximum number of players allowed. You cannot register player with ID %.',
        	NEW.tournament_id, NEW.player_id;
	END IF;

	RETURN NEW;
END;
$$ LANGUAGE plpgsql;


CREATE TRIGGER before_insert_registration
BEFORE INSERT ON tournament_registrations
FOR EACH ROW
EXECUTE FUNCTION before_insert_registration();
	

-- 2 : afterInsertMatch 

CREATE OR REPLACE FUNCTION after_insert_match()
RETURNS TRIGGER AS $$
BEGIN

	IF NEW.winner_id = NEW.player1_id THEN
    	UPDATE players
    	SET ranking = ranking + 20   
    	WHERE player_id = NEW.player1_id;
	ELSE
    	UPDATE players
    	SET ranking = ranking - 20
    	WHERE player_id = NEW.player1_id;
	END IF;

	IF NEW.winner_id = NEW.player2_id THEN
    	UPDATE players
    	SET ranking = ranking + 20
    	WHERE player_id = NEW.player2_id;
	ELSE
    	UPDATE players
    	SET ranking = ranking - 20
    	WHERE player_id = NEW.player2_id;
	END IF;

	RETURN NEW;
END;
$$ LANGUAGE plpgsql;


CREATE TRIGGER after_insert_match
AFTER INSERT ON matches
FOR EACH ROW
EXECUTE FUNCTION after_insert_match();
