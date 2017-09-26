using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RoechlingEquipment.Controllers
{
    [ServerAuthorize]
    public class ProblemController : BaseController
    {
        //
        // GET: /Problem/

        public ActionResult ProblemIndex()
        {
            return View();
        }

    }
}
