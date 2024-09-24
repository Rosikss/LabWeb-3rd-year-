﻿using System.ComponentModel.DataAnnotations;

namespace LabWeb.Models;

public class ItemCategory : BaseEntity
{
    [StringLength(50)]
    public string Name { get; set; }
    public IEnumerable<Item> Items { get; set; } = new List<Item>();
}