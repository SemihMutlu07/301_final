namespace HabitTracker.Views;

public partial class HabitDetailPage : ContentPage
{
	private readonly int _habitId;
	private readonly string _name;
	
	public HabitDetailPage(int habitId, string name)
	{
		InitializeComponent();
		_habitId = habitId;
		_name = name;
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();

		NameLabel.Text = _name;

		var today = DateTime.Now.Date;

		var streak = await AppState.Db.GetStreakAsync(_habitId, today);
		StreakLabel.Text = $"{streak} days";

		Rate7Label.Text = "Last 7 Days: " + await RateText(_habitId, today, 7); ;
		Rate30Label.Text = "Last 30 days: " + await RateText(_habitId, today, 30);
	}

	private static async Task<string> RateText(int habitId, DateTime today, int days)
	{
		var from = today.AddDays(-(days - 1));
		var doneDates = await AppState.Db.GetDoneDatesAsync(habitId, from, today);

		var doneCount = doneDates.Count();
        var pct = (int)Math.Round(doneCount * 100.0 / days);
        return $"{doneCount}/{days} ({pct}%)";
    }
}