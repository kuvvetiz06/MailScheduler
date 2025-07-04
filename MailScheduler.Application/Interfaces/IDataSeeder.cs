using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailScheduler.Application.Interfaces
{
    /// <summary>
    /// Uygulama başlangıcında gerekli temel verileri sağlar.
    /// </summary>
    public interface IDataSeeder
    {
        Task SeedAsync();
    }
}
