using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace HabitTracker.Models;

public class Completion
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public int HabitId { get; set; }

    public DateTime Date { get; set; }
}
    