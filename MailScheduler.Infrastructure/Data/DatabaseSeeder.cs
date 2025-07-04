using MailScheduler.Application.Interfaces;
using MailScheduler.Domain.Entities;
using MailScheduler.Domain.Interfaces;

namespace MailScheduler.Infrastructure.Data
{
    public class DatabaseSeeder : IDataSeeder
    {
        private readonly IEmailTemplateRepository _templateRepo;
        private readonly IRecipientRepository _recipientRepo;

        public DatabaseSeeder(
            IEmailTemplateRepository templateRepo,
            IRecipientRepository recipientRepo)
        {
            _templateRepo = templateRepo;
            _recipientRepo = recipientRepo;
        }

        public async Task SeedAsync()
        {
            // Şablonları kontrol et ve ekle
            var templates = new List<EmailTemplate>
            {
                new EmailTemplate("TemplateA", "Konu A", "Merhaba, bu şablon A içeriğidir."),
                new EmailTemplate("TemplateB", "Konu B", "Merhaba, bu şablon B içeriğidir."),
                new EmailTemplate("TemplateC", "Konu C", "Merhaba, bu şablon C içeriğidir.")
            };

            foreach (var t in templates)
            {
                var exists = await _templateRepo.GetByNameAsync(t.Name);
                if (exists == null)
                {
                    await _templateRepo.AddAsync(t);
                }
            }

            // Alıcıları kontrol et ve ekle
            var recipients = new List<Recipient>
            {
                new Recipient("user1@example.com", "User One"),
                new Recipient("user2@example.com", "User Two")
            };

            var existing = await _recipientRepo.GetAllAsync();
            var existingEmails = new HashSet<string>(existing.Select(r => r.EmailAddress));

            foreach (var r in recipients)
            {
                if (!existingEmails.Contains(r.EmailAddress))
                {
                    await _recipientRepo.AddAsync(r);
                }
            }
        }
    }
}
