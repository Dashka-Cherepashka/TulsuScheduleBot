﻿using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace ScheduleBot.Bot {
    static class Constants {
        public const string RK_Today = "Сегодня";
        public const string RK_Tomorrow = "Завтра";

        public const string RK_ByDays = "По дням";
        public const string RK_ForAWeek = "На неделю";

        public const string RK_Profile = "Профиль";

        public const string RK_Exam = "Экзамены";
        public const string RK_NextExam = "Ближайший экзамен";
        public const string RK_AllExams = "Все экзамены";

        public const string RK_AcademicPerformance = "Успеваемость";

        public const string RK_Monday = "Понедельник";
        public const string RK_Tuesday = "Вторник";
        public const string RK_Wednesday = "Среда";
        public const string RK_Thursday = "Четверг";
        public const string RK_Friday = "Пятница";
        public const string RK_Saturday = "Суббота";

        public const string RK_ThisWeek = "Эта неделя";
        public const string RK_NextWeek = "Следующая неделя";

        public const string RK_Back = "Назад";
        public const string RK_Cancel = "Отмена";

        public const string RK_Semester = "семестр";

        public struct IK_ViewAll {
            public const string text = "Посмотреть все";
            public const string callback = "All";
        }

        public struct IK_Edit {
            public const string text = "Редактировать";
            public const string callback = "Edit";
        }

        public struct IK_Back {
            public const string text = "Назад";
            public const string callback = "Back";
        }

        public struct IK_Add {
            public const string text = "Добавить";
            public const string callback = "Add";
        }

        public struct IK_SetEndTime {
            public const string text = "";
            public const string callback = "SetEndTime";
        }

        public static readonly string[] StagesOfAdding = {"Введите название", "Введите тип", "Введите лектора", "Введите аудиторию", "Введите время начала", "Введите время конца", "Дисциплина добавлена", "Ошибка"};
    }

    public partial class TelegramBot {
        private async Task GroupError(ITelegramBotClient botClient, ChatId chatId) => await botClient.SendTextMessageAsync(chatId: chatId, text: $"Для того, чтобы узнать расписание, необходимо указать номер группы в настройках профиля.", replyMarkup: MainKeyboardMarkup);
        private async Task StudentIdError(Message message, ITelegramBotClient botClient) => await botClient.SendTextMessageAsync(chatId: message.Chat, text: $"Для того, чтобы узнать успеваемость, необходимо указать номер зачетной книжки в настройках профиля.", replyMarkup: MainKeyboardMarkup);
        private async Task ScheduleRelevance(ChatId chatId, ITelegramBotClient botClient, IReplyMarkup? replyMarkup) => await botClient.SendTextMessageAsync(chatId: chatId, text: $"Расписание актуально на {Parser.scheduleLastUpdate.ToString("dd.MM HH:mm")}", replyMarkup: replyMarkup);
        private async Task ProgressRelevance(ChatId chatId, ITelegramBotClient botClient, IReplyMarkup? replyMarkup) => await botClient.SendTextMessageAsync(chatId: chatId, text: $"Успеваемость актуально на {Parser.scheduleLastUpdate.ToString("dd.MM HH:mm")}", replyMarkup: replyMarkup);
    }
}
