
using System;
public class Player
{
    private int playerId;
    private String username;
    private String email;
    private int ranking;
    private DateTime created_at;


    public Player(int playerId, String username, String email, int ranking, DateTime created_at)
    {
        this.playerId = playerId;
        this.username = username;
        this.email = email;
        this.ranking = ranking;
        this.created_at = created_at;
    }
    public Player(String username, String email)
    {
        this.username = username;
        this.email = email;
    }

    public String GetUsername()
    {
        return this.username;
    }

    public String GetEmail()
    {
        return this.email;
    }

    public int GetPlayerId() 
    {
        return this.playerId;
    }
}