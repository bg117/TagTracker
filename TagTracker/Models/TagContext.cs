﻿using System;
using System.IO;
using Microsoft.EntityFrameworkCore;

namespace TagTracker.Models;

public sealed class TagContext : DbContext
{
    public TagContext()
    {
        var local  = Environment.SpecialFolder.LocalApplicationData;
        var folder = Environment.GetFolderPath(local);
        DatabasePath = Path.Combine(folder, "tags.db");
        Database.EnsureCreated();
    }

    public  DbSet<Tag> Tags         { get; set; } = default!;
    private string     DatabasePath { get; }

    protected override void OnConfiguring(
        DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseSqlite($"Data Source={DatabasePath}");
    }
}
