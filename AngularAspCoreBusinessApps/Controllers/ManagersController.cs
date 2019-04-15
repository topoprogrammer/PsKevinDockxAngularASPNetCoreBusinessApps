using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using AngularAspCoreBusinessApps.Dtos;
using AngularAspCoreBusinessApps.Services;

namespace AngularAspCoreBusinessApps.Controllers
{
    [Route("api/managers")]
    public class ManagersController : Controller
    {
        private readonly ITourManagementRepository _tourManagementRepository;

        public ManagersController(ITourManagementRepository tourManagementRepository)
        {
            _tourManagementRepository = tourManagementRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetManagers()
        {
            var managersFromRepo = await _tourManagementRepository.GetManagers();

            var managers = Mapper.Map<IEnumerable<Manager>>(managersFromRepo);

            return Ok(managers);
        }
    }
}
