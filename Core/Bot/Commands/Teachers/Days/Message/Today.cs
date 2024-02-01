﻿using Core.Bot.Interfaces;

using ScheduleBot;
using ScheduleBot.DB;
using ScheduleBot.DB.Entity;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
namespace Core.Bot.Commands.Teachers.Days.Message {

    internal class TeachersToday : IMessageCommand {
        public ITelegramBotClient BotClient => TelegramBot.Instance.botClient;

        public List<string>? Commands => new() { UserCommands.Instance.Message["Today"] };

        public List<Mode> Modes => new() { Mode.TeacherSelected };

        public Manager.Check Check => Manager.Check.none;

        public async Task Execute(ScheduleDbContext dbContext, ChatId chatId, int messageId, TelegramUser user, string args) {
            ReplyKeyboardMarkup teacherWorkSchedule = DefaultMessage.GetTeacherWorkScheduleSelectedKeyboardMarkup(user.TelegramUserTmp.TmpData!);

            await Statics.TeacherWorkScheduleRelevance(dbContext, BotClient, chatId, user.TelegramUserTmp.TmpData!, teacherWorkSchedule);
            var date = DateOnly.FromDateTime(DateTime.Now);
            await BotClient.SendTextMessageAsync(chatId: chatId, text: Scheduler.GetTeacherWorkScheduleByDate(dbContext, date, user.TelegramUserTmp.TmpData!), replyMarkup: teacherWorkSchedule);
        }
    }
}
