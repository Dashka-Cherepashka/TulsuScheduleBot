﻿using Core.Bot.Interfaces;

using ScheduleBot;
using ScheduleBot.DB;
using ScheduleBot.DB.Entity;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
namespace Core.Bot.Commands.Student.Days.ByDays.Message {
    internal class Thursday : IMessageCommand {
        public ITelegramBotClient BotClient => TelegramBot.Instance.botClient;

        public List<string>? Commands => new() { UserCommands.Instance.Message["Thursday"] };

        public List<Mode> Modes => new() { Mode.Default };

        public Manager.Check Check => Manager.Check.group;

        public async Task Execute(ScheduleDbContext dbContext, ChatId chatId, int messageId, TelegramUser user, string args) {
            await Statics.ScheduleRelevance(dbContext, BotClient, chatId, user.ScheduleProfile.Group!, Statics.DaysKeyboardMarkup);
            foreach(((string, bool), DateOnly) day in Scheduler.GetScheduleByDay(dbContext, DayOfWeek.Thursday, user))
                await BotClient.SendTextMessageAsync(chatId: chatId, text: day.Item1.Item1, replyMarkup: DefaultCallback.GetInlineKeyboardButton(day.Item2, user, day.Item1.Item2), parseMode: ParseMode.Markdown);
        }
    }
}
