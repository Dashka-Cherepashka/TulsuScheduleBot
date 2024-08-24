﻿using Core.Bot.Commands.Interfaces;
using Core.Bot.Messages;

using ScheduleBot.DB;
using ScheduleBot.DB.Entity;

using Telegram.Bot;
using Telegram.Bot.Types;

namespace Core.Bot.Commands.Classrooms.Days.ByDays.Message {
    internal class ClassroomByDays : IMessageCommand {
        public ITelegramBotClient BotClient => TelegramBot.Instance.botClient;

        public List<string>? Commands => [UserCommands.Instance.Message["ByDays"]];

        public List<Mode> Modes => [Mode.ClassroomSelected];

        public Manager.Check Check => Manager.Check.none;

        public Task Execute(ScheduleDbContext dbContext, ChatId chatId, int messageId, TelegramUser user, string args) {
            MessageQueue.SendTextMessage(chatId: chatId, text: UserCommands.Instance.Message["ByDays"], replyMarkup: Statics.DaysKeyboardMarkup);
            return Task.CompletedTask;
        }
    }
}
