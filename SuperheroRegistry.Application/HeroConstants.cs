namespace SuperheroRegistry.Application
{
    public static class HeroConstants
    {
        public static readonly string[] ForbiddenPhrases =
        ["tragic past", "chosen one", "mysterious organization"];

        public const int MinimumOriginStoryLength = 30;
        public const int MaximumOriginStoryLength = 100;

        public const int MinimumCodenameLength = 1;
        public const int MaximumCodenameLength = 30;

        public const int MinimumPowerCountForRegisteringHeroes = 1;
    }
}
