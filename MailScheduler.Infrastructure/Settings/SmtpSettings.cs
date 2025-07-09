

namespace MailScheduler.Infrastructure.Settings
{
    public class SmtpSettings
    {
        /// <summary>Gönderen adı (isteğe bağlı).</summary>
        public string? DisplayName { get; set; }
        /// <summary>SMTP sunucu host adresi.</summary>
        public string Host { get; set; } = null!;
        /// <summary>SMTP sunucu port numarası.</summary>
        public int Port { get; set; }
        /// <summary>Kullanıcı adı veya gönderici e-posta adresi.</summary>
        public string UserName { get; set; } = null!;
        /// <summary>SMTP şifresi.</summary>
        public string Password { get; set; } = null!;
        /// <summary>SSL/TLS etkinleştirilsin mi.</summary>
        public bool EnableSsl { get; set; }
    }
}
