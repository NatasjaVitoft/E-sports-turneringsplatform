CREATE OR REPLACE PROCEDURE update_player_ranking(playerID INT)
LANGUAGE plpgsql
AS $$
DECLARE
    current_ranking INT;
BEGIN
    SELECT ranking INTO current_ranking
    FROM players
    WHERE player_id = playerID
    FOR UPDATE; 

    UPDATE players
    SET ranking = current_ranking + 100
    WHERE player_id = playerID;
END;
$$;