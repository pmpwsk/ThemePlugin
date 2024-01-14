using uwap.WebFramework.Elements;

namespace uwap.WebFramework.Plugins;

public partial class ThemePlugin : Plugin
{
    public override Task Handle(AppRequest req, string path, string pathPrefix)
    {
        Presets.CreatePage(req, "Themes", out var page, out var e);
        Presets.Navigation(req, page);
        switch (path)
        {
            case "":
                {
                    if (!req.LoggedIn)
                    {
                        req.RedirectToLogin();
                        break;
                    }
                    page.Title = "Theme settings";
                    page.Scripts.Add(new Script(pathPrefix + "/settings.js"));
                    string theme = req.User.Settings.TryGet("Theme") ?? "dark";
                    if (!Themes.Contains(theme))
                        theme = "dark";
                    e.Add(new LargeContainerElement("Theme settings"));
                    e.Add(new ContainerElement("Colors", new Selector("colors", theme, Themes) { OnChange = "Save()" }));
                }
                break;
            default:
                req.Status = 404;
                break;
        }
        return Task.CompletedTask;
    }
}