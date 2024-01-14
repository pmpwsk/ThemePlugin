using System.Text;

namespace uwap.WebFramework.Plugins;

public partial class ThemePlugin : Plugin
{
    public override async Task Handle(ApiRequest req, string path, string pathPrefix)
    {
        switch(path)
        {
            default:
                req.Status = 404;
                break;
        }
    }
}