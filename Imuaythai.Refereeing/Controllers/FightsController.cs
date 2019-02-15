using System.Collections.Generic;
using Imuaythai.Refereeing.Models;
using Imuaythai.Refereeing.Services;
using Microsoft.AspNetCore.Mvc;

namespace Imuaythai.Refereeing.Controllers
{
    [Route("api/fights")]
    [ApiController]
    public class FightsController : ControllerBase
    {
        private readonly IFightStorage _fightStorage;

        public FightsController(IFightStorage fightStorage)
        {
            _fightStorage = fightStorage;
        }

        [HttpPost]
        public ActionResult AddFights([FromBody] IEnumerable<Fight> fights)
        {
            foreach(var fight in fights)
            {
                _fightStorage.SaveAsync(fight);
            }

            return Ok();
        }
    }
}
