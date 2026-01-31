namespace UserDirectoryAPI.Domain.Entities;

public class User
{
    public Guid Id { get; set; }

    public string Name { get; set; } = default!;

    public int Age { get; set; }

    public string City { get; set; } = default!;

    public string State { get; set; } = default!;

    public string Pincode { get; set; } = default!;
}
