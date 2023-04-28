﻿using ScheduleBot.DB.Entity;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace ScheduleBot.Bot {
    public partial class TelegramBot {

        private async Task AddingDisciplineMessageModeAsync(ITelegramBotClient botClient, Message message, TelegramUser user) {
            switch(message.Text) {
                case Constants.RK_Cancel:
                    user.Mode = Mode.Default;
                    dbContext.TemporaryAddition.Remove(dbContext.TemporaryAddition.Where(i => i.TelegramUser == user).OrderByDescending(i => i.AddDate).First());
                    dbContext.SaveChanges();
                    await botClient.SendTextMessageAsync(chatId: message.Chat, text: "Основное меню", replyMarkup: MainKeyboardMarkup);
                    break;

                default:
                    await SetStagesAddingDisciplineAsync(user, message, botClient);
                    break;
            }
        }

        private async Task UpdateAddingDisciplineAsync(TelegramUser user, Message message, ITelegramBotClient botClient, TemporaryAddition temporaryAddition) {
            try {
                if(message.Text == null) throw new Exception();

                switch(temporaryAddition.Counter) {
                    case 0:
                        temporaryAddition.Name = message.Text;
                        break;
                    case 1:
                        temporaryAddition.Type = message.Text;
                        break;
                    case 2:
                        temporaryAddition.Lecturer = message.Text;
                        break;
                    case 3:
                        temporaryAddition.LectureHall = message.Text;
                        break;
                    case 4:
                        temporaryAddition.StartTime = TimeOnly.Parse(message.Text);
                        break;
                    case 5:
                        temporaryAddition.EndTime = TimeOnly.Parse(message.Text);
                        break;
                }
            } catch(Exception) {
                await botClient.SendTextMessageAsync(chatId: message.Chat, text: "Ошибка! " + GetStagesAddingDiscipline(user, temporaryAddition.Counter), replyMarkup: CancelKeyboardMarkup);
                return;
            }

            dbContext.SaveChanges();
        }


        private async Task SetStagesAddingDisciplineAsync(TelegramUser user, Message message, ITelegramBotClient botClient) {
            var temporaryAddition = dbContext.TemporaryAddition.Where(i => i.TelegramUser == user).OrderByDescending(i => i.AddDate).First();

            await UpdateAddingDisciplineAsync(user, message, botClient, temporaryAddition);

            switch(++temporaryAddition.Counter) {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                    await botClient.SendTextMessageAsync(chatId: message.Chat, text: GetStagesAddingDiscipline(user, temporaryAddition.Counter), replyMarkup: CancelKeyboardMarkup);
                    break;

                case 5:
                    var endTime = temporaryAddition.StartTime.AddMinutes(95);
                    await botClient.SendTextMessageAsync(chatId: message.Chat, text: GetStagesAddingDiscipline(user, temporaryAddition.Counter),
                            replyMarkup: new InlineKeyboardMarkup(InlineKeyboardButton.WithCallbackData(text: endTime.ToString(), callbackData: $"{Constants.IK_SetEndTime.callback} {endTime}")) { });
                    break;

                case 6:
                    await SaveAddingDisciplineAsync(user, message, botClient, temporaryAddition);
                    break;
            }
        }

        private async Task SaveAddingDisciplineAsync(TelegramUser user, Message message, ITelegramBotClient botClient, TemporaryAddition temporaryAddition) {
            dbContext.Disciplines.Add(temporaryAddition);
            dbContext.TemporaryAddition.Remove(temporaryAddition);
            user.Mode = Mode.Default;
            dbContext.SaveChanges();

            await botClient.SendTextMessageAsync(chatId: message.Chat, text: GetStagesAddingDiscipline(user, temporaryAddition.Counter), replyMarkup: MainKeyboardMarkup);
            await botClient.SendTextMessageAsync(chatId: message.Chat, text: scheduler.GetScheduleByDate(temporaryAddition.Date), replyMarkup: inlineAdminKeyboardMarkup);
        }

        private string GetStagesAddingDiscipline(TelegramUser user, int? counter = null) {
            if(counter != null)
                return Constants.StagesOfAdding[(int)counter];

            return Constants.StagesOfAdding[dbContext.TemporaryAddition.Where(i => i.TelegramUser == user).OrderByDescending(i => i.AddDate).First().Counter];
        }

    }
}