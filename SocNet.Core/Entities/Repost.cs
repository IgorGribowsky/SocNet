namespace SocNet.Core.Entities;

public class Repost
{
    public int Id { get; private set; }

    public int RepostedUserId { get; set; }

    public int PostId { get; set; }
}
