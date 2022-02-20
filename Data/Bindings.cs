using System.Collections.ObjectModel;
using VRCHotkeyAssistant.Data;

namespace VRCHotkeyAssistant.Data
{
    public class Bindings
    {


        public static ObservableCollection<BaseBinding> All { get {
                ObservableCollection<BaseBinding> result = new ObservableCollection<BaseBinding>();
                foreach(var binding in BoolBindings)result.Add(binding);
                foreach(var binding in IntBindings)result.Add(binding);
                foreach(var binding in FloatBindings)result.Add(binding);
                return result;
        } set { } }

        public static ObservableCollection<BoolBinding> BoolBindings { get; set; } = new ObservableCollection<BoolBinding>();
        public static ObservableCollection<IntBinding> IntBindings { get; set; } = new ObservableCollection<IntBinding>();
        public static ObservableCollection<FloatBinding> FloatBindings { get; set; } = new ObservableCollection<FloatBinding>();
    }
}
