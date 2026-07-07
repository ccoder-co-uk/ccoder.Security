using cCoder.Data.Models.Packaging;

namespace Security.Web.Exposures.Setup;

public static partial class UIBaseline
{
    public static Package[] Packages => [
        Components,
        Pages,
        Resources,
        Roles,
        PageRoles
    ];
}
