using campus_insider.Data;
using campus_insider.DTOs;
using campus_insider.Models;
using campus_insider.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;

namespace campus_insider.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/equipment")]
    public class EquipmentController : ControllerBase
    {
        private readonly EquipmentService _equipmentService;
        public EquipmentController(EquipmentService equipmentService)
        {
            _equipmentService = equipmentService;
        }

        [HttpGet]
        public async Task<ActionResult<List<EquipmentResponseDto>>> GetEquipment()
        {
            var equipment = await _equipmentService.GetAllEquipment();

            var result = equipment.Select(e=> new EquipmentResponseDto
            {
                Id = e.Id,
                Name = e.Name,
                Description = e.Description,
                Category = e.Category,
                OwnerId = e.OwnerId,
                CreatedAt = e.CreatedAt
            }).ToList();

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EquipmentResponseDto>> GetById(long id)
        {
            var equipment = await _equipmentService.GetByIdAsync(id);

            if (equipment == null)
                return NotFound();

            return Ok(new EquipmentResponseDto
            {
                Id = equipment.Id,
                Name = equipment.Name,
                Category = equipment.Category,
                Description = equipment.Description,
                OwnerId = equipment.OwnerId,
                CreatedAt = equipment.CreatedAt
            });
        }


        [HttpPost]
        public async Task<ActionResult<EquipmentResponseDto>> Create(
       [FromBody] EquipmentCreateDto dto)
        {
            //  normally OwnerId comes from auth (JWT)
            var equipment = new Equipment
            {
                Name = dto.Name,
                Category = dto.Category,
                Description = dto.Description,
                OwnerId = 1 // TEMP: replace with User.Identity
            };

            var created = await _equipmentService.ShareEquipment(equipment);

            return CreatedAtAction(nameof(GetById), new { id = created.Id },
                new EquipmentResponseDto
                {
                    Id = created.Id,
                    Name = created.Name,
                    Category = created.Category,
                    Description = created.Description,
                    OwnerId = created.OwnerId,
                    CreatedAt = created.CreatedAt
                });
        }

        [HttpDelete]
        public async Task<ActionResult> Delete([FromBody] EquipmentDto equipmentDto)
        {
            try
            {
                var equipment = new Equipment
                {
                    Id = equipmentDto.Id,
                    Name = equipmentDto.Name,
                    Category = equipmentDto.Category,
                    Description = equipmentDto.Description,
                    OwnerId = equipmentDto.OwnerId
                };
                
                await _equipmentService.UnshareEquipment(equipment);

                return Ok(true);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public async Task<ActionResult<EquipmentDto>> Update([FromBody] EquipmentDto equipmentDto)
        {

            try
            {
                var equipment = new Equipment
                {
                    Id = equipmentDto.Id,
                    Name = equipmentDto.Name,
                    Category = equipmentDto.Category,
                    Description = equipmentDto.Description,
                    OwnerId = equipmentDto.OwnerId
                };
                await _equipmentService.UpdateEquipment(equipment);

                return Ok(true);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
