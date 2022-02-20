using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using VRCHotkeyAssistant.Data;

namespace VRCHotkeyAssistant.Converters
{
	public class BindModeConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			switch ((keybindMode)value)
			{
				case keybindMode.OutsideRange:
				case keybindMode.InRange:
					return true;
				default:
					return false;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value is bool)
			{
				if ((bool)value == true)
					return keybindMode.InRange;
				else
					return keybindMode.IfGreater;
			}
			return keybindMode.IfGreater;
		}
	}
}
