namespace moderator.Models;

public class GroupConfig
{
    public string Confirm;
    public long Id;
    public string Secret;
    public string Token;

    public GroupConfig(long id, string token, string secret, string confirm)
    {
        Id = id;
        Token = token;
        Secret = secret;
        Confirm = confirm;
    }
}
