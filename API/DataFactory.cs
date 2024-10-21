using API.Models; // Importing the namespace that likely contains the Movie and Person models
using System.Globalization; // Used for text manipulation and formatting (e.g., for names)
using System.Reflection.PortableExecutable;
using System.Text; // Used for string manipulation (e.g., for generating random sentences)

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
            "Dreamweaver's Pact", "Beneath the Icebound Skies", "Eclipse Protocol", "Riddle of the Lost Key", "Neon Rebellion", "Reign of Shadows", "Celestial Veil", "Enigma in Black", "Moonstone Manor", "Lost in Neon Paradise", "Celestial Drift: A Cosmic Journey", "Dance of the Elementals", "The Last Whisper of Stardust", "Echoes of the Lost City", "The Midnight Serpent", "Enchanted Dreamscape", "The Phantom Key", "Echoes from the Shadowlands", "Nightshade Manor", "Midnight Firestorm", "Infinite Mirage", "Echoes of a Distant Star", "Last Train to Nowhere", "Echoes in the Neon Mist", "The Clockwork Serpent", "Quantum Drift: The Edge of Time", "Sunset Dreams", "Shadow Walker", "Neon Nightscape", "Stellar Secrets", "Tides of Infinity", "The Mechanical Heartbeat", "A Symphony of Silence", "Beneath the Velvet Sky", "The Last Ember Chronicles", "Celestial Fracture", "Whispers of the Forgotten", "Shadows in the Paradox", "The Clockwork Mirage", "Echoes of Tomorrow", "The Clockwork Whisper", "Celestial Rift", "Galactic Blossom", "Enchanted Dreamscape", "Shadowland Quest", "Starstruck City", "Wintermoon Wasteland", "Stellar Collision", "The Forgotten Kingdom", "Echoes of the Forgotten Realm", "Echoes of the Forgotten Sky", "Portals in Time", "Shadows in the Ether", "Galactic Diner: Cosmos Unplugged", "Shadow of the Unknown", "Shattered Realms", "Phantom Chronicles", "Nova Nightmares", "Firestorm Frontier", "Shadowbound", "Chronicles of the Celestial Tide", "The Clockwork Veil", "Galactic Drift", "Whispers from the Abyss", "Lunar Phantoms", "9 .Labyrinth of Dreams", "Phantom Serenade", "The Curse of Crimson Manor", "Enigma of Serenity", "Tales of the Lost Kingdom", "Shadows of a Forgotten Dawn", "Echoes from the Time Machine", "Beneath the Iron Sky", "Whispering Steel", "Phantom Rhapsody", "Cybernetic Reckoning", "Whisper in the Shadows", "Shadowfall", "Lunar Echoes", "Quantum Quest"
        };

        public List<Movie> Movies { get; set; } = new List<Movie>();
        public List<Person> Persons { get; set; } = new List<Person>();

        // Constructor that initializes the data by generating 'movieCount' number of movies and crew members
        public DataFactory(int movieCount) 
        {
            // Validating that the number of movies generated is between 0 and 80
            if (movieCount > 80 || movieCount < 0)
            {
                throw new InvalidOperationException("The movie count must be between 0 and 80.");
            }

            // Looping to create 'movieCount' number of movies
            for (int i = 0; i < movieCount; i++)
            {
                // List to store the crew members for each movie
                var crew = new List<Person>();

                // Generating a random number of crew members for each movie
                // gen.Next(10) generates a random integer between 0 (inclusive) and 10 (exclusive), so the loop will execute anywhere between 0 and 9 iterations
                for (int j = 0; j < gen.Next(10); j++)
                {
                    // Declare a Person object to hold the crew member being added
                    Person toAdd;

                    // Randomly reusing a crew member or creating a new one
                    // If gen.Next(10) returns 1 (10% chance) and there are people in the Persons list (Persons.Count > 0), 
                    // it selects a random person from the list
                    if (gen.Next(10) == 1 && Persons.Count > 0)
                    {
                        // Select a random person from the Persons list
                        toAdd = Persons[gen.Next(Persons.Count)];
                        // This adds a random decision (50% chance) to assign the current movie as this person's "most famous movie."
                        if (gen.Next(2) == 0)
                        {
                            toAdd.MostFamousMovie = i; // Assign this movie as the crew member's most famous movie
                        }
                    }
                    else
                    {
                        // Create a new person (crew member) with random name and birthdate
                        toAdd = new Person
                        {
                            Id = Guid.NewGuid(),
                            BirthDate = RandomDay(new DateTime(2005, 1, 1)), // Random birthdate
                            // Assigns the current movie as the person's most famous movie
                            MostFamousMovie = i,
                            // Generates a random name using a LoremIpsum function, and formats it using ToTitleCase to capitalize the first letter of each word.
                            Name = textInfo.ToTitleCase(LoremIpsum(2, 2, 1, 1).Trim().TrimEnd('.')) // Generating random name
                        };
                    }

                    // Adding the person to both the movie crew and global list of persons
                    crew.Add(toAdd);
                    Persons.Add(toAdd);
                }

                // Create a new movie with generated data
                Movie movie = new Movie
                {
                    Id = i, // Movie ID (index in list)
                    Title = _movieNames[i], // Title from predefined movie names
                    Description = LoremIpsum(5, 10, 1, 5), // Random description
                    ReleaseDate = RandomDay(), // Random release date
                    TotalBudget = Math.Round(gen.NextDouble() * 900_000 + 100_000, 2), // Random budget
                    TotalCost = Math.Round(gen.NextDouble() * 900_000 + 100_000, 2), // Random cost
                    // This selects the IDs of all the people (Person objects) in the crew and stores them in the CrewIds list.
                    // crew.Select(p => p.Id): For each person p in the crew list, it retrieves their Id property.
                    // .ToList(): Converts the resulting collection of crew IDs into a List<Guid>.
                    CrewIds = crew.Select(p => p.Id).ToList() // List of crew member IDs
                };
                
                // Adding the generated movie to the list of movies
                Movies.Add(movie);
            }
        }

        // Method to fetch a movie by its ID
        //  The return type is Movie, with a nullable modifier (?). 
        // This means the method can either return a Movie object or null if no movie with the specified movieId is found.
        public Movie? GetMovieById(int movieId)
        {
            // Returns a single movie by ID or null if not found
            //SingleOrDefault(): This is a LINQ method that returns the single element in a collection that matches the given condition (predicate). 
            return Movies.SingleOrDefault(m => m.Id == movieId);
        }

        // Method to fetch a person by their ID
        public Person? GetPersonById(Guid personId)
        {
            // Returns a single person by ID or null if not found
            return Persons.SingleOrDefault(p => p.Id == personId);
        }

        // Method to create a new movie with validation for crew IDs
        public Movie CreateMovie(string title, string description, double totalBudget, double totalCost, DateTime releaseDate, List<Guid> crewIds)
        {
            // Validating if the crew IDs exist in the current set of persons
            foreach (var cId in crewIds)
            {
                // The if condition is checking whether the cId is not found in the list of Person IDs
                if (!Persons.Select(p => p.Id).Contains(cId))
                {
                    throw new InvalidDataException("There is a crew Id that is not in the set of crew. Please create this person and try again.");
                }
            }

            // Create the new movie object
            var toAdd = new Movie
            {
                Id = Movies.Count, // Assigning a new ID based on the number of movies
                Title = title,
                Description = description,
                TotalBudget = totalBudget,
                TotalCost = totalCost,
                ReleaseDate = releaseDate,
                CrewIds = crewIds // Assigning provided crew IDs
            };

            // Adding the new movie to the movie list
            Movies.Add(toAdd);

            return toAdd;
        }

        // Method to create a new person with validation for their most famous movie
        public Person CreatePerson(string name, DateTime birthDate, int mostFamousMovie)
        {
            // Validating that the movie associated with this person exists
            // This if condition is checking whether the mostFamousMovie ID exists in the list of movies
            if (!Movies.Select(m => m.Id).Contains(mostFamousMovie))
            {
                throw new InvalidDataException("The movie Id associated with this person's most famous movie does not exist. Please create this movie and try again.");
            }

            // Create a new person object
            var toAdd = new Person
            {
                Id = Guid.NewGuid(), // Generating a new unique ID
                Name = name,
                BirthDate = birthDate,
                MostFamousMovie = mostFamousMovie
            };

            // Adding the new person to the person list
            Persons.Add(toAdd);
            return toAdd;
        }

        // Utility method to generate random Lorem Ipsum text
        private static string LoremIpsum(int minWords, int maxWords, int minSentences, int maxSentences)
        {
            var words = new[]{"lorem", "ipsum", "dolor", "sit", "amet", "consectetuer",
                "adipiscing", "elit", "sed", "diam", "nonummy", "nibh", "euismod",
                "tincidunt", "ut", "laoreet", "dolore", "magna", "aliquam", "erat"};

            int numSentences = gen.Next(maxSentences - minSentences) + minSentences;
            int numWords = gen.Next(maxWords - minWords) + minWords;

            StringBuilder result = new StringBuilder();

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
            DateTime start = new DateTime(1940, 1, 1); // Starting from 1940
            int range = (end - start).Days; // Calculate range in days
            return start.AddDays(gen.Next(range)); // Add a random number of days to the start date
        }

        // Overload method to generate a random date with no specified end, defaulting to 2 years from today
        private static DateTime RandomDay()
        {
            return RandomDay(DateTime.Today.AddYears(2));
        }
    }
}
