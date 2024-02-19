using VkNet;
using VkNet.Abstractions;
using VkNet.Model;
using ConfigurationManager = moderator.Configuration.ConfigurationManager;

namespace moderator;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddMvc(options => options.EnableEndpointRouting = false);
        services.AddMvc().AddNewtonsoftJson();
        services.AddSingleton<IVkApi>(_ =>
        {
            var config = ConfigurationManager.GetInstance().Config;
            var token = config.Group.Token;
            var vk = new VkApi();
            vk.Authorize(new ApiAuthParams {AccessToken = token});
            vk.RequestsPerSecond = 1;
            return vk;
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseMvc();

        app.UseRouting();
    }
}
