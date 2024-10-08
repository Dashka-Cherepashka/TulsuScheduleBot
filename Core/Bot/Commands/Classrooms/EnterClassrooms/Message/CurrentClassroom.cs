﻿using Core.Bot.Commands.Interfaces;
using Core.DB;
using Core.DB.Entity;

using Telegram.Bot.Types;
namespace Core.Bot.Commands.Classrooms.EnterClassrooms.Message {
    internal class CurrentClassroom : IMessageCommand {

        public List<string>? Commands => [UserCommands.Instance.Message["CurrentClassroom"]];

        public List<Mode> Modes => [Mode.ClassroomSelected];

        public Manager.Check Check => Manager.Check.none;

        public async Task Execute(ScheduleDbContext dbContext, ChatId chatId, int messageId, TelegramUser user, string args) {
            user.TelegramUserTmp.Mode = Mode.ClassroomSchedule;
            await dbContext.SaveChangesAsync();

            MessagesQueue.Message.SendTextMessage(chatId: chatId, text: UserCommands.Instance.Message["EnterClassroom"], replyMarkup: Statics.WorkScheduleBackKeyboardMarkup);
        }
    }
}
