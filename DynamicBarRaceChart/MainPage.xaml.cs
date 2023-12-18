using Syncfusion.Maui.Charts;

namespace DynamicBarRaceChart
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
        }

        private void Button_Clicked_1(object sender, EventArgs e)
        {
            viewModel.StopTimer();
        }

        private void NumericalAxis_LabelCreated(object sender, ChartAxisLabelEventArgs e)
        {
            e.Label = (e.Position / 1000000).ToString() + "M";
        }

        private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
        {
            if (text.Text == "Start")
            {
                viewModel.StartTimer();

            }
            else
            {
                viewModel.StopTimer();

            }
        }
    }

}
