using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Buschmann.Windows.Controls
{
    [TemplatePart(Name = Banner, Type = typeof(ItemsControl))]
    [TemplatePart(Name = CrawlCanvas, Type = typeof(Canvas))]
    public class CrawlList : Control
    {
        private const string Banner = "banner";
        private const string CrawlCanvas = "crawlCanvas";

        private ItemsControl _banner;
        private Canvas _crawlCanvas;

        public CrawlList()
        {
            DefaultStyleKey = typeof(CrawlList);
        }

        public override void OnApplyTemplate()
        {
            _banner = Template.FindName(Banner, this) as ItemsControl ?? new ItemsControl();
            _crawlCanvas = Template.FindName(CrawlCanvas, this) as Canvas ?? new Canvas();

            base.OnApplyTemplate();

            DataTemplate itemTemplate = ItemTemplate;

            if (itemTemplate != null)
                _banner.ItemTemplate = itemTemplate;
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(CrawlList), new PropertyMetadata(null));

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty ItemTemplateProperty =
            DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), typeof(CrawlList),
                                        new PropertyMetadata(null, ItemTemplateChanged));

        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        private static void ItemTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CrawlList crawlList = d as CrawlList;

            if (crawlList == null)
                return;

            if (crawlList._banner != null)
                crawlList._banner.ItemTemplate = e.NewValue as DataTemplate;
        }

        public static readonly DependencyProperty LeftProperty =
            DependencyProperty.Register("Left", typeof(double), typeof(CrawlList), new PropertyMetadata(0.0));

        public double Left
        {
            get { return (double)GetValue(LeftProperty); }
            set { SetValue(LeftProperty, value); }
        }

        public static readonly DependencyProperty CrawlSpeedProperty =
            DependencyProperty.Register("CrawlSpeed", typeof (double), typeof (CrawlList), new PropertyMetadata(100.0), o => (double) o > 0.0);

        /// <summary>
        /// The speed at which the text moves in device independent units per second.
        /// </summary>
        public double CrawlSpeed
        {
            get { return (double) GetValue(CrawlSpeedProperty); }
            set { SetValue(CrawlSpeedProperty, value); }
        }

        public static readonly DependencyProperty IsCrawlingProperty =
            DependencyProperty.Register("IsCrawling", typeof(bool), typeof(CrawlList), new PropertyMetadata(false, IsCrawlingChanged));

        public bool IsCrawling
        {
            get { return (bool) GetValue(IsCrawlingProperty); }
            set { SetValue(IsCrawlingProperty, value); }
        }

        private static void IsCrawlingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CrawlList crawlList = d as CrawlList;

            if (crawlList == null)
                return;

            if ((bool)e.NewValue)
                crawlList.StartCrawlAnimation();
            else
                crawlList.EndCrawlAnimation();
        }

        public static readonly DependencyProperty CrawlAnimationProperty =
            DependencyProperty.Register("CrawlAnimation", typeof (DoubleAnimationBase), typeof (CrawlList));

        public DoubleAnimationBase CrawlAnimation
        {
            get { return (DoubleAnimationBase) GetValue(CrawlAnimationProperty); }
            set { SetValue(CrawlAnimationProperty, value); }
        }

        private void StartCrawlAnimation()
        {
            if ((_banner != null) && (_banner.ActualWidth > 0))
            {
                DoubleAnimationBase doubleAnimation =
                    CrawlAnimation ?? BuildDefaultAnimation();
                BeginAnimation(LeftProperty, doubleAnimation);
            }
        }

        private void EndCrawlAnimation()
        {
            BeginAnimation(LeftProperty, null);
        }

        private DoubleAnimationBase BuildDefaultAnimation()
        {
            double bannerWidth = _banner.ActualWidth;
            double fromValue = _crawlCanvas.ActualWidth;
            double toValue = -1 * bannerWidth;
            double speed = CrawlSpeed;

            Duration duration = new Duration(
                TimeSpan.FromSeconds(bannerWidth / speed));

            return new DoubleAnimation(fromValue, toValue, duration)
                { RepeatBehavior = RepeatBehavior.Forever };
        }
    }
}
