﻿using System.Globalization;
using System.Text.RegularExpressions;

using Microsoft.EntityFrameworkCore;

using ScheduleBot.DB;
using ScheduleBot.DB.Entity;

using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace ScheduleBot.Bot {
    public partial class TelegramBot {
        private readonly ITelegramBotClient botClient;
        private readonly CommandManager commandManager;
        private readonly Parser parser;

        public TelegramBot() {
            botClient = new TelegramBotClient(Environment.GetEnvironmentVariable("TelegramBotToken")!);

            parser = new(commands, UpdatedDisciplinesAsync);

            commandManager = new(this, (string message, TelegramUser user, out string args) => {
                args = "";

                if(DefaultMessageRegex().IsMatch(message))
                    return $"{message} {user.Mode}".ToLower();

                Match match = TermsMessageRegex().Match(message);
                if(match.Success) {
                    args = match.Groups[1].ToString();
                    return $"{match.Groups[2]} {user.Mode}".ToLower();
                }

                match = GroupOrStudentIDMessageRegex().Match(message);
                if(match.Success)
                    return $"{match.Groups[1]} {user.Mode}".ToLower();

                match = CommandMessageRegex().Match(message);
                if(match.Success) {
                    args = match.Groups[2].ToString();
                    return $"{match.Groups[1]} {user.Mode}".ToLower();
                }

                return $"{message} {user.Mode}".ToLower();

            }, (string message, TelegramUser user, out string args) => {
                args = "";

                Match match = DisciplineCallbackRegex().Match(message);
                if(match.Success) {
                    args = match.Groups[2].ToString();
                    return $"{match.Groups[1]} {user.Mode}".ToLower();
                }

                match = NotificationsCallbackRegex().Match(message);
                if(match.Success) {
                    args = match.Groups[2].ToString();
                    return $"{match.Groups[1]} {user.Mode}".ToLower();
                }

                return $"{message} {user.Mode}".ToLower();
            });

            #region Message
            #region Main
            commandManager.AddMessageCommand("/start", Mode.Default, async (dbContext, chatId, messageId, user, args) => {
                await botClient.SendTextMessageAsync(chatId: chatId, text: "👋", replyMarkup: MainKeyboardMarkup);

                if(string.IsNullOrWhiteSpace(user.ScheduleProfile.Group)) {
                    user.Mode = Mode.GroupСhange;

                    user.RequestingMessageID = (await botClient.SendTextMessageAsync(chatId: chatId, text: "Для начала работы с ботом необходимо указать номер учебной группы", replyMarkup: CancelKeyboardMarkup)).MessageId;

                    dbContext.SaveChanges();
                }
            });
            commandManager.AddMessageCommand("/SetProfile", Mode.Default, async (dbContext, chatId, messageId, user, args) => {
                try {
                    if(Guid.TryParse(args, out Guid profile)) {
                        if(profile != user.ScheduleProfileGuid && dbContext.ScheduleProfile.Any(i => i.ID == profile)) {
                            user.ScheduleProfileGuid = profile;
                            dbContext.SaveChanges();
                            await botClient.SendTextMessageAsync(chatId: chatId, text: "Вы успешно сменили профиль", replyMarkup: MainKeyboardMarkup);
                        } else {
                            await botClient.SendTextMessageAsync(chatId: chatId, text: "Вы пытаетесь изменить свой профиль на текущий или на профиль, который не существует", replyMarkup: MainKeyboardMarkup);
                        }
                    } else {
                        await botClient.SendTextMessageAsync(chatId: chatId, text: "Идентификатор профиля не распознан", replyMarkup: MainKeyboardMarkup);
                    }
                } catch(IndexOutOfRangeException) { }
            });

            commandManager.AddMessageCommand(new[] { commands.Message["Back"], commands.Message["Cancel"] }, Mode.Default, async (dbContext, chatId, messageId, user, args) => {
                if(user.TempData == commands.Message["AcademicPerformance"] ||
                    user.TempData == commands.Message["Profile"] ||
                    user.TempData == commands.Message["Corps"] ||
                    user.TempData == commands.Message["Exam"]) {

                    await botClient.SendTextMessageAsync(chatId: chatId, text: commands.Message["Other"], replyMarkup: AdditionalKeyboardMarkup);
                } else {
                    await botClient.SendTextMessageAsync(chatId: chatId, text: commands.Message["MainMenu"], replyMarkup: MainKeyboardMarkup);
                }

                user.TempData = null;
                dbContext.SaveChanges();
            });
            commandManager.AddMessageCommand(commands.Message["Cancel"], Mode.AddingDiscipline, async (dbContext, chatId, messageId, user, args) => {
                CustomDiscipline tmp = dbContext.CustomDiscipline.Where(i => !i.IsAdded && i.ScheduleProfile == user.ScheduleProfile).OrderByDescending(i => i.AddDate).First();

                user.Mode = Mode.Default;
                dbContext.CustomDiscipline.Remove(tmp);

                await DeleteTempMessage(user, messageId);
                await DeleteInitialMessage(botClient, chatId, user);

                dbContext.SaveChanges();

                await ScheduleRelevance(dbContext, botClient, chatId, user.ScheduleProfile.Group!, MainKeyboardMarkup);
                await botClient.SendTextMessageAsync(chatId: chatId, text: Scheduler.GetScheduleByDate(dbContext, tmp.Date, user.ScheduleProfile, true), replyMarkup: GetEditAdminInlineKeyboardButton(dbContext, tmp.Date, user.ScheduleProfile));
            });
            commandManager.AddMessageCommand(commands.Message["Cancel"], new[] { Mode.GroupСhange, Mode.StudentIDСhange, Mode.ResetProfileLink }, async (dbContext, chatId, messageId, user, args) => {
                user.Mode = Mode.Default;

                await DeleteTempMessage(user, messageId);

                dbContext.SaveChanges();

                await botClient.SendTextMessageAsync(chatId: chatId, text: commands.Message["Profile"], replyMarkup: GetProfileKeyboardMarkup(user));
            });
            commandManager.AddMessageCommand(commands.Message["Cancel"], new[] { Mode.CustomEditName, Mode.CustomEditLecturer, Mode.CustomEditLectureHall, Mode.CustomEditType, Mode.CustomEditStartTime, Mode.CustomEditEndTime }, async (dbContext, chatId, messageId, user, args) => {
                await botClient.SendTextMessageAsync(chatId: chatId, text: commands.Message["MainMenu"], replyMarkup: MainKeyboardMarkup);

                if(!string.IsNullOrWhiteSpace(user.TempData)) {
                    if(user.IsOwner()) {
                        CustomDiscipline discipline = dbContext.CustomDiscipline.Single(i => i.ID == uint.Parse(user.TempData));
                        await botClient.SendTextMessageAsync(chatId: chatId, text: Scheduler.GetScheduleByDate(dbContext, discipline.Date, user.ScheduleProfile, true), replyMarkup: GetCustomEditAdminInlineKeyboardButton(discipline));
                    }
                }

                user.Mode = Mode.Default;
                user.TempData = null;

                await DeleteTempMessage(user, messageId);

                dbContext.SaveChanges();
            });
            commandManager.AddMessageCommand(commands.Message["Cancel"], Mode.DaysNotifications, async (dbContext, chatId, messageId, user, args) => {
                user.Mode = Mode.Default;

                await DeleteTempMessage(user, messageId);

                dbContext.SaveChanges();

                await botClient.SendTextMessageAsync(chatId: chatId, text: commands.Message["Profile"], replyMarkup: GetProfileKeyboardMarkup(user));
                await botClient.SendTextMessageAsync(chatId: chatId, text: commands.Message["NotificationSettings"], replyMarkup: GetNotificationsInlineKeyboardButton(user));
            });
            commandManager.AddMessageCommand(commands.Message["TeachersWorkScheduleBack"], Mode.TeachersWorkSchedule, async (dbContext, chatId, messageId, user, args) => {
                await botClient.SendTextMessageAsync(chatId: chatId, text: commands.Message["MainMenu"], replyMarkup: MainKeyboardMarkup);

                user.Mode = Mode.Default;
                user.TempData = null;

                dbContext.SaveChanges();
            });

            commandManager.AddMessageCommand(commands.Message["Today"], Mode.Default, async (dbContext, chatId, messageId, user, args) => {
                await ScheduleRelevance(dbContext, botClient, chatId, user.ScheduleProfile.Group!, MainKeyboardMarkup);
                var date = DateOnly.FromDateTime(DateTime.Now);
                await botClient.SendTextMessageAsync(chatId: chatId, text: Scheduler.GetScheduleByDate(dbContext, date, user.ScheduleProfile), replyMarkup: GetInlineKeyboardButton(date, user));
            }, CommandManager.Check.group);
            commandManager.AddMessageCommand(commands.Message["Tomorrow"], Mode.Default, async (dbContext, chatId, messageId, user, args) => {
                await ScheduleRelevance(dbContext, botClient, chatId, user.ScheduleProfile.Group!, MainKeyboardMarkup);
                var date = DateOnly.FromDateTime(DateTime.Now.AddDays(1));
                await botClient.SendTextMessageAsync(chatId: chatId, text: Scheduler.GetScheduleByDate(dbContext, date, user.ScheduleProfile), replyMarkup: GetInlineKeyboardButton(date, user));
            }, CommandManager.Check.group);

            commandManager.AddMessageCommand(commands.Message["ByDays"], Mode.Default, async (dbContext, chatId, messageId, user, args) => {
                await botClient.SendTextMessageAsync(chatId: chatId, text: commands.Message["ByDays"], replyMarkup: DaysKeyboardMarkup);
            }, CommandManager.Check.group);
            commandManager.AddMessageCommand(commands.Message["Monday"], Mode.Default, async (dbContext, chatId, messageId, user, args) => {
                await ScheduleRelevance(dbContext, botClient, chatId, user.ScheduleProfile.Group!, DaysKeyboardMarkup);
                foreach((string, DateOnly) day in Scheduler.GetScheduleByDay(dbContext, System.DayOfWeek.Monday, user.ScheduleProfile))
                    await botClient.SendTextMessageAsync(chatId: chatId, text: day.Item1, replyMarkup: GetInlineKeyboardButton(day.Item2, user));
            }, CommandManager.Check.group);
            commandManager.AddMessageCommand(commands.Message["Tuesday"], Mode.Default, async (dbContext, chatId, messageId, user, args) => {
                await ScheduleRelevance(dbContext, botClient, chatId, user.ScheduleProfile.Group!, DaysKeyboardMarkup);
                foreach((string, DateOnly) day in Scheduler.GetScheduleByDay(dbContext, System.DayOfWeek.Tuesday, user.ScheduleProfile))
                    await botClient.SendTextMessageAsync(chatId: chatId, text: day.Item1, replyMarkup: GetInlineKeyboardButton(day.Item2, user));
            }, CommandManager.Check.group);
            commandManager.AddMessageCommand(commands.Message["Wednesday"], Mode.Default, async (dbContext, chatId, messageId, user, args) => {
                await ScheduleRelevance(dbContext, botClient, chatId, user.ScheduleProfile.Group!, DaysKeyboardMarkup);
                foreach((string, DateOnly) day in Scheduler.GetScheduleByDay(dbContext, System.DayOfWeek.Wednesday, user.ScheduleProfile))
                    await botClient.SendTextMessageAsync(chatId: chatId, text: day.Item1, replyMarkup: GetInlineKeyboardButton(day.Item2, user));
            }, CommandManager.Check.group);
            commandManager.AddMessageCommand(commands.Message["Thursday"], Mode.Default, async (dbContext, chatId, messageId, user, args) => {
                await ScheduleRelevance(dbContext, botClient, chatId, user.ScheduleProfile.Group!, DaysKeyboardMarkup);
                foreach((string, DateOnly) day in Scheduler.GetScheduleByDay(dbContext, System.DayOfWeek.Thursday, user.ScheduleProfile))
                    await botClient.SendTextMessageAsync(chatId: chatId, text: day.Item1, replyMarkup: GetInlineKeyboardButton(day.Item2, user));
            }, CommandManager.Check.group);
            commandManager.AddMessageCommand(commands.Message["Friday"], Mode.Default, async (dbContext, chatId, messageId, user, args) => {
                await ScheduleRelevance(dbContext, botClient, chatId, user.ScheduleProfile.Group!, DaysKeyboardMarkup);
                foreach((string, DateOnly) day in Scheduler.GetScheduleByDay(dbContext, System.DayOfWeek.Friday, user.ScheduleProfile))
                    await botClient.SendTextMessageAsync(chatId: chatId, text: day.Item1, replyMarkup: GetInlineKeyboardButton(day.Item2, user));
            }, CommandManager.Check.group);
            commandManager.AddMessageCommand(commands.Message["Saturday"], Mode.Default, async (dbContext, chatId, messageId, user, args) => {
                await ScheduleRelevance(dbContext, botClient, chatId, user.ScheduleProfile.Group!, DaysKeyboardMarkup);
                foreach((string, DateOnly) day in Scheduler.GetScheduleByDay(dbContext, System.DayOfWeek.Saturday, user.ScheduleProfile))
                    await botClient.SendTextMessageAsync(chatId: chatId, text: day.Item1, replyMarkup: GetInlineKeyboardButton(day.Item2, user));
            }, CommandManager.Check.group);

            commandManager.AddMessageCommand(commands.Message["ForAWeek"], Mode.Default, async (dbContext, chatId, messageId, user, args) => {
                await botClient.SendTextMessageAsync(chatId: chatId, text: commands.Message["ForAWeek"], replyMarkup: WeekKeyboardMarkup);
            }, CommandManager.Check.group);
            commandManager.AddMessageCommand(commands.Message["ThisWeek"], Mode.Default, async (dbContext, chatId, messageId, user, args) => {
                await ScheduleRelevance(dbContext, botClient, chatId, user.ScheduleProfile.Group!, WeekKeyboardMarkup);
                foreach((string, DateOnly) item in Scheduler.GetScheduleByWeak(dbContext, CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(DateTime.Now, CalendarWeekRule.FirstFourDayWeek, System.DayOfWeek.Monday) - 1, user.ScheduleProfile))
                    await botClient.SendTextMessageAsync(chatId: chatId, text: item.Item1, replyMarkup: GetInlineKeyboardButton(item.Item2, user));
            }, CommandManager.Check.group);
            commandManager.AddMessageCommand(commands.Message["NextWeek"], Mode.Default, async (dbContext, chatId, messageId, user, args) => {
                await ScheduleRelevance(dbContext, botClient, chatId, user.ScheduleProfile.Group!, WeekKeyboardMarkup);
                foreach((string, DateOnly) item in Scheduler.GetScheduleByWeak(dbContext, CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(DateTime.Now, CalendarWeekRule.FirstFourDayWeek, System.DayOfWeek.Monday), user.ScheduleProfile))
                    await botClient.SendTextMessageAsync(chatId: chatId, text: item.Item1, replyMarkup: GetInlineKeyboardButton(item.Item2, user));
            }, CommandManager.Check.group);

            commandManager.AddMessageCommand(commands.Message["Exam"], Mode.Default, async (dbContext, chatId, messageId, user, args) => {
                user.TempData = commands.Message["Exam"];
                dbContext.SaveChanges();

                await botClient.SendTextMessageAsync(chatId: chatId, text: commands.Message["Exam"], replyMarkup: ExamKeyboardMarkup);
            }, CommandManager.Check.group);
            commandManager.AddMessageCommand(commands.Message["AllExams"], Mode.Default, async (dbContext, chatId, messageId, user, args) => {
                await ScheduleRelevance(dbContext, botClient, chatId, user.ScheduleProfile.Group!, ExamKeyboardMarkup);
                foreach(string item in Scheduler.GetExamse(dbContext, user.ScheduleProfile, true))
                    await botClient.SendTextMessageAsync(chatId: chatId, text: item, replyMarkup: ExamKeyboardMarkup);
            }, CommandManager.Check.group);
            commandManager.AddMessageCommand(commands.Message["NextExam"], Mode.Default, async (dbContext, chatId, messageId, user, args) => {
                await ScheduleRelevance(dbContext, botClient, chatId, user.ScheduleProfile.Group!, ExamKeyboardMarkup);
                foreach(string item in Scheduler.GetExamse(dbContext, user.ScheduleProfile, false))
                    await botClient.SendTextMessageAsync(chatId: chatId, text: item, replyMarkup: ExamKeyboardMarkup);
            }, CommandManager.Check.group);

            commandManager.AddMessageCommand(commands.Message["Other"], Mode.Default, async (dbContext, chatId, messageId, user, args) => {
                await botClient.SendTextMessageAsync(chatId: chatId, text: commands.Message["Other"], replyMarkup: AdditionalKeyboardMarkup);
            });

            commandManager.AddMessageCommand(commands.Message["AcademicPerformance"], Mode.Default, async (dbContext, chatId, messageId, user, args) => {
                user.TempData = commands.Message["AcademicPerformance"];
                dbContext.SaveChanges();

                string StudentID = user.ScheduleProfile.StudentID!;

                await ProgressRelevance(dbContext, botClient, chatId, StudentID, null, false);
                await botClient.SendTextMessageAsync(chatId: chatId, text: commands.Message["AcademicPerformance"], replyMarkup: GetTermsKeyboardMarkup(dbContext, StudentID));
            }, CommandManager.Check.studentId);
            commandManager.AddMessageCommand(commands.Message["Semester"], Mode.Default, async (dbContext, chatId, messageId, user, args) => {
                string StudentID = user.ScheduleProfile.StudentID!;

                await ProgressRelevance(dbContext, botClient, chatId, StudentID, GetTermsKeyboardMarkup(dbContext, StudentID));
                await botClient.SendTextMessageAsync(chatId: chatId, text: Scheduler.GetProgressByTerm(dbContext, int.Parse(args), StudentID), replyMarkup: GetTermsKeyboardMarkup(dbContext, StudentID));
            }, CommandManager.Check.studentId);

            commandManager.AddMessageCommand(commands.Message["Profile"], Mode.Default, async (dbContext, chatId, messageId, user, args) => {
                user.TempData = commands.Message["Profile"];
                dbContext.SaveChanges();
                await botClient.SendTextMessageAsync(chatId: chatId, text: commands.Message["Profile"], replyMarkup: GetProfileKeyboardMarkup(user));
            });
            commandManager.AddMessageCommand(commands.Message["GetProfileLink"], Mode.Default, async (dbContext, chatId, messageId, user, args) => {
                if(user.IsOwner()) {
                    await botClient.SendTextMessageAsync(chatId: chatId, text: $"Если вы хотите поделиться своим расписанием с кем-то, просто отправьте им следующую команду: " +
                    $"\n`/SetProfile {user.ScheduleProfileGuid}`" +
                    $"\nЕсли другой пользователь введет эту команду, он сможет видеть расписание с вашими изменениями.", replyMarkup: GetProfileKeyboardMarkup(user), parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
                } else {
                    await botClient.SendTextMessageAsync(chatId: chatId, text: "Поделиться профилем может только его владелец!", replyMarkup: MainKeyboardMarkup);
                }
            });
            commandManager.AddMessageCommand(commands.Message["ResetProfileLink"], Mode.Default, async (dbContext, chatId, messageId, user, args) => {
                if(!user.IsOwner()) {
                    user.Mode = Mode.ResetProfileLink;
                    dbContext.SaveChanges();
                    await botClient.SendTextMessageAsync(chatId: chatId, text: "Вы точно уверены что хотите восстановить свой профиль?", replyMarkup: ResetProfileLinkKeyboardMarkup);
                } else {
                    await botClient.SendTextMessageAsync(chatId: chatId, text: "Владельцу профиля нет смысла его восстанавливать!", replyMarkup: MainKeyboardMarkup);
                }
            });
            commandManager.AddMessageCommand(commands.Message["Reset"], Mode.ResetProfileLink, async (dbContext, chatId, messageId, user, args) => {
                user.Mode = Mode.Default;

                ScheduleProfile? profile = dbContext.ScheduleProfile.FirstOrDefault(i => i.OwnerID == user.ChatID);
                if(profile is not null) {
                    user.ScheduleProfile = profile;
                } else {
                    profile = new() { TelegramUser = user };
                    dbContext.ScheduleProfile.Add(profile);
                    user.ScheduleProfile = profile;
                }

                dbContext.SaveChanges();

                await botClient.SendTextMessageAsync(chatId: chatId, text: commands.Message["Profile"], replyMarkup: GetProfileKeyboardMarkup(user));
            });

            commandManager.AddMessageCommand(commands.Message["GroupNumber"], Mode.Default, async (dbContext, chatId, messageId, user, args) => {
                if(user.IsOwner()) {
                    user.Mode = Mode.GroupСhange;

                    user.RequestingMessageID = (await botClient.SendTextMessageAsync(chatId: chatId, text: "Хотите сменить номер учебной группы? Если да, то напишите новый номер", replyMarkup: CancelKeyboardMarkup)).MessageId;

                    dbContext.SaveChanges();
                }
            });
            commandManager.AddMessageCommand(commands.Message["StudentIDNumber"], Mode.Default, async (dbContext, chatId, messageId, user, args) => {
                if(user.IsOwner()) {
                    user.Mode = Mode.StudentIDСhange;

                    user.RequestingMessageID = (await botClient.SendTextMessageAsync(chatId: chatId, text: "Хотите сменить номер зачётки? Если да, то напишите новый номер", replyMarkup: CancelKeyboardMarkup)).MessageId;

                    dbContext.SaveChanges();
                }
            });
            commandManager.AddMessageCommand(commands.Message["Notifications"], Mode.Default, async (dbContext, chatId, messageId, user, args) => {
                await botClient.SendTextMessageAsync(chatId: chatId, text: commands.Message["NotificationSettings"], replyMarkup: GetNotificationsInlineKeyboardButton(user));
            });

            commandManager.AddMessageCommand(Mode.Default, async (dbContext, chatId, messageId, user, args) => {
                if(DateRegex().IsMatch(args)) {
                    try {
                        DateOnly date = DateTime.TryParse(args, out DateTime _date)
                            ? DateOnly.FromDateTime(_date)
                            : DateOnly.FromDateTime(DateTime.Parse($"{args} {DateTime.Now.Month}"));
                        await ScheduleRelevance(dbContext, botClient, chatId, user.ScheduleProfile.Group!, MainKeyboardMarkup);
                        await botClient.SendTextMessageAsync(chatId: chatId, text: Scheduler.GetScheduleByDate(dbContext, date, user.ScheduleProfile), replyMarkup: GetInlineKeyboardButton(date, user));
                    } catch(Exception) {
                        await botClient.SendTextMessageAsync(chatId: chatId, text: $"Команда распознана как дата, но не соответствует формату \"день месяц год\".\nНапример: \"1 мая 2023\", \"1 05 23\", \"1 5\", \"1\"", replyMarkup: MainKeyboardMarkup);
                    }

                    return true;
                }

                await botClient.SendTextMessageAsync(chatId: chatId, text: $"Команда не распознана пожалуйста используйте кнопки или укажите дату в формате \"день месяц год\".\nНапример: \"1 мая 2023\", \"1 05 23\", \"1 5\", \"1\"", replyMarkup: MainKeyboardMarkup);

                return false;
            }, CommandManager.Check.group);

            commandManager.AddMessageCommand(Mode.AddingDiscipline, async (dbContext, chatId, messageId, user, args) => {
                await SetStagesAddingDisciplineAsync(dbContext, botClient, chatId, messageId, args, user);
                return true;
            });

            commandManager.AddMessageCommand(Mode.GroupСhange, async (dbContext, chatId, messageId, user, args) => {
                await DeleteTempMessage(user, messageId);

                if(args.Length > 15) {
                    user.RequestingMessageID = (await botClient.SendTextMessageAsync(chatId: chatId, text: "Номер группы не может содержать более 15 символов.", replyMarkup: CancelKeyboardMarkup)).MessageId;
                    return false;
                }

                int _messageId = (await botClient.SendTextMessageAsync(chatId: chatId, text: "Нужно подождать...", replyMarkup: CancelKeyboardMarkup)).MessageId;
                GroupLastUpdate? group = dbContext.GroupLastUpdate.FirstOrDefault(i => i.Group == args);

                if(group is null && parser.UpdatingDisciplines(dbContext, args))
                    group = dbContext.GroupLastUpdate.FirstOrDefault(i => i.Group == args);

                if(group is not null) {
                    user.Mode = Mode.Default;
                    user.ScheduleProfile.GroupLastUpdate = group;
                    dbContext.SaveChanges();

                    await botClient.SendTextMessageAsync(chatId: chatId, text: $"Номер группы успешно изменен на {args} ", replyMarkup: GetProfileKeyboardMarkup(user));

                } else {
                    user.RequestingMessageID = (await botClient.SendTextMessageAsync(chatId: chatId, text: "Сайт ТулГУ не отвечает или такой группы не существует", replyMarkup: CancelKeyboardMarkup)).MessageId;
                }

                dbContext.SaveChanges();

                await botClient.DeleteMessageAsync(chatId: chatId, messageId: _messageId);
                return true;
            });
            commandManager.AddMessageCommand(Mode.StudentIDСhange, async (dbContext, chatId, messageId, user, args) => {
                await DeleteTempMessage(user, messageId);

                if(args.Length > 10) {
                    user.RequestingMessageID = (await botClient.SendTextMessageAsync(chatId: chatId, text: "Номер зачетки не может содержать более 10 символов.", replyMarkup: CancelKeyboardMarkup)).MessageId;
                    return false;
                }

                int _messageId = (await botClient.SendTextMessageAsync(chatId: chatId, text: "Нужно подождать...", replyMarkup: CancelKeyboardMarkup)).MessageId;

                if(int.TryParse(args, out int id)) {
                    StudentIDLastUpdate? studentID = dbContext.StudentIDLastUpdate.FirstOrDefault(i => i.StudentID == args);

                    if(studentID is null && parser.UpdatingProgress(dbContext, args))
                        studentID = dbContext.StudentIDLastUpdate.FirstOrDefault(i => i.StudentID == args);

                    if(studentID is not null) {
                        user.Mode = Mode.Default;
                        user.ScheduleProfile.StudentIDLastUpdate = studentID;
                        dbContext.SaveChanges();

                        await botClient.SendTextMessageAsync(chatId: chatId, text: $"Номер зачётки успешно изменен на {args} ", replyMarkup: GetProfileKeyboardMarkup(user));

                    } else {
                        user.RequestingMessageID = (await botClient.SendTextMessageAsync(chatId: chatId, text: "Сайт ТулГУ не отвечает или указан неверный номер зачётки", replyMarkup: CancelKeyboardMarkup)).MessageId;
                    }
                } else {
                    user.RequestingMessageID = (await botClient.SendTextMessageAsync(chatId: chatId, text: "Не удалось распознать введенный номер зачётной книжки", replyMarkup: CancelKeyboardMarkup)).MessageId;
                }

                dbContext.SaveChanges();

                await botClient.DeleteMessageAsync(chatId: chatId, messageId: _messageId);
                return true;
            });

            commandManager.AddMessageCommand(Mode.ResetProfileLink, async (dbContext, chatId, messageId, user, args) => {
                await botClient.SendTextMessageAsync(chatId: chatId, text: "Выберите один из представленных вариантов!", replyMarkup: ResetProfileLinkKeyboardMarkup);
                return true;
            });

            commandManager.AddMessageCommand(Mode.CustomEditName, async (dbContext, chatId, messageId, user, args) => {
                if(!string.IsNullOrWhiteSpace(user.TempData)) {
                    CustomDiscipline discipline = dbContext.CustomDiscipline.Single(i => i.ID == uint.Parse(user.TempData));
                    discipline.Name = args;

                    user.Mode = Mode.Default;
                    user.TempData = null;

                    await DeleteTempMessage(user, messageId);

                    dbContext.SaveChanges();

                    await botClient.SendTextMessageAsync(chatId: chatId, text: "Название предмета успешно изменено.", replyMarkup: MainKeyboardMarkup);
                    await botClient.SendTextMessageAsync(chatId: chatId, text: Scheduler.GetScheduleByDate(dbContext, discipline.Date, user.ScheduleProfile, true), replyMarkup: GetCustomEditAdminInlineKeyboardButton(discipline));
                }

                return true;
            });
            commandManager.AddMessageCommand(Mode.CustomEditLecturer, async (dbContext, chatId, messageId, user, args) => {
                if(!string.IsNullOrWhiteSpace(user.TempData)) {
                    CustomDiscipline discipline = dbContext.CustomDiscipline.Single(i => i.ID == uint.Parse(user.TempData));
                    discipline.Lecturer = args;

                    user.Mode = Mode.Default;
                    user.TempData = null;

                    await DeleteTempMessage(user, messageId);

                    dbContext.SaveChanges();

                    await botClient.SendTextMessageAsync(chatId: chatId, text: "Лектор успешно изменен.", replyMarkup: MainKeyboardMarkup);
                    await botClient.SendTextMessageAsync(chatId: chatId, text: Scheduler.GetScheduleByDate(dbContext, discipline.Date, user.ScheduleProfile, true), replyMarkup: GetCustomEditAdminInlineKeyboardButton(discipline));
                }

                return true;
            });
            commandManager.AddMessageCommand(Mode.CustomEditType, async (dbContext, chatId, messageId, user, args) => {
                if(!string.IsNullOrWhiteSpace(user.TempData)) {
                    CustomDiscipline discipline = dbContext.CustomDiscipline.Single(i => i.ID == uint.Parse(user.TempData));
                    discipline.Type = args;

                    user.Mode = Mode.Default;
                    user.TempData = null;

                    await DeleteTempMessage(user, messageId);

                    dbContext.SaveChanges();

                    await botClient.SendTextMessageAsync(chatId: chatId, text: "Тип предмета успешно изменен.", replyMarkup: MainKeyboardMarkup);
                    await botClient.SendTextMessageAsync(chatId: chatId, text: Scheduler.GetScheduleByDate(dbContext, discipline.Date, user.ScheduleProfile, true), replyMarkup: GetCustomEditAdminInlineKeyboardButton(discipline));
                }

                return true;
            });
            commandManager.AddMessageCommand(Mode.CustomEditLectureHall, async (dbContext, chatId, messageId, user, args) => {
                if(!string.IsNullOrWhiteSpace(user.TempData)) {
                    CustomDiscipline discipline = dbContext.CustomDiscipline.Single(i => i.ID == uint.Parse(user.TempData));
                    discipline.LectureHall = args;

                    user.Mode = Mode.Default;
                    user.TempData = null;

                    await DeleteTempMessage(user, messageId);

                    dbContext.SaveChanges();

                    await botClient.SendTextMessageAsync(chatId: chatId, text: "Аудитория успешно изменена.", replyMarkup: MainKeyboardMarkup);
                    await botClient.SendTextMessageAsync(chatId: chatId, text: Scheduler.GetScheduleByDate(dbContext, discipline.Date, user.ScheduleProfile, true), replyMarkup: GetCustomEditAdminInlineKeyboardButton(discipline));
                }

                return true;
            });
            commandManager.AddMessageCommand(Mode.CustomEditStartTime, async (dbContext, chatId, messageId, user, args) => {
                if(!string.IsNullOrWhiteSpace(user.TempData)) {
                    CustomDiscipline discipline = dbContext.CustomDiscipline.Single(i => i.ID == uint.Parse(user.TempData));
                    try {
                        discipline.StartTime = ParseTime(args);
                        user.Mode = Mode.Default;
                        user.TempData = null;

                        await DeleteTempMessage(user, messageId);

                        dbContext.SaveChanges();

                        await botClient.SendTextMessageAsync(chatId: chatId, text: "Время начала успешно изменено.", replyMarkup: MainKeyboardMarkup);
                        await botClient.SendTextMessageAsync(chatId: chatId, text: Scheduler.GetScheduleByDate(dbContext, discipline.Date, user.ScheduleProfile, true), replyMarkup: GetCustomEditAdminInlineKeyboardButton(discipline));
                    } catch(Exception) {
                        await botClient.SendTextMessageAsync(chatId: chatId, text: "Ошибка в формате времени!", replyMarkup: CancelKeyboardMarkup);
                    }
                }

                return true;
            });
            commandManager.AddMessageCommand(Mode.CustomEditEndTime, async (dbContext, chatId, messageId, user, args) => {
                if(!string.IsNullOrWhiteSpace(user.TempData)) {
                    CustomDiscipline discipline = dbContext.CustomDiscipline.Single(i => i.ID == uint.Parse(user.TempData));
                    try {
                        discipline.EndTime = ParseTime(args);
                        user.Mode = Mode.Default;
                        user.TempData = null;

                        await DeleteTempMessage(user, messageId);

                        dbContext.SaveChanges();

                        await botClient.SendTextMessageAsync(chatId: chatId, text: "Время конца успешно изменено.", replyMarkup: MainKeyboardMarkup);
                        await botClient.SendTextMessageAsync(chatId: chatId, text: Scheduler.GetScheduleByDate(dbContext, discipline.Date, user.ScheduleProfile, true), replyMarkup: GetCustomEditAdminInlineKeyboardButton(discipline));
                    } catch(Exception) {
                        await botClient.SendTextMessageAsync(chatId: chatId, text: "Ошибка в формате времени!", replyMarkup: CancelKeyboardMarkup);
                    }
                }

                return true;
            });

            commandManager.AddMessageCommand(Mode.DaysNotifications, async (dbContext, chatId, messageId, user, args) => {
                try {
                    user.Notifications.Days = Math.Abs(int.Parse(args));
                    user.Mode = Mode.Default;

                    await DeleteTempMessage(user, messageId);

                    dbContext.SaveChanges();

                    await botClient.SendTextMessageAsync(chatId: chatId, text: "Количество дней успешно изменено.", replyMarkup: GetProfileKeyboardMarkup(user));
                    await botClient.SendTextMessageAsync(chatId: chatId, text: commands.Message["NotificationSettings"], replyMarkup: GetNotificationsInlineKeyboardButton(user));
                } catch(Exception) {
                    await botClient.SendTextMessageAsync(chatId: chatId, text: "Ошибка в формате количества дней!", replyMarkup: CancelKeyboardMarkup);
                }

                return true;
            });

            commandManager.AddCallbackCommand(commands.Callback["Edit"].callback, Mode.Default, async (dbContext, chatId, messageId, user, message, args) => {
                if(DateOnly.TryParse(args, out DateOnly date)) {
                    if(user.IsOwner())
                        await botClient.EditMessageTextAsync(chatId: chatId, messageId: messageId, text: Scheduler.GetScheduleByDate(dbContext, date, user.ScheduleProfile, true), replyMarkup: GetEditAdminInlineKeyboardButton(dbContext, date, user.ScheduleProfile));
                    else
                        await botClient.EditMessageTextAsync(chatId: chatId, messageId: messageId, text: Scheduler.GetScheduleByDate(dbContext, date, user.ScheduleProfile), replyMarkup: GetInlineKeyboardButton(date, user));
                }
            }, CommandManager.Check.group);
            commandManager.AddCallbackCommand(commands.Callback["All"].callback, Mode.Default, async (dbContext, chatId, messageId, user, message, args) => {
                if(DateOnly.TryParse(args, out DateOnly date))
                    await botClient.EditMessageTextAsync(chatId: chatId, messageId: messageId, text: Scheduler.GetScheduleByDate(dbContext, date, user.ScheduleProfile, true), replyMarkup: GetBackInlineKeyboardButton(date, user));
            }, CommandManager.Check.group);
            commandManager.AddCallbackCommand(commands.Callback["Back"].callback, Mode.Default, async (dbContext, chatId, messageId, user, message, args) => {
                if(DateOnly.TryParse(args, out DateOnly date))
                    await botClient.EditMessageTextAsync(chatId: chatId, messageId: messageId, text: Scheduler.GetScheduleByDate(dbContext, date, user.ScheduleProfile), replyMarkup: GetInlineKeyboardButton(date, user));
            }, CommandManager.Check.group);
            commandManager.AddCallbackCommand(commands.Callback["Add"].callback, Mode.Default, async (dbContext, chatId, messageId, user, message, args) => {
                if(DateOnly.TryParse(args, out DateOnly date)) {
                    if(user.IsOwner()) {
                        user.Mode = Mode.AddingDiscipline;
                        user.TempData = $"{messageId}";
                        dbContext.CustomDiscipline.Add(new(user.ScheduleProfile, date));
                        dbContext.SaveChanges();

                        await botClient.EditMessageTextAsync(chatId: chatId, messageId: messageId, text: Scheduler.GetScheduleByDate(dbContext, date, user.ScheduleProfile));
                        user.RequestingMessageID = (await botClient.SendTextMessageAsync(chatId: chatId, text: GetStagesAddingDiscipline(dbContext, user), replyMarkup: CancelKeyboardMarkup)).MessageId;

                        dbContext.SaveChanges();
                    } else {
                        await botClient.EditMessageTextAsync(chatId: chatId, messageId: messageId, text: Scheduler.GetScheduleByDate(dbContext, date, user.ScheduleProfile), replyMarkup: GetInlineKeyboardButton(date, user));
                    }
                }
            }, CommandManager.Check.group);

            commandManager.AddCallbackCommand(commands.Callback["SetEndTime"].callback, Mode.AddingDiscipline, async (dbContext, chatId, messageId, user, message, args) => {
                await SetStagesAddingDisciplineAsync(dbContext, botClient, chatId, messageId, args, user);
            });

            commandManager.AddCallbackCommand("DisciplineDay", Mode.Default, async (dbContext, chatId, messageId, user, message, args) => {
                string[] tmp = args.Split('|');
                Discipline? discipline = dbContext.Disciplines.FirstOrDefault(i => i.ID == uint.Parse(tmp[0]));
                if(discipline is not null) {
                    if(user.IsOwner()) {
                        var completedDisciplines = dbContext.CompletedDisciplines.Where(i => i.ScheduleProfileGuid == user.ScheduleProfileGuid).ToList();

                        CompletedDiscipline dayTmp = new(discipline, user.ScheduleProfileGuid);
                        CompletedDiscipline? dayCompletedDisciplines = completedDisciplines.FirstOrDefault(i => i.Equals(dayTmp));

                        if(dayCompletedDisciplines is not null)
                            dbContext.CompletedDisciplines.Remove(dayCompletedDisciplines);
                        else
                            dbContext.CompletedDisciplines.Add(dayTmp);

                        dbContext.SaveChanges();
                        await botClient.EditMessageReplyMarkupAsync(chatId: chatId, messageId: messageId, replyMarkup: GetEditAdminInlineKeyboardButton(dbContext, discipline.Date, user.ScheduleProfile));

                        return;
                    }
                }

                if(DateOnly.TryParse(tmp[1], out DateOnly date))
                    await botClient.EditMessageTextAsync(chatId: chatId, messageId: messageId, text: Scheduler.GetScheduleByDate(dbContext, date, user.ScheduleProfile), replyMarkup: GetInlineKeyboardButton(date, user));
            }, CommandManager.Check.group);
            commandManager.AddCallbackCommand("DisciplineAlways", Mode.Default, async (dbContext, chatId, messageId, user, message, args) => {
                string[] tmp = args.Split('|');
                Discipline? discipline = dbContext.Disciplines.FirstOrDefault(i => i.ID == uint.Parse(tmp[0]));
                if(discipline is not null) {
                    if(user.IsOwner()) {
                        var completedDisciplines = dbContext.CompletedDisciplines.Where(i => i.ScheduleProfileGuid == user.ScheduleProfileGuid).ToList();

                        CompletedDiscipline alwaysTmp = new(discipline, user.ScheduleProfileGuid) { Date = null };
                        CompletedDiscipline? alwaysCompletedDisciplines = completedDisciplines.FirstOrDefault(i => i.Equals(alwaysTmp));

                        if(alwaysCompletedDisciplines is not null) {
                            dbContext.CompletedDisciplines.Remove(alwaysCompletedDisciplines);
                        } else {
                            dbContext.CompletedDisciplines.RemoveRange(dbContext.CompletedDisciplines.Where(i => i.ScheduleProfileGuid == user.ScheduleProfileGuid && i.Date != null && i.Name == discipline.Name && i.Lecturer == discipline.Lecturer && i.Class == discipline.Class && i.Subgroup == discipline.Subgroup));
                            dbContext.CompletedDisciplines.Add(alwaysTmp);
                        }

                        dbContext.SaveChanges();
                        await botClient.EditMessageReplyMarkupAsync(chatId: chatId, messageId: messageId, replyMarkup: GetEditAdminInlineKeyboardButton(dbContext, discipline.Date, user.ScheduleProfile));
                        return;
                    }
                }

                if(DateOnly.TryParse(tmp[1], out DateOnly date))
                    await botClient.EditMessageTextAsync(chatId: chatId, messageId: messageId, text: Scheduler.GetScheduleByDate(dbContext, date, user.ScheduleProfile), replyMarkup: GetInlineKeyboardButton(date, user));
            }, CommandManager.Check.group);

            commandManager.AddCallbackCommand("CustomDelete", Mode.Default, async (dbContext, chatId, messageId, user, message, args) => {
                string[] tmp = args.Split('|');
                CustomDiscipline? customDiscipline = dbContext.CustomDiscipline.FirstOrDefault(i => i.ID == uint.Parse(tmp[0]));
                if(customDiscipline is not null) {
                    if(user.IsOwner()) {
                        dbContext.CustomDiscipline.Remove(customDiscipline);
                        dbContext.SaveChanges();

                        await botClient.EditMessageTextAsync(chatId: chatId, messageId: messageId, text: Scheduler.GetScheduleByDate(dbContext, customDiscipline.Date, user.ScheduleProfile), replyMarkup: GetEditAdminInlineKeyboardButton(dbContext, customDiscipline.Date, user.ScheduleProfile));
                        return;
                    }
                }

                if(DateOnly.TryParse(tmp[1], out DateOnly date))
                    await botClient.EditMessageTextAsync(chatId: chatId, messageId: messageId, text: Scheduler.GetScheduleByDate(dbContext, date, user.ScheduleProfile), replyMarkup: GetInlineKeyboardButton(date, user));
            }, CommandManager.Check.group);
            commandManager.AddCallbackCommand(commands.Callback["CustomEditCancel"].callback, Mode.Default, async (dbContext, chatId, messageId, user, message, args) => {
                if(DateOnly.TryParse(args, out DateOnly date))
                    if(user.IsOwner())
                        await botClient.EditMessageTextAsync(chatId: chatId, messageId: messageId, text: Scheduler.GetScheduleByDate(dbContext, date, user.ScheduleProfile), replyMarkup: GetEditAdminInlineKeyboardButton(dbContext, date, user.ScheduleProfile));
                    else
                        await botClient.EditMessageTextAsync(chatId: chatId, messageId: messageId, text: Scheduler.GetScheduleByDate(dbContext, date, user.ScheduleProfile), replyMarkup: GetInlineKeyboardButton(date, user));
            }, CommandManager.Check.group);
            commandManager.AddCallbackCommand("CustomEdit", Mode.Default, async (dbContext, chatId, messageId, user, message, args) => {
                string[] tmp = args.Split('|');
                CustomDiscipline? customDiscipline = dbContext.CustomDiscipline.FirstOrDefault(i => i.ID == uint.Parse(tmp[0]));
                if(customDiscipline is not null) {
                    if(user.IsOwner()) {
                        await botClient.EditMessageReplyMarkupAsync(chatId: chatId, messageId: messageId, replyMarkup: GetCustomEditAdminInlineKeyboardButton(customDiscipline));
                        return;
                    }
                }

                if(DateOnly.TryParse(tmp[1], out DateOnly date))
                    await botClient.EditMessageTextAsync(chatId: chatId, messageId: messageId, text: Scheduler.GetScheduleByDate(dbContext, date, user.ScheduleProfile), replyMarkup: GetInlineKeyboardButton(date, user));
            }, CommandManager.Check.group);
            commandManager.AddCallbackCommand("CustomEditName", Mode.Default, async (dbContext, chatId, messageId, user, message, args) => {
                await CustomEdit(dbContext, chatId, messageId, user, args, Mode.CustomEditName,
                "Хотите изменить название предмета? Если да, то напишите новое");
            });
            commandManager.AddCallbackCommand("CustomEditLecturer", Mode.Default, async (dbContext, chatId, messageId, user, message, args) => {
                await CustomEdit(dbContext, chatId, messageId, user, args, Mode.CustomEditLecturer,
                "Хотите изменить лектора? Если да, то напишите нового");
            });
            commandManager.AddCallbackCommand("CustomEditType", Mode.Default, async (dbContext, chatId, messageId, user, message, args) => {
                await CustomEdit(dbContext, chatId, messageId, user, args, Mode.CustomEditType,
                "Хотите изменить тип предмета? Если да, то напишите новый");
            });
            commandManager.AddCallbackCommand("CustomEditLectureHall", Mode.Default, async (dbContext, chatId, messageId, user, message, args) => {
                await CustomEdit(dbContext, chatId, messageId, user, args, Mode.CustomEditLectureHall,
                "Хотите изменить аудиторию? Если да, то напишите новую");
            });
            commandManager.AddCallbackCommand("CustomEditStartTime", Mode.Default, async (dbContext, chatId, messageId, user, message, args) => {
                await CustomEdit(dbContext, chatId, messageId, user, args, Mode.CustomEditStartTime,
                "Хотите изменить время начала пары? Если да, то напишите новое");
            });
            commandManager.AddCallbackCommand("CustomEditEndTime", Mode.Default, async (dbContext, chatId, messageId, user, message, args) => {
                await CustomEdit(dbContext, chatId, messageId, user, args, Mode.CustomEditEndTime,
                "Хотите изменить время конца пары? Если да, то напишите новое");
            });

            commandManager.AddCallbackCommand("ToggleNotifications", Mode.Default, async (dbContext, chatId, messageId, user, message, args) => {
                switch(args) {
                    case "on":
                        user.Notifications.IsEnabled = true;
                        break;
                    case "off":
                        user.Notifications.IsEnabled = false;
                        break;
                }

                dbContext.SaveChanges();

                await botClient.EditMessageTextAsync(chatId: chatId, messageId: messageId, text: commands.Message["NotificationSettings"], replyMarkup: GetNotificationsInlineKeyboardButton(user));
            });
            commandManager.AddCallbackCommand("DaysNotifications", Mode.Default, async (dbContext, chatId, messageId, user, message, args) => {
                user.Mode = Mode.DaysNotifications;

                await botClient.DeleteMessageAsync(chatId: chatId, messageId: messageId);
                user.RequestingMessageID = (await botClient.SendTextMessageAsync(chatId: chatId, text: "Хотите изменить количество дней? Если да, то напишите новое", replyMarkup: CancelKeyboardMarkup)).MessageId;

                dbContext.SaveChanges();
            });
            #endregion

            #region Corps
            commandManager.AddMessageCommand(commands.Message["Corps"], Mode.Default, async (dbContext, chatId, messageId, user, args) => {
                user.TempData = commands.Message["Corps"];
                dbContext.SaveChanges();
                await botClient.SendTextMessageAsync(chatId: chatId, text: "Выберите корпус, и я покажу где он на карте", replyMarkup: CorpsKeyboardMarkup);
            });

            foreach(BotCommands.CorpsStruct item in commands.Corps) {
                commandManager.AddMessageCommand(item.text, Mode.Default, async (dbContext, chatId, messageId, user, args) => {
                    await botClient.SendVenueAsync(chatId: chatId, latitude: item.latitude, longitude: item.longitude, title: item.title, address: item.address, replyMarkup: CorpsKeyboardMarkup);
                });
            }

            commandManager.AddMessageCommand(commands.College.text, Mode.Default, async (dbContext, chatId, messageId, user, args) => {
                await botClient.SendTextMessageAsync(chatId: chatId, text: commands.College.title, replyMarkup: CancelKeyboardMarkup);

                foreach(BotCommands.CorpsStruct item in commands.College.corps)
                    await botClient.SendVenueAsync(chatId: chatId, latitude: item.latitude, longitude: item.longitude, title: "", address: item.address, replyMarkup: CorpsKeyboardMarkup);
            });
            #endregion

            #region TeachersWorkSchedule
            commandManager.AddMessageCommand(commands.Message["TeachersWorkSchedule"], Mode.Default, async (dbContext, chatId, messageId, user, args) => {
                user.Mode = Mode.TeachersWorkSchedule;
                dbContext.SaveChanges();

                await botClient.SendTextMessageAsync(chatId: chatId, text: "Введите фамилию интересующего вас преподавателя", replyMarkup: TeachersWorkScheduleBackKeyboardMarkup);

                NGramSearch.Instance.PrecomputeNGrams(parser.GetTeachers() ?? throw new NullReferenceException("GetTeachers"), 2);

                foreach(Tuple<string, double> item in NGramSearch.Instance.FindMatch(args))
                    await botClient.SendTextMessageAsync(chatId: chatId, text: $"{item.Item1} {item.Item2}");
            });

            commandManager.AddMessageCommand(Mode.TeachersWorkSchedule, async (dbContext, chatId, messageId, user, args) => {

                foreach(Tuple<string, double> item in NGramSearch.Instance.FindMatch(args))
                    await botClient.SendTextMessageAsync(chatId: chatId, text: $"{item.Item1} {item.Item2}");

                return true;
            });
            #endregion

            #endregion

            commandManager.TrimExcess();

            Console.WriteLine("Запущен бот " + botClient.GetMeAsync().Result.FirstName + "\n");

            botClient.ReceiveAsync(
                HandleUpdateAsync,
                HandleError,
            new ReceiverOptions {
                AllowedUpdates = { },
#if DEBUG
                ThrowPendingUpdates = true
#else
                ThrowPendingUpdates = false
#endif
            },
            new CancellationTokenSource().Token
           ).Wait();
        }

        private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken) {
            string msg = Newtonsoft.Json.JsonConvert.SerializeObject(update) + "\n";
#if DEBUG
            Console.WriteLine(msg);
#endif
            Message? message = update.Message ?? update.EditedMessage ?? update.CallbackQuery?.Message;

            try {

                using(ScheduleDbContext dbContext = new()) {
                    if(message is not null) {
                        if(message.From is null) return;

                        TelegramUser? user = dbContext.TelegramUsers.Include(u => u.ScheduleProfile).Include(u => u.Notifications).FirstOrDefault(u => u.ChatID == message.Chat.Id);

                        if(user is null) {
                            ScheduleProfile scheduleProfile = new();
                            dbContext.ScheduleProfile.Add(scheduleProfile);

                            Notifications notifications = new();
                            dbContext.Notifications.Add(notifications);
                            dbContext.SaveChanges();

                            user = new() {
                                ChatID = message.Chat.Id,
                                Username = message.From.Username,
                                FirstName = message.From.FirstName,
                                LastName = message.From.LastName,
                                ScheduleProfile = scheduleProfile,
                                Notifications = notifications
                            };

                            dbContext.TelegramUsers.Add(user);

                            notifications.TelegramUser = scheduleProfile.TelegramUser = user;

                            dbContext.SaveChanges();
                        }

                        switch(update.Type) {
                            case Telegram.Bot.Types.Enums.UpdateType.Message:
                            case Telegram.Bot.Types.Enums.UpdateType.EditedMessage:
                                if(message.Text is null) return;

                                await commandManager.OnMessageAsync(dbContext, message.Chat, message.MessageId, message.Text, user);
                                dbContext.MessageLog.Add(new() { Message = message.Text, TelegramUser = user });
                                break;

                            case Telegram.Bot.Types.Enums.UpdateType.CallbackQuery:
                                if(update.CallbackQuery?.Data is null || message.Text is null) return;

                                await commandManager.OnCallbackAsync(dbContext, message.Chat, message.MessageId, update.CallbackQuery.Data, message.Text, user);
                                dbContext.MessageLog.Add(new() { Message = update.CallbackQuery.Data, TelegramUser = user });
                                break;
                        }

                        user.LastAppeal = user.ScheduleProfile.LastAppeal = DateTime.UtcNow;
                        user.TodayRequests++;
                        user.TotalRequests++;

                        dbContext.SaveChanges();
                    } else {
                        if(update.Type == Telegram.Bot.Types.Enums.UpdateType.InlineQuery) {
                            await InlineQuery(dbContext, botClient, update);
                            return;
                        }
                    }
                }
            } catch(Telegram.Bot.Exceptions.ApiRequestException e) {
                Console.Error.WriteLine($"{msg}\n{new('-', 25)}\n{e.Message}");
            } catch(Exception e) {
                throw new Exception($"{msg}\n{new('-', 25)}\n{e.Message}");
            } finally {
                GC.Collect();
            }
        }

        private Task HandleError(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken) => Task.CompletedTask;

        private async Task DeleteTempMessage(TelegramUser user, int? messageId) {
            try {
                if(user.RequestingMessageID is not null) {
                    await botClient.DeleteMessageAsync(chatId: user.ChatID, messageId: (int)user.RequestingMessageID);
                    user.RequestingMessageID = null;
                }

                if(messageId is not null)
                    await botClient.DeleteMessageAsync(chatId: user.ChatID, messageId: (int)messageId);

            } catch(Exception) { }
        }
    }
}