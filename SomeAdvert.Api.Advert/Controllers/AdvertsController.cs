using System;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SomeAdvert.Api.Advert.Models;
using SomeAdvert.Api.Advert.Services;
using SomeAdvert.Contracts.Advert;

namespace SomeAdvert.Api.Advert.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    public class AdvertsController : ControllerBase
    {
        private readonly IAdvertStorage _storage;
        private readonly ILogger<AdvertsController> _logger;

        public AdvertsController(IAdvertStorage storage, ILogger<AdvertsController> logger)
        {
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CreateAdvertResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(AdvertModel advert)
        {
            try
            {
                var advertId = await _storage.Add(advert);
                return StatusCode(201, new CreateAdvertResponse {Id = advertId});
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unable to create advert `{advert.Title}`: {ex}");
                return StatusCode(500);
            }
        }
        
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Confirm(ConfirmationAdvertModel confirmation)
        {
            try
            {
                await _storage.Confirm(confirmation);
                return Ok();
            }
            catch (AdvertNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unable to confirm advert `{confirmation.AdvertId}`: {ex}");
                return StatusCode(500);
            }
        }
    }
}