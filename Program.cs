using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length < 2 || args[0] != "days")
        {
            Console.WriteLine("Invalid command or not enough commands.");
            return;
        }

        // load events from csv file
        List<Event> events = LoadEvents();

        switch (args[1])
        {
            case "list": // if user wants to list events
                ListEvents(args, events);
                break;
            case "add": // if user wants to add events
                AddEvents(args);
                break;
            case "delete": // if user wants to delete events
                DeleteEvents(args, events);
                break;
            default:
                Console.WriteLine("Invalid command.");
                break;
        }
    }

    static List<Event> LoadEvents()
    {
        List<Event> events = new List<Event>();

        // use csvHelper to read contents of events.csv
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            PrepareHeaderForMatch = args => args.Header.ToLower(),
        };
        try
        {
            using (var reader = new StreamReader("events.csv"))
            using (var csv = new CsvReader(reader, config))
            {
                var records = csv.GetRecords<dynamic>();

                foreach (var record in records)
                {
                    string dateString = record.date;
                    DateOnly date =DateOnly.Parse(dateString);

                    string description = record.description;

                    string category = record.category;

                    Event e = new Event(date, description, category);
                    events.Add(e);
                }
            }
        }
        // if file was not found
        catch (FileNotFoundException e)
        {
            Console.WriteLine($"{e.Message}");
            return new List<Event>();
        }

        return events;
    }

    // function for listing the wanted events
    static void ListEvents(string[] args, List<Event> events)
    {
        // default values
        DateOnly today = DateOnly.FromDateTime(DateTime.Today);
        DateOnly? beforeDate = null;
        DateOnly? afterDate = null;
        DateOnly? specificDate = null;
        List<string> categories = new List<string>(); // categories needs to be a list because there could be many
        bool exclude = false;

        // parse the possible options
        for (int i = 2; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "--today":
                    specificDate = today;
                    break;
                case "--before-date":
                    if (i < args.Length - 1 && DateOnly.TryParse(args[i + 1], out DateOnly before))
                    {
                        beforeDate = before;
                        i++; // need to skip nect parameter because it is the date
                    }
                    else
                    {
                        Console.WriteLine("Invalid or missing date.");
                        return;
                    }
                    break;
                case "--after-date":
                    if (i < args.Length - 1 && DateOnly.TryParse(args[i + 1], out DateOnly after))
                    {
                        afterDate = after;
                        i++; // skip date
                    }
                    else
                    {
                        Console.WriteLine("Invalid or missing date.");
                        return;
                    }
                    break;
                case "--date":
                    if (i < args.Length - 1 && DateOnly.TryParse(args[i + 1], out DateOnly date))
                    {
                        specificDate = date;
                        i++; // skip date
                    }
                    else
                    {
                        Console.WriteLine("Invalid or missing date.");
                        return;
                    }
                    break;
                case "--categories":
                    if (i < args.Length - 1)
                    {
                        categories.AddRange(args[i + 1].Split(",")); // put categories into a list with ',' as dividor
                        i++; // skip category
                    }
                    else
                    {
                        Console.WriteLine("Missing categories.");
                        return;
                    }
                    break;
                case "--exclude":
                    exclude = true; // if user wants to exlude the categories
                    break;
                default:
                    Console.WriteLine("Invalid options.");
                    return;
            }
        }

        // filter the events based on the options
        IEnumerable<Event> filteredEvents = events;
        if (beforeDate.HasValue && afterDate.HasValue)
        {
            filteredEvents = filteredEvents.Where(e => e.Date < beforeDate && e.Date > afterDate);
        }
        else
        {
            if (beforeDate.HasValue)
            {
                filteredEvents = filteredEvents.Where(e => e.Date < beforeDate);
            }
            if (afterDate.HasValue)
            {
                filteredEvents = filteredEvents.Where(e => e.Date > afterDate);
            }
        }
        if (specificDate.HasValue)
        {
            filteredEvents = filteredEvents.Where(e => e.Date == specificDate);
        }
        if (categories.Count != 0)
        {
            if (exclude == false)
            {
                filteredEvents = filteredEvents.Where(e => categories.Contains(e.Category));
            }
            else
            {
                filteredEvents = filteredEvents.Where(e => !categories.Contains(e.Category));
            }
        }

        // print the filtered events
        foreach (Event e in filteredEvents)
        {
            Console.WriteLine(e);
        }
    }

    // function for adding events
    static void AddEvents(string[] args)
    {
        // default values
        DateOnly date = DateOnly.FromDateTime(DateTime.Today); // default date is today because if user does not provide date, it is today
        string category = null;
        string description = null;

        // parse the arguments
        for (int i = 2; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "--date":
                    if (i < args.Length - 1 && DateOnly.TryParse(args[i + 1], out DateOnly parsedDate))
                    {
                        date = parsedDate;
                        i++; // skip date
                    }
                    else
                    {
                        Console.WriteLine("Invalid or missing date.");
                        return;
                    }
                    break;
                case "--category":
                    if (i < args.Length - 1)
                    {
                        category = args[i + 1];
                        i++; // skip category
                    }
                    else
                    {
                        Console.WriteLine("Missing category");
                        return;
                    }
                    break;
                case "--description":
                    if (i < args.Length - 1)
                    {
                        description = args[i + 1];
                        i++; // skip description
                    }
                    else
                    {
                        Console.WriteLine("Missing description");
                        return;
                    }
                    break;
                default:
                    Console.WriteLine("Invalid arguments.");
                    return;
            }
        }

        // check if there is description since it is the only required value
        if (string.IsNullOrEmpty(description))
        {
            Console.WriteLine("Description is required.");
            return;
        }

        // create a new event object
        Event newEvent = new Event(date, description, category);

        // check to see if a csv file already exists
        bool fileExists = File.Exists("events.csv");

        // write the new event to the data file
        using (var writer = new StreamWriter("events.csv", append: true))
        using (var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)))
        {
            // if file didn't already exist, let's create it and add column headers for it
            if (!fileExists)
            {
                csv.WriteHeader<Event>();
                csv.NextRecord();
            }

            csv.WriteRecord(newEvent);
            csv.NextRecord();
        }

        Console.WriteLine("Event added successfully");
    }

    // function for deleting events
    static void DeleteEvents(string[] args, List<Event> events)
    {
        // default values
        DateOnly? date = null;
        string description = null;
        string category = null;
        bool deleteAll = false;
        bool dryRun = false;

        // parse the arguments
        for (int i = 2; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "--date":
                    if (i < args.Length - 1 && DateOnly.TryParse(args[i + 1], out DateOnly parsedDate))
                    {
                        date = parsedDate;
                        i++; // skip date
                    }
                    else
                    {
                        Console.WriteLine("Invalid or missing date.");
                        return;
                    }
                    break;
                case "--description":
                    if (i < args.Length - 1)
                    {
                        description = args[i + 1];
                        i++; // skip description
                    }
                    else
                    {
                        Console.WriteLine("Missing description");
                        return;
                    }
                    break;
                case "--category":
                    if (i < args.Length - 1)
                    {
                        category = args[i + 1];
                        i++; // skip category
                    }
                    else
                    {
                        Console.WriteLine("Missing category");
                        return;
                    }
                    break;
                case "--all":
                    deleteAll = true;
                    break;
                case "--dry-run":
                    dryRun = true;
                    break;
                default:
                    Console.WriteLine("Invalid arguments.");
                    return;
            }
        }

        // create new list which contains deletable events
        List<Event> eventsToDelete = new List<Event>();
        if (deleteAll)
        {
            eventsToDelete.AddRange(events);
        }
        else
        {
            if (date.HasValue)
            {
                eventsToDelete.AddRange(events.Where(e => e.Date == date));
            }
            if (!string.IsNullOrEmpty(description))
            {
                eventsToDelete.AddRange(events.Where(e => e.Description.Contains(description)));
            }
            if (!string.IsNullOrEmpty(category))
            {
                eventsToDelete.AddRange(events.Where(e => e.Category == category));
            }
        }

        // check for dry run
        if (dryRun)
        {
            // only show events to be deleted, don't delete them
            Console.WriteLine("Events to be deleted:");
            foreach (Event e in eventsToDelete)
            {
                Console.WriteLine(e);
            }
            return;
        }

        // remove eventsToDelete from events and then write events to the file
        foreach (Event e in eventsToDelete)
        {
            events.Remove(e);
        }

        using (var writer = new StreamWriter("events.csv"))
        using (var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)))
        {
            csv.WriteRecords(events);
        }

        Console.WriteLine("Events deleted successfully");
    }
}
