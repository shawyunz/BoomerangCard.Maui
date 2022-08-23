namespace BoomerangCard.Maui;

public partial class MainPage : ContentPage
{
    private bool isflying = false;

    private View topview;

    public MainPage()
    {
        InitializeComponent();
        BindingContext = this;
    }

    public double AniHeight { get; set; } = 240;
    public int AniSpins { get; set; } = 1;

    private async void Button_Clicked(object sender, EventArgs e)
    {
        await FlyBoomerang();

        isflying = false;
    }

    private async Task FlyBoomerang()
    {
        isflying = true;
        var viewList = GetCardList();

        var view1 = viewList[0];
        var view2 = viewList[1];

        view1.RelRotateTo(AniSpins * 360, 800);
        view1.ScaleTo(0.95, 800);

        view2.ScaleTo(1, 200);
        view2.TranslateTo(0, 10, 200);

        await view1.TranslateTo(0, -AniHeight, 400, Easing.CubicOut);
        //gridbox.LowerChild(view1);
        view2.ZIndex = 2;
        view1.ZIndex = 1;
        view1.Margin = new Thickness(0, 0, 0, 20);
        await view1.TranslateTo(0, 0, 400, Easing.CubicIn);

        view1.Scale = 0.95;
        view2.Scale = 1;
    }

    private List<View> GetCardList()
    {
        var cardList = new List<View>();
        if (card1.Scale == 1)
        {
            cardList.Add(card1);
            cardList.Add(card2);
        }
        else
        {
            cardList.Add(card2);
            cardList.Add(card1);
        }

        return cardList;
    }

    private async void PanGestureRecognizer_PanUpdated(object sender, PanUpdatedEventArgs e)
    {
        if (isflying)
        {
            return;
        }

        if (topview == null)
        {
            topview = GetCardList()[0];
        }

        var offset = e.TotalY;
        LabelStatus.Text = "Status: " + e.StatusType.ToString();
        //LabelY.Text = "Movement: " + Math.Round(e.TotalY, 1);

        switch (e.StatusType)
        {
            case GestureStatus.Started:
                if (topview.Rotation != -10)
                {
                    topview.Rotation = -10;
                }
                break;

            case GestureStatus.Running:
                topview.TranslationX += e.TotalX;
                topview.TranslationY += offset;
                break;

            case GestureStatus.Completed:
                if (topview.TranslationY < 0)
                {
                    await FlyBoomerang();
                }
                else if (topview.TranslationY > 100)
                {
                    AniHeight = 350;
                    AniSpins = 6;
                    await FlyBoomerang();
                    AniHeight = 240;
                    AniSpins = 1;
                }
                else
                {
                    topview.TranslationX = 0;
                    topview.TranslationY = 10;
                }

                topview.Rotation = 0;
                topview = null;

                isflying = false;
                break;
        }
    }

    private void Spinslider_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        ((Slider)sender).Value = (int)e.NewValue;
    }
}