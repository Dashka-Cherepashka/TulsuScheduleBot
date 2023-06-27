﻿using ScheduleBot.DB;
using ScheduleBot.DB.Entity;

using Telegram.Bot;
using Telegram.Bot.Types;

namespace ScheduleBot.Bot {
    public partial class TelegramBot {
        private async Task CustomEdit(ScheduleDbContext dbContext, ChatId chatId, int messageId, TelegramUser user, string args, Mode mode, string text) {
            var tmp = args.Split('|');
            var discipline = dbContext.CustomDiscipline.FirstOrDefault(i => i.ID == uint.Parse(tmp[0]));
            if(discipline is not null) {
                if(user.IsAdmin()) {
                    user.Mode = mode;
                    user.CurrentPath = $"{discipline.ID}";
                    dbContext.SaveChanges();

                    await botClient.DeleteMessageAsync(chatId: chatId, messageId: messageId);
                    await botClient.SendTextMessageAsync(chatId: chatId, text: text, replyMarkup: CancelKeyboardMarkup);
                    return;
                }
            }
            if(DateOnly.TryParse(tmp[1], out DateOnly date))
                await botClient.EditMessageTextAsync(chatId: chatId, messageId: messageId, text: Scheduler.GetScheduleByDate(dbContext, date, user.ScheduleProfile), replyMarkup: GetInlineKeyboardButton(date, user));
        }
    }
}