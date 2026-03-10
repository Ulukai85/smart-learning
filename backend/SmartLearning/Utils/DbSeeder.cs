using Microsoft.AspNetCore.Identity;
using SmartLearning.Models;

namespace SmartLearning.Utils;

public class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        var userManager = services.GetRequiredService<UserManager<AppUser>>();
        var context = services.GetRequiredService<AppDbContext>();

        if (await userManager.FindByEmailAsync("demo@mail.de") == null)
        {
            var demoUser = new AppUser
            {
                Email = "demo@mail.de",
                UserName = "demo@mail.de",
                Handle = "demo",
            };
            
            var result = await userManager.CreateAsync(demoUser, "Password1!");
            
            if (!result.Succeeded)
                throw new Exception("Failed to create demo user");
            
            var demoDeck = BuildDeck(demoUser.Id);
            context.Decks.Add(demoDeck);
            context.Cards.AddRange(BuildAlgorithmCards(demoDeck.Id));
            await context.SaveChangesAsync();
        }
    }

    private static Deck BuildDeck(string ownerId)
    {
        return new Deck
        {
            Name = "Algorithms",
            Description = "Example Deck with questions about algorithms",
            IsPublished = true,
            OwnerUserId = ownerId,
        };
    }

    private static IEnumerable<Card> BuildAlgorithmCards(Guid deckId)
    {
        return new List<Card>
        {
            new Card
            {
                DeckId = deckId,
                Front = "What is an algorithm?",
                Back = "A finite sequence of well-defined steps used to solve a problem or perform a computation."
            },
            new Card
            {
                DeckId = deckId,
                Front = "What does time complexity describe?",
                Back = "Time complexity describes how the running time of an algorithm grows with the size of the input."
            },
            new Card
            {
                DeckId = deckId,
                Front = "What is Big-O notation?",
                Back = "Big-O notation describes the upper bound of an algorithm’s time or space complexity."
            },
            new Card
            {
                DeckId = deckId,
                Front = "How does binary search work?",
                Back = "Binary search repeatedly divides a sorted list in half to locate a target value."
            },
            new Card
            {
                DeckId = deckId,
                Front = "What is the time complexity of binary search?",
                Back = "Binary search runs in O(log n) time because the search space is halved each step."
            },
            new Card
            {
                DeckId = deckId,
                Front = "What is a sorting algorithm?",
                Back = "A sorting algorithm rearranges elements of a list into a specific order, usually ascending or descending."
            },
            new Card
            {
                DeckId = deckId,
                Front = "What is the key idea of quicksort?",
                Back = "Quicksort selects a pivot and partitions the array into elements smaller and larger than the pivot."
            },
            new Card
            {
                DeckId = deckId,
                Front = "What is recursion?",
                Back = "Recursion is a technique where a function calls itself to solve smaller parts of a problem."
            },
            new Card
            {
                DeckId = deckId,
                Front = "What is dynamic programming?",
                Back = "Dynamic programming solves complex problems by storing results of overlapping subproblems."
            },
            new Card
            {
                DeckId = deckId,
                Front = "What is a greedy algorithm?",
                Back = "A greedy algorithm always chooses the locally optimal step hoping it leads to a global optimum."
            }
        };
    }
}