﻿using Core.DB;
using Core.DB.Entity;

using Telegram.Bot.Types;

using static Core.Bot.Commands.Manager;

namespace Core.Bot.Commands.Interfaces {
    public interface ICallbackCommand {
        public string Command { get; }

        public Mode Mode { get; }

        public Check Check { get; }

        public Task Execute(ScheduleDbContext dbContext, ChatId chatId, int messageId, TelegramUser user, string message, string args);
    }
}
