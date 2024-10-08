﻿using Core.DB;
using Core.DB.Entity;

using Telegram.Bot.Types.ReplyMarkups;

namespace Core.Bot.Commands {
    public static class DefaultMessage {
        private static readonly UserCommands commands = UserCommands.Instance;

        public static ReplyKeyboardMarkup GetTermsKeyboardMarkup(ScheduleDbContext dbContext, string StudentID) {
            List<KeyboardButton[]> TermsKeyboardMarkup = [];

            int[] terms = [.. dbContext.Progresses.Where(i => i.StudentID == StudentID).Select(i => i.Term).Distinct().OrderBy(i => i)];
            for(int i = 0; i < terms.Length; i++)
                TermsKeyboardMarkup.Add([$"{terms[i]} {commands.Message["Semester"]}", i + 1 < terms.Length ? $"{terms[++i]} {commands.Message["Semester"]}" : ""]);

            TermsKeyboardMarkup.Add([commands.Message["Back"]]);

            return new(TermsKeyboardMarkup) { ResizeKeyboard = true };
        }

        public static ReplyKeyboardMarkup GetProfileKeyboardMarkup(TelegramUser user) {
            List<KeyboardButton[]> ProfileKeyboardMarkup = [];

            if(user.IsOwner()) {
                if(!user.IsSupergroup()) {
                    ProfileKeyboardMarkup.AddRange([[$"{commands.Message["GroupNumber"]}:\n{user.ScheduleProfile.Group}", $"{commands.Message["StudentIDNumber"]}:\n{user.ScheduleProfile.StudentID}"],
                                                        [commands.Message["GetProfileLink"]]
                                                     ]);
                } else {
                    ProfileKeyboardMarkup.Add([$"{commands.Message["GroupNumber"]}:\n{user.ScheduleProfile.Group}"]);
                }
            } else {
                ProfileKeyboardMarkup.Add([commands.Message["ResetProfileLink"]]);
            }

            ProfileKeyboardMarkup.AddRange([
                [commands.Message["Settings"]],
                [commands.Message["Back"]]
            ]);

            return new(ProfileKeyboardMarkup) { ResizeKeyboard = true };
        }

        public static ReplyKeyboardMarkup GetMainKeyboardMarkup(TelegramUser user) {
            List<KeyboardButton[]> ProfileKeyboardMarkup = new([
                [commands.Message["Today"], commands.Message["Tomorrow"]],
                [commands.Message["ByDays"], commands.Message["ForAWeek"]],
                [commands.Message["Corps"], commands.Message["Schedule"]],
                [commands.Message["Other"]]
            ]);

            if(user.IsAdmin) {
                ProfileKeyboardMarkup.Add([commands.Message["AdminPanel"]]);
            }

            return new(ProfileKeyboardMarkup) { ResizeKeyboard = true };
        }

        public static ReplyKeyboardMarkup GetOtherKeyboardMarkup(TelegramUser user) {
            List<KeyboardButton[]> ProfileKeyboardMarkup = new([
                [commands.Message["Profile"]],
            ]);

            if(!user.IsSupergroup()) {
                ProfileKeyboardMarkup.AddRange([[commands.Message["AcademicPerformance"], commands.Message["GroupList"]]]);
            } else {
                ProfileKeyboardMarkup.Add([commands.Message["GroupList"]]);
            }

            ProfileKeyboardMarkup.Add([commands.Message["Back"]]);

            return new(ProfileKeyboardMarkup) { ResizeKeyboard = true };
        }

        public static ReplyKeyboardMarkup GetSettingsKeyboardMarkup(TelegramUser user) {
            string notificationEnabled = user.Settings.NotificationEnabled ? "\U0001f7e2" : "🔴";

            bool _teacherLincsEnabled = user.Settings.TeacherLincsEnabled;
            string teacherLincsEnabled = _teacherLincsEnabled ? "\U0001f7e2" : "🔴";

            bool _displayingGroupList = user.Settings.DisplayingGroupList;
            string displayingGroupList = _displayingGroupList ? "\U0001f7e2" : "🔴";

            List<KeyboardButton[]> ProfileKeyboardMarkup = new([
                [$"{notificationEnabled} {commands.Message["Notifications"]} {notificationEnabled}"],
                [$"{teacherLincsEnabled} {commands.Message["TeacherLincsEnabled"]} {teacherLincsEnabled} \n({(_teacherLincsEnabled ? "Выключить" : "Включить")})"]
            ]);

            if(!user.IsSupergroup()) {
                ProfileKeyboardMarkup.Add([$"{displayingGroupList} {commands.Message["DisplayingGroupList"]} {displayingGroupList} \n({(_displayingGroupList ? "Выключить" : "Включить")})"]);
            }

            ProfileKeyboardMarkup.Add([commands.Message["Back"]]);
            return new(ProfileKeyboardMarkup) { ResizeKeyboard = true };
        }

        public static ReplyKeyboardMarkup GetCorpsKeyboardMarkup() {
            List<KeyboardButton[]> ProfileKeyboardMarkup = [
                [commands.Corps[0].text]
            ];

            for(int i = 0; i < 3; i++) {
                List<KeyboardButton> keyboardButtonsLine = [];
                for(int j = 0; j < 5; j++)
                    keyboardButtonsLine.Add(commands.Corps[1 + i * 5 + j].text);

                ProfileKeyboardMarkup.Add([.. keyboardButtonsLine]);
            }

            for(int i = 16; i < commands.Corps.Length; i++)
                ProfileKeyboardMarkup.Add([commands.Corps[i].text]);

            ProfileKeyboardMarkup.AddRange([[commands.College.text], [commands.Message["Back"]]]);

            return new(ProfileKeyboardMarkup) { ResizeKeyboard = true };
        }

        public static ReplyKeyboardMarkup GetTeacherWorkScheduleSelectedKeyboardMarkup(string teacher) {
            List<KeyboardButton[]> KeyboardMarkup = [
                [$"{commands.Message["CurrentTeacher"]}:\n{teacher}"],
                [commands.Message["Today"], commands.Message["Tomorrow"]],
                [commands.Message["ByDays"], commands.Message["ForAWeek"]],
                [commands.Message["WorkScheduleBack"]]
            ];

            return new(KeyboardMarkup) { ResizeKeyboard = true };
        }

        public static ReplyKeyboardMarkup GetClassroomWorkScheduleSelectedKeyboardMarkup(string teacher) {
            List<KeyboardButton[]> KeyboardMarkup = [
                [$"{commands.Message["CurrentClassroom"]}:\n{teacher}"],
                [commands.Message["Today"], commands.Message["Tomorrow"]],
                [commands.Message["ByDays"], commands.Message["ForAWeek"]],
                [commands.Message["WorkScheduleBack"]]
            ];

            return new(KeyboardMarkup) { ResizeKeyboard = true };
        }
    }
}