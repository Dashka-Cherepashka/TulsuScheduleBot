﻿using Core.Bot.Interfaces;

using ScheduleBot.DB;
using ScheduleBot.DB.Entity;

using Telegram.Bot;
using Telegram.Bot.Types;
namespace Core.Bot.Commands.Student.Other.Profile.Settings.TeacherLincs.Message {
    internal class TeacherLincsEnabled : IMessageCommand {
        public ITelegramBotClient BotClient => TelegramBot.Instance.botClient;

        public List<string>? Commands => new() { UserCommands.Instance.Message["TeacherLincsEnabled"] };

        public List<Mode> Modes => new() { Mode.Default };

        public Manager.Check Check => Manager.Check.none;

        public async Task Execute(ScheduleDbContext dbContext, ChatId chatId, int messageId, TelegramUser user, string args) {
            user.Settings.TeacherLincsEnabled = !user.Settings.TeacherLincsEnabled;

            user.RequestingMessageID = (await BotClient.SendTextMessageAsync(chatId: chatId, text: UserCommands.Instance.Message["Settings"], replyMarkup: DefaultMessage.GetSettingsKeyboardMarkup(user))).MessageId;
        }
    }
}
