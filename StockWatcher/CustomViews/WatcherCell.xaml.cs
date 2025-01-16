using StockWatcher.Core.Watchers;

namespace StockWatcher.CustomViews;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class WatcherCell : ContentView
{
    private IWatcher _watcher;

    public WatcherCell()
    {
        InitializeComponent();
    }

    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();
        if (BindingContext == null) return;
        _watcher = (IWatcher)BindingContext;
        InitCell();
    }

    private void InitCell()
    {
        DirectionImg.Source = _watcher.Type == Enums.WatcherType.Up ? "Up.png" : "Down.png";
        SecIdLabel.Text = _watcher.SecId;
    }

    public static readonly BindableProperty SecIdProperty =
      BindableProperty.Create(nameof(SecId), typeof(string), typeof(WatcherCell));
    public string SecId
    {
        get { return (string)GetValue(SecIdProperty); }
        set { SetValue(SecIdProperty, value); }
    }
}
