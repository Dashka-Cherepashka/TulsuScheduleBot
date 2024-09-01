﻿using System.Text;

using Core.Bot.Commands.Interfaces;
using Core.DB;
using Core.DB.Entity;

using Telegram.Bot.Types;

namespace Core.Bot.Commands.Admin.Statistics.Message {
    internal class Statistics : IMessageCommand {

        public List<string> Commands => ["Статистика"];

        public List<Mode> Modes => [Mode.Admin];

        public Manager.Check Check => Manager.Check.none;

        private static readonly UserCommands.ConfigStruct config = UserCommands.Instance.Config;

        public Task Execute(ScheduleDbContext dbContext, ChatId chatId, int messageId, TelegramUser user, string args) {
            StringBuilder sb = new();

            DateTime today = DateTime.Now.Date;

            DateTime startOfWeek = today.AddDays(-7);

            DateTime startOfMonth = today.AddMonths(-1);

            IQueryable<TelegramUser> telegramUsers = dbContext.TelegramUsers.AsQueryable();
            IQueryable<MessageLog> messageLogs = dbContext.MessageLog.AsQueryable();

            sb.AppendLine($"Всего пользователей: {telegramUsers.Count()}");
            sb.AppendLine($"Администраторы: {telegramUsers.Count(u => u.IsAdmin)}");
            sb.AppendLine();

            AppendNewUsersStats(sb, telegramUsers, today, startOfWeek, startOfMonth);
            AppendActiveUsersStats(sb, telegramUsers, today, startOfWeek, startOfMonth);
            AppendTopUsers(sb, telegramUsers);
            AppendMessageStats(sb, telegramUsers, messageLogs, today, startOfWeek, startOfMonth);
            AppendMessageDistributionStats(sb, messageLogs);
            AppendScheduleProfileStats(sb, dbContext);

            MessagesQueue.Message.SendTextMessage(chatId: chatId, text: sb.ToString(), replyMarkup: Statics.AdminPanelKeyboardMarkup, parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);

            return Task.CompletedTask;
        }

        private static void AppendNewUsersStats(StringBuilder sb, IQueryable<TelegramUser> users, DateTime today, DateTime startOfWeek, DateTime startOfMonth) {
            sb.AppendLine($"--Новых пользователей--");
            sb.AppendLine($"За сегодня: {users.Count(u => u.DateOfRegistration.HasValue && u.DateOfRegistration.Value.ToLocalTime() >= today)}");
            sb.AppendLine($"За неделю: {users.Count(u => u.DateOfRegistration.HasValue && u.DateOfRegistration.Value.ToLocalTime() >= startOfWeek)}");
            sb.AppendLine($"За месяц: {users.Count(u => u.DateOfRegistration.HasValue && u.DateOfRegistration.Value.ToLocalTime() >= startOfMonth)}");
            sb.AppendLine();
        }

        private static void AppendActiveUsersStats(StringBuilder sb, IQueryable<TelegramUser> users, DateTime today, DateTime startOfWeek, DateTime startOfMonth) {
            sb.AppendLine($"--Активных пользователей--");
            sb.AppendLine($"За сегодня: {users.Count(u => u.LastAppeal.ToLocalTime() >= today)}");
            sb.AppendLine($"За неделю: {users.Count(u => u.LastAppeal.ToLocalTime() >= startOfWeek)}");
            sb.AppendLine($"За месяц: {users.Count(u => u.LastAppeal.ToLocalTime() >= startOfMonth)}");
            sb.AppendLine();
        }

        private static void AppendTopUsers(StringBuilder sb, IQueryable<TelegramUser> users) {
            sb.AppendLine($"--Топ пользователей по активности--");
            var topUsers = users.OrderByDescending(u => u.TotalRequests).Take(5).ToList();

            foreach(TelegramUser? topUser in topUsers) {
                string userName = Statics.EscapeSpecialCharacters($"{topUser.FirstName} {topUser.LastName}");
                sb.AppendLine(!string.IsNullOrWhiteSpace(topUser.Username)
                    ? $"[{userName}](https://t.me/{topUser.Username})"
                    : userName);
            }

            sb.AppendLine();
        }

        private static void AppendMessageStats(StringBuilder sb, IQueryable<TelegramUser> users, IQueryable<MessageLog> messages, DateTime today, DateTime startOfWeek, DateTime startOfMonth) {
            sb.AppendLine($"--Получено сообщений--");
            sb.AppendLine($"Всего сообщений: {messages.Count()}");
            sb.AppendLine($"За сегодня: {messages.Count(ml => ml.Date.ToLocalTime() >= today)}");
            sb.AppendLine($"За неделю: {messages.Count(ml => ml.Date.ToLocalTime() >= startOfWeek)}");
            sb.AppendLine($"За месяц: {messages.Count(ml => ml.Date.ToLocalTime() >= startOfMonth)}");
            sb.AppendLine();

            sb.AppendLine($"Среднее количество запросов на пользователя: {users.Average(u => u.TotalRequests):F2}");
            sb.AppendLine();
        }

        private static void AppendMessageDistributionStats(StringBuilder sb, IQueryable<MessageLog> messages) {
            sb.AppendLine($"--Распределение сообщений по времени--");

            int morningMessages = messages.Count(ml => ml.Date.ToLocalTime().TimeOfDay < TimeSpan.FromHours(12));
            int afternoonMessages = messages.Count(ml => ml.Date.ToLocalTime().TimeOfDay >= TimeSpan.FromHours(12) && ml.Date.ToLocalTime().TimeOfDay < TimeSpan.FromHours(18));
            int eveningMessages = messages.Count(ml => ml.Date.ToLocalTime().TimeOfDay >= TimeSpan.FromHours(18));

            sb.AppendLine($"Утро (00:00 - 12:00): {morningMessages}");
            sb.AppendLine($"День (12:00 - 18:00): {afternoonMessages}");
            sb.AppendLine($"Вечер (18:00 - 24:00): {eveningMessages}");

            var mostActiveHour = messages
                .GroupBy(ml => ml.Date.ToLocalTime().Hour)
                .OrderByDescending(g => g.Count())
                .Select(g => new { Hour = g.Key, Count = g.Count() })
                .FirstOrDefault();

            if(mostActiveHour != null) {
                sb.AppendLine($"Самое активное время: {mostActiveHour.Hour}:00 - {mostActiveHour.Hour + 1}:00 с {mostActiveHour.Count} сообщениями");
            }

            sb.AppendLine();
        }

        private static void AppendScheduleProfileStats(StringBuilder sb, ScheduleDbContext dbContext) {
            sb.AppendLine($"Общее количество обновляемых групп: {dbContext.ScheduleProfile.Where(i => !string.IsNullOrEmpty(i.Group) && (DateTime.Now - i.LastAppeal.ToLocalTime()).TotalDays <= config.DisciplineUpdateDays).Select(i => i.Group!).Distinct().Count()}");

            var groups = dbContext.GroupLastUpdate.OrderByDescending(i => i.Update.ToLocalTime()).ToList();
            if(groups.Count > 1) {
                sb.AppendLine($"Время между последними обновлениями: {(groups[0].Update.ToLocalTime() - groups[1].Update.ToLocalTime()).ToString(@"hh\:mm\:ss")}");
            }
        }
    }
}