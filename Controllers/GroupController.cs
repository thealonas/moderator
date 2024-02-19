using Microsoft.AspNetCore.Mvc;
using moderator.Models;
using nng.Helpers;
using nng.VkFrameworks;
using VkNet.Abstractions;
using VkNet.Exception;
using VkNet.Model.GroupUpdate;
using VkNet.Model.RequestParams;
using VkNet.Utils;
using ConfigurationManager = moderator.Configuration.ConfigurationManager;

namespace moderator.Controllers;

[ApiController]
[Route("")]
public class GroupController : Controller
{
    private readonly List<long> _allowedGroups;
    private readonly Config _config;
    private readonly IVkApi _framework;

    private readonly ILogger<GroupController> _logger;

    public GroupController(ILogger<GroupController> logger, IVkApi framework)
    {
        _logger = logger;
        _framework = framework;
        _config = ConfigurationManager.GetInstance().Config;

        var data = DataHelper.GetDataAsync(_config.DataUrl).GetAwaiter().GetResult();

        _allowedGroups = new List<long>();
        foreach (var group in data.GroupList) _allowedGroups.Add(group);

        var configGroups = _config.AllowedGroups;

        foreach (var group in configGroups) _allowedGroups.Add(group);
    }

    [HttpPost]
    public IActionResult Callback(Event vkEvent)
    {
        if (_config.Group.Id != vkEvent.GroupId)
        {
            _logger.LogWarning("Невалидный Id сообщества");
            _logger.LogInformation("Получено: {Id}", vkEvent.GroupId);
            _logger.LogInformation("В конфигурации: {Id}", _config.Group.Id);
            return Ok("ok");
        }

        if (vkEvent.Secret != _config.Group.Secret)
        {
            _logger.LogInformation("Неправильный Secret");
            _logger.LogInformation("Получено: {Secret}", vkEvent.Secret);
            return Ok("ok");
        }

        if (vkEvent.Type == "confirmation") return Ok(_config.Group.Confirm);

        if (vkEvent.Type != "wall_reply_new") return Ok("ok");

        var comment = WallReply.FromJson(new VkResponse(vkEvent.Object));

        if (comment?.FromId is null || comment.PostId is null)
        {
            _logger.LogWarning("Пустой объект коммента: {@Object}", vkEvent.Object);
            return Ok("ok");
        }

        if (comment.FromId is >= 0 || _allowedGroups.Contains(-(long) comment.FromId)) return Ok("ok");

        try
        {
            VkFrameworkExecution.Execute(() => _framework.Wall.DeleteComment(-_config.Group.Id, comment.Id));

            var banParams = new GroupsBanUserParams
            {
                Comment = string.Empty, GroupId = _config.Group.Id,
                OwnerId = comment.FromId
            };
            VkFrameworkExecution.Execute(() => _framework.Groups.BanUser(banParams));

            _logger.LogInformation("Заблокировали и удалили комментарий сообщества {GroupId} в посте {PostId}",
                comment.FromId, comment.PostId);
        }
        catch (VkApiException e)
        {
            _logger.LogError("Ошибка при удалении комментария: {@Exception}",
                $"{e.GetType()}: {e.Message}\n{e.StackTrace}");
        }

        return Ok("ok");
    }
}
