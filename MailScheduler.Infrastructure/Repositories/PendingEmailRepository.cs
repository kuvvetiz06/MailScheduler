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
    public class PendingEmailRepository : IPendingEmailRepository
    {
        private readonly ApplicationDbContext _context;
        public PendingEmailRepository(ApplicationDbContext context) => _context = context;

        public async Task AddAsync(PendingEmail email)
        {
            await _context.PendingEmails.AddAsync(email);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<PendingEmail>> GetPendingAsync()
        {
            return await _context.PendingEmails
                .Where(p => !p.IsDeleted)
                .ToListAsync();
        }

        public async Task UpdateAsync(PendingEmail email)
        {
            _context.PendingEmails.Update(email);
            await _context.SaveChangesAsync();
        }
    }
}
