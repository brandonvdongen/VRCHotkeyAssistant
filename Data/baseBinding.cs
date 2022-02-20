using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InputSimulatorStandard;
using InputSimulatorStandard.Native;
using System.Windows.Input;
using System.Collections.ObjectModel;
using VRCHotkeyAssistant.Converters;
using VRCHotkeyAssistant;
using System.ComponentModel;

namespace VRCHotkeyAssistant.Data
{
    public abstract class BaseBinding : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected InputSimulator inputSimulator = new InputSimulator();
        

        protected BaseBinding(string address)
        {
            Address = address;
            Keybind.CollectionChanged += (sender, e) =>
            {
                KeybindAsText = String.Join("+", Keybind.Keys);
                if (Keybind.Keys.Count > 0)
                {
                    onPropertyChanged(nameof(KeybindAsText));
                }

            };

        }

        private string _address { get; set; } = "";
        public string Address
        {
            get => _address; set
            {
                if (value != _address)
                {
                    _address = value;
                    onPropertyChanged(nameof(Address));
                }
            }
        }

        public string KeybindAsText { get; set; } = "Click to Set";

        public readonly ObservableDictionary<Key, VirtualKeyCode> Keybind = new ObservableDictionary<Key, VirtualKeyCode>();
        public keybindMode BindMode { get => _bindMode; set { _bindMode = value; onPropertyChanged(nameof(BindMode)); } }
        private keybindMode _bindMode = keybindMode.Disabled;
        public keybindMode[] BindingModes { get { return UsableBindModes; } }

        protected abstract keybindMode[] UsableBindModes { get; }

        internal abstract void TryExecute(string value);

        public void onPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

    }
    public class BoolBinding : BaseBinding
    {
        public BoolBinding(string address) : base(address) { }
        protected override keybindMode[] UsableBindModes => new[] { keybindMode.Disabled, keybindMode.onAny, keybindMode.onTrue, keybindMode.onFalse };

        internal override void TryExecute(string value)
        {
            var converter = new Key2StringConverter();
            if (bool.TryParse(value, out bool result))
            {
                if (BindMode == keybindMode.Disabled) return;
                if (result && BindMode == keybindMode.onFalse) return;
                if (!result && BindMode == keybindMode.onTrue) return;

                if(Keybind.Values.Count > 0) { 
                    OutputLogger.Log($"Caught {Address}, {BindMode} Calling: {KeybindAsText}");
                    inputSimulator.Keyboard.KeyPress(Keybind.Values.ToArray());
                }
            }
        }
    }

    public class IntBinding : BaseBinding
    {
        public IntBinding(string address) : base(address) { }
        protected override keybindMode[] UsableBindModes => new[] { keybindMode.Disabled, keybindMode.IfGreater, keybindMode.IfLess, keybindMode.InRange, keybindMode.OutsideRange };

        public int MinValue { get => _minValue; set { if (_minValue != value) { _minValue = value; onPropertyChanged(nameof(MinValue)); wasTrue = false; } } }
        private int _minValue;
        public int MaxValue { get => _maxValue; set { if (_maxValue != value) { _maxValue = value; onPropertyChanged(nameof(MaxValue)); wasTrue = false; } } }
        private int _maxValue;

        private bool wasTrue = false;

        internal override void TryExecute(string value)
        {
            if (int.TryParse(value, out int result))
            {

                if ((BindMode == keybindMode.Disabled) ||
                    (BindMode == keybindMode.IfGreater && result < MinValue) ||
                    (BindMode == keybindMode.IfLess && result > MinValue) ||
                    (BindMode == keybindMode.InRange && (result > MaxValue || result < MinValue)) ||
                    BindMode == keybindMode.OutsideRange && result < MaxValue && result > MinValue){ 
                    wasTrue = false; 
                    return; 
                }

                if (!wasTrue && Keybind.Values.Count > 0) { 
                    OutputLogger.Log($"Caught {Address}, {BindMode} Calling: {KeybindAsText}");
                    inputSimulator.Keyboard.KeyPress(Keybind.Values.ToArray());
                    wasTrue = true;
                }
            }
        }
    }

    public class FloatBinding : BaseBinding
    {
        public FloatBinding(string address) : base(address) { }
        protected override keybindMode[] UsableBindModes => new[] { keybindMode.Disabled, keybindMode.IfGreater, keybindMode.IfLess, keybindMode.InRange, keybindMode.OutsideRange };

        public float MinValue { get => _minValue; set { if (_minValue != value) { _minValue = value; onPropertyChanged(nameof(MinValue)); wasTrue = false; } } }
        private float _minValue;
        public float MaxValue { get => _maxValue; set { if (_maxValue != value) { _maxValue = value; onPropertyChanged(nameof(MaxValue)); wasTrue = false; } } }
        private float _maxValue;

        private bool wasTrue = false;
        

        internal override void TryExecute(string value)
        {
            value = value.Substring(0, value.Length - 2);
            if (float.TryParse(value, out float result))
            {
                if ((BindMode == keybindMode.Disabled) ||
                    (BindMode == keybindMode.IfGreater && result < MinValue) ||
                    (BindMode == keybindMode.IfLess && result > MinValue) ||
                    (BindMode == keybindMode.InRange && (result > MaxValue || result < MinValue)) ||
                    BindMode == keybindMode.OutsideRange && result < MaxValue && result > MinValue)
                {
                    wasTrue = false;
                    return;
                }

                if (!wasTrue && Keybind.Values.Count > 0)
                {
                    OutputLogger.Log($"Caught {Address}, {BindMode} Calling: {KeybindAsText}");
                    inputSimulator.Keyboard.KeyPress(Keybind.Values.ToArray());
                    wasTrue = true;
                }
            }
        }
    }

    public enum keybindMode
    {
        Disabled,
        onTrue,
        onFalse,
        onAny,
        IfGreater,
        IfLess,
        InRange,
        OutsideRange
    }
}
