using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using SParser;
using System.Threading.Tasks;
using System;
using DSharpPlus.Entities;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using DSharpPlus.Interactivity.Extensions;

namespace Steamer
{
    class Commands : BaseCommandModule
    {
        [Command("YesOrNo")]
        [Aliases("YN", "Y", "N")]
        public async Task YesOrNo(CommandContext ctx)
        {
            YesOrNoAnswer answer;
            using (HttpClient client = new HttpClient(new HttpClientHandler { AllowAutoRedirect = false }))
            {
                answer = await client.GetFromJsonAsync<YesOrNoAnswer>("https://yesno.wtf/api");
            }

            await ctx.Channel.SendMessageAsync(
                new DiscordEmbedBuilder
                {
                    Title = answer.Answer.ToUpper(),
                    ImageUrl = answer.Image
                }
            ).ConfigureAwait(false);
        }

        [Command("Error")]
        [Aliases("E")]
        public async Task Cat(CommandContext ctx, int error_number)
        {
            await ctx.Channel.SendMessageAsync(
                new DiscordEmbedBuilder
                {
                    Description = "Если картинки внизу нет, то, возможно, ошибки с таким номером тоже",
                    ImageUrl = $"https://http.cat/{error_number}"
                }
            ).ConfigureAwait(false);
        }

        [Command("Weather")]
        [Aliases("W")]
        [Cooldown(1, 5, CooldownBucketType.Guild)]
        public async Task Weather(CommandContext ctx, string city)
        {
            try
            {
                Forecast forecast;
                using (HttpClient client = new HttpClient(new HttpClientHandler { AllowAutoRedirect = false }))
                {
                    forecast = await client.GetFromJsonAsync<Forecast>(
                        $"http://api.openweathermap.org/data/2.5/weather?q={city}&appid=ef9e9aa18a86a409611737d47560695b&lang=ru"
                    );
                }

                var embed = new DiscordEmbedBuilder
                {
                    Title = $"Прогноз погоды: {city}",
                    Color = DiscordColor.Blue,
                    Footer = new DiscordEmbedBuilder.EmbedFooter
                    {
                        Text = $"Спасибо за вызов команды, {ctx.Message.Author.Username}",
                        IconUrl = ctx.Message.Author.AvatarUrl
                    }
                }
                     .AddField("Страна", forecast.Country.CountryName)
                     .AddField("Сводка", forecast.Weather[0].Main + " | " + forecast.Weather[0].Description, true)
                     .AddField("Температура",
                     $"{forecast.Summary["temp"]}K или {forecast.Summary["temp"] - 273}°C или {CommandUtils.ConvertToFahrenheit(forecast.Summary["temp"] - 273)}F"
                     )
                     .AddField("Ощущается как",
                     $"{forecast.Summary["feels_like"]}K или {forecast.Summary["feels_like"] - 273}°C или {CommandUtils.ConvertToFahrenheit(forecast.Summary["feels_like"] - 273)}F"
                     )
                     .AddField("Влажность", forecast.Summary["humidity"] + "%", true)
                     .AddField("Давление", forecast.Summary["pressure"] + " гПа", true)
                     .AddField("Ветер", forecast.Wind["speed"] + " м/с", true)
                     .AddField("Облака", forecast.Clouds["all"] + "%", true)
                     .AddField("Рассвет", CommandUtils.ConvertFromUnixToString(forecast.Country.Sunrise), true)
                     .AddField("Закат", CommandUtils.ConvertFromUnixToString(forecast.Country.Sunset), true);

                await ctx.Channel.SendMessageAsync(embed: embed)
                    .ConfigureAwait(false);
            }
            catch (HttpRequestException)
            {
                await ctx.Channel.SendMessageAsync(
                    CommandUtils.GetErrorEmbed("указан несуществующий город или сервер временно недоступен")
                ).ConfigureAwait(false);
            }
        }

        [Command("Steam")]
        [Aliases("S")]
        [Cooldown(1, 5, CooldownBucketType.Guild)]
        public async Task Steam(CommandContext ctx, long id)
        {
            var inter = ctx.Client.GetInteractivity();

            try
            {
                SUser user = await Parser.GetUserData(id);

                var embed = new DiscordEmbedBuilder
                {
                    Title = $"Информация о пользователе {WebUtility.HtmlDecode(user.Name)}",
                    Description = string.Join(" | ", user.Summary),
                    Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail
                    {
                        Url = user.AvatarUrl
                    },
                    Color = ctx.Member.Color,
                    Footer = new DiscordEmbedBuilder.EmbedFooter
                    {
                        IconUrl = ctx.Member.AvatarUrl,
                        Text = $"Спасибо за вызов команды, {ctx.Member.DisplayName}"
                    }
                }
                .AddField("Полное имя", WebUtility.HtmlDecode(user.Name), true)
                .AddField("Друзья", $"{(user.FriendsCount.HasValue ? user.FriendsCount.ToString() : "Недоступны")}", true)
                .AddField("Стоимость", user.AccountPrice, true)
                .AddField("Идентификаторы", CommandUtils.BuildStringFromDict(user.Ids, DictTypes.Ids))
                .AddField("Платформы", CommandUtils.BuildStringFromDict(user.Platforms, DictTypes.Platforms))
                .AddField("Блокировки", CommandUtils.BuildStringFromDict(user.Prohibitions, DictTypes.Bans))
                .AddField("Игры", CommandUtils.BuildStringFromDict(user.Games, DictTypes.Games))
                .AddField("Дополнительно", "При отсутствии определённой информации нажмите на знак вопроса внизу");

                var message = await ctx.Channel.SendMessageAsync(embed: embed)
                    .ConfigureAwait(false);

                await message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":question:"))
                    .ConfigureAwait(false);

                if(!(await inter.WaitForReactionAsync(
                    (msg) => msg.Message == message
                    && msg.User == ctx.User
                    && msg.Emoji.Equals(DiscordEmoji.FromName(ctx.Client, ":question:"))
                ).ConfigureAwait(false)).TimedOut)
                    await ctx.Channel.SendMessageAsync(Properties.Resources.steam_command_help)
                        .ConfigureAwait(false);
            }
            catch (SParserExceptions.UserNotFoundException)
            {
                await ctx.Channel.SendMessageAsync(
                    CommandUtils.GetErrorEmbed($"указанный пользователь не найден")
                ).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                await ctx.Channel.SendMessageAsync(
                    e.StackTrace + " " + e.Message
                ).ConfigureAwait(false);
            }
        }
    }
}
