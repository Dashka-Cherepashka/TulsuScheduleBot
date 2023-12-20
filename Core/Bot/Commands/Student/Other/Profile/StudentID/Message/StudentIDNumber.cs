﻿using Core.Bot.Interfaces;

using ScheduleBot.DB;
using ScheduleBot.DB.Entity;

using Telegram.Bot;
using Telegram.Bot.Types;
namespace Core.Bot.Commands.Student.Other.Profile.StudentID.Message {
    internal class StudentIDNumber : IMessageCommand {
        public ITelegramBotClient BotClient => TelegramBot.Instance.botClient;

        public List<string>? Commands => new() { UserCommands.Instance.Message["StudentIDNumber"] };

        public List<Mode> Modes => new() { Mode.Default };

        public Manager.Check Check => Manager.Check.none;

        public async Task Execute(ScheduleDbContext dbContext, ChatId chatId, int messageId, TelegramUser user, string args) {
            if(user.IsOwner()) {
                user.Mode = Mode.StudentIDСhange;

                user.RequestingMessageID = (await BotClient.SendTextMessageAsync(chatId: chatId, text: "Хотите сменить номер зачётки? Если да, то напишите новый номер", replyMarkup: Statics.CancelKeyboardMarkup)).MessageId;
            }
        }
    }
}
