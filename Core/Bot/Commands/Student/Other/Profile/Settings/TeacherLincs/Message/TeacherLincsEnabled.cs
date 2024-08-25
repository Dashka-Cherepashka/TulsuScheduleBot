﻿using Core.Bot.Commands.Interfaces;
using Core.Bot.MessagesQueue;
using Core.DB;
using Core.DB.Entity;

using Telegram.Bot.Types;
namespace Core.Bot.Commands.Student.Other.Profile.Settings.TeacherLincs.Message {
    internal class TeacherLincsEnabled : IMessageCommand {

        public List<string>? Commands => [UserCommands.Instance.Message["TeacherLincsEnabled"]];

        public List<Mode> Modes => [Mode.Default];

        public Manager.Check Check => Manager.Check.none;

        public async Task Execute(ScheduleDbContext dbContext, ChatId chatId, int messageId, TelegramUser user, string args) {
            user.Settings.TeacherLincsEnabled = !user.Settings.TeacherLincsEnabled;

            MessagesQueue.Message.SendTextMessage(chatId: chatId, text: UserCommands.Instance.Message["Settings"], replyMarkup: DefaultMessage.GetSettingsKeyboardMarkup(user));

            await dbContext.SaveChangesAsync();
        }
    }
}
