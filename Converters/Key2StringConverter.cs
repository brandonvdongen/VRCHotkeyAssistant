using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using VRCHotkeyAssistant.Data;
using InputSimulatorStandard.Native;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Windows.Markup;

namespace VRCHotkeyAssistant.Converters
{
	public class Key2StringConverter : MarkupExtension, IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			Trace.WriteLine(value.ToString());
			List<string> output = new List<string>();
            VirtualKeyCode[] input = (VirtualKeyCode[])value;

			foreach( var key in input)
            {
				var dirty = key.ToString().ToLower();
				/*if(dirty.StartsWith("D") && dirty.Length > 1)
                {
					output.Add(dirty.Substring(1));
                }
                if (dirty.StartsWith("VK_"))
                {
                    output.Add(dirty.Substring(3));
                }*/
				Trace.WriteLine("WRITING:" + dirty);
				output.Add(dirty);
            }
            return String.Join("+",output);
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			ObservableCollection<VirtualKeyCode> keys = (ObservableCollection<VirtualKeyCode>)value;
			keys.Clear();
			string input = (string)value;
			List<VirtualKeyCode> output = new List<VirtualKeyCode>();
			string[] newKeys = input.Split("+");

			foreach(string k in newKeys)
            {
				if (Enum.TryParse(k, true, out VirtualKeyCode key))
                {
					keys.Add(key);
                }
            }

			return keys;
		}

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
			return this;
        }
    }
}
