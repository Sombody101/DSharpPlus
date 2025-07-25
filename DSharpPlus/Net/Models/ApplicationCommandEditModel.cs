using System;
using System.Collections.Generic;
using DSharpPlus.Entities;

namespace DSharpPlus.Net.Models;

public class ApplicationCommandEditModel
{
    /// <summary>
    /// Sets the command's new name.
    /// </summary>
    public Optional<string> Name
    {
        internal get => this.name;
        set
        {
            if (value.Value.Length > 32)
            {
                throw new ArgumentException("Application command name cannot exceed 32 characters.", nameof(value));
            }

            this.name = value;
        }
    }

    private Optional<string> name;

    /// <summary>
    /// Sets the command's new description
    /// </summary>
    public Optional<string> Description
    {
        internal get => this.description;
        set
        {
            if (value.Value.Length > 100)
            {
                throw new ArgumentException("Application command description cannot exceed 100 characters.", nameof(value));
            }

            this.description = value;
        }
    }

    private Optional<string> description;

    /// <summary>
    /// Sets the command's new options.
    /// </summary>
    public Optional<IReadOnlyList<DiscordApplicationCommandOption>> Options { internal get; set; }

    /// <summary>
    /// Sets whether the command is enabled by default when the application is added to a guild.
    /// </summary>
    public Optional<bool?> DefaultPermission { internal get; set; }

    /// <summary>
    /// Sets whether the command can be invoked in DMs.
    /// </summary>
    public Optional<bool> AllowDMUsage { internal get; set; }

    /// <summary>
    /// A dictionary of localized names mapped by locale.
    /// </summary>
    public IReadOnlyDictionary<string, string>? NameLocalizations { internal get; set; }

    /// <summary>
    /// A dictionary of localized descriptions mapped by locale.
    /// </summary>
    public IReadOnlyDictionary<string, string>? DescriptionLocalizations { internal get; set; }

    /// <summary>
    /// Sets the requisite permissions for the command.
    /// </summary>
    public Optional<DiscordPermissions?> DefaultMemberPermissions { internal get; set; }

    /// <summary>
    /// Sets whether this command is age restricted.
    /// </summary>
    public Optional<bool?> NSFW { internal get; set; }

    /// <summary>
    /// Interaction context(s) where the command can be used.
    /// </summary>
    public Optional<IEnumerable<DiscordInteractionContextType?>> AllowedContexts { internal get; set; }

    /// <summary>
    /// Installation context(s) where the command is available.
    /// </summary>
    public Optional<IEnumerable<DiscordApplicationIntegrationType?>> IntegrationTypes { internal get; set; }
}
