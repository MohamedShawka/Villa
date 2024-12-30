using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using VillaApi.Data;
using VillaApi.Dtos;
using VillaApi.Logging;
using VillaApi.Models;
using VillaApi.Repository.IRepository;

namespace VillaApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VillaController : ControllerBase
    {
        //  private readonly ILogging _logger;
        protected APIResponse _response;
        private readonly IVillaRepo _dbVilla; 
        private readonly IMapper _mapper;
        public VillaController(IVillaRepo dbVilla, IMapper mapper)
        {
            _dbVilla = dbVilla;
            _mapper = mapper;
            //    _logger = logger;
            this._response = new ();
        }
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]

          public async Task<ActionResult<APIResponse>> Get()
          {
            // _logger.LogError("Getting All Villas","");
            try
            {
                IEnumerable<Villa> VillaList = await _dbVilla.GetAll();
                _response.Result = _mapper.Map<List<VillaDto>>(VillaList);
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex) { 
             _response.IsSuccess = false;
                _response.Errors = new List<string> { ex.Message };
            }
            return _response;

        }
          [HttpGet("id",Name ="GetVilla")]
          [ProducesResponseType(StatusCodes.Status200OK)]
          [ProducesResponseType(StatusCodes.Status400BadRequest)]
          [ProducesResponseType(StatusCodes.Status404NotFound)]
          [ProducesResponseType(StatusCodes.Status403Forbidden)]
          [ProducesResponseType(StatusCodes.Status401Unauthorized)]
   

          public  async Task<ActionResult<APIResponse>> GetVilla(int id)
          {
            try
            {
                if (id == 0)
                {
                    //  _logger.LogError("Bad Request With Id "+ id,"error");
                    return BadRequest();
                }
                var villa = await _dbVilla.Get(i => i.Id == id);
                if (villa == null)
                {
                    return NotFound();
                }
                _response.Result = _mapper.Map<VillaDto>(villa);
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Errors = new List<string> { ex.Message };
            }
            return _response;

        }
          [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> Create([FromBody] VillaDto villaDto) {

            try
            {
                if (await _dbVilla.Get(u => u.Name.ToLower() == villaDto.Name.ToLower()) != null)
                {
                    ModelState.AddModelError("CustomError", "Villa Name already exist");
                    return BadRequest(ModelState);
                }
                if (villaDto == null)
                {
                    return BadRequest(villaDto);

                }
                /*  if (villa.Id > 0) {
                      return StatusCode(StatusCodes.Status500InternalServerError);
                  }*/
                Villa villa = _mapper.Map<Villa>(villaDto);
                /*  Villa model = new()
                  {
                      Amenity = villa.Amenity,
                      Details = villa.Details,
                      ImageUrl = villa.ImageUrl,
                      Name = villa.Name,
                      Occupancy = villa.Occupancy,
                      Rate = villa.Rate,
                      Sqft = villa.Sqft

                  };*/
                await _dbVilla.Create(villa);
                _response.Result = _mapper.Map<VillaDto>(villa);
                _response.StatusCode = HttpStatusCode.Created;


                return CreatedAtRoute("GetVilla", new { id = villa.Id }, _response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Errors = new List<string> { ex.Message };
            }
            return Ok(_response);
        }
          [ProducesResponseType(StatusCodes.Status200OK)]
          [ProducesResponseType(StatusCodes.Status204NoContent)]
          [ProducesResponseType(StatusCodes.Status400BadRequest)]
          [ProducesResponseType(StatusCodes.Status403Forbidden)]
          [ProducesResponseType(StatusCodes.Status401Unauthorized)]
          [HttpDelete("{id}",Name ="DeleteVilla")]
         
          public async Task<ActionResult<APIResponse>> DeleteVilla(int id) 
          {
            try
            {
                if (id == 0)
                {
                    return BadRequest();
                }
                var villa = await _dbVilla.Get(i => i.Id == id);
                if (villa == null)
                {
                    return NotFound();
                }
                await _dbVilla.Remove(villa);
                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Errors = new List<string> { ex.Message };
            }
            return _response;
        }
        [HttpPut("{id:int}", Name = "UpdateVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<ActionResult<APIResponse>> Update(int id, [FromBody] VillaUpdateDto villaDto)
        {
            try {
                if (villaDto == null || id != villaDto.Id)
                {
                    return BadRequest();
                }
                //   var Villa = _context.Villas.FirstOrDefault(i => i.Id == id);
                Villa model = _mapper.Map<Villa>(villaDto);

                /*    Villa model = new()
                  {
                      Id= villaDto.Id,
                      Amenity = villaDto.Amenity,
                      Details = villaDto.Details,
                      ImageUrl = villaDto.ImageUrl,
                      Name = villaDto.Name,
                      Occupancy = villaDto.Occupancy,
                      Rate = villaDto.Rate,
                      Sqft = villaDto.Sqft

                  };*/
                _dbVilla.Update(model);
                _response.Result = HttpStatusCode.NoContent;
                _response.IsSuccess = true;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Errors = new List<string> { ex.Message };
            }
            return _response;
        }
        [HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDto> patchDTO)
        {
            if (patchDTO == null || id == 0)
            {
                return BadRequest();
            }
            var villa = await _dbVilla.Get(u => u.Id == id, tracked: false);

            VillaUpdateDto villaDTO = _mapper.Map<VillaUpdateDto>(villa);


            if (villa == null)
            {
                return BadRequest();
            }
            patchDTO.ApplyTo(villaDTO, ModelState);
            Villa model = _mapper.Map<Villa>(villaDTO);

            await _dbVilla.Update(model);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return NoContent();
        }
    } 
}
