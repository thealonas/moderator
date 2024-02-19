namespace moderator.Models;

public class Config
{
    public Config(string dataUrl, List<long> allowedGroups, GroupConfig group)
    {
        DataUrl = dataUrl;
        AllowedGroups = allowedGroups;
        Group = group;
    }

    public List<long> AllowedGroups { get; }
    public string DataUrl { get; }
    public GroupConfig Group { get; }
}
