using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CoreCodeCamp.Data;
using CoreCodeCamp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace CoreCodeCamp.Controllers
{
    [Route("api/[controller]")]
    [ApiController] //or [FromBody]
    public class CampsController : ControllerBase
    {
        private readonly ICampRepository _repository;
        private readonly IMapper _mapper;
        private readonly LinkGenerator _linkGenerator;
        public CampsController(ICampRepository repository, IMapper mapper, LinkGenerator linkGenerator)//linkgen asp.net core 2 above
        {
            _repository = repository;
            _mapper = mapper;
            _linkGenerator = linkGenerator;
        }

        [HttpGet]
        public async Task<ActionResult<CampModel[]>> Get(bool includeTalks = false) //object type returns the json. but we want to return status codes: so we use IActionResult
        {
            //if (false) return BadRequest($"Errr Something Bad happened!!");
            //new { Moniker = "ALT102", Name = "Atlanta" }
            try
            {
                //var camModel = new CampModel();
                var results = await _repository.GetAllCampsAsync(includeTalks);

                return _mapper.Map<CampModel[]>(results);
                /*foreach(Camp res in results)
                {
                    camModel.Name = res.Name;
                    camModel.Moniker = res.Moniker;
                    camModel.Length = res.Length;                          //this kind of mapping is correct. Instead of this can use AUTO MAPPER from nuget. install and reggister in services.
                    camModel.EventDate = res.EventDate;
                } */
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database failure");
            }
        }
        [HttpGet("{moniker}")]
        public async Task<ActionResult<CampModel>> Get(string moniker)
        {
            try
            {
                CampModel result = _mapper.Map<Camp, CampModel>(await _repository.GetCampAsync(moniker));
                return result;
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, $"Db fetch error");
            }
        }
        [HttpGet("search")]
        public async Task<ActionResult<CampModel[]>> SearchByDate(DateTime theDate, bool includetalks)
        {
            try
            {
                var results = await _repository.GetAllCampsByEventDate(theDate, includetalks);
                if (!results.Any()) return NotFound();
                return _mapper.Map<CampModel[]>(results);
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, $"Db fetch error");

            }
        }
        [HttpPost]
        public async Task<ActionResult<CampModel[]>> Post(CampModel campModel)// [ApiController] or [FromBody]
        {
            try
            {
                var existing = await _repository.GetCampAsync(campModel.Moniker);
                //if moniker already exists 
                if (existing != null)
                {
                    return BadRequest("Moniker is in use");
                }

                //to generate for the specific moniker
                var location = _linkGenerator.GetPathByAction("Get",
                    "Camps",
                    new { moniker = campModel.Moniker });
                if (string.IsNullOrWhiteSpace(location))
                {
                    return BadRequest($"Could not use current moniker");
                }

                //Create a new Camp
                var camp = _mapper.Map<Camp>(campModel);    //for this add .ReverseMap in profile class. Woah!!
                _repository.Add(camp);

                if (await _repository.SaveChangesAsync())
                {
                    //we want to return a Created 201 status code for creating a camp
                    return Created($"/api/camps/{camp.Moniker}", _mapper.Map<CampModel>(camp));
                }
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, $"Db fetch error");

            }
            return BadRequest();
        }
        [HttpPut("{moniker}")]
        public async Task<ActionResult<CampModel>> Put(string moniker, CampModel campModel)
        {
            try
            {
                var oldCamp = await _repository.GetCampAsync(moniker);
                if (oldCamp == null)
                {
                    BadRequest($"Moniker {moniker} does not exist");
                }
                //oldCamp.Name = campModel.Name; //mapping can be done this way , but used Automapper here.
                _mapper.Map(campModel, oldCamp); //S, D
                if (await _repository.SaveChangesAsync())
                {
                    return _mapper.Map<CampModel>(oldCamp);
                }
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, $"Db fetch error");

            }
            return BadRequest();
        }
        [HttpDelete("{moniker}")]
        public async Task<IActionResult> Delete(string moniker)
        {
            try
            {
                var campToBeDeleted = await _repository.GetCampAsync(moniker);
                if (campToBeDeleted == null) NotFound($"Moniker {moniker} does not exists");

                _repository.Delete<Camp>(campToBeDeleted);

                if (await _repository.SaveChangesAsync())
                {
                    return Ok();
                }
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, $"Db fetch error");

            }
            return BadRequest("Failed to delete the camp");
        }
    }}