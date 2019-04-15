using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using AngularAspCoreBusinessApps.Dtos;
using AngularAspCoreBusinessApps.Services;

namespace AngularAspCoreBusinessApps.Controllers
{
    [Route("api/bands")]
    public class BandsController : Controller
    {
        private readonly ITourManagementRepository _tourManagementRepository;

        public BandsController(ITourManagementRepository tourManagementRepository)
        {
            _tourManagementRepository = tourManagementRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetBands()
        {
            var bandsFromRepo = await _tourManagementRepository.GetBands();

            var bands = Mapper.Map<IEnumerable<Band>>(bandsFromRepo);

            return Ok(bands);
        }
    }
}
