using MailScheduler.Domain.Entities;
using MailScheduler.Domain.Interfaces;
using MailScheduler.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailScheduler.Infrastructure.Repositories
{
    public class RecipientRepository : IRecipientRepository
    {
        private readonly ApplicationDbContext _context;
        public RecipientRepository(ApplicationDbContext context) => _context = context;

        public async Task AddAsync(Recipient recipient)
        {
            await _context.Recipients.AddAsync(recipient);
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<Recipient>> GetAllAsync() =>
            await _context.Recipients.ToListAsync();
    }
}
