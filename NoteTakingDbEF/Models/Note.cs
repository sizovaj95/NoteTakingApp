using System;
using System.Collections.Generic;

namespace NoteTakingDbEF.Models;

public partial class Note
{
    public int Id { get; set; }

    public string Note1 { get; set; } = null!;

    public DateTime Date { get; set; }

    public TimeSpan Time { get; set; }
}
