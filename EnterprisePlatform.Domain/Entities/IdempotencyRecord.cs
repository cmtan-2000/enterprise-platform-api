using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnterprisePlatform.Domain.Entities
{
    public class IdempotencyRecord
    {
        public string Key { get; private set; } = null!;

        public Guid OrderId { get; private set; }

        public DateTime CreatedAt { get; private set; }

        private IdempotencyRecord()
        {
        }

        public IdempotencyRecord(string key, Guid orderId)
        {
            Key = key;
            OrderId = orderId;
            CreatedAt = DateTime.UtcNow;
        }

    }
}