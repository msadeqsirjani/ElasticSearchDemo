using ElasticSearch.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nest;
using System.Diagnostics;

namespace ElasticSearch.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly ElasticClient _client;

        public HomeController(ILogger<HomeController> logger, ElasticClient client)
        {
            _logger = logger;
            _client = client;
        }

        public IActionResult Index(string query)
        {
            ISearchResponse<Book> results;
            if (!string.IsNullOrWhiteSpace(query))
            {
                results = _client.Search<Book>(s => s
                    .Query(q => q
                        .Term(t => t
                            .Field(f => f.Isbn)
                            .Value(query)
                        )
                    )
                );
            }
            else
            {
                results = _client.Search<Book>(s => s
                    .Query(q => q
                        .MatchAll()
                    )
                );
            }
            return View(results);
        }

        public IActionResult Search(string query)
        {
            ISearchResponse<Book> results;
            if (!string.IsNullOrWhiteSpace(query))
            {
                results = _client.Search<Book>(s => s
                    .Query(q => q
                        .Match(t => t
                            .Field(f => f.Title)
                            .Query(query)
                        )
                    )
                );
            }
            else
            {
                results = _client.Search<Book>(s => s
                    .Query(q => q
                        .MatchAll()
                    )
                );
            }
            return View(results);
        }

        public IActionResult AdvanceSearch()
        {
            var results = _client.Search<Book>(s => s
                .Query(q => q
                    .MatchAll()
                )
                .Aggregations(a => a
                    .Range("pageCounts", r => r
                        .Field(f => f.PageCount)
                        .Ranges(r => r.From(0),
                            r => r.From(200).To(400),
                            r => r.From(400).To(600),
                            r => r.From(600)
                        )
                    )
                    .Terms("categories", t => t
                        .Field("categories.keyword")
                    )
                ));

            return View(results);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
