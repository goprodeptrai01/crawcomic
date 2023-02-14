﻿using System.ComponentModel.DataAnnotations;

namespace ReadMangaTest.Models;

public class Artist
{
    public string Name { get; set; }

    [StringLength(500)] public string Description { get; set; }
    public string Url { get; set; }
    public bool IsHidden { get; set; } = false;
    public List<Comic> Comics { get; set; }
}