using SuperheroRegistry.Domain.Enums;
using SuperheroRegistry.Domain.Exceptions;

namespace SuperheroRegistry.Domain.Entities
{
    public class Hero : BaseEntity
{
    private static readonly string[] ForbiddenPhrases =
        ["tragic past", "chosen one", "mysterious organization"];
    private const int MinimumOriginStoryLength = 30;
    public string UserId { get; set; }
    public string Codename { get; set; }
    public string OriginStory { get; set; }
    public Race Race { get; set; }
    public Alignment Alignment { get; set; }
    public HeroStatus Status { get; private set; }
    public List<Power> Powers { get; set; }


    public Hero(string codename, string originStory, Race race, Alignment alignment, string userId)
    {
        Codename = codename;
        OriginStory = originStory;
        Race = race;
        Alignment = alignment;
        UserId = userId;
        Powers = new List<Power>();
        Status = HeroStatus.Draft;
    }


    public void Register()
    {
        // Validations are performed in HeroService.ValidateHeroForRegistration
        // Here we just perform the state transition and forbidden phrase check

        if (string.IsNullOrWhiteSpace(OriginStory) || OriginStory.Length < MinimumOriginStoryLength)
            throw new DomainException($"Origin story must be at least '{MinimumOriginStoryLength}' characters.");

        foreach (var phrase in ForbiddenPhrases)
            if (OriginStory.Contains(phrase, StringComparison.OrdinalIgnoreCase))
                throw new DomainException($"Origin story contains forbidden phrase: '{phrase}'.");

        Status = HeroStatus.Registered;
    }

    public void Retire()
    {
        if(Status  != HeroStatus.Registered)
            throw new DomainException("Only registered heroes can be retired.");

        Status = HeroStatus.Retired;
    }

    public void AddPower(Power power)
    {
        if(power == null)
            throw new DomainException("Power cannot be null.");

        if (Status == HeroStatus.Retired)
            throw new DomainException("Cannot manage powers for retired heroes.");
        
        Powers.Add(power);
    }

    public bool RemovePower(int powerId)
    {
        if (Status == HeroStatus.Retired)
        {
            throw new DomainException("Cannot manage powers for retired heroes.");
        }

        if (Powers.Count == 1 && Status == HeroStatus.Registered)
        {
            throw new DomainException("Registered hero must have at least 1 power.");
        }

        var power = Powers.FirstOrDefault(p => p.Id == powerId);
        if (power == null)
            return false;

        Powers.Remove(power);
        return true;
    }
}
}