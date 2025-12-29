using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace HabitTracker.Models;

public class Habit
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [SQLite.MaxLength(100)]
    public string Name { get; set; } = "";
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
