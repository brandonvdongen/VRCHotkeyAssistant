using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using VRCHotkeyAssistant.Data;
using InputSimulatorStandard.Native;
using System.Windows.Input;

namespace VRCHotkeyAssistant
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool editingKeybind = false;

        public MainWindow()
        {
            InitializeComponent();
            //DataContext = this;
        }

        
        private void OuputLogUpdate(object sender, DependencyPropertyChangedEventArgs e)
        {
            Trace.WriteLine(sender);
        }
        private void BtnConnect(object sender, RoutedEventArgs e)
        {
            Button control = (Button)sender;
            BaseBinding context = (BaseBinding)control.DataContext;

            if (!OSCThreadHandler.IsConnected) { 
                OSCThreadHandler.start();
                control.Content = "Disconnect";
            }
            else
            {
                OSCThreadHandler.stop();
                control.Content = "Connect";
            }
        }

        private void BtnAddBool(object sender, RoutedEventArgs e)
        {
            Bindings.BoolBindings.Add(new BoolBinding(""));
        }

        private void BtnAddInt(object sender, RoutedEventArgs e)
        {
            Bindings.IntBindings.Add(new IntBinding(""));
        }

        private void BtnAddFloat(object sender, RoutedEventArgs e)
        {
            Bindings.FloatBindings.Add(new FloatBinding(""));
        }

        private void blurTextbox(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter) { 
                Keyboard.ClearFocus();
                e.Handled = true;
            }
        }

        private void keybindTextbox(object sender, KeyEventArgs e)
        {
            e.Handled = true;
            Control control = (Control)sender;
            if (e.Key == Key.Escape)
            {
                Keyboard.ClearFocus();
                return;
            }
            if(control.DataContext is BaseBinding) {
                var context = (BaseBinding)control.DataContext;
                VirtualKeyCode[] inserts = new [] {
                    VirtualKeyCode.SHIFT,
                    VirtualKeyCode.RSHIFT,
                    VirtualKeyCode.LSHIFT,
                    VirtualKeyCode.MENU,
                    VirtualKeyCode.LMENU,
                    VirtualKeyCode.RMENU,
                    VirtualKeyCode.CONTROL,
                    VirtualKeyCode.LCONTROL,
                    VirtualKeyCode.LCONTROL 
                };

                if (!editingKeybind)
                {
                    editingKeybind = true;
                    context.Keybind.Clear();
                }
                VirtualKeyCode vKey = (VirtualKeyCode)KeyInterop.VirtualKeyFromKey(e.Key);
                if (!context.Keybind.ContainsKey(e.Key))
                {
                    if (inserts.Contains(vKey))
                    {
                        context.Keybind.Insert(0, new ObservableKeyValuePair<Key, VirtualKeyCode>() { Key = e.Key, Value = vKey});
                    }
                    else
                    {
                        context.Keybind.Add(e.Key,vKey);
                    }
                }
                
                
            }

        }
        private void KeybindTextboxUp(object sender, KeyEventArgs e)
        {
            editingKeybind = false;
            e.Handled = true;
            Control control = (Control)sender;
            if (control.DataContext is BaseBinding)
            {
                BaseBinding context = (BaseBinding)control.DataContext;
                Keyboard.ClearFocus();
                OutputLogger.Log($"Modified Keybind: {String.Join("+",context.KeybindAsText)}");

            }
        }

        private void onKeybindClick(object sender, RoutedEventArgs e)
        {
            Control control = (Control)sender;
            BaseBinding context = (BaseBinding)control.DataContext;
            context.Keybind.Clear();
            context.KeybindAsText = "Press Keys...";
        }

        private void HandlePreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!e.Handled)
            {
                e.Handled = true;
                var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
                eventArg.RoutedEvent = UIElement.MouseWheelEvent;
                eventArg.Source = sender;
                UIElement parent = (UIElement)((Control)sender).Parent;
                parent.RaiseEvent(eventArg);
            }
        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListView? listView = sender as ListView;
            if (listView != null)
            {
                listView.SelectedIndex = -1;
            }
        }

        private void DeleteEntry(object sender, RoutedEventArgs e)
        {
            Control control = (Control)sender;
            BaseBinding binding = (BaseBinding)control.DataContext;

            if (binding is BoolBinding)Bindings.BoolBindings.Remove((BoolBinding)binding);
            if (binding is IntBinding) Bindings.IntBindings.Remove((IntBinding)binding);
            if (binding is FloatBinding) Bindings.FloatBindings.Remove((FloatBinding)binding);



        }
    }

    public class OutputLogger
    {
        public static ObservableCollection<string> OutputLog { get; set; } = new ObservableCollection<string>();

        public static void Log(string text)
        {
            if (!Application.Current.Dispatcher.CheckAccess())
            {
                Application.Current.Dispatcher.Invoke(new Action<string>(Log), new object[] { text });
                return;
            }
            string date = DateTime.Now.ToString("HH:mm:ss");
            OutputLog.Insert(0,$"[{date}] {text}");
            if(OutputLog.Count > 100)
            {
                OutputLog.RemoveAt(OutputLog.Count-1);
            }
        }

    }
}
