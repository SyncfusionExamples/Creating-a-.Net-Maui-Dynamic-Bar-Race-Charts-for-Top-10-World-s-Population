# Creating a .Net Maui Dynamic Bar Race Charts for Top 10 World's Population
Bar Race Charts are effective tools for demonstrating changes in data over a period. Syncfusion .Net  MAUI Charts can easily create dynamic and interactive Bar Race Charts that captivate audiences and convey valuable insights by summarizing a large data set in a live visual bar graph, with bars racing to the top based on rankings.
This sample demonstrates how to create a .Net Maui Dynamic Bar Race Charts for Top 10 World’s Population

![BarRaceWindowsgif](https://github.com/SyncfusionExamples/Creating-a-.Net-Maui-Dynamic-Bar-Race-Charts-for-Top-10-World-s-Population/assets/105496706/dcf4ee46-35ae-4435-a955-3d1f017d4df9)


## Step 1: Gather data for the world population 
Before creating the chart, we need to gather the world population. For this demo, we are getting data from 1960 to 2021.

## Step 2: Prepare the data for the bar race chart
Create the Model class for holding population data with the help of the XValue, YValue, Index, XString, and others properties.

Refer to the following code example.

**C#**
```
public Model(int i, string xName, double x, double y)
{
    Index = i;
    XString = xName;
    XValue = x;
    YValue = y;
    ItemColor = brush[i % brush.Count];
}
```

Generate the data collection with the help of the ViewModel class and Data property.
Then, convert the CSV data to a collection of population data using the ReadCSV method.

**C#**
```
Public Ienumerable<Model> ReadCSV()
{
    Assembly executingAssembly = typeof(App).GetTypeInfo().Assembly;
    Stream inputStream = executingAssembly.GetManifestResourceStream(“BarRaceChart.Resources.Raw.populationdata.csv”);

    if (inputStream == null)
    {
        return new List<Model>();
    }

    string? Line;
    List<string> lines = new List<string>();

    using StreamReader reader = new StreamReader(inputStream);
    while ((line = reader.ReadLine()) != null)
    {
        lines.Add(line);
    }
    int index = -1;
    double previousYear = 0;
    string previousString = string.Empty;

    return lines.Select(line =>
    {
        string[] data = line.Split(‘,’);
        string currentString = data[0];
        double currentYear = Convert.ToDouble(data[1]);
        if (currentString != previousString)
        {
            if (currentYear == previousYear)
            {
                index = -1;
            }
            index = index + 1;
            previousString = currentString;
        }

        return new Model(index, data[0], currentYear, Convert.ToDouble(data[2]));
    });
}
```

Data on the entire population of all nations is divided, filtered annually, and stored in the dataCollection field.

**C#**
```
Public ViewModel()
{
    var models = new List<Model>(ReadCSV());
    dataCollection = new List<List<Model>>();
    StartYear = models.First().Xvalue;
    EndYear = models.Last().Xvalue;
    int count = 0;
    var previousData = new List<Model>();
    for (double I = StartYear; I <= EndYear; i++)
    {
        Ienumerable<Model> data = models.Where(x => x.Xvalue == i).OrderByDescending(x => x.Yvalue).Take(11);
        dataCollection.Insert(count, UpdateDataIndex(I, previousData, data));
        count++;
        previousData = data.ToList();
    }
            
    Data = dataCollection[0];
    Year = dataCollection[0].First().Xvalue;
}
```

## Step 3: Configure Syncfusion .NET MAUI Cartesian Charts
Next, configure the Syncfusion .NET MAUI Cartesian Charts control using this documentation.

Refer to the following code example.

**XAML**
```
<chart:SfCartesianChart IsTransposed=”True” >
 <chart:SfCartesianChart.Xaxes>
  <chart:NumericalAxis />        
 </chart:SfCartesianChart.Xaxes>

 <chart:SfCartesianChart.Yaxes>
  <chart:NumericalAxis />       
 </chart:SfCartesianChart.Yaxes>
</chart:SfCartesianChart>
```

## Step 4: Create an Amazing Bar Racing Chart
Frist, create a BarRaceSegment class inherited by ColumnSegment for .NET MAUI Animating Bar Race Chart using the segment's Draw method.

**C#**
```
public class BarRaceSegment : ColumnSegment, ITextElement
{
    public Model Item { get; set; }
    public FontAttributes FontAttributes => FontAttributes.None;

    public string FontFamily => Font.Family != null ? Font.Family : string.Empty;

    public double FontSize => 12;
    public Microsoft.Maui.Font Font => Microsoft.Maui.Font.Default;

    private Color _textColor = Colors.Black;
    public Color TextColor { get => _textColor; set => _textColor = value; }

    private string xString = string.Empty;

    protected override void Draw(ICanvas canvas)
    {
        var series = (Series as BarRaceSeries);

        // Do animating bar racing customization here
        if (series != null && series.ChartSegments != null && series.ChartSegments.Count > 0)
        {
            var index = series.ChartSegments.IndexOf(this);

            if (index >= 0 && series.PreviousSegments != null && series.PreviousSegments.Count > 0 && series.PreviousSegments.Count > index)
            {
                BarRaceSegment? previousSegment = series.PreviousSegments[index] as BarRaceSegment;
                if (previousSegment != null)
                {
                    float previousTop = previousSegment.Top;
                    float previousBottom = previousSegment.Bottom;
                    float previousLeft = previousSegment.Left;
                    float previousRight = previousSegment.Right;

                    if (AnimatedValue > 0)
                    {
                        float rectTop = GetColumnDynamicAnimationValue(AnimatedValue, previousTop, Top == 0 ? previousTop : Top);
                        float rectBottom = GetColumnDynamicAnimationValue(AnimatedValue, previousBottom, Bottom);
                        float rectLeft = GetColumnDynamicAnimationValue(AnimatedValue, previousLeft, Left);
                        float rectRight = GetColumnDynamicAnimationValue(AnimatedValue, previousRight, Right);


                        if (AnimatedValue >= 0.5)
                            xString = Item.XString;
                        else
                            xString = previousSegment.Item.XString;

                        if (!float.IsNaN(rectLeft) && !float.IsNaN(rectTop) && !float.IsNaN(rectRight) && !float.IsNaN(rectBottom))
                        {

                            canvas.Alpha = Opacity;
                            CornerRadius cornerRadius = series.CornerRadius;

                            var rect = new Rect() { Left = rectLeft, Top = rectTop, Right = rectRight, Bottom = rectBottom };

                            canvas.SetFillPaint(AnimatedValue >= 0.5 ? Item.ItemColor : previousSegment.Item.itemColor, rect);

                            if (cornerRadius.TopLeft > 0 || cornerRadius.TopRight > 0 || cornerRadius.BottomLeft > 0 || cornerRadius.BottomRight > 0)
                            {
                                canvas.FillRoundedRectangle(rect, cornerRadius.TopLeft, cornerRadius.TopRight, cornerRadius.BottomLeft, cornerRadius.BottomRight);
                            }
                            else
                            {
                                canvas.FillRectangle(rect);
                            }

                            float textWidth = (float)series.ActualYAxis.PlotOffsetStart;
                            canvas.DrawText(xString, new Rect(rect.Left - textWidth - 2, rect.Top - 2, textWidth, rect.Height), HorizontalAlignment.Right, VerticalAlignment.Center, this);

                            string yValueString = AnimatedValue == 1 ? Item.YValue.ToString("#,###,###,###") : GetColumnDynamicAnimationValue(AnimatedValue, previousSegment.Item.YValue, Item.YValue).ToString("#,###,###,###");

                            canvas.DrawText(yValueString, new Rect(rect.Right + 2, rect.Top - 2, textWidth, rect.Height), HorizontalAlignment.Left, VerticalAlignment.Center, this);

                        }
                    }
                    else
                    {
                        canvas.Alpha = Opacity;
                        CornerRadius cornerRadius = series.CornerRadius;

                        var rect = new Rect() { Left = previousSegment.Left, Top = previousSegment.Top, Right = previousSegment.Right, Bottom = previousSegment.Bottom };

                        canvas.SetFillPaint(AnimatedValue >= 0.5 ? Item.ItemColor : previousSegment.Item.itemColor, rect);

                        if (cornerRadius.TopLeft > 0 || cornerRadius.TopRight > 0 || cornerRadius.BottomLeft > 0 || cornerRadius.BottomRight > 0)
                        {
                            canvas.FillRoundedRectangle(rect, cornerRadius.TopLeft, cornerRadius.TopRight, cornerRadius.BottomLeft, cornerRadius.BottomRight);
                        }
                        else
                        {
                            canvas.FillRectangle(rect);
                        }

                        float textWidth = (float)series.ActualYAxis.PlotOffsetStart;
                        canvas.DrawText(xString, new Rect(rect.Left - textWidth - 2, rect.Top - 2, textWidth, rect.Height), HorizontalAlignment.Right, VerticalAlignment.Center, this);

                        string yValueString = AnimatedValue == 1 ? previousSegment.xString : previousSegment.Item.YValue.ToString("#,###,###,###");

                        canvas.DrawText(yValueString, new Rect(rect.Right + 2, rect.Top - 2, textWidth, rect.Height), HorizontalAlignment.Left, VerticalAlignment.Center, this);
                    }
                }
            }

        }

    }

    private float GetColumnDynamicAnimationValue(float animationValue, double oldValue, double currentValue)
    {
        if (!double.IsNaN(oldValue) && !double.IsNaN(currentValue))
        {
            return (float)((currentValue > oldValue) ?
                oldValue + ((currentValue - oldValue) * animationValue)
                : oldValue - ((oldValue - currentValue) * animationValue));
        }
        else
        {
            return double.IsNaN(oldValue) ? (float)currentValue * animationValue : (float)(oldValue - (oldValue * animationValue));
        }
    }

    public void OnFontFamilyChanged(string oldValue, string newValue)
    {

    }

    public void OnFontSizeChanged(double oldValue, double newValue)
    {

    }

    public double FontSizeDefaultValueCreator()
    {
        return 0;
    }

    public void OnFontAttributesChanged(FontAttributes oldValue, FontAttributes newValue)
    {

    }

    public void OnFontChanged(Microsoft.Maui.Font oldValue, Microsoft.Maui.Font newValue)
    {

    }
}
```

Then, create a BarRaceSeries class inherited by ColumnSeries for .Net MAUI DemoGraphics Statistic Chart.

**C#**
```
public class BarRaceSeries : ColumnSeries
{
    public List<ChartSegment> PreviousSegments { get; set; }

    public List<ChartSegment> ChartSegments { get; set; }

    public BarRaceSeries()
    {
        
    }

    protected override ChartSegment CreateSegment()
    {
        return new BarRaceSegment();
    }

}
```

## Step 5: Bind data to the Bar Race Chart
To visualize the top 10 countries in world population statistics for a given year, implement the BarRaceSeries and bind the data to it.

Refer to the following code example.

**XAML**
```
<local:BarRaceSeries CornerRadius="0,100,0,100" EnableAnimation="True" ItemsSource="{Binding Data}" XBindingPath="Ranking" YBindingPath="YValue" >
</local:BarRaceSeries>
```

## Step 6: Customize the chart appearance
We can enhance the appearance of the charts by changing the axis elements’ appearance and adding titles to the charts.

Refer to the following code example to customize the chart title.

**XAML**
```
 <chart:SfCartesianChart.Title>
     <HorizontalStackLayout Margin="{OnPlatform iOS='0,5,0,5', Android='20,5,0,5', Default= '10,10,0,10'}">
         <BoxView Margin="{OnPlatform iOS='0,10,0,5', Android='0,0,0,0', Default= '10,0,0,10'}" BackgroundColor="DarkBlue" WidthRequest="20" HeightRequest="{OnPlatform iOS='60', Android='50', Default= '70'}"/>
         <VerticalStackLayout HeightRequest="{OnPlatform iOS='40', Android='40', Default= '70'}"  Margin="{OnPlatform iOS='0,0,0,10', Android='0,-10,0,0', Default= '10,0,0,10'}">
             <Label VerticalOptions="Start" Text="Demographic Statistics | Top 10 World's Total Population" Padding="{OnPlatform iOS='5,0,5,0', Android='5,0,5,0', Default= '10,5,5,0'}" FontSize="20" FontAttributes="Bold" >
             </Label>
             <Label VerticalOptions="Start" Text="Since 1960 to 2021"  Padding="{OnPlatform iOS='5,5,5,0', Android='5,0,5,0', Default= '10,5,5,0'}" FontSize="15" FontAttributes="Bold" >
             </Label>
         </VerticalStackLayout>
     </HorizontalStackLayout>
 </chart:SfCartesianChart.Title>
```

Configure the axis and modify the axis elements, as shown in the following code example.

**XAML**
```
<chart:SfCartesianChart.XAxes>
    <chart:NumericalAxis IsInversed="True" IsVisible="False"  AutoScrollingDelta="10" AutoScrollingMode="Start" Interval="1" ShowMajorGridLines="False"  />
</chart:SfCartesianChart.XAxes>
<chart:SfCartesianChart.YAxes>
    <chart:NumericalAxis  PlotOffsetStart="110" LabelCreated="NumericalAxis_LabelCreated"  RangePadding="AppendInterval"  >
        <chart:NumericalAxis.MajorTickStyle>
            <chart:ChartAxisTickStyle TickSize="0" />
        </chart:NumericalAxis.MajorTickStyle>
        <chart:NumericalAxis.AxisLineStyle>
            <chart:ChartLineStyle StrokeWidth="0" />
        </chart:NumericalAxis.AxisLineStyle>
    </chart:NumericalAxis>
</chart:SfCartesianChart.YAxes>
```

## Step 7:  Configure the Play/Pause function for animating the bar race chart
We must include the following code to achieve this: a play/stop button and an interactive progress bar.

**XAML**
```
<Border Stroke="Transparent" Margin="{OnPlatform iOS='0,5,0,0', Android='0,5,0,0', Default= '20,0,0,10'}" Grid.Row="1" Padding="5">
    <Grid WidthRequest="{OnPlatform iOS='810',Android='810', WinUI='1200'}">
        <Grid.ColumnDefinitions>
            <ColumnDefinition Width="50" />
            <ColumnDefinition />
        </Grid.ColumnDefinitions>

        <Border Grid.Column="0" WidthRequest="45" HeightRequest="{OnPlatform Android='40', Default='45'}" Padding="{OnPlatform Default='0,5,0,0', Android='0'}" Stroke="Blue" BackgroundColor="White">
            <Border.GestureRecognizers>
                <TapGestureRecognizer Tapped="TapGestureRecognizer_Tapped"/>
            </Border.GestureRecognizers>
            <Border.StrokeShape>
                <RoundRectangle CornerRadius="10" />
            </Border.StrokeShape>
            <Label x:Name="text" Text="{Binding ProgressString}" TextColor="Blue" HorizontalOptions="Center" VerticalOptions="Center" />
        </Border>
        <progress:SfLinearProgressBar Margin="{OnPlatform iOS='0,20,0,0', Android='0,20,0,0', Default= '10,20,0,0'}" Grid.Column="1" Minimum="{Binding StartYear}" Maximum="{Binding EndYear}" Progress="{Binding Year}" 
                         TrackHeight="10" 
                         TrackCornerRadius="5"
                         ProgressHeight="10"
                         ProgressCornerRadius="5" />
    </Grid>
</Border>
```

Now we attach the method from above to the Play/Pause parts:

**C#**
```
        public void Pause()
        {
            canStopTimer = true;
            ProgressString = "Play";
        }

        public async void Play()
        {
            ProgressString = "Pause";
            await Task.Delay(500);

            if (Application.Current != null)
                Application.Current.Dispatcher.StartTimer(new TimeSpan(0, 0, 0, 1, 500), UpdateData);

            canStopTimer = false;
        }
```

Finally, we can use the series ItemsSource to continuously update the chart with population data from the top 10 countries each year.

**C#**
```
        private bool UpdateData()
        {
            if(dataCollection.Count < count + 1)
            {
                ProgressString = "Play";
                count = 0;
                return false;
            }

            if (canStopTimer) return false;

            MainThread.InvokeOnMainThreadAsync(() =>
            {
                Data = dataCollection[count];
                Year = dataCollection[count].First().XValue;
                count++;
            });

            return true;
        }
```
