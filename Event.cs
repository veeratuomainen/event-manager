public class Event {

    // Private backing field, validation in getter and setter
    private string description;
    public string Description
    {
        get => description;
        set
        {
            // The incoming new value is always named `value`.
            ArgumentNullException.ThrowIfNullOrEmpty(value);

            if (value.Length > 500)
            {
                throw new ArgumentException("Description is too long");
            }

            description = value;
        }
    }

    private string category;
    public string Category
    {
        get => category;
        set
        {
            category = value;
        }
    }

    // Automatic property: direct access to private field
    public DateOnly Date { get; set; }

    public Event(DateOnly date, string description, string category)
    {
        Date = date;
        Description = description;
        Category = category;
    }

    public override string ToString()
    {
        return $"{Date}: {Description} ({Category})";
    }
}
