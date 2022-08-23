namespace BoomerangCard.Maui;

public partial class CardView : ContentView
{
    public static readonly BindableProperty CardColorProperty =
        BindableProperty.Create(nameof(CardColor), typeof(Color), typeof(CardView), Colors.White, BindingMode.TwoWay);

    public CardView()
    {
        InitializeComponent();
        BindingContext = this;
    }

    public Color CardColor
    {
        get => (Color)GetValue(CardColorProperty);
        set => SetValue(CardColorProperty, value);
    }
}