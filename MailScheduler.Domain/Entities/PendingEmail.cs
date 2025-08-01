﻿using MailScheduler.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailScheduler.Domain.Entities
{
    /// <summary>
    /// Gönderilemeyen e-postaları tutan tablo, to ve cc bilgilerini içerir.
    /// </summary>
    public class PendingEmail : BaseEntity
    {
        public string Recipient { get; private set; } = null!;
        public IEnumerable<string>? Cc { get; private set; }
        public string Subject { get; private set; } = null!;
        public string Body { get; private set; } = null!;
        public int AttemptCount { get; private set; }
        public DateTime LastAttempt { get; private set; }
        public bool IsSuccess { get; private set; }
        public bool IsDeleted { get; private set; }

        private PendingEmail() { }

        public PendingEmail(string recipient, IEnumerable<string>? cc, string subject, string body)
            : base()
        {
            Recipient = recipient;
            Cc = cc;
            Subject = subject;
            Body = body;
            AttemptCount = 0;
            LastAttempt = CreatedDate;
            IsSuccess = false;
            IsDeleted = false;
        }

        public void RecordAttempt(bool success)
        {
            AttemptCount++;
            LastAttempt = DateTime.UtcNow;
            IsSuccess = success;
            if (success)
                IsDeleted = true;
        }

        public void MarkFailed()
        {
            IsDeleted = true;
        }
    }
}
