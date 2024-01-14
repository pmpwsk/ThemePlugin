using System.Text;

namespace uwap.WebFramework.Plugins;

public partial class ThemePlugin : Plugin
{
    public override async Task Handle(ApiRequest req, string path, string pathPrefix)
    {
        switch(path)
        {
            case "/build.css":
                {
                    if (!(req.Query.TryGetValue("colors", out var colors)) || !Themes.Contains(colors))
                    {
                        req.Status = 400;
                        return;
                    }

                    //save domain + timestamp
                    string domain = req.Domain;
                    string timestamp = new[] { "/base.css", $"/{colors}.css" }.Max(x => long.Parse(GetFileVersion(x) ?? "0")).ToString();

                    //content type
                    if (Server.Config.MimeTypes.TryGetValue(".css", out string? type)) req.Context.Response.ContentType = type;

                    //browser cache
                    if (Server.Config.BrowserCacheMaxAge.TryGetValue(".css", out int maxAge))
                    {
                        if (maxAge == 0) req.Context.Response.Headers.CacheControl = "no-cache, private";
                        else
                        {
                            if (!req.Context.Response.Headers.ContainsKey("Cache-Control"))
                                req.Context.Response.Headers.CacheControl = "public, max-age=" + maxAge;
                            else req.Context.Response.Headers.CacheControl = "public, max-age=" + maxAge;
                            try
                            {
                                if (req.Context.Request.Headers.TryGetValue("If-None-Match", out var oldTag) && oldTag == timestamp)
                                {
                                    req.Context.Response.StatusCode = 304;
                                    if (Server.Config.FileCorsDomain != null)
                                        req.Context.Response.Headers.AccessControlAllowOrigin = Server.Config.FileCorsDomain;
                                    break; //browser already has the current version
                                }
                                else req.Context.Response.Headers.ETag = timestamp;
                            }
                            catch { }
                        }
                    }

                    //send
                    await req.SendBytes(
                    [
                        ..Encoding.UTF8.GetBytes($"/* {colors}.css */\n\n"),
                        ..GetFile($"/{colors}.css", pathPrefix, domain) ?? [],
                        ..Encoding.UTF8.GetBytes($"\n\n\n/* base.css */\n\n"),
                        ..GetFile("/base.css", pathPrefix, domain) ?? []
                    ]);
                }
                break;
            case "/set":
                {
                    if (!req.LoggedIn)
                    {
                        req.Status = 403;
                        return;
                    }
                    if (!(req.Query.TryGetValue("colors", out var colors)) || !Themes.Contains(colors))
                    {
                        req.Status = 400;
                        return;
                    }

                    req.User.Settings["Theme"] = colors;
                }
                break;
            default:
                req.Status = 404;
                break;
        }
    }
}