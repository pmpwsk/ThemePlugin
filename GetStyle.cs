using uwap.WebFramework.Elements;

namespace uwap.WebFramework.Plugins;

public partial class ThemePlugin : Plugin
{
    private string[] Themes = ["code", "dark", "light", "white"];

    public Style GetStyle(IRequest request, out string fontUrl, string pathPrefix)
        => GetStyle((request.LoggedIn && request.User.Settings.TryGetValue("Theme", out string? colors) && Themes.Contains(colors)) ? colors : "dark", out fontUrl, pathPrefix);

    private Style GetStyle(string colors, out string fontUrl, string pathPrefix)
    {
        if (Themes.Contains(colors))
        {
            fontUrl = $"{pathPrefix}/fonts/ubuntu{(colors == "code" ? "-mono" : "")}.woff2";
            return new Style($"/api{pathPrefix}/build.css?colors={colors}&t={new[] { "/base.css", $"/{colors}.css" }.Max(x => long.Parse(GetFileVersion(x) ?? "0"))}");
        }
        else throw new ArgumentOutOfRangeException(nameof(colors));
    }
}