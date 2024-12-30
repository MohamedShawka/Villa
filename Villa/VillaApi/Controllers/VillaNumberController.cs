using Asp.Versioning;
using AutoMapper;
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
   /* [ApiVersion("1.0")]
    [ApiVersion("2.0")]*/
    public class VillaNumberController : ControllerBase
    {
        //  private readonly ILogging _logger;
        protected APIResponse _response;
        private readonly IVillaNumberRepo _dbVilla;
        private readonly IVillaRepo _villaRepo;
        private readonly IMapper _mapper;
        public VillaNumberController(IVillaNumberRepo dbVilla, IMapper mapper,IVillaRepo villaRepo)
        {
            _dbVilla = dbVilla;
            _mapper = mapper;
            _villaRepo = villaRepo;
            //    _logger = logger;
            this._response = new ();
        }
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
      //  [MapToApiVersion("1.0")]
        public async Task<ActionResult<APIResponse>> GetVillaNumbers()
          {
            // _logger.LogError("Getting All Villas","");
            try
            {
                IEnumerable<VillaNumber> VillaList = await _dbVilla.GetAll(includeProperties:"Villa");
                _response.Result = _mapper.Map<List<VillaNumberDto>>(VillaList);
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex) { 
             _response.IsSuccess = false;
                _response.Errors = new List<string> { ex.Message };
            }
            return _response;

        }

    /*     [MapToApiVersion("2.0")]
         [HttpGet]
         public IEnumerable<string> Get()
         {
         return new string[] { "value1", "value2" };
         }
*/



          [HttpGet("id",Name ="GetVillaNumber")]
          [ProducesResponseType(StatusCodes.Status200OK)]
          [ProducesResponseType(StatusCodes.Status400BadRequest)]
          [ProducesResponseType(StatusCodes.Status404NotFound)]

          public  async Task<ActionResult<APIResponse>> GetVillaNumber(int id)
          {
            try
            {
                if (id == 0)
                {
                    //  _logger.LogError("Bad Request With Id "+ id,"error");
                    return BadRequest();
                }
                var villa = await _dbVilla.Get(i => i.VillaNo == id);
                if (villa == null)
                {
                    return NotFound();
                }
                _response.Result = _mapper.Map<VillaNumberDto>(villa);
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
          public async Task<ActionResult<APIResponse>> CreateVillaNumber([FromBody] VillaNumberDto villaDto) {

            try
            {
                if (await _dbVilla.Get(u => u.VillaNo == villaDto.VillaNo) != null)
                {
                    ModelState.AddModelError("CustomError", "Villa Number already exist");
                    return BadRequest(ModelState);
                }
                if(await _villaRepo.Get(u => u.Id == villaDto.VillaId) == null)
                {
                    ModelState.AddModelError("CustomError", "Villa Id  not valid");
                    return BadRequest(ModelState);
                }
                if (villaDto == null)
                {
                    return BadRequest(villaDto);

                }
             
                VillaNumber villa = _mapper.Map<VillaNumber>(villaDto);
              
                await _dbVilla.Create(villa);
                _response.Result = _mapper.Map<VillaNumberDto>(villa);
                _response.StatusCode = HttpStatusCode.Created;


                return CreatedAtRoute("GetVilla", new { id = villa.VillaNo }, _response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Errors = new List<string> { ex.Message };
            }
            return _response;
        }
          [ProducesResponseType(StatusCodes.Status200OK)]
          [ProducesResponseType(StatusCodes.Status400BadRequest)]
          [HttpDelete("{id}",Name ="DeleteVillaNumber")]
          public async Task<ActionResult<APIResponse>> DeleteVillaNumber(int id) 
          {
            try
            {
                if (id == 0)
                {
                    return BadRequest();
                }
                var villa = await _dbVilla.Get(i => i.VillaNo == id);
                if (villa == null)
                {
                    return NotFound();
                }
                _dbVilla.Remove(villa);
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
        [HttpPut("{id:int}", Name = "UpdateVillaNumber")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<ActionResult<APIResponse>> UpdateVillaNumber(int id, [FromBody] VillaNumberDto villaDto)
        {
            try {
                if (villaDto == null || id != villaDto.VillaNo)
                {
                    return BadRequest();
                }
                //   var Villa = _context.Villas.FirstOrDefault(i => i.Id == id);
                VillaNumber model = _mapper.Map<VillaNumber>(villaDto);
                if (await _villaRepo.Get(u => u.Id == villaDto.VillaId) == null)
                {
                    ModelState.AddModelError("CustomError", "Villa Id  not valid");
                    return BadRequest(ModelState);
                }
           
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
   /*     [HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdatePartialVilla(int id, JsonPatchDocument<VillaNumberDto> patchDTO)
        {
            if (patchDTO == null || id == 0)
            {
                return BadRequest();
            }
            var villa = await _dbVilla.Get(u => u.VillaNo == id, tracked: false);

            VillaNumberDto villaDTO = _mapper.Map<VillaNumberDto>(villa);


            if (villa == null)
            {
                return BadRequest();
            }
            patchDTO.ApplyTo(villaDTO, ModelState);
            VillaNumber model = _mapper.Map<VillaNumber>(villaDTO);

            await _dbVilla.Update(model);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return NoContent();
        }*/
    } 
}
