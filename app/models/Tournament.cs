public class Tournament

{
    public int TournamentId;
    public string TournamentName;
    public string Game;
    public int MaxPlayers;
    public DateTime StartDate;
    public DateTime CreatedAt;

    public Tournament(int tournamentId, string tournamentName, string game, int maxPlayers, DateTime startDate)
    {   
        this.TournamentId = tournamentId;
        this.TournamentName = tournamentName;
        this.Game = game;
        this.MaxPlayers = maxPlayers;
        this.StartDate = startDate;
        this.CreatedAt = DateTime.Now; 
    }

    public int GetTournamentId()
    {
        return this.TournamentId;
    }

    public string GetTournamentName()
    {
        return this.TournamentName;
    }

    public string GetGame()
    {
        return this.Game;
    }

    public int GetMaxPlayers()
    {
        return this.MaxPlayers;
    }

    public DateTime GetStartDate()
    {
        return this.StartDate;
    }

    public DateTime GetCreatedAt()
    {
        return this.CreatedAt;
    }

}
