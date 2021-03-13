using System;
using System.Threading.Tasks;
using AutoMapper;
using CoreCodeCamp.Data;
using CoreCodeCamp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace CoreCodeCamp.Controllers
{
    [Route("api/camps/{moniker}/talks")]   //For creating Associated API's .
    [ApiController]
    public class TalksController: ControllerBase
    {
        private readonly ICampRepository _repository;
        private readonly IMapper _mapper;
        private readonly LinkGenerator _linkGenerator;

        public TalksController(ICampRepository repository, IMapper mapper, LinkGenerator linkGenerator)
        {
            _repository = repository;
            _mapper = mapper;
            _linkGenerator = linkGenerator;
        }
        [HttpGet]
        public async Task<ActionResult<TalkModel[]>> Get(string moniker)
        {
            try
            {
              var talks = await _repository.GetTalksByMonikerAsync(moniker);

                return _mapper.Map<TalkModel[]>(talks);
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database failure");

            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<TalkModel>> Get(string moniker, int id)
        {
            try
            {
                var talk = await _repository.GetTalkByMonikerAsync(moniker, id);
                if (talk == null) return NotFound("Could not find the talk");

                return _mapper.Map<TalkModel>(talk);
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database failure");

            }
        }

        [HttpPost]
        public async Task<ActionResult<TalkModel>> Post(string moniker, TalkModel model)
        {
            try
            {
                var camp = await _repository.GetCampAsync(moniker);
                if (camp == null) return BadRequest("Camp Doesnot exist");

                var talk = _mapper.Map<Talk>(model);
                talk.Camp = camp;


                if (model.Speaker == null) return BadRequest("Speaker ID is required");
                var speaker = await _repository.GetSpeakerAsync(model.Speaker.SpeakerId);
                if (speaker == null) return BadRequest("Speaker could not be found");
                talk.Speaker = speaker;

                _repository.Add(talk);

                if (await _repository.SaveChangesAsync())
                {
                    var url = _linkGenerator.GetPathByAction(HttpContext, "Get",
                        values: new { moniker, id = talk.TalkId });

                    return Created(url, _mapper.Map<TalkModel>(talk));
                }
                else
                {
                    return BadRequest("Failed to save a new Talk");
                }
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database failure");

            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<TalkModel>> Put(string moniker, int id, TalkModel model)
        {
            try
            {
                var talk = await _repository.GetTalkByMonikerAsync(moniker, id, true);
                if (talk == null) return NotFound("Could not find the talk");

                _mapper.Map(model, talk);

                if (model.Speaker != null)
                {
                    var speaker = await _repository.GetSpeakerAsync(model.Speaker.SpeakerId);
                    if(speaker!= null)
                    {
                        talk.Speaker = speaker;
                    }
                }

                if(await _repository.SaveChangesAsync())
                {
                    return _mapper.Map<TalkModel>(talk);
                }
                else
                {
                    return BadRequest("Database cant be updated");
                }
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database failure");

            }
        }
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(string moniker, int id)
        {
            try
            {
                var talk = await _repository.GetTalkByMonikerAsync(moniker, id);
                if (talk == null) return NotFound("Failed to fetch the talk to delete");
                _repository.Delete(talk);
                if (await _repository.SaveChangesAsync())
                {
                    return Ok();
                }
                else
                {
                    return BadRequest("Failed to delete a talk");
                }
            }
            catch (Exception )
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database failure");

            }
        }

    }
}
