using Xunit;
using Moq;
using SuperheroRegistry.Application;
using SuperheroRegistry.Application.Interfaces;
using SuperheroRegistry.Application.Services;
using SuperheroRegistry.Domain.Entities;
using SuperheroRegistry.Domain.Enums;
using SuperheroRegistry.Domain.Exceptions;
using SuperheroRegistry.Domain.Model;

namespace SuperheroRegistry.Tests
{
    public class HeroServiceTests
    {
        private readonly Mock<IHeroRepository> _mockRepository;
        private readonly HeroService _heroService;

        public HeroServiceTests()
        {
            _mockRepository = new Mock<IHeroRepository>();
            _heroService = new HeroService(_mockRepository.Object);
        }

        [Fact]
        public async Task CreateAsync_WithValidData_ShouldCreateHeroInDraftStatus()
        {
            var createHero = new CreateHero
            {
                Codename = "Batman",
                OriginStory = "A billionaire who witnessed a crime and vowed to fight justice",
                Race = Race.Human,
                Alignment = Alignment.LawfulGood,
                UserId = "user-123"
            };

            var createdHero = new Hero(
                createHero.Codename,
                createHero.OriginStory,
                createHero.Race,
                createHero.Alignment,
                createHero.UserId
            )
            { Id = 1 };

            _mockRepository.Setup(r => r.AddAsync(It.IsAny<Hero>()))
                .ReturnsAsync(createdHero);

            var result = await _heroService.CreateAsync(createHero);

            Assert.Equal("Batman", result.Codename);
            Assert.Equal(HeroStatus.Draft, result.Status);
            Assert.Equal("user-123", result.UserId);
            Assert.Empty(result.Powers);
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<Hero>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ShouldInitializeWithEmptyPowers()
        {
            var createHero = new CreateHero
            {
                Codename = "Superman",
                OriginStory = "An alien from distant planet with extraordinary powers and abilities",
                Race = Race.Human,
                Alignment = Alignment.LawfulGood,
                UserId = "user-456"
            };

            var createdHero = new Hero(
                createHero.Codename,
                createHero.OriginStory,
                createHero.Race,
                createHero.Alignment,
                createHero.UserId
            )
            { Id = 2 };

            _mockRepository.Setup(r => r.AddAsync(It.IsAny<Hero>()))
                .ReturnsAsync(createdHero);

            var result = await _heroService.CreateAsync(createHero);

            Assert.NotNull(result.Powers);
            Assert.Empty(result.Powers);
        }

        [Fact]
        public async Task RegisterAsync_WithDraftHeroAndValidConditions_ShouldTransitionToRegistered()
        {
            var hero = new Hero("Spider-Man", "A teenager bitten by a radioactive spider gained extraordinary powers", Race.Human, Alignment.LawfulGood, "user-789")
            {
                Id = 1,
                Status = HeroStatus.Draft
            };

            var power = new Power("Web Slinging", "Can shoot webs from wrists", 1) { Id = 1 };
            hero.Powers.Add(power);

            var registeredHero = new Hero(hero.Codename, hero.OriginStory, hero.Race, hero.Alignment, hero.UserId)
            {
                Id = hero.Id,
                Status = HeroStatus.Registered,
                Powers = hero.Powers
            };

            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Hero>()))
                .ReturnsAsync(registeredHero);

            var result = await _heroService.RegisterAsync(hero);

            Assert.Equal(HeroStatus.Registered, result.Status);
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Hero>()), Times.Once);
        }

        [Fact]
        public async Task RegisterAsync_WithNonDraftHero_ShouldThrowInvalidOperationException()
        {
            var hero = new Hero("Wolverine", "A mutant with healing abilities and adamantium claws", Race.Elf, Alignment.LawfulGood, "user-101")
            {
                Id = 2,
                Status = HeroStatus.Registered
            };

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _heroService.RegisterAsync(hero)
            );
        }

        [Fact]
        public async Task RegisterAsync_WithRetiredHero_ShouldThrowInvalidOperationException()
        {
            var hero = new Hero("Iron Man", "A genius inventor with a powered suit", Race.Human, Alignment.LawfulGood, "user-102")
            {
                Id = 3,
                Status = HeroStatus.Retired
            };

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _heroService.RegisterAsync(hero)
            );
        }

        [Fact]
        public async Task RegisterAsync_WithOriginStoryBelowMinimumLength_ShouldThrowDomainException()
        {
            var originStoryTooShort = "Short story"; // Less than 30 characters
            var hero = new Hero("Shorthand", originStoryTooShort, Race.Human, Alignment.LawfulGood, "user-103")
            {
                Id = 4,
                Status = HeroStatus.Draft
            };

            var power = new Power("Power", "Description", 4) { Id = 1 };
            hero.Powers.Add(power);

            var exception = await Assert.ThrowsAsync<DomainException>(
                () => _heroService.RegisterAsync(hero)
            );
            Assert.Contains("Origin story must be at least", exception.Message);
        }

        [Fact]
        public async Task RegisterAsync_WithOriginStoryAtMinimumLength_ShouldSucceed()
        {
            var originStory = "A".PadRight(HeroConstants.MinimumOriginStoryLength); // Exactly 30 characters
            var hero = new Hero("MinimumHero", originStory, Race.Human, Alignment.LawfulGood, "user-104")
            {
                Id = 5,
                Status = HeroStatus.Draft
            };

            var power = new Power("Power", "Description", 5) { Id = 1 };
            hero.Powers.Add(power);

            var registeredHero = new Hero(hero.Codename, hero.OriginStory, hero.Race, hero.Alignment, hero.UserId)
            {
                Id = hero.Id,
                Status = HeroStatus.Registered,
                Powers = hero.Powers
            };

            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Hero>()))
                .ReturnsAsync(registeredHero);

            var result = await _heroService.RegisterAsync(hero);
            Assert.Equal(HeroStatus.Registered, result.Status);
        }

        [Fact]
        public async Task RegisterAsync_WithEmptyOriginStory_ShouldThrowDomainException()
        {
            var hero = new Hero("EmptyStory", "", Race.Human, Alignment.LawfulGood, "user-105")
            {
                Id = 6,
                Status = HeroStatus.Draft
            };

            var power = new Power("Power", "Description", 6) { Id = 1 };
            hero.Powers.Add(power);

            await Assert.ThrowsAsync<DomainException>(
                () => _heroService.RegisterAsync(hero)
            );
        }

        [Fact]
        public async Task RegisterAsync_WithWhitespaceOnlyOriginStory_ShouldThrowDomainException()
        {
            var hero = new Hero("WhitespaceStory", "   ", Race.Human, Alignment.LawfulGood, "user-106")
            {
                Id = 7,
                Status = HeroStatus.Draft
            };

            var power = new Power("Power", "Description", 7) { Id = 1 };
            hero.Powers.Add(power);

            await Assert.ThrowsAsync<DomainException>(
                () => _heroService.RegisterAsync(hero)
            );
        }

        [Theory]
        [InlineData("tragic past")]
        [InlineData("Tragic Past")]
        [InlineData("TRAGIC PAST")]
        [InlineData("chosen one")]
        [InlineData("Chosen One")]
        [InlineData("CHOSEN ONE")]
        [InlineData("mysterious organization")]
        [InlineData("Mysterious Organization")]
        [InlineData("MYSTERIOUS ORGANIZATION")]
        public async Task RegisterAsync_WithForbiddenPhrase_ShouldThrowDomainException(string forbiddenPhrase)
        {
            var originStoryWithForbidden = $"This hero has a {forbiddenPhrase} that shaped their destiny";
            var hero = new Hero("ForbiddenHero", originStoryWithForbidden, Race.Human, Alignment.LawfulGood, "user-107")
            {
                Id = 8,
                Status = HeroStatus.Draft
            };

            var power = new Power("Power", "Description", 8) { Id = 1 };
            hero.Powers.Add(power);

            var exception = await Assert.ThrowsAsync<DomainException>(
                () => _heroService.RegisterAsync(hero)
            );
            Assert.Contains("forbidden phrase", exception.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task RegisterAsync_WithoutForbiddenPhrases_ShouldSucceed()
        {
            var originStory = "This hero comes from a humble background and became a beacon of hope";
            var hero = new Hero("CleanHero", originStory, Race.Human, Alignment.LawfulGood, "user-108")
            {
                Id = 9,
                Status = HeroStatus.Draft
            };

            var power = new Power("Power", "Description", 9) { Id = 1 };
            hero.Powers.Add(power);

            var registeredHero = new Hero(hero.Codename, hero.OriginStory, hero.Race, hero.Alignment, hero.UserId)
            {
                Id = hero.Id,
                Status = HeroStatus.Registered,
                Powers = hero.Powers
            };

            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Hero>()))
                .ReturnsAsync(registeredHero);

            var result = await _heroService.RegisterAsync(hero);
            Assert.Equal(HeroStatus.Registered, result.Status);
        }

        [Fact]
        public async Task RegisterAsync_WithMultipleForbiddenPhrases_ShouldThrowForFirst()
        {
            var originStory = "Hero with tragic past and mysterious organization involvement";
            var hero = new Hero("MultipleForbidden", originStory, Race.Human, Alignment.LawfulGood, "user-109")
            {
                Id = 10,
                Status = HeroStatus.Draft
            };

            var power = new Power("Power", "Description", 10) { Id = 1 };
            hero.Powers.Add(power);

            var exception = await Assert.ThrowsAsync<DomainException>(
                () => _heroService.RegisterAsync(hero)
            );
            Assert.Contains("forbidden phrase", exception.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task RegisterAsync_WithNoPowers_ShouldThrowDomainException()
        {
            var hero = new Hero("Powerless", "A hero without any powers but with strong will and intelligence", Race.Human, Alignment.LawfulGood, "user-110")
            {
                Id = 11,
                Status = HeroStatus.Draft,
                Powers = new List<Power>()
            };

            var exception = await Assert.ThrowsAsync<DomainException>(
                () => _heroService.RegisterAsync(hero)
            );
            Assert.Contains("at least", exception.Message);
        }

        [Fact]
        public async Task RegisterAsync_WithAtLeastOnePower_ShouldSucceed()
        {
            var hero = new Hero("Powered", "A hero with supernatural abilities granted by an ancient artifact", Race.Human, Alignment.LawfulGood, "user-111")
            {
                Id = 12,
                Status = HeroStatus.Draft
            };

            var power = new Power("Super Strength", "Incredible physical strength", 12) { Id = 1 };
            hero.Powers.Add(power);

            var registeredHero = new Hero(hero.Codename, hero.OriginStory, hero.Race, hero.Alignment, hero.UserId)
            {
                Id = hero.Id,
                Status = HeroStatus.Registered,
                Powers = hero.Powers
            };

            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Hero>()))
                .ReturnsAsync(registeredHero);

            var result = await _heroService.RegisterAsync(hero);

            Assert.Equal(HeroStatus.Registered, result.Status);
            Assert.Single(result.Powers);
        }

        [Fact]
        public async Task RegisterAsync_WithMultiplePowers_ShouldSucceed()
        {
            var hero = new Hero("MultiPowered", "A dwarf with multiple extraordinary abilities and enhanced senses", Race.Dwarf, Alignment.LawfulGood, "user-112")
            {
                Id = 13,
                Status = HeroStatus.Draft
            };

            hero.Powers.Add(new Power("Telepathy", "Mind reading and control", 13) { Id = 1 });
            hero.Powers.Add(new Power("Telekinesis", "Moving objects with mind", 13) { Id = 2 });
            hero.Powers.Add(new Power("Flight", "Ability to fly", 13) { Id = 3 });

            var registeredHero = new Hero(hero.Codename, hero.OriginStory, hero.Race, hero.Alignment, hero.UserId)
            {
                Id = hero.Id,
                Status = HeroStatus.Registered,
                Powers = hero.Powers
            };

            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Hero>()))
                .ReturnsAsync(registeredHero);

            var result = await _heroService.RegisterAsync(hero);
            Assert.Equal(HeroStatus.Registered, result.Status);
            Assert.Equal(3, result.Powers.Count);
        }

        [Fact]
        public async Task RegisterAsync_ShouldTransitionFromDraftToRegistered()
        {
            var hero = new Hero("TransitionHero", "A hero that transitions through different states during lifecycle", Race.Human, Alignment.LawfulGood, "user-113")
            {
                Id = 14,
                Status = HeroStatus.Draft
            };

            hero.Powers.Add(new Power("Power", "Description", 14) { Id = 1 });

            var registeredHero = new Hero(hero.Codename, hero.OriginStory, hero.Race, hero.Alignment, hero.UserId)
            {
                Id = hero.Id,
                Status = HeroStatus.Registered,
                Powers = hero.Powers
            };

            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Hero>()))
                .ReturnsAsync(registeredHero);

            var result = await _heroService.RegisterAsync(hero);
            Assert.Equal(HeroStatus.Registered, result.Status);
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Hero>()), Times.Once);
        }

        [Fact]
        public async Task RetireAsync_WithRegisteredHero_ShouldTransitionToRetired()
        {
            var hero = new Hero("RetirableHero", "A hero ready to step back from active duty", Race.Human, Alignment.LawfulGood, "user-114")
            {
                Id = 15,
                Status = HeroStatus.Registered
            };

            var retiredHero = new Hero(hero.Codename, hero.OriginStory, hero.Race, hero.Alignment, hero.UserId)
            {
                Id = hero.Id,
                Status = HeroStatus.Retired,
                Powers = hero.Powers
            };

            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Hero>()))
                .ReturnsAsync(retiredHero);

            var result = await _heroService.RetireAsync(hero);
            Assert.Equal(HeroStatus.Retired, result.Status);
        }

        [Fact]
        public async Task RetireAsync_WithDraftHero_ShouldThrowDomainException()
        {
            var hero = new Hero("DraftHero", "A hero still in development phase", Race.Human, Alignment.LawfulGood, "user-115")
            {
                Id = 16,
                Status = HeroStatus.Draft
            };

            var exception = await Assert.ThrowsAsync<DomainException>(
                () => _heroService.RetireAsync(hero)
            );
            Assert.Contains("registered", exception.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task RetireAsync_WithRetiredHero_ShouldThrowDomainException()
        {
            var hero = new Hero("AlreadyRetired", "A hero already retired from active service", Race.Human, Alignment.LawfulGood, "user-116")
            {
                Id = 17,
                Status = HeroStatus.Retired
            };

            var exception = await Assert.ThrowsAsync<DomainException>(
                () => _heroService.RetireAsync(hero)
            );
            Assert.Contains("registered", exception.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task DeleteAsync_WithDraftHero_ShouldSucceed()
        {
            var hero = new Hero("DeleteableDraft", "A draft hero that can be deleted", Race.Human, Alignment.LawfulGood, "user-117")
            {
                Id = 18,
                Status = HeroStatus.Draft
            };

            _mockRepository.Setup(r => r.DeleteAsync(It.IsAny<Hero>()))
                .Returns(Task.CompletedTask);

            await _heroService.DeleteAsync(hero);

            _mockRepository.Verify(r => r.DeleteAsync(hero), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_WithRegisteredHero_ShouldThrowInvalidOperationException()
        {
            var hero = new Hero("NonDeleteableRegistered", "A registered hero that cannot be deleted", Race.Human, Alignment.LawfulGood, "user-118")
            {
                Id = 19,
                Status = HeroStatus.Registered
            };

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _heroService.DeleteAsync(hero)
            );
            Assert.Contains("draft", exception.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task DeleteAsync_WithRetiredHero_ShouldThrowInvalidOperationException()
        {
            var hero = new Hero("NonDeleteableRetired", "A retired hero that cannot be deleted", Race.Human, Alignment.LawfulGood, "user-119")
            {
                Id = 20,
                Status = HeroStatus.Retired
            };

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _heroService.DeleteAsync(hero)
            );
            Assert.Contains("draft", exception.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task AddPowerAsync_ToDraftHero_ShouldSucceed()
        {
            var hero = new Hero("PowerReceiver", "A hero that receives powers before registration", Race.Human, Alignment.LawfulGood, "user-120")
            {
                Id = 21,
                Status = HeroStatus.Draft
            };

            var createPower = new CreatePower { Name = "New Power", Description = "A newly acquired power", HeroId = 21 };

            var updatedHero = new Hero(hero.Codename, hero.OriginStory, hero.Race, hero.Alignment, hero.UserId)
            {
                Id = hero.Id,
                Status = hero.Status,
                Powers = new List<Power>
                {
                    new Power(createPower.Name, createPower.Description, createPower.HeroId) { Id = 1 }
                }
            };

            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Hero>()))
                .ReturnsAsync(updatedHero);

            var result = await _heroService.AddPowerAsync(hero, createPower);

            Assert.Single(result.Powers);
            Assert.Equal("New Power", result.Powers.First().Name);
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Hero>()), Times.Once);
        }

        [Fact]
        public async Task AddPowerAsync_ToRegisteredHero_ShouldSucceed()
        {
            var hero = new Hero("RegisteredPowerReceiver", "A registered hero receiving additional powers", Race.Human, Alignment.LawfulGood, "user-121")
            {
                Id = 22,
                Status = HeroStatus.Registered
            };

            hero.Powers.Add(new Power("Existing Power", "Original power", 22) { Id = 1 });

            var createPower = new CreatePower { Name = "New Power", Description = "A newly acquired power", HeroId = 22 };

            var updatedHero = new Hero(hero.Codename, hero.OriginStory, hero.Race, hero.Alignment, hero.UserId)
            {
                Id = hero.Id,
                Status = hero.Status,
                Powers = new List<Power>
                {
                    hero.Powers.First(),
                    new Power(createPower.Name, createPower.Description, createPower.HeroId) { Id = 2 }
                }
            };

            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Hero>()))
                .ReturnsAsync(updatedHero);

            var result = await _heroService.AddPowerAsync(hero, createPower);

            Assert.Equal(2, result.Powers.Count);
        }

        [Fact]
        public async Task AddPowerAsync_ToRetiredHero_ShouldThrowDomainException()
        {
            var hero = new Hero("RetiredImmutable", "A retired hero with immutable powers", Race.Human, Alignment.LawfulGood, "user-122")
            {
                Id = 23,
                Status = HeroStatus.Retired
            };

            var createPower = new CreatePower { Name = "New Power", Description = "Cannot add to retired hero", HeroId = 23 };

            var exception = await Assert.ThrowsAsync<DomainException>(
                () => _heroService.AddPowerAsync(hero, createPower)
            );
            Assert.Contains("retired", exception.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task RemovePowerAsync_FromDraftHero_ShouldSucceed()
        {
            var hero = new Hero("DraftPowerRemover", "A draft hero losing a power", Race.Human, Alignment.LawfulGood, "user-123")
            {
                Id = 24,
                Status = HeroStatus.Draft
            };

            var power = new Power("Removable Power", "This power can be removed", 24) { Id = 1 };
            hero.Powers.Add(power);

            var updatedHero = new Hero(hero.Codename, hero.OriginStory, hero.Race, hero.Alignment, hero.UserId)
            {
                Id = hero.Id,
                Status = hero.Status,
                Powers = new List<Power>()
            };

            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Hero>()))
                .ReturnsAsync(updatedHero);

            var result = await _heroService.RemovePowerAsync(hero, 1);
            Assert.Empty(result.Powers);
        }

        [Fact]
        public async Task RemovePowerAsync_FromRegisteredHeroWithOnePower_ShouldThrowDomainException()
        {
            var hero = new Hero("RegisteredSinglePower", "A registered hero with only one power", Race.Human, Alignment.LawfulGood, "user-124")
            {
                Id = 25,
                Status = HeroStatus.Registered
            };

            var power = new Power("Only Power", "The only power this hero has", 25) { Id = 1 };
            hero.Powers.Add(power);
            var exception = await Assert.ThrowsAsync<DomainException>(
                () => _heroService.RemovePowerAsync(hero, 1)
            );
            Assert.Contains("at least 1 power", exception.Message);
        }

        [Fact]
        public async Task RemovePowerAsync_FromRegisteredHeroWithMultiplePowers_ShouldSucceed()
        {
            var hero = new Hero("RegisteredMultiplePowers", "A registered hero with multiple powers", Race.Human, Alignment.LawfulGood, "user-125")
            {
                Id = 26,
                Status = HeroStatus.Registered
            };

            var power1 = new Power("Power 1", "First power", 26) { Id = 1 };
            var power2 = new Power("Power 2", "Second power", 26) { Id = 2 };
            hero.Powers.Add(power1);
            hero.Powers.Add(power2);

            var updatedHero = new Hero(hero.Codename, hero.OriginStory, hero.Race, hero.Alignment, hero.UserId)
            {
                Id = hero.Id,
                Status = hero.Status,
                Powers = new List<Power> { power2 }
            };

            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Hero>()))
                .ReturnsAsync(updatedHero);

            var result = await _heroService.RemovePowerAsync(hero, 1);
            Assert.Single(result.Powers);
            Assert.Equal("Power 2", result.Powers.First().Name);
        }

        [Fact]
        public async Task RemovePowerAsync_FromRetiredHero_ShouldThrowDomainException()
        {
            var hero = new Hero("RetiredPowerImmutable", "A retired hero with immutable powers", Race.Human, Alignment.LawfulGood, "user-126")
            {
                Id = 27,
                Status = HeroStatus.Retired
            };

            var power = new Power("Immutable Power", "Cannot remove from retired hero", 27) { Id = 1 };
            hero.Powers.Add(power);

            var exception = await Assert.ThrowsAsync<DomainException>(
                () => _heroService.RemovePowerAsync(hero, 1)
            );
            Assert.Contains("retired", exception.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task RemovePowerAsync_WithNonExistentPower_ShouldThrowKeyNotFoundException()
        {
            var hero = new Hero("MissingPowerHero", "A hero with missing power id", Race.Human, Alignment.LawfulGood, "user-127")
            {
                Id = 28,
                Status = HeroStatus.Draft
            };

            var power = new Power("Existing Power", "This power exists", 28) { Id = 1 };
            hero.Powers.Add(power);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _heroService.RemovePowerAsync(hero, 999)
            );
        }

        [Fact]
        public async Task RegisterAsync_ValidateAllConditionsMustPass()
        {
            var hero = new Hero(
                "AllConditionsMet",
                "A hero that meets all registration conditions and requirements",
                Race.Human,
                Alignment.LawfulGood,
                "user-128"
            )
            {
                Id = 29,
                Status = HeroStatus.Draft
            };

            hero.Powers.Add(new Power("Power", "Description", 29) { Id = 1 });

            var registeredHero = new Hero(hero.Codename, hero.OriginStory, hero.Race, hero.Alignment, hero.UserId)
            {
                Id = hero.Id,
                Status = HeroStatus.Registered,
                Powers = hero.Powers
            };

            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Hero>()))
                .ReturnsAsync(registeredHero);

            var result = await _heroService.RegisterAsync(hero);

            Assert.Equal(HeroStatus.Registered, result.Status);
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Hero>()), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnHero()
        {
            var hero = new Hero("RetrievableHero", "A hero that can be retrieved by id", Race.Human, Alignment.LawfulGood, "user-129")
            {
                Id = 30
            };

            _mockRepository.Setup(r => r.GetByIdAsync(30))
                .ReturnsAsync(hero);

            var result = await _heroService.GetByIdAsync(30);

            Assert.NotNull(result);
            Assert.Equal("RetrievableHero", result.Codename);
        }

        [Fact]
        public async Task CodenameExistsAsync_WithExistingCodename_ShouldReturnTrue()
        {
            _mockRepository.Setup(r => r.CodenameExistsAsync("ExistingCodename"))
                .ReturnsAsync(true);

            var result = await _heroService.CodenameExistsAsync("ExistingCodename");

            Assert.True(result);
        }

        [Fact]
        public async Task CodenameExistsAsync_WithNonExistingCodename_ShouldReturnFalse()
        {
            _mockRepository.Setup(r => r.CodenameExistsAsync("NonExistingCodename"))
                .ReturnsAsync(false);

            var result = await _heroService.CodenameExistsAsync("NonExistingCodename");

            Assert.False(result);
        }
    }
}
