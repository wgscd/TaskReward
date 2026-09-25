using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
namespace TaskReward.Converters
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type t, object p, CultureInfo c)
            => (value is bool b && b) ? Visibility.Visible : Visibility.Collapsed;
        public object ConvertBack(object value, Type t, object p, CultureInfo c)
            => value is Visibility v && v == Visibility.Visible;
    }

    public class InverseBoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type t, object p, CultureInfo c)
            => (value is bool b && b) ? Visibility.Collapsed : Visibility.Visible;
        public object ConvertBack(object value, Type t, object p, CultureInfo c)
            => value is Visibility v && v != Visibility.Visible;
    }

    /// <summary>数值为 0（或空）时显示，用于空状态</summary>
    public class ZeroToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type t, object p, CultureInfo c)
        {
            if (value == null) return Visibility.Visible;
            if (value is int i) return i == 0 ? Visibility.Visible : Visibility.Collapsed;
            if (value is long l) return l == 0 ? Visibility.Visible : Visibility.Collapsed;
            return Visibility.Collapsed;
        }
        public object ConvertBack(object value, Type t, object p, CultureInfo c) => null;
    }

    /// <summary>未解锁成就置灰显示</summary>
    public class LockedOpacityConverter : IValueConverter
    {
        public object Convert(object value, Type t, object p, CultureInfo c)
            => (value is bool b && b) ? 1.0 : 0.45;
        public object ConvertBack(object value, Type t, object p, CultureInfo c) => null;
    }

    /// <summary>徽章等级配色</summary>
    public class TierToBrushConverter : IValueConverter
    {
        private static readonly Brush Bronze = new SolidColorBrush(Color.FromRgb(0xB0, 0x6A, 0x3E));
        private static readonly Brush Silver = new SolidColorBrush(Color.FromRgb(0x8E, 0x96, 0xA8));
        private static readonly Brush Gold = new SolidColorBrush(Color.FromRgb(0xE0, 0xA0, 0x24));
        private static readonly Brush Diamond = new SolidColorBrush(Color.FromRgb(0x3E, 0x9F, 0xD0));
        private static readonly Brush Legend = new SolidColorBrush(Color.FromRgb(0x8E, 0x54, 0xD4));

        public object Convert(object value, Type t, object p, CultureInfo c)
        {
            string tier = value as string;
            switch (tier)
            {
                case "bronze": return Bronze;
                case "silver": return Silver;
                case "gold": return Gold;
                case "diamond": return Diamond;
                case "legend": return Legend;
                default: return Silver;
            }
        }
        public object ConvertBack(object value, Type t, object p, CultureInfo c) => null;
    }

    /// <summary>点数 -> 带金色渐变文字</summary>
    public class PointsTextConverter : IValueConverter
    {
        public object Convert(object value, Type t, object p, CultureInfo c)
            => value?.ToString();
        public object ConvertBack(object value, Type t, object p, CultureInfo c) => null;
    }

    /// <summary>double 转 ProgressBar 范围</summary>
    public class ClampConverter : IValueConverter
    {
        public object Convert(object value, Type t, object p, CultureInfo c)
        {
            double v = value is double d ? d : 0;
            return Math.Max(0, Math.Min(100, v));
        }
        public object ConvertBack(object value, Type t, object p, CultureInfo c) => null;
    }

    /// <summary>库存：-1 显示不限量</summary>
    public class StockTextConverter : IValueConverter
    {
        public object Convert(object value, Type t, object p, CultureInfo c)
        {
            if (value is int i) return i < 0 ? "不限量" : i.ToString();
            if (value is long l) return l < 0 ? "不限量" : l.ToString();
            return value?.ToString() ?? "";
        }
        public object ConvertBack(object value, Type t, object p, CultureInfo c) => null;
    }

    /// <summary>成就进度：[threshold, lifetime] -> 0-100</summary>
    public class AchProgressMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type t, object p, CultureInfo c)
        {
            double threshold = ToDouble(values, 0);
            double lifetime = ToDouble(values, 1);
            if (threshold <= 0) return 100;
            return Math.Max(0, Math.Min(100, lifetime / threshold * 100));
        }
        public object[] ConvertBack(object value, Type[] t, object p, CultureInfo c) => null;

        private static double ToDouble(object[] v, int i)
        {
            if (i >= v.Length || v[i] == null) return 0;
            try { return System.Convert.ToDouble(v[i], CultureInfo.InvariantCulture); }
            catch { return 0; }
        }
    }
}
