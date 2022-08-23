namespace BoomerangCard.Maui;

public partial class MainPage : ContentPage
{
    private bool _isflying = false;
    private bool _isTravelUp;
    private View _topview;
    private double _travelDistance;
    private DateTime _travelStart;

    public MainPage()
    {
        InitializeComponent();
        BindingContext = this;
    }

    public double AniHeight { get; set; } = 240;
    public int AniSpins { get; set; } = 1;

    private async void Button_Clicked(object sender, EventArgs e)
    {
        if (_isflying)
        {
            return;
        }

        LabelStatus.Text = "Status: Flying";
        LabelPower.Text = "Power: As slider below";

        await FlyBoomerang(AniSpins, AniHeight);

        _isflying = false;
    }

    private async Task FlyBoomerang(int spins, double height)
    {
        _isflying = true;

        var cardList = gridbox.Children.Cast<CardView>().OrderBy(x => x.ZIndex).ToList();
        var topCardIndex = cardList.Count - 1;

        //fly starts
        var viewFly = cardList[topCardIndex];
        //viewFly.RotateTo(spins * (isRightSide ? -360 : 360), 800);
        viewFly.RelRotateTo(spins * 360, 800);
        viewFly.ScaleTo(1 - 0.05 * topCardIndex, 800);

        for (int i = 0; i < topCardIndex; i++)
        {
            cardList[i].ScaleTo(cardList[i].Scale + 0.05, 200);
            cardList[i].TranslateTo(0, 10, 200);
            cardList[i].ZIndex = i + 2;
        }

        await viewFly.TranslateTo(0, -height, 400, Easing.CubicOut);
        viewFly.ZIndex = 1;
        viewFly.Margin = new Thickness(0, 0, 0, 20 * topCardIndex);
        await viewFly.TranslateTo(0, 0, 400, Easing.CubicIn);

        //refresh positions when fly ends
        for (int i = 0; i < topCardIndex; i++)
        {
            cardList[i].TranslationY = 0;
            cardList[i].Margin = new Thickness(0, 0, 0, cardList[i].Margin.Bottom - 20);
        }
    }

    private double GetFlyCapacity(double velocity)
    {
        // v < 0.5 means least spin, which is 0.1
        // v > 1.5 means 100%
        return (velocity < 0.5 ? 0 : (velocity > 1.5 ? 0.9 : ((velocity - 0.5) * 0.9))) + 0.1;
    }

    private async void PanGestureRecognizer_PanUpdated(object sender, PanUpdatedEventArgs e)
    {
        if (_isflying)
        {
            return;
        }

        switch (e.StatusType)
        {
            case GestureStatus.Started:
                if (_topview == null)
                {
                    _topview = sender as View;
                }

                if (_topview.Rotation != -10)
                {
                    _topview.Rotation = -10;
                }
                break;

            case GestureStatus.Running:
                var offset = e.TotalY;
                LabelStatus.Text = "Status: " + e.StatusType.ToString();
                //LabelY.Text = "Movement: " + Math.Round(e.TotalY, 1);

                if (offset > 0)
                {
                    _travelDistance = 0;
                    _isTravelUp = false;
                }
                else
                {
                    if (!_isTravelUp)
                    {
                        _travelStart = DateTime.Now;
                        _isTravelUp = true;
                    }
                    _travelDistance -= offset;
                }

                _topview.TranslationX += e.TotalX;
                _topview.TranslationY += offset;
                break;

            case GestureStatus.Completed:
                if (_isTravelUp && _topview.TranslationY < 0)
                {
                    //have to throw higher than original position to fly
                    var portion = GetFlyCapacity(_travelDistance / (int)DateTime.Now.Subtract(_travelStart).TotalMilliseconds);
                    LabelPower.Text = $"Power: {portion:0 %}";
                    await FlyBoomerang((int)(10 * portion), _travelDistance + 300 * portion + 100);
                }
                else if (_topview.TranslationY > 100)
                {
                    LabelPower.Text = "Power: 100%";
                    await FlyBoomerang(10, 450);
                }
                else
                {
                    _topview.TranslationX = 0;
                    _topview.TranslationY = 0;
                }

                _topview.Rotation = 0;
                LabelPower.Text = "Power: 00%";
                _topview = null;

                _travelDistance = 0;
                _isTravelUp = false;
                _isflying = false;
                break;
        }
    }

    private void Spinslider_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        ((Slider)sender).Value = (int)e.NewValue;
    }
}