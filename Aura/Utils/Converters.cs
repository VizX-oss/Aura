using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;
using System.Globalization;
using Aura.Models;

namespace Aura.Utils
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool b)
            {
                return b ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is Visibility v)
            {
                return v == Visibility.Visible;
            }
            return false;
        }
    }

    public class ObjectComparatorToVisbility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null || parameter == null) return Visibility.Collapsed;
            return value.ToString() == parameter.ToString() ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class ObjectEmptyToVisbility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is string valueAsString)
            {
                return !string.IsNullOrEmpty(valueAsString) ? Visibility.Visible : Visibility.Collapsed;
            }

            return value != null ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class DateTimeToTimeSpanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is DateTime dt)
            {
                return dt.TimeOfDay;
            }
            return TimeSpan.Zero;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is TimeSpan ts)
            {
                return DateTime.Today.Add(ts);
            }
            return DateTime.MinValue;
        }
    }

    public class UpdateStatusToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is UpdateStatus status && parameter is string targetStatusStr)
            {
                if (Enum.TryParse<UpdateStatus>(targetStatusStr, out var targetStatus))
                {
                    return status == targetStatus ? Visibility.Visible : Visibility.Collapsed;
                }
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class StringFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return string.Empty;
            string format = parameter as string;
            if (string.IsNullOrEmpty(format)) return value.ToString();
            return string.Format(format, value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}

