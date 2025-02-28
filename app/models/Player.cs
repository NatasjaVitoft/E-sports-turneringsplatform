public class Player
{
    public int PlayerId;
    public String username;
    public String email;
    public int ranking;
    public DateTime created_at;

    public Player(int playerId, String username, String email, int ranking, DateTime created_at)
    {
        this.PlayerId = playerId;
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