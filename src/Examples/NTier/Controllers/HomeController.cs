using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Contextually;
using Microsoft.AspNetCore.Mvc;
using NTier.DAL;

namespace NTier.Controllers
{
    public class HomeController : Controller
    {
        private SomeDataRepository DataRepository { get; }

        public HomeController()
        {
            DataRepository = new SomeDataRepository();
        }

        public async Task<IActionResult> Index(CancellationToken token)
        {
            using(Relevant.Info(new NameValueCollection { ["Route"] = nameof(Index) }))
            {
                return await GetViewAsync(token);
            }
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            using(Contextually.Relevant.Info(new NameValueCollection { ["Route"] = nameof(About) }))
            {
                return GetViewAsync(CancellationToken.None).Result;
            }
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            var ourLocalContext = new NameValueCollection { ["Route"] = nameof(Contact) };

            using(Relevant.Info(ourLocalContext))
            {
                var data = DataRepository.GetSomeData();

                return GetViewAsync(CancellationToken.None).Result;
            }
        }

        public IActionResult Error()
        {
            return View();
        }

        private Task<ViewResult> GetViewAsync(CancellationToken token)
        {
            var currentInfo = Contextually.Relevant.Info();

            var routeName = currentInfo["Route"];

            Console.WriteLine($"User requested: {routeName}");

            return Task.FromResult(View());
        }
    }
}
