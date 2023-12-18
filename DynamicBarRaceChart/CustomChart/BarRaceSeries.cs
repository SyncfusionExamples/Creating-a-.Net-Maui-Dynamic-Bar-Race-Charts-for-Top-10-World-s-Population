using Syncfusion.Maui.Charts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBarRaceChart
{
    public class BarRaceSeries : ColumnSeries
    {
        public BarRaceSeries()
        {
            PropertyChanged += AnimatedSeries_PropertyChanged;
        }
        int segment = 0;
        private void AnimatedSeries_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ItemsSource))
            {
                PreviousSegments = ChartSegments;
            }

        }

        public List<ChartSegment> PreviousSegments { get; set; }

        public List<ChartSegment> ChartSegments { get; set; }

        protected override void DrawSeries(ICanvas canvas, ReadOnlyObservableCollection<ChartSegment> segments, RectF clipRect)
        {
            if (ItemsSource == null)
                return;

            if (ItemsSource is IEnumerable<Model> items)
            {
                ChartSegments = segments.ToList();

                if (PreviousSegments == null)
                {
                    PreviousSegments = ChartSegments;
                }

                for (int i = 0; i < segments.Count; i++)
                {
                    if (ChartSegments[i] is BarRaceSegment segment)
                    {
                        if (PreviousSegments.Count > i && PreviousSegments[i] is BarRaceSegment previousItem)
                            segment.Item = previousItem.Item;

                        segment.Item = items.ElementAt(i);


                    }
                }
            }

            base.DrawSeries(canvas, segments, clipRect);

        }

        protected override ChartSegment CreateSegment()
        {
            return new BarRaceSegment();
        }
    }
}
