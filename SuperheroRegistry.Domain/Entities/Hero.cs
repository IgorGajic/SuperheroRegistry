using SuperheroRegistry.Domain.Enums;
using SuperheroRegistry.Domain.Exceptions;
using SuperheroRegistry.Domain.Entities;

public class Hero : BaseEntity
{
    private static readonly string[] ForbiddenPhrases =
        ["tragic past", "chosen one", "mysterious organization"];

    public string Codename { get; set; }
    public string OriginStory { get; set; }
    public Race Race { get; set; }
    public Alignment Alignment { get; set; }
    public HeroStatus Status { get; private set; }
    public List<Power> Powers { get; set; }
    private const int MinimumOriginStoryLength = 30;

    public Hero(string codename, string originStory, Race race, Alignment alignment)
    {
        Codename = codename;
        OriginStory = originStory;
        Race = race;
        Alignment = alignment;

        Powers = new List<Power>();
        Status = HeroStatus.Draft;
    }


    public void Register()
    {
        if(Status != HeroStatus.Draft)
            throw new DomainException("Only heroes in draft status can be registered.");
        
        if (string.IsNullOrWhiteSpace(Codename))
            throw new DomainException("Codename is required.");

        if (string.IsNullOrWhiteSpace(OriginStory) || OriginStory.Length < MinimumOriginStoryLength)
            throw new DomainException($"Origin story must be at least '{MinimumOriginStoryLength}' characters.");

        foreach (var phrase in ForbiddenPhrases)
            if (OriginStory.Contains(phrase, StringComparison.OrdinalIgnoreCase))
                throw new DomainException($"Origin story contains forbidden phrase: '{phrase}'.");

        if (Powers == null || Powers.Count == 0)
            throw new DomainException("Hero must have at least one power to be registered.");

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

        if(Powers.Count == 1 && Status == HeroStatus.Registered)
        {
            throw new DomainException("Registered hero must have at least 1 power.");
        }

        for (int i = 0; i < Powers.Count; i++)
        {
            if (Powers[i].Id == powerId)
            {
                Powers.RemoveAt(i);
                return true;
            }
        }
        return false;
    }
}