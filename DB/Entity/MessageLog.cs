﻿namespace ScheduleBot.DB.Entity {
#pragma warning disable CS8618
    public class MessageLog {
        public long ID { get; set; }

        public TelegramUser User { get; set; }

        public string Message { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;
    }
}
