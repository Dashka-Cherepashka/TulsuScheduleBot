﻿using Core.Bot.Commands.Interfaces;
using Core.DB;
using Core.DB.Entity;

using Telegram.Bot.Types;
namespace Core.Bot.Commands.Student.Slash.SetProfile.Message {
    public class SetProfile : IMessageCommand {

        public List<string> Commands => ["/SetProfile"];

        public List<Mode> Modes => [Mode.Default];

        public Manager.Check Check => Manager.Check.none;

        public async Task Execute(ScheduleDbContext dbContext, ChatId chatId, int messageId, TelegramUser user, string args) {
            if(user.IsSupergroup()) return;

            try {
                if(Guid.TryParse(args, out Guid profile)) {
                    if(profile != user.ScheduleProfileGuid && dbContext.ScheduleProfile.Any(i => i.ID == profile)) {
                        user.ScheduleProfileGuid = profile;
                        await dbContext.SaveChangesAsync();

                        MessagesQueue.Message.SendTextMessage(chatId: chatId, text: "Вы успешно сменили профиль", replyMarkup: DefaultMessage.GetMainKeyboardMarkup(user));
                    } else {
                        MessagesQueue.Message.SendTextMessage(chatId: chatId, text: "Вы пытаетесь изменить свой профиль на текущий или на профиль, который не существует", replyMarkup: DefaultMessage.GetMainKeyboardMarkup(user));
                    }
                } else {
                    MessagesQueue.Message.SendTextMessage(chatId: chatId, text: "Идентификатор профиля не распознан", replyMarkup: DefaultMessage.GetMainKeyboardMarkup(user));
                }
            } catch(IndexOutOfRangeException) { }
        }
    }
}
