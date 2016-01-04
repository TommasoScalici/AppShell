using System;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace TommasoScalici.AppShell
{
    public class MenuItem
    {
        public Action Action { get; set; } = () => { };
        public object Arguments { get; set; }
        public string DestinationPage { get; set; }
        public bool IsSelectable { get; set; } = true;
        public string Label { get; set; }
        public string Name { get; set; }
        public Symbol Symbol { get; set; }
        public char SymbolAsChar { get { return (char)Symbol; } }
        public UIElement UIContent { get; set; }
        public Visibility LabelVisibility { get; set; }
        public Visibility SymbolVisibility { get; set; }
        public Visibility UIContentVisibility { get; set; }
    }
}
