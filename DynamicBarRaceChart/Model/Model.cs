using System.ComponentModel;

namespace DynamicBarRaceChart
{
    public class Model
    {
        private int ranking;
        public int Ranking
        {
            get
            {
                return ranking;
            }
            set
            {
                ranking = value;
                OnPropertyChanged(nameof(Ranking));
            }
        }

        private int index;
        public int Index
        {
            get
            {
                return index;
            }
            set
            {
                ranking = value;
                OnPropertyChanged(nameof(Index));
            }
        }


        string xString;
        public string XString
        {
            get
            {
                return xString;
            }
            set
            {
                xString = value;
                OnPropertyChanged(nameof(XString));
            }
        }

        double xValue;
        public double XValue
        {
            get
            {
                return xValue;
            }
            set
            {
                xValue = value;
                OnPropertyChanged(nameof(XValue));
            }
        }
        double yValue;
        public double YValue
        {
            get
            {
                return yValue;
            }
            set
            {
                yValue = value;
                OnPropertyChanged(nameof(YValue));
            }
        }

        public Brush itemColor;
        public Brush ItemColor
        {
            get
            {
                return itemColor;
            }
            set
            {
                itemColor = value;
                OnPropertyChanged(nameof(ItemColor));
            }
        }

        List<Brush> GetBrushes()
        {
            return new List<Brush>
    {
        new SolidColorBrush(Color.FromArgb("#00bdae")),
        new SolidColorBrush(Color.FromArgb("#404041")),
        new SolidColorBrush(Color.FromArgb("#357cd2")),
        new SolidColorBrush(Color.FromArgb("#e56590")),
        new SolidColorBrush(Color.FromArgb("#f8b883")),
        new SolidColorBrush(Color.FromArgb("#70ad47")),
        new SolidColorBrush(Color.FromArgb("#dd8abd")),
        new SolidColorBrush(Color.FromArgb("#7f84e8")),
        new SolidColorBrush(Color.FromArgb("#7bb4eb")),
        new SolidColorBrush(Color.FromArgb("#ea7a57")),
        new SolidColorBrush(Color.FromArgb("#F7CB05"))
    };
        }

        public Model(int i, string xName, double x, double y)
        {
            Index = i;
            XString = xName;
            XValue = x;
            YValue = y;
            var brush = GetBrushes();
            ItemColor = brush[i % brush.Count];
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
