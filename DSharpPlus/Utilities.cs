using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using DSharpPlus.Net;
using Microsoft.Extensions.Logging;

namespace DSharpPlus;

/// <summary>
/// Various Discord-related utilities.
/// </summary>
public static partial class Utilities
{
    /// <summary>
    /// Gets the version of the library
    /// </summary>
    private static string VersionHeader { get; set; }

    internal static UTF8Encoding UTF8 { get; } = new UTF8Encoding(false);

    static Utilities()
    {
        Assembly assembly = typeof(DiscordClient).GetTypeInfo().Assembly;

        AssemblyInformationalVersionAttribute? informationalVersion = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
        string versionString = informationalVersion != null
            ? informationalVersion.InformationalVersion
            : assembly.GetName().Version!.ToString(3);

        VersionHeader = $"DiscordBot (https://github.com/DSharpPlus/DSharpPlus, v{versionString})";
    }

    internal static string GetApiBaseUri()
        => Endpoints.BASE_URI;

    internal static Uri GetApiUriFor(string path)
        => new($"{GetApiBaseUri()}{path}");

    internal static Uri GetApiUriFor(string path, string queryString)
        => new($"{GetApiBaseUri()}{path}{queryString}");

    internal static QueryUriBuilder GetApiUriBuilderFor(string path)
        => new($"{GetApiBaseUri()}{path}");

    internal static string GetUserAgent()
        => VersionHeader;

    internal static bool ContainsUserMentions(string message)
    {
        Regex regex = UserMentionRegex();
        return regex.IsMatch(message);
    }

    internal static bool ContainsNicknameMentions(string message)
    {
        Regex regex = NicknameMentionRegex();
        return regex.IsMatch(message);
    }

    internal static bool ContainsChannelMentions(string message)
    {
        Regex regex = ChannelMentionRegex();
        return regex.IsMatch(message);
    }

    internal static bool ContainsRoleMentions(string message)
    {
        Regex regex = RoleMentionRegex();
        return regex.IsMatch(message);
    }

    internal static bool ContainsEmojis(string message)
    {
        Regex regex = EmojiMentionRegex();
        return regex.IsMatch(message);
    }

    internal static IEnumerable<ulong> GetUserMentions(DiscordMessage message)
    {
        Regex regex = UserMentionRegex();
        MatchCollection matches = regex.Matches(message.Content);
        foreach (Match match in matches.Cast<Match>())
        {
            yield return ulong.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
        }
    }

    internal static IEnumerable<ulong> GetRoleMentions(DiscordMessage message)
    {
        Regex regex = RoleMentionRegex();
        MatchCollection matches = regex.Matches(message.Content);
        foreach (Match match in matches.Cast<Match>())
        {
            yield return ulong.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
        }
    }

    internal static IEnumerable<ulong> GetChannelMentions(DiscordMessage message) => GetChannelMentions(message.Content);

    internal static IEnumerable<ulong> GetChannelMentions(string messageContent)
    {
        Regex regex = ChannelMentionRegex();
        MatchCollection matches = regex.Matches(messageContent);
        foreach (Match match in matches.Cast<Match>())
        {
            yield return ulong.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
        }
    }

    internal static IEnumerable<ulong> GetEmojis(DiscordMessage message)
    {
        Regex regex = EmojiMentionRegex();
        MatchCollection matches = regex.Matches(message.Content);
        foreach (Match match in matches.Cast<Match>())
        {
            yield return ulong.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture);
        }
    }

    internal static bool IsValidSlashCommandName(string name)
    {
        Regex regex = SlashCommandNameRegex();
        return regex.IsMatch(name);
    }

    internal static bool HasMessageIntents(DiscordIntents intents)
        => (intents.HasIntent(DiscordIntents.GuildMessages) && intents.HasIntent(DiscordIntents.MessageContents)) || intents.HasIntent(DiscordIntents.DirectMessages);

    internal static bool HasReactionIntents(DiscordIntents intents)
        => intents.HasIntent(DiscordIntents.GuildMessageReactions) || intents.HasIntent(DiscordIntents.DirectMessageReactions);

    internal static bool HasTypingIntents(DiscordIntents intents)
        => intents.HasIntent(DiscordIntents.GuildMessageTyping) || intents.HasIntent(DiscordIntents.DirectMessageTyping);

    internal static bool IsTextableChannel(DiscordChannel channel)
        => channel.Type switch
        {
            DiscordChannelType.Text => true,
            DiscordChannelType.Voice => true,
            DiscordChannelType.Group => true,
            DiscordChannelType.Private => true,
            DiscordChannelType.PublicThread => true,
            DiscordChannelType.PrivateThread => true,
            DiscordChannelType.NewsThread => true,
            DiscordChannelType.News => true,
            DiscordChannelType.Stage => true,
            _ => false,
        };

    // https://discord.com/developers/docs/topics/gateway#sharding-sharding-formula
    /// <summary>
    /// Gets a shard id from a guild id and total shard count.
    /// </summary>
    /// <param name="guildId">The guild id the shard is on.</param>
    /// <param name="shardCount">The total amount of shards.</param>
    /// <returns>The shard id.</returns>
    public static int GetShardId(ulong guildId, int shardCount)
        => (int)((guildId >> 22) % (ulong)shardCount);

    /// <summary>
    /// Helper method to create a <see cref="DateTimeOffset"/> from Unix time seconds for targets that do not support this natively.
    /// </summary>
    /// <param name="unixTime">Unix time seconds to convert.</param>
    /// <param name="shouldThrow">Whether the method should throw on failure. Defaults to true.</param>
    /// <returns>Calculated <see cref="DateTimeOffset"/>.</returns>
    public static DateTimeOffset GetDateTimeOffset(long unixTime, bool shouldThrow = true)
    {
        try
        {
            return DateTimeOffset.FromUnixTimeSeconds(unixTime);
        }
        catch (Exception)
        {
            if (shouldThrow)
            {
                throw;
            }

            return DateTimeOffset.MinValue;
        }
    }

    /// <summary>
    /// Helper method to create a <see cref="DateTimeOffset"/> from Unix time milliseconds for targets that do not support this natively.
    /// </summary>
    /// <param name="unixTime">Unix time milliseconds to convert.</param>
    /// <param name="shouldThrow">Whether the method should throw on failure. Defaults to true.</param>
    /// <returns>Calculated <see cref="DateTimeOffset"/>.</returns>
    public static DateTimeOffset GetDateTimeOffsetFromMilliseconds(long unixTime, bool shouldThrow = true)
    {
        try
        {
            return DateTimeOffset.FromUnixTimeMilliseconds(unixTime);
        }
        catch (Exception)
        {
            if (shouldThrow)
            {
                throw;
            }

            return DateTimeOffset.MinValue;
        }
    }

    /// <summary>
    /// Helper method to calculate Unix time seconds from a <see cref="DateTimeOffset"/> for targets that do not support this natively.
    /// </summary>
    /// <param name="dto"><see cref="DateTimeOffset"/> to calculate Unix time for.</param>
    /// <returns>Calculated Unix time.</returns>
    public static long GetUnixTime(DateTimeOffset dto)
        => dto.ToUnixTimeMilliseconds();

    /// <summary>
    /// Computes a timestamp from a given snowflake.
    /// </summary>
    /// <param name="snowflake">Snowflake to compute a timestamp from.</param>
    /// <returns>Computed timestamp.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DateTimeOffset GetSnowflakeTime(this ulong snowflake)
        => DiscordClient.discordEpoch.AddMilliseconds(snowflake >> 22);

    /// <summary>
    /// Checks whether this string contains given characters.
    /// </summary>
    /// <param name="str">String to check.</param>
    /// <param name="characters">Characters to check for.</param>
    /// <returns>Whether the string contained these characters.</returns>
    public static bool Contains(this string str, params char[] characters)
        => Array.Exists(characters, str.Contains);

    internal static void LogTaskFault(this Task task, ILogger logger, LogLevel level, EventId eventId, string message)
    {
        ArgumentNullException.ThrowIfNull(task);
        if (logger == null)
        {
            return;
        }

        task.ContinueWith(t => logger.Log(level, eventId, t.Exception, "{Message}", message), TaskContinuationOptions.OnlyOnFaulted);
    }

    internal static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> kvp, out TKey key, out TValue value)
    {
        key = kvp.Key;
        value = kvp.Value;
    }

    /// <summary>
    /// Creates a snowflake from a given <see cref="DateTimeOffset"/>. This can be used to provide "timestamps" for methods
    /// like <see cref="DiscordChannel.GetMessagesAfterAsync"/>.
    /// </summary>
    /// <param name="dateTimeOffset">DateTimeOffset to create a snowflake from.</param>
    /// <returns>Returns a snowflake representing the given date and time.</returns>
    public static ulong CreateSnowflake(DateTimeOffset dateTimeOffset)
    {
        long diff = dateTimeOffset.ToUnixTimeMilliseconds() - DiscordClient.discordEpoch.ToUnixTimeMilliseconds();
        return (ulong)diff << 22;
    }

    [GeneratedRegex("<@(\\d+)>", RegexOptions.ECMAScript)]
    private static partial Regex UserMentionRegex();

    [GeneratedRegex("<@!(\\d+)>", RegexOptions.ECMAScript)]
    private static partial Regex NicknameMentionRegex();

    [GeneratedRegex("<#(\\d+)>", RegexOptions.ECMAScript)]
    private static partial Regex ChannelMentionRegex();

    [GeneratedRegex("<@&(\\d+)>", RegexOptions.ECMAScript)]
    private static partial Regex RoleMentionRegex();

    [GeneratedRegex("<a?:(.*):(\\d+)>", RegexOptions.ECMAScript)]
    private static partial Regex EmojiMentionRegex();

    [GeneratedRegex("^[\\w-]{1,32}$")]
    private static partial Regex SlashCommandNameRegex();
}
