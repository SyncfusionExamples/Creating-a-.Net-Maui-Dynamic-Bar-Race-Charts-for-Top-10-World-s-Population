
using Microsoft.Maui.Graphics;
using Syncfusion.Maui.Charts;

namespace DynamicBarRaceChart
{
    public class BarRaceSegment : ColumnSegment
    {
        public Model Item { get; set; }

        private string xString = string.Empty;

        protected override void Draw(ICanvas canvas)
        {
            var series = (Series as BarRaceSeries);

            if (series != null && series.ChartSegments != null && series.ChartSegments.Count > 0)
            {
                var index = series.ChartSegments.IndexOf(this);

                if (index >= 0 && series.PreviousSegments != null && series.PreviousSegments.Count > 0 && series.PreviousSegments.Count > index)
                {
                    var previousSegment = series.PreviousSegments[index] as BarRaceSegment;
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

                                canvas.DrawString(xString, (float)rect.Left - 5, (float)Math.Round(rect.Center.Y), HorizontalAlignment.Right);

                                if (AnimatedValue == 1)
                                    canvas.DrawString(Item.YValue.ToString("#,###,###,###"), (float)rect.Right + 2, (float)rect.Center.Y + 2, HorizontalAlignment.Left);
                                else
                                    canvas.DrawString(GetColumnDynamicAnimationValue(AnimatedValue, previousSegment.Item.YValue, Item.YValue).ToString("#,###,###,###"), (float)rect.Right + 2, (float)rect.Center.Y + 2, HorizontalAlignment.Left);

                            }
                            else
                            {

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

                            canvas.DrawString(previousSegment.xString, (float)rect.Left - 5, (float)Math.Round(rect.Center.Y), HorizontalAlignment.Right);

                            canvas.DrawString(previousSegment.Item.YValue.ToString("#,###,###,###"), (float)rect.Right + 2, (float)rect.Center.Y + 2, HorizontalAlignment.Left);

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
    }
}
