using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HabitTracker.Data;

namespace HabitTracker
{
    internal class AppState
    {
        public static AppDb Db { get; } = new AppDb();  
    }
}
