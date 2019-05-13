using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AngularAspCoreBusinessApps.Dtos;
using AngularAspCoreBusinessApps.Services;
using AngularAspCoreBusinessApps.Helpers;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Authorization;

namespace AngularAspCoreBusinessApps.Controllers
{
    [Route("api/tours")]
    [Authorize]
    public class ToursController : Controller
    {
        private readonly ITourManagementRepository _tourManagementRepository;
        private readonly IUserInfoService _userInfoService;

        public ToursController(ITourManagementRepository tourManagementRepository, IUserInfoService userInfoService)
        {
            _tourManagementRepository = tourManagementRepository;
            _userInfoService = userInfoService;
        }

        [HttpGet]
        public async Task<IActionResult> GetTours()
        {
            IEnumerable<Entities.Tour> toursFromRepo = new List<Entities.Tour>();

            if (_userInfoService.Role == "Administrator")
            {
                toursFromRepo = await _tourManagementRepository.GetTours();
            }
            else
            {
                if (!Guid.TryParse(_userInfoService.UserId, out Guid userIdAsGuid))
                {
                    return Forbid();
                }

                toursFromRepo = await _tourManagementRepository.GetToursForManager(userIdAsGuid);
            }

            var tours = Mapper.Map<IEnumerable<Tour>>(toursFromRepo);
            return Ok(tours);
        }

        [HttpGet("{tourId}")]
        public async Task<IActionResult> GetDefaultTour(Guid tourId)
        {
            return await GetSpecificTour<Tour>(tourId);
        }

        [HttpGet("{tourId}", Name = "GetTour")]
        [Authorize(Policy = "UserMustBeTourManager")]
        [RequestHeaderMatchesMediaType("Accept",
            new[] { "application/vnd.marvin.tour+json" })]
        public async Task<IActionResult> GetTour(Guid tourId)
        {
            return await GetSpecificTour<Tour>(tourId);
        }

        [HttpGet("{tourId}")]
        [Authorize(Policy = "UserMustBeTourManager")]
        [Authorize(Policy = "UserMustBeAdministrator")]
        [RequestHeaderMatchesMediaType("Accept",
            new[] { "application/vnd.marvin.tourwithestimatedprofits+json" })]
        public async Task<IActionResult> GetTourWithEstimatedProfits(Guid tourId)
        {
            return await GetSpecificTour<TourWithEstimatedProfits>(tourId);
        }

        [HttpGet("{tourId}")]
        [Authorize(Policy = "UserMustBeTourManager")]
        [RequestHeaderMatchesMediaType("Accept",
        new[] { "application/vnd.marvin.tourwithshows+json" })]
        public async Task<IActionResult> GetTourWithShows(Guid tourId)
        {
            return await GetSpecificTour<TourWithShows>(tourId, true);
        }

        [HttpGet("{tourId}")]
        [Authorize(Policy = "UserMustBeTourManager")]
        [Authorize(Policy = "UserMustBeAdministrator")]
        [RequestHeaderMatchesMediaType("Accept",
           new[] { "application/vnd.marvin.tourwithestimatedprofitsandshows+json" })]
        public async Task<IActionResult> GetTourWithEstimatedProfitsAndShows(Guid tourId)
        {
            return await GetSpecificTour<TourWithEstimatedProfitsAndShows>(tourId, true);
        }

        private async Task<IActionResult> GetSpecificTour<T>(Guid tourId,
                bool includeShows = false) where T : class
        {
            var tourFromRepo = await _tourManagementRepository.GetTour(tourId, includeShows);

            if (tourFromRepo == null)
            {
                return BadRequest();
            }

            return Ok(Mapper.Map<T>(tourFromRepo));
        }


        [HttpPost]
        [RequestHeaderMatchesMediaType("Content-Type",
            new[] { "application/json",
                    "application/vnd.marvin.tourforcreation+json" })]
        public async Task<IActionResult> AddTour([FromBody] TourForCreation tour)
        {
            return await AddSpecificTour(tour);
        }

        [HttpPost]
        [Authorize(Policy = "UserMustBeAdministrator")]
        [RequestHeaderMatchesMediaType("Content-Type",
            new[] { "application/vnd.marvin.tourwithmanagerforcreation+json" })]
        public async Task<IActionResult> AddTourWithManager(
            [FromBody] TourWithManagerForCreation tour)
        {
            return await AddSpecificTour(tour);
        }

        [HttpPost]
        [RequestHeaderMatchesMediaType("Content-Type",
           new[] { "application/vnd.marvin.tourwithshowsforcreation+json" })]
        public async Task<IActionResult> AddTourWithShows(
           [FromBody] TourWithShowsForCreation tour)
        {
            return await AddSpecificTour(tour);
        }

        [HttpPost]
        [RequestHeaderMatchesMediaType("Content-Type",
            new[] { "application/vnd.marvin.tourwithmanagerandshowsforcreation+json" })]
        [Authorize(Policy = "UserMustBeAdministrator")]
        public async Task<IActionResult> AddTourWithManagerAndShows(
            [FromBody] TourWithManagerAndShowsForCreation tour)
        {
            return await AddSpecificTour(tour);
        }

        public async Task<IActionResult> AddSpecificTour<T>(T tour) where T : class
        {
            if (tour == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return new Helpers.UnprocessableEntityObjectResult(ModelState);
            }

            var tourEntity = Mapper.Map<Entities.Tour>(tour);

            if (tourEntity.ManagerId == Guid.Empty)
            {
                if (!Guid.TryParse(_userInfoService.UserId, out Guid userIdAsGuid))
                {
                    return Forbid();
                }

                tourEntity.ManagerId = userIdAsGuid;
            }

            await _tourManagementRepository.AddTour(tourEntity);

            if (!await _tourManagementRepository.SaveAsync())
            {
                throw new Exception("Adding a tour failed on save.");
            }

            var tourToReturn = Mapper.Map<Tour>(tourEntity);

            return CreatedAtRoute("GetTour",
                new { tourId = tourToReturn.TourId },
                tourToReturn);
        }


        [HttpPatch("{tourId}")]
        public async Task<IActionResult> PartiallyUpdateTour(Guid tourId,
            [FromBody] JsonPatchDocument<TourForUpdate> jsonPatchDocument)
        {
            if (jsonPatchDocument == null)
            {
                return BadRequest();
            }

            var tourFromRepo = await _tourManagementRepository.GetTour(tourId);

            if (tourFromRepo == null)
            {
                return BadRequest();
            }

            var tourToPatch = Mapper.Map<TourForUpdate>(tourFromRepo);

            //To validate is patch request is well form
            jsonPatchDocument.ApplyTo(tourToPatch, ModelState);
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            //Validate DTO
            if (!TryValidateModel(tourToPatch))
            {
                return new Helpers.UnprocessableEntityObjectResult(ModelState);
            }

            Mapper.Map(tourToPatch, tourFromRepo);

            await _tourManagementRepository.UpdateTour(tourFromRepo);

            if (!await _tourManagementRepository.SaveAsync())
            {
                throw new Exception("Updating a tour failed on save.");
            }

            return NoContent();
        }
    }
}
