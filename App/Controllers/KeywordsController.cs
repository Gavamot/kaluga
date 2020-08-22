using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.KeywordsSearchService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Logging;

namespace App.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class KeywordsController : ControllerBase
    {
        readonly IKeywordsSearchService keywordsSearchService;

        public KeywordsController(IKeywordsSearchService keywordsSearchService)
        {
            this.keywordsSearchService = keywordsSearchService;
        }

        [HttpGet]
        [Route("Get")]
        public IActionResult Get(string keywords)
        {
            var _ = keywords.Split(',').Select(x=>x.Trim());
            var res = keywordsSearchService.GetItems(_);
            return Ok(res);
        }
    }
}
