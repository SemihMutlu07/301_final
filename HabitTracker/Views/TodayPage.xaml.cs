using System.Linq;
using HabitTracker.Models;
namespace HabitTracker.Views;

public partial class TodayPage : ContentPage
{
    public TodayPage()
    {
        InitializeComponent();
    }

    private DateTime Today => DateTime.Now.Date;

    private class TodayItem
    {
        public int HabitId { get; set; }
        public string Name { get; set; } = "";
        public bool IsDone { get; set; }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadAsync();
    }

    private async Task LoadAsync()
    {
        var habits = await AppState.Db.GetHabitsAsync();

        var items = new List<TodayItem>();
        foreach (var h in habits)
        {
            var done = await AppState.Db.IsDoneAsync(h.Id, Today);
            items.Add(new TodayItem { HabitId = h.Id, Name = h.Name, IsDone = done });
        }

        TodayList.ItemsSource = items;

        var doneCount = items.Count(i => i.IsDone);
        ProgressLabel.Text = $"Done: {doneCount}/{items.Count}";

        EmptyContainer.IsVisible = items.Count == 0;
        TodayList.IsVisible = items.Count > 0;
        ProgressLabel.IsVisible = items.Count > 0;
    }

    private async void OnDoneChanged(object sender, CheckedChangedEventArgs e)
    {
        if (sender is CheckBox cb && cb.BindingContext is TodayItem item)
        {
            await AppState.Db.SetDoneAsync(item.HabitId, Today, e.Value);
            item.IsDone = e.Value;

            var list = TodayList.ItemsSource as IEnumerable<TodayItem>;
            var total = list?.Count() ?? 0;
            var done = list?.Count(i => i.IsDone) ?? 0;
            ProgressLabel.Text = $"Done: {done}/{total}";
        } 
    }

    private async void OnOpenDetailClicked(object sender, EventArgs e)
    {
        if (sender is Button b && b.BindingContext is TodayItem item)
            await Navigation.PushAsync(new HabitDetailPage(item.HabitId, item.Name));
    }

    private async void OnAddHabitClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//Habits");
    }

}
