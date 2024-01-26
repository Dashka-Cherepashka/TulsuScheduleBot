﻿using Core.Bot.Interfaces;

using ScheduleBot;
using ScheduleBot.DB;
using ScheduleBot.DB.Entity;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
namespace Core.Bot.Commands.Student.Custom.Message {
    internal class CustomEditName : IMessageCommand {
        public ITelegramBotClient BotClient => TelegramBot.Instance.botClient;

        public List<string>? Commands => null;

        public List<Mode> Modes => new() { Mode.CustomEditName };

        public Manager.Check Check => Manager.Check.none;

        public async Task Execute(ScheduleDbContext dbContext, ChatId chatId, int messageId, TelegramUser user, string args) {
            if(!string.IsNullOrWhiteSpace(user.TempData)) {
                CustomDiscipline discipline = dbContext.CustomDiscipline.Single(i => i.ID == uint.Parse(user.TempData));
                discipline.Name = args;

                user.Mode = Mode.Default;
                user.TempData = null;

                await Statics.DeleteTempMessage(user, messageId);

                await dbContext.SaveChangesAsync();

                await BotClient.SendTextMessageAsync(chatId: chatId, text: "Название предмета успешно изменено.", replyMarkup: Statics.MainKeyboardMarkup);
                await BotClient.SendTextMessageAsync(chatId: chatId, text: Scheduler.GetScheduleByDate(dbContext, discipline.Date, user, all: true).Item1, replyMarkup: DefaultCallback.GetCustomEditAdminInlineKeyboardButton(discipline), parseMode: ParseMode.Markdown);
            }
        }
    }
}
