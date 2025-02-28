
using System.ComponentModel.DataAnnotations;

class Player
{
    private int player_id;
    private String username;
    private String email;
    private int ranking;
    private DateTime created_at;


    public Player(int player_id, String username, String email, int ranking, DateTime created_at)
    {
        this.player_id = player_id;
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
}