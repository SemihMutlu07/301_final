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
    }

    private async void OnDoneChanged(object sender, CheckedChangedEventArgs e)
    {
        if (sender is CheckBox cb && cb.BindingContext is TodayItem item)
        {
            await AppState.Db.SetDoneAsync(item.HabitId, Today, e.Value);
            item.IsDone = e.Value; 
        }
    }

    private async void OnOpenDetailClicked(object sender, EventArgs e)
    {
        if (sender is Button b && b.BindingContext is TodayItem item)
            await Navigation.PushAsync(new HabitDetailPage(item.HabitId, item.Name));
    }



}
