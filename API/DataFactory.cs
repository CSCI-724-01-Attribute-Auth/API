using API.Models;
using System.Globalization;
using System.Reflection.PortableExecutable;
using System.Text;

namespace API
{
    public class DataFactory
    {
        private static Random gen = new Random();

        private static TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

        private static string[] _movieNames =
        {
            "Dreamweaver's Pact", "Beneath the Icebound Skies", "Eclipse Protocol", "Riddle of the Lost Key", "Neon Rebellion", "Reign of Shadows", "Celestial Veil", "Enigma in Black", "Moonstone Manor", "Lost in Neon Paradise", "Celestial Drift: A Cosmic Journey", "Dance of the Elementals", "The Last Whisper of Stardust", "Echoes of the Lost City", "The Midnight Serpent", "Enchanted Dreamscape", "The Phantom Key", "Echoes from the Shadowlands", "Nightshade Manor", "Midnight Firestorm", "Infinite Mirage", "Echoes of a Distant Star", "Last Train to Nowhere", "Echoes in the Neon Mist", "The Clockwork Serpent", "Quantum Drift: The Edge of Time", "Sunset Dreams", "Shadow Walker", "Neon Nightscape", "Stellar Secrets", "Tides of Infinity", "The Mechanical Heartbeat", "A Symphony of Silence", "Beneath the Velvet Sky", "The Last Ember Chronicles", "Celestial Fracture", "Whispers of the Forgotten", "Shadows in the Paradox", "The Clockwork Mirage", "Echoes of Tomorrow", "The Clockwork Whisper", "Celestial Rift", "Galactic Blossom", "Enchanted Dreamscape", "Shadowland Quest", "Starstruck City", "Wintermoon Wasteland", "Stellar Collision", "The Forgotten Kingdom", "Echoes of the Forgotten Realm", "Echoes of the Forgotten Sky", "Portals in Time", "Shadows in the Ether", "Galactic Diner: Cosmos Unplugged", "Shadow of the Unknown", "Shattered Realms", "Phantom Chronicles", "Nova Nightmares", "Firestorm Frontier", "Shadowbound", "Chronicles of the Celestial Tide", "The Clockwork Veil", "Galactic Drift", "Whispers from the Abyss", "Lunar Phantoms", "9 .Labyrinth of Dreams", "Phantom Serenade", "The Curse of Crimson Manor", "Enigma of Serenity", "Tales of the Lost Kingdom", "Shadows of a Forgotten Dawn", "Echoes from the Time Machine", "Beneath the Iron Sky", "Whispering Steel", "Phantom Rhapsody", "Cybernetic Reckoning", "Whisper in the Shadows", "Shadowfall", "Lunar Echoes", "Quantum Quest"
        };

        public List<Movie> Movies { get; set; } = new List<Movie>();
        public List<Person> Persons { get; set; } = new List<Person>();

        public DataFactory(int movieCount) 
        {
            if (movieCount > 80 || movieCount < 0)
            {
                throw new InvalidOperationException("The movie count must between 0 and 80.");
            }

            for (int i = 0; i < movieCount; i++)
            {
                var crew = new List<Person>();

                for (int j = 0; j < gen.Next(10); j++)
                {
                    Person toAdd;

                    if (gen.Next(10) == 1 && Persons.Count > 0)
                    {
                        toAdd = Persons[gen.Next(Persons.Count)];
                        if (gen.Next(2) == 0)
                        {
                            toAdd.MostFamousMovie = i;
                        }
                    }
                    else
                    {
                        toAdd = new Person
                        {
                            Id = Guid.NewGuid(),
                            BirthDate = RandomDay(new DateTime(2005, 1, 1)),
                            MostFamousMovie = i,
                            Name = textInfo.ToTitleCase(LoremIpsum(2, 2, 1, 1).Trim().TrimEnd('.'))
                        };
                    }

                    crew.Add(toAdd);
                    Persons.Add(toAdd);
                }

                Movie movie = new Movie
                {
                    Id = i,
                    Title = _movieNames[i],
                    Description = LoremIpsum(5, 10, 1, 5),
                    ReleaseDate = RandomDay(),
                    TotalBudget = Math.Round(gen.NextDouble() * 900_000 + 100_000, 2),
                    TotalCost = Math.Round(gen.NextDouble() * 900_000 + 100_000, 2),
                    CrewIds = crew.Select(p => p.Id).ToList()
                };
                Movies.Add(movie);
            }
        }

        public Movie? GetMovieById(int movieId)
        {
            return Movies.SingleOrDefault(m => m.Id == movieId);
        }

        public Person? GetPersonById(Guid personId)
        {
            return Persons.SingleOrDefault(p => p.Id == personId);
        }

        public Movie CreateMovie(string title, string description, double totalBudget, double totalCost, DateTime releaseDate, List<Guid> crewIds)
        {
            foreach (var cId in crewIds)
            {
                if (!Persons.Select(p => p.Id).Contains(cId))
                {
                    throw new InvalidDataException("There is a crew Id that is not in the set of crew. Please create this person and try again.");
                }
            }

            var toAdd = new Movie
            {
                Id = Movies.Count,
                Title = title,
                Description = description,
                TotalBudget = totalBudget,
                TotalCost = totalCost,
                ReleaseDate = releaseDate,
                CrewIds = crewIds
            };

            Movies.Add(toAdd);

            return toAdd;
        }

        public Person CreatePerson(string name, DateTime birthDate, int mostFamousMovie)
        {
            if (!Movies.Select(m => m.Id).Contains(mostFamousMovie))
            {
                throw new InvalidDataException("The movie Id associated with this person's most famous movie does not exist. Please create this movie and try again.");
            }

            var toAdd = new Person
            {
                Id = Guid.NewGuid(),
                Name = name,
                BirthDate = birthDate,
                MostFamousMovie = mostFamousMovie
            };

            Persons.Add(toAdd);
            return toAdd;
        }

        private static string LoremIpsum(int minWords, int maxWords,
            int minSentences, int maxSentences)
        {

            var words = new[]{"lorem", "ipsum", "dolor", "sit", "amet", "consectetuer",
        "adipiscing", "elit", "sed", "diam", "nonummy", "nibh", "euismod",
        "tincidunt", "ut", "laoreet", "dolore", "magna", "aliquam", "erat"};

            int numSentences = gen.Next(maxSentences - minSentences)
                + minSentences;
            int numWords = gen.Next(maxWords - minWords) + minWords;

            StringBuilder result = new StringBuilder();

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

        private static DateTime RandomDay(DateTime end)
        {
            DateTime start = new DateTime(1940, 1, 1);
            int range = (end - start).Days;
            return start.AddDays(gen.Next(range));
        }

        private static DateTime RandomDay()
        {
            return RandomDay(DateTime.Today.AddYears(2));
        }

    }
}
