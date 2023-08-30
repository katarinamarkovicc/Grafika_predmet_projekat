using Microsoft.Win32;
using PZ2.Models;
using System;
using System.Collections.Generic;
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
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Point = System.Windows.Point;
using static PZ2.ScreenShot;



namespace PZ2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool cekiranoVodovi = false;
        bool first = true;
        bool cekiraniVodoviOpseg = false;
        bool pritisnutoDugme = false;
        bool izabranaJeBoja = false;
        byte vr;
        byte vg;
        byte vb;
        // PZ2.Models.Grid gd { get; set; }
        //List<SwitchEntity> switches = new List<SwitchEntity>();
        //---------------------------------------------------------------------------------------------------------------------------------------

        public static string selectedButtonName = "";
        public static Object transferProperty = null;
        static List<Point> polygonPoints = new List<Point>();

        //public static List<UIElement> allElements = new List<UIElement>();

        public static Stack<UndoRedoModel> undoStack = new Stack<UndoRedoModel>();
        public static Stack<UndoRedoModel> redoStack = new Stack<UndoRedoModel>();
        //---------------------------------------------------------------------------------------------------------------------------------------


        ImportAndDraw mainImportDraw = new ImportAndDraw();
        string location = "";
       
        public MainWindow()
        {
            InitializeComponent();
          

        }

        private void checkInactive_Checked(object sender, RoutedEventArgs e)
        {
            cekiranoVodovi = true;
            if (first != true)
            {
                OpenFile(location);
                first = false;
            }
           
        }


        private void checkInactive_Unchecked(object sender, RoutedEventArgs e)
        {
            cekiranoVodovi = false;
            
            if (first != true)
            {
                OpenFile(location);
                List<NodeEntity> nds = mainImportDraw.NodesToDelete();
                mainImportDraw.DeleteNode(nds, this.GridCanvas);

                first = false;
            }
            //OpenFile(location);
        }

        private void checkLineColor_Checked(object sender, RoutedEventArgs e)
        {
            cekiraniVodoviOpseg = true;
            if (first != true)
            {
                OpenFile(location);
                first = false;
            }

        }

        private void checkLineColor_Unchecked(object sender, RoutedEventArgs e)
        {
            cekiraniVodoviOpseg = false;
            if (first != true)
            {
                OpenFile(location);
                first = false;
            }
        }


        private void OpenFile(string location)
        {
            mainImportDraw = new ImportAndDraw();
            GridCanvas.Children.Clear();
            //location = openFileDialog.FileName;
            mainImportDraw.LoadAndParseXML(location); // ucitavam geographic xml i parsiram elemente, posle ovog imam napravljene moje konkretne objekte sa vrednostima za sva polja iz geographic.xml
            mainImportDraw.ScaleFromLatLonToCanvas(GridCanvas.Width, GridCanvas.Height);// skaliram latlon koordinate na canvas
                                                                                        //5000x5000
            mainImportDraw.ConvertFromLatLonToCanvasCoord();// sada konvertujem latlon koordinate na canvas koordinate
            mainImportDraw.DrawElements(this.GridCanvas, cekiranoVodovi, cekiraniVodoviOpseg); // iscrtavaju se elementi, prosledjuje se canvas a5000x5000
        }

        private void OpenFileBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.DefaultExt = "xml";                  
            openFileDialog.Filter = "XML Files|*.xml";

            if (openFileDialog.ShowDialog()==true)
            {
                first = false;
                try
                {
                    if (GridCanvas.Children.Count > 0) // grid canvas je canvas 5000x5000 u xamlu
                    {
                        string message = "Are you sure? Old data will be lost.";
                        string caption = "Confirmation";
                        MessageBoxButton buttons = MessageBoxButton.YesNo;
                        MessageBoxImage icon = MessageBoxImage.Question;
                        if (MessageBox.Show(message, caption, buttons, icon) == MessageBoxResult.Yes)
                        {
                            using (new WaitCursor())
                            {
                                mainImportDraw = new ImportAndDraw();
                                GridCanvas.Children.Clear();
                                location = openFileDialog.FileName;
                                mainImportDraw.LoadAndParseXML(location); // ucitavam geographic xml i parsiram elemente, posle ovog imam napravljene moje konkretne objekte sa vrednostima za sva polja iz geographic.xml
                                mainImportDraw.ScaleFromLatLonToCanvas(GridCanvas.Width, GridCanvas.Height);// skaliram latlon koordinate na canvas
                                //5000x5000
                                mainImportDraw.ConvertFromLatLonToCanvasCoord();// sada konvertujem latlon koordinate na canvas koordinate
                                mainImportDraw.DrawElements(this.GridCanvas, cekiranoVodovi, cekiraniVodoviOpseg); // iscrtavaju se elementi, prosledjuje se canvas a5000x5000
                               
                            }
                        }
                    }
                    else
                    {
                        using (new WaitCursor())
                        {
                            mainImportDraw = new ImportAndDraw();
                            GridCanvas.Children.Clear();
                            location = openFileDialog.FileName;
                            mainImportDraw.LoadAndParseXML(location);
                            mainImportDraw.ScaleFromLatLonToCanvas(GridCanvas.Width, GridCanvas.Height);
                            mainImportDraw.ConvertFromLatLonToCanvasCoord();
                            mainImportDraw.DrawElements(this.GridCanvas, cekiranoVodovi, cekiraniVodoviOpseg);
                        }

                        
                    }
                }
                catch(Exception exc)
                {
                    MessageBox.Show("Please, provide a valid xml file.", "Invalid file", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }

        }



        //-------------------------------------------------------------------------------------------------------------------------------

        
        private void SelectEllipse_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
      
        private void SelectPolygon_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void SelectText_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Undo_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            //TODO
            e.CanExecute = false;

            if (undoStack.Count > 0)
            {
                e.CanExecute = true;
            }

            
        }
        private void Redo_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            //TODO
            e.CanExecute = false;
            if (redoStack.Count > 0)
            {
                e.CanExecute = true;
            }

           
        }
        private void SelectEllipse_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            polygonPoints = new List<Point>();


            ElipseBtn.Background = Brushes.MediumPurple;
          
            PolygonBtn.Background = Brushes.Yellow;
            TextBtn.Background = Brushes.Yellow;


            selectedButtonName = "ElipseBtn";   //pamti na koje dugme je kliknuto, bitno je da patimo koji oblik stavljamo na canvas zbog razlicitih propertija
        }

       
        private void SelectPolygon_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            polygonPoints = new List<Point>();

            PolygonBtn.Background = Brushes.MediumPurple;
         
            ElipseBtn.Background = Brushes.Yellow;
            TextBtn.Background = Brushes.Yellow;


            selectedButtonName = "PolygonBtn";
        }

        private void SelectText_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            polygonPoints = new List<Point>();

            TextBtn.Background = Brushes.MediumPurple;

            ElipseBtn.Background = Brushes.Yellow;
            PolygonBtn.Background = Brushes.Yellow;


            selectedButtonName = "TextBtn";
        }

        private void Undo_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (undoStack.Count > 0)
            {
                UndoRedoModel tempmodel = undoStack.Pop();
                switch (tempmodel.OperationName)
                {
                    case "AddEllipse":
                        Image lastEllipse = tempmodel.mainObject as Image;
                        GridCanvas.Children.Remove(lastEllipse);
                        tempmodel.OperationName = "UndoEllipse";
                        redoStack.Push(tempmodel);
                        break;
                    
                   
                    case "AddPolygon":
                        Polygon tempPolygon = tempmodel.mainObject as Polygon;
                        GridCanvas.Children.Remove(tempPolygon);
                        tempmodel.OperationName = "UndoPolygon";
                        redoStack.Push(tempmodel);
                        break;

                    case "AddText":
                        TextBlock tb = tempmodel.mainObject as TextBlock;
                        GridCanvas.Children.Remove(tb);
                        tempmodel.OperationName = "UndoText";
                        redoStack.Push(tempmodel);
                        break;

                    case "EditText":
                        int cnt3 = 0;
                        foreach (UIElement child in GridCanvas.Children)    //indeks od tog el nad kojim hocu nesto da radim
                        {
                            if (child == tempmodel.mainObject)
                            {
                                break;
                            }
                            cnt3++;
                        }
                        foreach (UndoRedoModel tmp in undoStack)    //da vratim ono sto se poremetilo
                        {
                            if ((tmp) != tempmodel && (tmp.mainObject as UIElement) == GridCanvas.Children[cnt3])
                            {
                                tmp.mainObject = tempmodel.editedObject;
                            }
                        }
                        GridCanvas.Children.Remove(GridCanvas.Children[cnt3]);
                        (tempmodel.editedObject as TextBlock).MouseLeftButtonDown += Polygon_MouseLeftButtonDown;
                        GridCanvas.Children.Insert(cnt3, tempmodel.editedObject as TextBlock);
                        tempmodel.OperationName = "UndoEditText";
                        redoStack.Push(tempmodel);
                        break;

                    case "Clear":

                        foreach (UIElement child in GridCanvas.Children)
                        {
                            child.Visibility = Visibility.Visible;
                        }
                        tempmodel.OperationName = "UndoClear";
                        redoStack.Push(tempmodel);
                        break;
                    case "EditE":
                        
                        int br = 0;
                        foreach (UIElement child in GridCanvas.Children)
                        {
                            if (child == tempmodel.mainObject)
                            {

                                break;
                            }
                            br++;
                        }
                        foreach (UndoRedoModel tmp in undoStack)
                        {
                            if ((tmp) != tempmodel && (tmp.mainObject as UIElement) == GridCanvas.Children[br])
                            {
                                tmp.mainObject = tempmodel.editedObject;
                            }
                        }
                        GridCanvas.Children.Remove(GridCanvas.Children[br]);
                        (tempmodel.editedObject as Image).MouseLeftButtonDown += Ellipse_MouseLeftButtonDown;
                        GridCanvas.Children.Insert(br, tempmodel.editedObject as Image);
                        //PaintCanvas.Children.Add(tempmodel.editedObject as Image);
                        tempmodel.OperationName = "UndoEditE";
                        redoStack.Push(tempmodel);
                        break;
                    
                    case "EditPol":
                        int br2 = 0;
                        foreach (UIElement child in GridCanvas.Children)
                        {
                            if (child == tempmodel.mainObject)
                            {

                                break;
                            }
                            br2++;
                        }
                        foreach (UndoRedoModel tmp in undoStack)
                        {
                            if ((tmp) != tempmodel && (tmp.mainObject as UIElement) == GridCanvas.Children[br2])
                            {
                                tmp.mainObject = tempmodel.editedObject;
                            }
                        }
                        GridCanvas.Children.Remove(GridCanvas.Children[br2]);
                        (tempmodel.editedObject as Polygon).MouseLeftButtonDown += Polygon_MouseLeftButtonDown;
                        GridCanvas.Children.Insert(br2, tempmodel.editedObject as Polygon);
                        //PaintCanvas.Children.Add(tempmodel.editedObject as Polygon);
                        tempmodel.OperationName = "UndoEditPol";
                        redoStack.Push(tempmodel);
                        break;
                }
            }
        }
        private void Redo_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (redoStack.Count > 0)
            {
                UndoRedoModel tempmodel = redoStack.Pop();
                switch (tempmodel.OperationName)
                {
                    case "UndoEllipse":
                        Image lastEllipse = tempmodel.mainObject as Image;
                        GridCanvas.Children.Add(lastEllipse);
                        tempmodel.OperationName = "AddEllipse";
                        undoStack.Push(tempmodel);
                        break;
                    case "UndoPolygon":
                        Polygon tempPolygon = tempmodel.mainObject as Polygon;
                        GridCanvas.Children.Add(tempPolygon);
                        tempmodel.OperationName = "AddPolygon";
                        undoStack.Push(tempmodel);
                        break;

                    case "UndoText":
                        TextBlock tb = tempmodel.mainObject as TextBlock;
                        GridCanvas.Children.Add(tb);
                        tempmodel.OperationName = "AddText";
                        undoStack.Push(tempmodel);
                        break;

                    case "UndoEditText":
                        int cnt4 = 0;
                        foreach (UIElement uie in GridCanvas.Children)
                        {
                            if (uie == tempmodel.editedObject)
                            {

                                break;
                            }
                            cnt4++;
                        }
                        foreach (UndoRedoModel urm in undoStack)
                        {
                            if ((urm) != tempmodel && (urm.editedObject as UIElement) == GridCanvas.Children[cnt4])
                            {
                                urm.mainObject = tempmodel.mainObject;
                            }
                        }
                        GridCanvas.Children.Remove(GridCanvas.Children[cnt4]);
                        GridCanvas.Children.Insert(cnt4, tempmodel.mainObject as TextBlock);
                        tempmodel.OperationName = "EditText";
                        undoStack.Push(tempmodel);
                        break;

                    case "UndoClear":
                        foreach (UIElement child in GridCanvas.Children)
                        {
                            var xaml = System.Windows.Markup.XamlWriter.Save(child);
                            var deepCopy = System.Windows.Markup.XamlReader.Parse(xaml) as UIElement;
                            child.Visibility = Visibility.Collapsed;
                        }
                        //PaintCanvas.Children.Clear();

                        tempmodel.OperationName = "Clear";
                        undoStack.Push(tempmodel);
                        break;
                    case "UndoEditE":
                        int br = 0;
                        foreach (UIElement child in GridCanvas.Children)
                        {
                            if (child == tempmodel.editedObject)
                            {

                                break;
                            }
                            br++;
                        }
                        foreach (UndoRedoModel tmp in undoStack)
                        {
                            if ((tmp) != tempmodel && (tmp.editedObject as UIElement) == GridCanvas.Children[br])
                            {
                                tmp.mainObject = tempmodel.mainObject;
                            }
                        }
                        GridCanvas.Children.Remove(GridCanvas.Children[br]);
                        GridCanvas.Children.Insert(br, tempmodel.mainObject as Image);
                        //PaintCanvas.Children.Add(tempmodel.mainObject as Image);
                        tempmodel.OperationName = "EditE";
                        undoStack.Push(tempmodel);
                        break;
                    
                    case "UndoEditPol":
                        int br3 = 0;
                        foreach (UIElement child in GridCanvas.Children)
                        {
                            if (child == tempmodel.editedObject)
                            {

                                break;
                            }
                            br3++;
                        }
                        foreach (UndoRedoModel tmp in undoStack)
                        {
                            if ((tmp) != tempmodel && (tmp.editedObject as UIElement) == GridCanvas.Children[br3])
                            {
                                tmp.mainObject = tempmodel.mainObject;
                            }
                        }
                        GridCanvas.Children.Remove(GridCanvas.Children[br3]);
                        GridCanvas.Children.Insert(br3, tempmodel.mainObject as Polygon);
                        //PaintCanvas.Children.Add(tempmodel.mainObject as Polygon);
                        tempmodel.OperationName = "EditPol";
                        undoStack.Push(tempmodel);
                        break;

                }
            }
        }

        

        private void PaintCanvas_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (selectedButtonName == "ElipseBtn")
            {
                Point p = e.GetPosition(GridCanvas);    
                polygonPoints = new List<Point>();
                AttributesWindow aw = new AttributesWindow(p, polygonPoints);
                aw.ShowDialog();
                if (transferProperty == null)
                {
                    return;
                }
                Image retval = (Image)transferProperty;
                retval.MouseLeftButtonDown += Ellipse_MouseLeftButtonDown;
                GridCanvas.Children.Add(retval);
                undoStack.Push(new UndoRedoModel("AddEllipse", retval, null));
                transferProperty = null;
            }
            else if(selectedButtonName == "TextBtn")
            {
            
                Point p = e.GetPosition(GridCanvas);
                polygonPoints = new List<Point>();
                AttributesWindow aw = new AttributesWindow(p, polygonPoints);
                aw.ShowDialog();

                if (transferProperty == null)
                {
                    return;
                }
                TextBlock tb = (TextBlock)transferProperty;
                tb.MouseLeftButtonDown += Text_MouseLeftButtonDown;
                GridCanvas.Children.Add(tb);
                undoStack.Push(new UndoRedoModel("AddText", tb, null));
                transferProperty = null;
            }
            else if(selectedButtonName == "PolygonBtn")
            {
                polygonPoints.Add(e.GetPosition(GridCanvas));
            }
        }

        private void PaintCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point p = e.GetPosition(GridCanvas);
            if (polygonPoints.Count < 3)
            {
                return;
            }
            AttributesWindow aw = new AttributesWindow(p, polygonPoints);
            aw.ShowDialog();
            polygonPoints = new List<Point>();
            if (transferProperty == null)
            {
                return;
            }
            Polygon retval = (Polygon)transferProperty;
            retval.MouseLeftButtonDown += Polygon_MouseLeftButtonDown;
            GridCanvas.Children.Add(retval);

            undoStack.Push(new UndoRedoModel("AddPolygon", retval, null));

            transferProperty = null;

            polygonPoints = new List<Point>();
        }

        //eventi custom
        private void Ellipse_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (polygonPoints.Count != 0)
            {
                return;
            }

            Image tmp = (Image)sender;

            EditWindow ew = new EditWindow(tmp, "Ellipse");
            ew.ShowDialog();
        }
      
        private void Polygon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (polygonPoints.Count != 0)
            {
                return;
            }
            Polygon tmp = (Polygon)sender;

            EditWindow ew = new EditWindow(tmp, "Polygon");
            ew.ShowDialog();
        }

        private void Text_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (polygonPoints.Count != 0)
            {
                return;
            }

            TextBlock tmp = (TextBlock)sender;

            EditWindow ew = new EditWindow(tmp, "Text");
            ew.ShowDialog();
        }

        private void ClearBtn_Click(object sender, RoutedEventArgs e)
        {

            foreach (UIElement child in GridCanvas.Children)
            {
                var xaml = System.Windows.Markup.XamlWriter.Save(child);
                var deepCopy = System.Windows.Markup.XamlReader.Parse(xaml) as UIElement;
                child.Visibility = Visibility.Collapsed;
            }


            undoStack.Push(new UndoRedoModel("Clear", null, null));
            //PaintCanvas.Children.Clear();

        }

    

        private void Unselect_Click(object sender, RoutedEventArgs e)
        {
            selectedButtonName = "";

            ElipseBtn.Background = Brushes.Yellow;
          
            PolygonBtn.Background = Brushes.Yellow;

            TextBtn.Background = Brushes.Yellow;
         
        }

        private void ScreenShot_Click(object sender, RoutedEventArgs e)
        {
            takeScreenShoot();
        }

        //---------------------------------------SLIKA I BOJA--------------------------------------------------------

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog OpenFile = new OpenFileDialog();
            OpenFile.Multiselect = true;
            OpenFile.Title = "Select Picture(s)";
            OpenFile.Filter = "ALL supported Graphics| *.jpeg; *.jpg;*.png;";
            if (OpenFile.ShowDialog() == true)
            {
                foreach (String file in OpenFile.FileNames)
                {
                    Add_Image(file);
                    break;
                }
            }
        }

        private void ClrPcker_Background_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            ChangeColour(new SolidColorBrush(Color.FromRgb(ClrPcker_Background.SelectedColor.Value.R, ClrPcker_Background.SelectedColor.Value.G, ClrPcker_Background.SelectedColor.Value.B)), null);
        }

        public void ChangeColour(SolidColorBrush brush, ImageBrush image)
        {
            List<SwitchEntity> switches = mainImportDraw.SwchToDelete();
            if (brush != null)
            {
                foreach (var item in switches)
                {
                    item.PowerEntityShape.Fill = brush;
                }
            }
            if (image != null)
            {
                foreach (var item in switches)
                {
                    item.PowerEntityShape.Fill = image;
                }
            }

        }

        private void Add_Image(string file)
        {
            Console.WriteLine("Une image" + file);
            ImageBrush new_img = new ImageBrush();
            new_img.ImageSource = new BitmapImage(new Uri(file));

            ChangeColour(null, new_img);
        }

        //-------------------------------------------------------------------------------------------------------------------------------

    }



    public class WaitCursor : IDisposable
    {
        private Cursor _previousCursor;

        public WaitCursor()
        {
            _previousCursor = Mouse.OverrideCursor;

            Mouse.OverrideCursor = Cursors.Wait;
        }

        #region IDisposable Members

        public void Dispose()
        {
            Mouse.OverrideCursor = _previousCursor;
        }

        #endregion
    }
}
