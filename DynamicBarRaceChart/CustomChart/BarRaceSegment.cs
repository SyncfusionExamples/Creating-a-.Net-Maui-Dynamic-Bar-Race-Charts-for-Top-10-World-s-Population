using Syncfusion.Maui.Charts;
using Syncfusion.Maui.Graphics.Internals;

namespace DynamicBarRaceChart
{
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
}
