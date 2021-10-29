using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PersonRecordService.Interfaces;
using System;
using System.IO;
using System.Threading.Tasks;

namespace GRWebApi.Controllers
{
    [ApiController]
    [Route("[controller]/records/")]
    public class GRWebApiController : ControllerBase
    {
        private IRecordRepository _repo;
        public GRWebApiController(IRecordRepository repo)
        {
            _repo = repo;
        }

        [HttpPost]
        public async Task<IActionResult> SaveRecords()
        {
            using(var reader = new StreamReader(Request.Body))
            {
                string record;
                while ((record = await reader.ReadLineAsync()) != null)
                {
                    _repo.SaveRecord(record);
                }
            }
            return Ok();
        }

        [HttpGet("{orderBy}")]
        public IActionResult GetRecords(string orderBy)
        {
            try
            {
                string orderByField = orderBy.ToLower() switch
                {
                    "color" => "FavoriteColor",
                    "birthdate" => "DateOfBirth",
                    "name" => "LastName",
                    _ => throw new ArgumentException($"Invalid order by field: {orderBy}"),
                };
                var records = _repo.GetRecords(orderByField);
                return new JsonResult(records);
            }

            catch (ArgumentException ae)
            {
                return BadRequest(ae.Message);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
