using System.Collections.Generic;
using DSharpPlus.Net.Abstractions;
using Newtonsoft.Json;

namespace DSharpPlus.Entities;

internal class DiscordInteractionApplicationCommandCallbackData
{
    [JsonProperty("tts", NullValueHandling = NullValueHandling.Ignore)]
    public bool? IsTTS { get; internal set; }

    [JsonProperty("custom_id", NullValueHandling = NullValueHandling.Ignore)]
    public string? CustomId { get; internal set; }

    [JsonProperty("title", NullValueHandling = NullValueHandling.Ignore)]
    public string? Title { get; internal set; }

    [JsonProperty("content", NullValueHandling = NullValueHandling.Ignore)]
    public string? Content { get; internal set; }

    [JsonProperty("embeds", NullValueHandling = NullValueHandling.Ignore)]
    public IEnumerable<DiscordEmbed>? Embeds { get; internal set; }

    [JsonProperty("allowed_mentions", NullValueHandling = NullValueHandling.Ignore)]
    public DiscordMentions? Mentions { get; internal set; }

    [JsonProperty("flags", NullValueHandling = NullValueHandling.Ignore)]
    public DiscordMessageFlags? Flags { get; internal set; }

    [JsonProperty("components", NullValueHandling = NullValueHandling.Ignore)]
    public IReadOnlyList<DiscordComponent>? Components { get; internal set; }

    [JsonProperty("choices")]
    public IReadOnlyList<DiscordAutoCompleteChoice>? Choices { get; internal set; }

    [JsonProperty("poll", NullValueHandling = NullValueHandling.Ignore)]
    public PollCreatePayload? Poll { get; internal set; }
}
