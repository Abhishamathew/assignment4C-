using System;
using System.Collections.Generic;

namespace Assignment4DB.Models.Entities;

public partial class Account
{
    public int AccountId { get; set; }

    public string AccountType { get; set; } = null!;

    public int UserId { get; set; }

    public double? CurrentBalance { get; set; }

    public DateTime? DateCreated { get; set; }
}
