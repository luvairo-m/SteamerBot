using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Steamer
{
    internal static class CommandUtils
    {
        private static IDictionary<DictTypes, string[]> emojies = new Dictionary<DictTypes, string[]>()
        {
            { DictTypes.Games, new string[] { ":video_game:" } },
            { DictTypes.Platforms, new string[] { ":computer:" } },
            { DictTypes.Bans, new string[] { ":white_check_mark:", ":no_entry:" } },
            { DictTypes.Ids, new string[] { ":gear:" } }
        };

        public static DiscordEmbedBuilder GetErrorEmbed(string error) =>
            new DiscordEmbedBuilder
            {
                Title = "Ошибка выполнения команды",
                Description = $"***Сводка***: {error}",
                Color = DiscordColor.Red
            };

        public static double ConvertToFahrenheit(double number) =>
            1.8 * number + 32;

        public static string ConvertFromUnixToString(int unix) =>
            new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(unix).ToLongTimeString();

        public static string BuildStringFromDict(IDictionary<string, string> dict, DictTypes type)
        {
            if (dict is null)
                return "Не найдены (смотрите след. страницу)";

            StringBuilder result = new StringBuilder();

            foreach(KeyValuePair<string, string> pair in dict)
            {
                result.Append($"{(pair.Value.ToLower().Equals("в хороших отношениях") ? emojies[type][0] : emojies[type][emojies[type].Length - 1])}" +
                    $" {pair.Key}: {pair.Value}\n");
            }

            return result.ToString();
        }
    }
}
