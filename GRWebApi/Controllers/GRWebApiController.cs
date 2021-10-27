using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RecordProcesssor.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRWebApi.Controllers
{
    [ApiController]
    [Route("[controller]/records/")]

    public class GRWebApiController : ControllerBase
    {
        public GRWebApiController()
        {
        }

        [HttpGet()]
        public IActionResult Authors()
        {
            return new JsonResult("authors");
        }

        [HttpPost]
        public IActionResult SaveRecords(PersonRecord record)
        {
            return Ok();
        }


        [HttpGet("{orderBy}")]
        public IActionResult GetRecords(string orderBy)
        {
            return new JsonResult(orderBy);
        }
    }
}
