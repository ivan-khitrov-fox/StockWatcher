namespace StockWatcher.CustomViews;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class WatcherCell : ContentView
{
    public WatcherCell()
    {
        InitializeComponent();
    }

    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();
        if (BindingContext == null) return;

        InitCell();
    }

    private void InitCell()
    {
    }

    public static readonly BindableProperty SecIdProperty =
      BindableProperty.Create(nameof(SecId), typeof(string), typeof(WatcherCell));
    public string SecId
    {
        get { return (string)GetValue(SecIdProperty); }
        set { SetValue(SecIdProperty, value); }
    }
}
