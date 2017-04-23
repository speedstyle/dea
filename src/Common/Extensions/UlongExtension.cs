﻿using Discord;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace DEA.Common.Extensions
{
    public static class UlongExtension
    {
        public static async Task<IUserMessage> DMAsync(this ulong userId, DiscordSocketClient client, string description, string title = null, Color color = default(Color))
        {
            var user = client.GetUser(userId);

            if (user != null)
            {
                try
                {
                    var channel = await user.CreateDMChannelAsync();

                    var builder = new EmbedBuilder()
                    {
                        Description = description,
                        Color = Config.Color()
                    };
                    if (title != null) builder.Title = title;
                    if (color.RawValue != default(Color).RawValue) builder.Color = color;

                    return await channel.SendMessageAsync(string.Empty, embed: builder);
                }
                catch { }
            }
            return null;
        }
    }
}
