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
    public class EmailLogRepository : IEmailLogRepository
    {
        private readonly ApplicationDbContext _context;
        public EmailLogRepository(ApplicationDbContext context) => _context = context;

        public async Task AddAsync(EmailLog log)
        {
            await _context.EmailLogs.AddAsync(log);
            await _context.SaveChangesAsync();
        }


    }
}
