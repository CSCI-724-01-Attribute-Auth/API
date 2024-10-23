using API.Data;
using API.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text;

namespace API
{
    public class DataFactory
    {
        // Static random generator to be used across the class for generating random data
        private static Random gen = new Random();

        // TextInfo is used to format text such as converting names to title case
        private static TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

        // Static array of movie names to be randomly assigned when generating movies
        private static string[] _movieNames =
        {
            "Dreamweaver's Pact", "Beneath the Icebound Skies", "Eclipse Protocol", "Riddle of the Lost Key", "Neon Rebellion", "Reign of Shadows", "Celestial Veil", "Enigma in Black", "Moonstone Manor", "Lost in Neon Paradise", "Celestial Drift: A Cosmic Journey", "Dance of the Elementals", "The Last Whisper of Stardust", "Echoes of the Lost City", "The Midnight Serpent", "Enchanted Dreamscape", "The Phantom Key", "Echoes from the Shadowlands", "Nightshade Manor", "Midnight Firestorm", "Infinite Mirage", "Echoes of a Distant Star", "Last Train to Nowhere", "Echoes in the Neon Mist", "The Clockwork Serpent", "Quantum Drift: The Edge of Time", "Sunset Dreams", "Shadow Walker", "Neon Nightscape", "Stellar Secrets", "Tides of Infinity", "The Mechanical Heartbeat", "A Symphony of Silence", "Beneath the Velvet Sky", "The Last Ember Chronicles", "Celestial Fracture", "Whispers of the Forgotten", "Shadows in the Paradox", "The Clockwork Mirage", "Echoes of Tomorrow", "The Clockwork Whisper", "Celestial Rift", "Galactic Blossom", "Enchanted Dreamscape 2", "Shadowland Quest", "Starstruck City", "Wintermoon Wasteland", "Stellar Collision", "The Forgotten Kingdom", "Echoes of the Forgotten Realm", "Echoes of the Forgotten Sky", "Portals in Time", "Shadows in the Ether", "Galactic Diner: Cosmos Unplugged", "Shadow of the Unknown", "Shattered Realms", "Phantom Chronicles", "Nova Nightmares", "Firestorm Frontier", "Shadowbound", "Chronicles of the Celestial Tide", "The Clockwork Veil", "Galactic Drift", "Whispers from the Abyss", "Lunar Phantoms", "9 .Labyrinth of Dreams", "Phantom Serenade", "The Curse of Crimson Manor", "Enigma of Serenity", "Tales of the Lost Kingdom", "Shadows of a Forgotten Dawn", "Echoes from the Time Machine", "Beneath the Iron Sky", "Whispering Steel", "Phantom Rhapsody", "Cybernetic Reckoning", "Whisper in the Shadows", "Shadowfall", "Lunar Echoes", "Quantum Quest"
        };

        public void Seed(DBContext dbContext)
        {
            for (int i = 0; i < 80; i++)
            {
                Movie movie = new()
                {
                    Title = _movieNames[i],
                    Description = LoremIpsum(5, 10, 1, 5),
                    ReleaseDate = RandomDay(),
                    TotalBudget = Math.Round(gen.NextDouble() * 900_000 + 100_000, 2),
                    TotalCost = Math.Round(gen.NextDouble() * 900_000 + 100_000, 2),
                };
                dbContext.Movies.Add(movie);
                dbContext.SaveChanges();

                movie = dbContext.Movies.Single(m => m.Title == _movieNames[i]);

                var crew = new List<Person>();

                // Generating a random number of crew members for each movie
                // gen.Next(10) generates a random integer between 0 (inclusive) and 10 (exclusive), so the loop will execute anywhere between 0 and 9 iterations
                for (int j = 0; j < gen.Next(10); j++)
                {
                    // Declare a Person object to hold the crew member being added
                    Person toAdd;

                    if (gen.Next(10) == 1 && dbContext.Persons.Count() > 0)
                    {
                        try
                        {
                            toAdd = dbContext.Persons.OrderBy(p => Guid.NewGuid()).First(p => !crew.Select(c => c.Id).Contains(p.Id));

                            if (gen.Next(2) == 0)
                            {
                                toAdd.MostFamousMovieId = movie.Id;
                            }
                            dbContext.Persons.Update(toAdd);
                        }
                        catch (InvalidOperationException) {

                            toAdd = dbContext.Persons.OrderBy(p => Guid.NewGuid()).First();
                        }
                    }
                    else
                    {
                        toAdd = new()
                        {
                            BirthDate = RandomDay(new DateTime(2005, 1, 1)),
                            MostFamousMovieId = movie.Id,
                            Name = textInfo.ToTitleCase(LoremIpsum(2, 2, 1, 1).Trim().TrimEnd('.'))
                        };
                        dbContext.Persons.Add(toAdd);
                    }

                    dbContext.SaveChanges();
                    toAdd = dbContext.Persons.Single(p => p.Name == toAdd.Name && p.BirthDate == toAdd.BirthDate);

                    try
                    {
                        dbContext.CrewMembers.Add(new CrewMember
                        {
                            MovieId = movie.Id,
                            PersonId = toAdd.Id
                        });
                        dbContext.SaveChanges();
                    }
                    catch (InvalidOperationException) { }

                    crew.Add(toAdd);
                }
            }
        }

        private static string LoremIpsum(int minWords, int maxWords,
            int minSentences, int maxSentences)
        {
            var words = new[]{"lorem", "ipsum", "dolor", "sit", "amet", "consectetuer",
                "adipiscing", "elit", "sed", "diam", "nonummy", "nibh", "euismod",
                "tincidunt", "ut", "laoreet", "dolore", "magna", "aliquam", "erat"};

            int numSentences = gen.Next(maxSentences - minSentences) + minSentences;
            int numWords = gen.Next(maxWords - minWords) + minWords;

            StringBuilder result = new();

            // Building random sentences with random words from the predefined list
            for (int s = 0; s < numSentences; s++)
            {
                for (int w = 0; w < numWords; w++)
                {
                    if (w > 0) { result.Append(" "); }
                    result.Append(words[gen.Next(words.Length)]);
                }
                result.Append(". ");
            }

            return result.ToString();
        }

        // Utility method to generate a random date within a range
        private static DateTime RandomDay(DateTime end)
        {
            DateTime start = new(1940, 1, 1);
            int range = (end - start).Days;
            return start.AddDays(gen.Next(range));
        }

        // Overload method to generate a random date with no specified end, defaulting to 2 years from today
        private static DateTime RandomDay()
        {
            return RandomDay(DateTime.Today.AddYears(2));
        }
    }
}
