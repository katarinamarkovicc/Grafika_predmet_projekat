using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PZ2
{
    public static class RoutedCommands
    {
        public static readonly RoutedUICommand SelectEllipse = new RoutedUICommand("Select Ellipse", "EllipseCommand",
            typeof(RoutedCommands));

        public static readonly RoutedUICommand SelectPolygon = new RoutedUICommand("Select Polygon", "PolygonCommand",
            typeof(RoutedCommands));

        public static readonly RoutedUICommand SelectText = new RoutedUICommand("Select Text", "TextCommand",
           typeof(RoutedCommands));

        public static readonly RoutedUICommand Undo = new RoutedUICommand("Undo", "UndoCommand",
            typeof(RoutedCommands));

        public static readonly RoutedUICommand Redo = new RoutedUICommand("Redo", "RedoCommand",
            typeof(RoutedCommands));
    }
}
