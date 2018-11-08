using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using FIUChat.Enums;
using FIUChat.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Primitives;

namespace FIU_Chat.Controllers
{
    public class HomeController : Controller
    {
        private AuthenticateUser authenticateUser = new AuthenticateUser();

        private ICompositeViewEngine _viewEngine;

        public HomeController(ICompositeViewEngine viewEngine)
        {
            _viewEngine = viewEngine;
        }

        public async Task<IActionResult> Index()
        {
            var request = Request;
            var headers = request.Headers;

            StringValues token;
            if (headers.TryGetValue("Authorization", out token))
            {
                var result = this.authenticateUser.ValidateToken(token);
                if (result.Result == AuthenticateResult.Success)
                {
                    var successfulToken = Json(new
                    {
                        page = await this.RenderPartialViewToString("Index"),
                        classes = await this.authenticateUser.GetUserDictionaryFromToken(token),
                        success = true
                    });
                    return Ok(successfulToken);
                }
                else
                {
                    // Unsuccessful login
                    var badToken = Json(new
                    {
                        success = false
                    });
                    return Ok(badToken);
                }
            }

            // Unsuccessful login
            var unsuccessfulToken = Json(new
            {
                success = false
            });
            return Ok(unsuccessfulToken);
        }

        private async Task<string> RenderPartialViewToString(string viewName)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = ControllerContext.ActionDescriptor.ActionName;

            using (var writer = new StringWriter())
            {
                ViewEngineResult viewResult =
                    _viewEngine.FindView(ControllerContext, viewName, true);

                ViewContext viewContext = new ViewContext(
                    ControllerContext,
                    viewResult.View,
                    ViewData,
                    TempData,
                    writer,
                    new HtmlHelperOptions()
                );

                await viewResult.View.RenderAsync(viewContext);

                return writer.GetStringBuilder().ToString();
            }
        }
    }
}