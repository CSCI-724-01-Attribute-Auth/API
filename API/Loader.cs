using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

public class Loader
{
    private readonly string _dataSourcePath;

    // Constructor to initialize the loader with the path from the configuration
    public Loader(string dataSourcePath)
    {
        _dataSourcePath = dataSourcePath;
    }

    // Method to load authorization records from the data source
    public List<AuthorizationRecord> LoadAuthorizationRecords()
    {
        try
        {
            // Read the JSON file containing the authorization records
            var jsonData = File.ReadAllText(_dataSourcePath);

            // Deserialize the JSON into a list of AuthorizationRecords
            var records = JsonConvert.DeserializeObject<List<AuthorizationRecord>>(jsonData);

            if (records != null)
            {
                Console.WriteLine("Successfully loaded authorization records.");
                return records;
            }

            throw new Exception("No records found.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading authorization records: {ex.Message}");
            return new List<AuthorizationRecord>();
        }
    }
}
