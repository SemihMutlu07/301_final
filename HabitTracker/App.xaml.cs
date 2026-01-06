namespace HabitTracker
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();

            Task.Run(async () => await SeedTestDataAsync());
        }

        private async Task SeedTestDataAsync()
        {
            var h1 = await AppState.Db.AddHabitAsync("Morning Workout");
            var h2 = await AppState.Db.AddHabitAsync("Read 30 min");
            var h3 = await AppState.Db.AddHabitAsync("Drink 2L Water");
                
            var today = DateTime.Now.Date;
            var random = new Random();

            for (int i = 0; i < 30; i++)
            {
                var date = today.AddDays(-i);

                if (random.Next(100) < 80)
                    await AppState.Db.SetDoneAsync(h1, date, true);

                if (random.Next(100) < 60)
                    await AppState.Db.SetDoneAsync(h2, date, true);

                if (random.Next(100) < 40)
                    await AppState.Db.SetDoneAsync(h3, date, true);
            }
        }
    }
}
