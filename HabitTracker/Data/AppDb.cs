using SQLite;
using System.Linq;
using HabitTracker.Models;
using System.Xml;

namespace HabitTracker.Data;

public class AppDb
{
    private SQLiteAsyncConnection? _db;

    private async Task InitAsync()
    {
        if (_db != null) return;

        var path = Path.Combine(FileSystem.AppDataDirectory, "habits.db3");
        _db = new SQLiteAsyncConnection(path);

        await _db.CreateTableAsync<Habit>();
        await _db.CreateTableAsync<Completion>();
    }

    public async Task<List<Habit>> GetHabitsAsync()
    {
        await InitAsync();
        return await _db!.Table<Habit>().Where(h => h.IsActive).OrderBy(h => h.Name).ToListAsync();
    }

    public async Task<int> AddHabitAsync(string name)
    {
        await InitAsync();
        var habit = new Habit { Name = name };
        await _db!.InsertAsync(habit);
        return habit.Id;
    }

    public async Task DeleteHabitAsync(int id)
    {
        await InitAsync();
        await _db!.DeleteAsync<Habit>(id);
    }
    private static DateTime DateOnly(DateTime dt) => new DateTime(dt.Year, dt.Month, dt.Day);

    public async Task<bool> IsDoneAsync(int habitId, DateTime date)
    {
        await InitAsync();
        var d = DateOnly(date);
        var count = await _db!.Table<Completion>()
            .Where(c => c.HabitId == habitId && c.Date == d)
            .CountAsync();
        return count > 0;
    }

    public async Task SetDoneAsync(int habitId, DateTime date, bool done)
    {
        await InitAsync();
        var d = DateOnly(date);

        var existing = await _db!.Table<Completion>()
            .Where(c => c.HabitId == habitId && c.Date == d)
            .FirstOrDefaultAsync();

        if (done)
        {
            if (existing == null)
                await _db.InsertAsync(new Completion { HabitId = habitId, Date = d });
        }
        else
        {
            if (existing != null)
                await _db.DeleteAsync(existing);
        }
    }

    ///streak duolingo logic
    public async Task<List<DateTime>> GetDoneDatesAsync(int habitId, DateTime from, DateTime to)
    {
        await InitAsync();
        var f = from.Date;
        var t = to.Date;

        var rows = await _db!.Table<Completion>()
            .Where(c => c.HabitId == habitId && c.Date >= f && c.Date <= t)
            .ToListAsync();

        return rows.Select(r => r.Date).ToList();
    }

    public async Task<int> GetStreakAsync(int habitId, DateTime today)
    {
        await InitAsync();
        var d = today.Date;
        int streak = 0;

        while(true)
        {
            var done = await IsDoneAsync(habitId, d);
            if (!done) break;
            streak++;
            d = d.AddDays(-1);
        }

        return streak;
    }


}

