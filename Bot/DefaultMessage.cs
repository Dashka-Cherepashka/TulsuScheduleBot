﻿using System.Text.RegularExpressions;

using ScheduleBot.DB.Entity;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace ScheduleBot.Bot {
    public partial class TelegramBot {
        #region ReplyKeyboardMarkup
        public static readonly ReplyKeyboardMarkup MainKeyboardMarkup = new(new[] {
                            new KeyboardButton[] { Constants.RK_Today, Constants.RK_Tomorrow },
                            new KeyboardButton[] { Constants.RK_ByDays, Constants.RK_ForAWeek },
                            new KeyboardButton[] { Constants.RK_Exam },
                            new KeyboardButton[] { Constants.RK_Other }
                        }) { ResizeKeyboard = true };

        private readonly ReplyKeyboardMarkup AdditionalKeyboardMarkup = new(new[] {
                            new KeyboardButton[] { Constants.RK_Profile },
                            new KeyboardButton[] { Constants.RK_AcademicPerformance },
                            new KeyboardButton[] { Constants.RK_Corps },
                             new KeyboardButton[] { Constants.RK_Back }
                        }) { ResizeKeyboard = true };

        private readonly ReplyKeyboardMarkup ExamKeyboardMarkup = new(new[] {
                            new KeyboardButton[] { Constants.RK_NextExam, Constants.RK_AllExams },
                            new KeyboardButton[] { Constants.RK_Back }
                        }) { ResizeKeyboard = true };

        private readonly ReplyKeyboardMarkup DaysKeyboardMarkup = new(new[] {
                            new KeyboardButton[] { Constants.RK_Monday, Constants.RK_Tuesday },
                            new KeyboardButton[] { Constants.RK_Wednesday, Constants.RK_Thursday },
                            new KeyboardButton[] { Constants.RK_Friday, Constants.RK_Saturday },
                            new KeyboardButton[] { Constants.RK_Back }
                        }) { ResizeKeyboard = true };

        private readonly ReplyKeyboardMarkup CancelKeyboardMarkup = new(Constants.RK_Cancel) { ResizeKeyboard = true };

        private readonly ReplyKeyboardMarkup ResetProfileLinkKeyboardMarkup = new(new KeyboardButton[] { Constants.RK_Reset, Constants.RK_Cancel }) { ResizeKeyboard = true };

        private readonly ReplyKeyboardMarkup WeekKeyboardMarkup = new(new[] {
                            new KeyboardButton[] { Constants.RK_ThisWeek, Constants.RK_NextWeek },
                            new KeyboardButton[] { Constants.RK_Back }
                        }) { ResizeKeyboard = true };

        private readonly ReplyKeyboardMarkup CorpsKeyboardMarkup = new(new[] {
                            new KeyboardButton[] { Constants.RK_MainCorps.text },
                            new KeyboardButton[] { Constants.RK_1Corps.text, Constants.RK_2Corps.text, Constants.RK_3Corps.text, Constants.RK_4Corps.text, Constants.RK_5Corps.text },
                            new KeyboardButton[] { Constants.RK_6Corps.text, Constants.RK_7Corps.text, Constants.RK_8Corps.text, Constants.RK_9Corps.text, Constants.RK_10Corps.text },
                            new KeyboardButton[] { Constants.RK_11Corps.text, Constants.RK_12Corps.text, Constants.RK_13Corps.text, Constants.LaboratoryCorps.text, Constants.RK_FOC.text },
                            new KeyboardButton[] { Constants.RK_SanatoriumDispensary.text },
                            new KeyboardButton[] { Constants.RK_Stadium.text },
                            new KeyboardButton[] { Constants.RK_PoolOnBoldin.text },
                            new KeyboardButton[] { Constants.RK_SportsComplexOnBoldin.text },
                            new KeyboardButton[] { Constants.RK_TechnicalCollege },
                            new KeyboardButton[] { Constants.RK_Back }
                        }) { ResizeKeyboard = true };
        #endregion

        private ReplyKeyboardMarkup GetTermsKeyboardMarkup(string StudentID) {
            List<KeyboardButton[]> TermsKeyboardMarkup = new();

            var terms = dbContext.Progresses.Where(i => i.StudentID == StudentID && i.Mark != null).Select(i => i.Term).Distinct().OrderBy(i => i).ToArray();
            for(int i = 0; i < terms.Length; i++)
                TermsKeyboardMarkup.Add(new KeyboardButton[] { $"{terms[i]} {Constants.RK_Semester}", i + 1 < terms.Length ? $"{terms[++i]} {Constants.RK_Semester}" : "" });

            TermsKeyboardMarkup.Add(new KeyboardButton[] { Constants.RK_Back });

            return new(TermsKeyboardMarkup) { ResizeKeyboard = true };
        }

        private ReplyKeyboardMarkup GetProfileKeyboardMarkup(TelegramUser user) {
            List<KeyboardButton[]> ProfileKeyboardMarkup = new();

            if(user.ScheduleProfile.OwnerID == user.ChatID) {
                ProfileKeyboardMarkup.AddRange(new[] {  new KeyboardButton[] { $"{Constants.RK_GroupNumber}: {user.ScheduleProfile.Group}" },
                                                        new KeyboardButton[] { $"{Constants.RK_StudentIDNumber}: {user.ScheduleProfile.StudentID}" },
                                                        new KeyboardButton[] { Constants.RK_GetProfileLink }
                                                     });
            } else {
                ProfileKeyboardMarkup.Add(new KeyboardButton[] { Constants.RK_ResetProfileLink });
            }

            ProfileKeyboardMarkup.Add(new KeyboardButton[] { Constants.RK_Back });

            return new(ProfileKeyboardMarkup) { ResizeKeyboard = true };
        }
    }
}