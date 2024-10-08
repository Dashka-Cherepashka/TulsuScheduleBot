﻿using Core.Bot.Commands.Interfaces;
using Core.DB;
using Core.DB.Entity;

using ScheduleBot;

using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
namespace Core.Bot.Commands.Classrooms.Days.Message {

    internal class ClassroomToday : IMessageCommand {

        public List<string>? Commands => [UserCommands.Instance.Message["Today"]];

        public List<Mode> Modes => [Mode.ClassroomSelected];

        public Manager.Check Check => Manager.Check.none;

        public async Task Execute(ScheduleDbContext dbContext, ChatId chatId, int messageId, TelegramUser user, string args) {
            ReplyKeyboardMarkup teacherWorkSchedule = DefaultMessage.GetClassroomWorkScheduleSelectedKeyboardMarkup(user.TelegramUserTmp.TmpData!);

            await Statics.ClassroomWorkScheduleRelevanceAsync(dbContext, chatId, user.TelegramUserTmp.TmpData!, teacherWorkSchedule);
            var date = DateOnly.FromDateTime(DateTime.Now);
            MessagesQueue.Message.SendTextMessage(chatId: chatId, text: Scheduler.GetClassroomWorkScheduleByDate(dbContext, date, user.TelegramUserTmp.TmpData!, user), replyMarkup: teacherWorkSchedule, parseMode: ParseMode.Markdown);
        }
    }
}
