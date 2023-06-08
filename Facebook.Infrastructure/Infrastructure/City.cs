using System;
using System.Collections.Generic;

namespace Facebook.Infrastructure.Infrastructure;

public partial class City
{
    public int CityId { get; set; }

    public string CityName { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int CountryId { get; set; }

    public virtual Country Country { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
