using System.Globalization;

namespace PersonalFinanceClient.Converters
{
    public class TransactionTypeToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string type)
            {
                return type == "Income" ? Color.FromHex("#4CAF50") : Color.FromHex("#F44336");
            }
            return Color.FromHex("#000000");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}