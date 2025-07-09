using MailScheduler.Domain.Entities;
using MailScheduler.Domain.Enums;
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
    public class EmailTemplateRepository : IEmailTemplateRepository
    {
        private readonly ApplicationDbContext _context;
        public EmailTemplateRepository(ApplicationDbContext context) => _context = context;

        public async Task AddAsync(EmailTemplate template)
        {
            await _context.EmailTemplates.AddAsync(template);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(EmailTemplate template)
        {
            template.UpdateModified();
            _context.EmailTemplates.Update(template);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(EmailTemplate template)
        {
            template.MarkDeleted();
            _context.EmailTemplates.Update(template);
            await _context.SaveChangesAsync();
        }
        public async Task<EmailTemplate> GetByTypeAsync(MailRecipientType type)
        {
            return await _context.EmailTemplates
                .FirstOrDefaultAsync(t => t.RecipientType == type)
                ?? throw new KeyNotFoundException($"Template for {type} not found.");
        }
    }
}
