using moderator.Models;
using nng.Constants;
using nng.Helpers;

namespace moderator.Configuration;

public class ConfigurationManager
{
    private static ConfigurationManager? _instance;

    private ConfigurationManager()
    {
        Config = GetConfig();
    }

    public Config Config { get; }

    public static ConfigurationManager GetInstance()
    {
        return _instance ??= new ConfigurationManager();
    }

    private static Config GetConfig()
    {
        var dataUrl = EnvironmentHelper.GetString(EnvironmentConstants.DataUrl);
        var allowedGroups = EnvironmentHelper.GetString("AllowedGroups")
            .Split(",")
            .Select(long.Parse)
            .ToList();
        var groupId = EnvironmentHelper.GetLong(EnvironmentConstants.DialogGroupId);
        var userToken = EnvironmentHelper.GetString(EnvironmentConstants.UserToken);
        var dialogGroupSecret = EnvironmentHelper.GetString(EnvironmentConstants.DialogGroupSecret);
        var dialogGroupConfirm = EnvironmentHelper.GetString(EnvironmentConstants.DialogGroupConfirm);

        return new Config(dataUrl, allowedGroups,
            new GroupConfig(groupId, userToken, dialogGroupSecret, dialogGroupConfirm));
    }
}
