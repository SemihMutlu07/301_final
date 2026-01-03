using System.Linq;

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

		#Rate7Label.Text = "Last 7 Days: " + await RateText(_habitId, today, 7); ;
		#Rate30Label.Text = "Last 30 days: " + await RateText(_habitId, today, 30);

		await LoadLast7Async(today);
		await LoadLast30Async(today);
	}
	
	private async Task LoadLast7Async(DateTime today) {
		Last7Container.Children.Clear();

		var from = today.AddDays(-6);
		var doneDates = await AppState.Db.GetDoneDatesAsync(_habitId, from, today);
		var doneSet = doneDates.Select(d => d.Date).ToHashSet();

		for (int i = 0; i < 7; i++) {
			var d = from.AddDays(i);
			var isDone = doneSet.Contains(d);

			Last7Container.Children.Add(new BoxView) {
				WidthRequest = 18,
				HeightRequest = 18,
				CornerRadius = 4,
				Color = isDone ? Colors.LimeGreen : Colors.DarkGray
			}
		}
	}

	private async Task Load30Async(DateTime today) {
		var from = today.AddDays(-29); 
		var doneDates = await AppState.Db.GetDoneDatesAsync(_habitId, from, today);

		var doneCount = doneDates.Count;
		var progress = doneCount / 30.0;

		Rate30Bar.Progress = progress;
		Rate30Text.Text = $"{doneCount}/30 ({(int)Math.Round(progress * 100)}%)"
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