using System.Globalization;
using System.Linq;
using Microsoft.Maui.Controls;
using Windows.ApplicationModel.Calls.Background;
namespace HabitTracker.Views;

public partial class HabitDetailPage : ContentPage
{
	private readonly int _habitId;
	private readonly string _name;
	private DateTime _currentMonth;

    public HabitDetailPage(int habitId, string name)
	{
		InitializeComponent();
		_habitId = habitId;
		_name = name;
		_currentMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
    }

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		NameLabel.Text = _name;
		var today = DateTime.Now.Date;

		var streak = await AppState.Db.GetStreakAsync(_habitId, today);
		StreakLabel.Text = $"{streak} days";

		await LoadLast7Async(today);
		await Load30Async(today);
		await LoadCalendarAsync(_currentMonth);
    }
	
	private async Task LoadLast7Async(DateTime today) {
		
		Last7Container.Children.Clear();
		var from = today.AddDays(-6);
		var doneDates = await AppState.Db.GetDoneDatesAsync(_habitId, from, today);
		var doneSet = doneDates.Select(d => d.Date).ToHashSet();

		for (int i = 0; i < 7; i++) {
			var d = from.AddDays(i);
			var isDone = doneSet.Contains(d);

			var frame = new Frame
			{
				WidthRequest = 40,
				HeightRequest = 40,
				CornerRadius = 20,
				Padding = 0,
				BorderColor = Colors.Transparent,
				BackgroundColor = isDone ? Color.FromArgb("#4ade80") : Color.FromArgb("#333"),
				HasShadow = false,
				Content = new Label
				{
					Text = d.Day.ToString(),
					FontSize = 14,
					TextColor = isDone ? Colors.Black : Color.FromArgb("#666"),
					HorizontalOptions = LayoutOptions.Center,
					VerticalOptions = LayoutOptions.Center,
					FontAttributes = isDone ? FontAttributes.Bold : FontAttributes.None
				}
			};

            Last7Container.Children.Add(frame);

        }
    }

	private async Task Load30Async(DateTime today) {
		var from = today.AddDays(-29); 
		var doneDates = await AppState.Db.GetDoneDatesAsync(_habitId, from, today);

		var doneCount = doneDates.Count;
		var progress = doneCount / 30.0;

		Rate30Bar.Progress = progress;
		Rate30Text.Text = $"{doneCount}/30 ({(int)Math.Round(progress * 100)}%)";
	}

	private async Task LoadCalendarAsync(DateTime month)
	{
		CalendarGrid.Children.Clear();
		CalendarGrid.RowDefinitions.Clear();

		MonthLabel.Text = month.ToString("MMMM yyyy");

		var firstDay = new DateTime(month.Year, month.Month, 1);
		var lastDay = firstDay.AddMonths(1).AddDays(-1);

		int startDayOfWeek = ((int)firstDay.DayOfWeek + 6) % 7;

		var doneDates = await AppState.Db.GetDoneDatesAsync(_habitId, firstDay, lastDay);
		var doneSet = doneDates.Select(d => d.Date).ToHashSet();

		int row = 0;
		int col = startDayOfWeek;

		for (int day = 1; day <= lastDay.Day; day++)
		{
			var currentDate = new DateTime(month.Year, month.Month, day);
			var isDone = doneSet.Contains(currentDate);
			var isToday = currentDate == DateTime.Now.Date;
			var isFuture = currentDate > DateTime.Now.Date;

			if (CalendarGrid.RowDefinitions.Count <= row)
				CalendarGrid.RowDefinitions.Add(new RowDefinition { Height = 40 });

            var frame = new Frame
            {
                WidthRequest = 40,
                HeightRequest = 40,
                CornerRadius = 20,
                Padding = 0,
                BorderColor = isToday ? Color.FromArgb("#6366f1") : Colors.Transparent,
                BackgroundColor = isDone ? Color.FromArgb("#4ade80") :
                                 isFuture ? Color.FromArgb("#1a1a1a") :
                                 Color.FromArgb("#333"),
                HasShadow = false,
                Content = new Label
                {
                    Text = day.ToString(),
                    FontSize = 14,
                    TextColor = isDone ? Colors.Black :
                               isFuture ? Color.FromArgb("#444") :
                               Color.FromArgb("#999"),
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    FontAttributes = isDone ? FontAttributes.Bold : FontAttributes.None
                }
            };

            Grid.SetRow(frame, row);
            Grid.SetColumn(frame, col);
            CalendarGrid.Children.Add(frame);

            col++;
            if (col > 6)
            {
                col = 0;
                row++;
            }
        }
    }

    private async void OnPrevMonthClicked(object sender, EventArgs e)
    {
        _currentMonth = _currentMonth.AddMonths(-1);
        await LoadCalendarAsync(_currentMonth);
    }

    private async void OnNextMonthClicked(object sender, EventArgs e)
    {
        _currentMonth = _currentMonth.AddMonths(1);
        await LoadCalendarAsync(_currentMonth);
    }
}