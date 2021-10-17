using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Novinichka.Common;
using Novinichka.Web.Controllers;

namespace Novinichka.Web.Areas.Administration.Controllers
{
    [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
    [Area("Administration")]
    public class AdministrationController : BaseController
    {
    }
}
