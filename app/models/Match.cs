using System;

public class Match
{
    private int matchId;
    private int tournamentId;
    private int player1Id;
    private int player2Id;
    private int? winnerId; 
    private DateTime matchDate;

    public Match(int matchId, int tournamentId, int player1Id, int player2Id, int? winnerId, DateTime matchDate)
    {
        this.matchId = matchId;
        this.tournamentId = tournamentId;
        this.player1Id = player1Id;
        this.player2Id = player2Id;
        this.winnerId = winnerId;
        this.matchDate = matchDate;
    }

    public Match(int tournamentId, int player1Id, int player2Id, DateTime matchDate)
    {
        this.tournamentId = tournamentId;
        this.player1Id = player1Id;
        this.player2Id = player2Id;
        this.matchDate = matchDate;
    }

    public int GetMatchId()
    {
        return this.matchId;
    }

    public int GetTournamentId()
    {
        return this.tournamentId;
    }

    public int GetPlayer1Id()
    {
        return this.player1Id;
    }

    public int GetPlayer2Id()
    {
        return this.player2Id;
    }

    public int? GetWinnerId()
    {
        return this.winnerId;
    }

    public DateTime GetMatchDate()
    {
        return this.matchDate;
    }

    public void SetWinnerId(int winnerId)
    {
        this.winnerId = winnerId;
    }
}
