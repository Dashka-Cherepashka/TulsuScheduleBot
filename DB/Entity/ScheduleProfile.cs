﻿using System.ComponentModel.DataAnnotations.Schema;

namespace ScheduleBot.DB.Entity {

    public class ScheduleProfile {
        public Guid ID { get; set; }

        [ForeignKey("TelegramUser")]
        public long? OwnerID { get; set; }
        public TelegramUser? TelegramUser { get; set; }

        [ForeignKey("GroupLastUpdate")]
        public string? Group { get; set; }
        public GroupLastUpdate? GroupLastUpdate { get; set; }

        [ForeignKey("StudentIDLastUpdate")]
        public string? StudentID { get; set; }
        public StudentIDLastUpdate? StudentIDLastUpdate { get; set; }

        public DateTime LastAppeal { get; set; } = DateTime.UtcNow;
    }
}
