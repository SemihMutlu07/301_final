namespace HabitTracker.Views;

public partial class HabitsPage : ContentPage
{
    public HabitsPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await RefreshAsync();
    }

    private async Task RefreshAsync()
    {
        HabitsList.ItemsSource = await AppState.Db.GetHabitsAsync();
    }

    private async void OnAddClicked(object sender, EventArgs e)
    {
        var name = await DisplayPromptAsync("New habit", "Name:");
        await AppState.Db.AddHabitAsync(name ?? "");
        await RefreshAsync();
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        if (sender is Button btn && int.TryParse(btn.CommandParameter?.ToString(), out var id))
        {
            await AppState.Db.DeleteHabitAsync(id);
            await RefreshAsync();
        }
    }
}
