ALTER TABLE tournament_registrations
ADD CONSTRAINT p_fkey
FOREIGN KEY (player_id)
REFERENCES players (player_id);

ALTER TABLE tournament_registrations
ADD CONSTRAINT unique_tournament_player
UNIQUE (tournament_id, player_id);

ALTER TABLE matches ADD CONSTRAINT tm_fkey FOREIGN KEY (tournament_id) REFERENCES tournaments (tournament_id);

ALTER TABLE matches ADD CONSTRAINT p1_fkey FOREIGN KEY (player1_id) REFERENCES players (player_id);

ALTER TABLE matches ADD CONSTRAINT p2_fkey FOREIGN KEY (player2_id) REFERENCES players (player_id);

ALTER TABLE matches ADD CONSTRAINT w_fkey FOREIGN KEY (winner_id) REFERENCES players (player_id);