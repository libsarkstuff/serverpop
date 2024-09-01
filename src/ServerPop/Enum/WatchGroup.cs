using Discord.Interactions;

namespace ServerPop.Enum;

public enum WatchGroup
{
    None = 0,
    [ChoiceDisplay("tribe")]
    Tribe,
    [ChoiceDisplay("enemy")]
    Enemy,
    [ChoiceDisplay("watched")]
    Watched
}
