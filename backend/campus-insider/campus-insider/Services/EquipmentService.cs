using campus_insider.Data;
using campus_insider.DTOs;
using campus_insider.Models;
using Microsoft.EntityFrameworkCore;

namespace campus_insider.Services
{
    public class EquipmentService
    {
        private readonly AppDbContext _context;

        public EquipmentService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Equipment>> GetAllEquipment()
        {
            return await _context.Equipment.AsNoTracking().ToListAsync();

        }

        public async Task<Equipment> GetByIdAsync(long id)
        {
            return await _context.Equipment.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<Equipment> ShareEquipment(Equipment equipment)
        {
            _context.Equipment.Add(equipment);
            await _context.SaveChangesAsync();
            return equipment;
        }

        public async Task UnshareEquipment(Equipment equipment)
        {
            await _context.Equipment.Where(e => e.Id == equipment.Id).ExecuteDeleteAsync();

        }

        public async Task<Equipment> UpdateEquipment(Equipment equipment)
        {

            await _context.Equipment
            .Where(e => e.Id == equipment.Id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(e => e.Name, equipment.Name)
                .SetProperty(e => e.Description, equipment.Description)
                .SetProperty(e => e.Category, equipment.Category)
            );

            return equipment;

        }

        public async Task<List<Equipment>> GetEquipmentByOwner(int userId)
        {
            return await _context.Equipment
                .Where(e => e.OwnerId == userId)
                .Select(e => new Equipment
                {
                    Name = e.Name,
                    Category = e.Category,
                    Description = e.Description
                })
                .ToListAsync();
        }
    }
}
