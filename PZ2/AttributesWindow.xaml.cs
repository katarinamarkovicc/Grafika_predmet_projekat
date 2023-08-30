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
using System.Windows.Shapes;

namespace PZ2
{
    /// <summary>
    /// Interaction logic for AttributesWindow.xaml
    /// </summary>
    public partial class AttributesWindow : Window
    {
        
        Point cursorPosition;
        string selectedOption = "";
        List<Point> polPoints;
        Brush novaBoja;
        public AttributesWindow(Point p, List<Point> polygonPoints)
        {
            InitializeComponent();
            cursorPosition = p;
            
            BorderColorCB.ItemsSource = typeof(Colors).GetProperties();
            FillCB.ItemsSource = typeof(Colors).GetProperties();
            TextColorCB.ItemsSource = typeof(Colors).GetProperties();

            switch (MainWindow.selectedButtonName)
            {
                case "ElipseBtn":
                    this.Title = "Select attributes for ellipse";
                    this.WidthLabel.Content = "X radius:";
                    this.HeightLabel.Content = "Y radius: ";
                    this.TextTb.IsEnabled = false;
                    this.TextColorCB.IsEnabled = false;
                    this.FontSizeTb.IsEnabled = false;

                    break;
     
                case "PolygonBtn":
                    this.Title = "Select attributes for polygon";
                    this.WidthTb.IsEnabled = false;
                    this.HeightTb.IsEnabled = false;
                    this.TextTb.IsEnabled = false;
                    this.TextColorCB.IsEnabled = false;
                    this.FontSizeTb.IsEnabled = false;
                    polPoints = polygonPoints;

                    break;

                case "TextBtn":
                    this.Title = "Select attributes for text";
                    this.WidthTb.IsEnabled = false;
                    this.HeightTb.IsEnabled = false;
                    this.BordThickTb.IsEnabled = false;
                    this.BorderColorCB.IsEnabled = false;
                    this.FillCB.IsEnabled = false;

                    break;


            }


        }




       

        private void DrawBtn_Click(object sender, RoutedEventArgs e)
        {
            switch (MainWindow.selectedButtonName)
            {
                case "ElipseBtn":
                    int width;
                    int height;

                    int bthickness;

                    Color selectedBorderColor;
                    Color selectedFillColor;
                    
                    //validacija brojeva
                    if (!Int32.TryParse(this.WidthTb.Text, out width) || width < 0)
                    {
                        MessageBox.Show("Error! Enter valid width.");
                        this.WidthTb.BorderBrush = Brushes.Red;
                        return;
                    }
                    else
                    {
                        this.WidthTb.BorderBrush = Brushes.Green;
                    }

                    if (!Int32.TryParse(this.HeightTb.Text, out height) || height < 0)
                    {
                        MessageBox.Show("Error! Enter valid height.");
                        this.HeightTb.BorderBrush = Brushes.Red;
                        return;
                    }
                    else
                    {
                        this.HeightTb.BorderBrush = Brushes.Green;
                    }
                    if (!Int32.TryParse(this.BordThickTb.Text, out bthickness) || bthickness < 0)
                    {
                        this.BordThickTb.BorderBrush = Brushes.Red;
                        MessageBox.Show("Error! Enter valid border thickness.");
                        return;
                    }
                    else
                    {
                        this.BordThickTb.BorderBrush = Brushes.Green;
                    }
                    //validacija selektovane boje
                    if (!(this.BorderColorCB.SelectedIndex > -1))
                    {
                        MessageBox.Show("Error! Select color for border.");
                        return;
                    }
                    else
                    {
                        string colstr = this.BorderColorCB.SelectedItem.ToString().Split(' ')[1];
                        selectedBorderColor = (Color)ColorConverter.ConvertFromString(colstr);
                    }
                    if (!(this.FillCB.SelectedIndex > -1))
                    {
                        MessageBox.Show("Error! Select fill color.");
                        return;
                    }
                    else
                    {
                        if (myCheckBox.IsChecked == false)
                        {
                            string colstr = this.FillCB.SelectedItem.ToString().Split(' ')[1];
                            selectedFillColor = (Color)ColorConverter.ConvertFromString(colstr);
                        }
                        else
                        {
                            string colstr = this.FillCB.SelectedItem.ToString().Split(' ')[1];
                            selectedFillColor = (Color)ColorConverter.ConvertFromString(colstr);
                            novaBoja = new SolidColorBrush(selectedFillColor);
                            novaBoja.Opacity = 0.25;
                        }
                        
                    }

                    

                    Image img = new Image();
                    DrawingImage di = new DrawingImage();
                    Canvas.SetLeft(img, cursorPosition.X);  
                    Canvas.SetTop(img, cursorPosition.Y);
                    if (myCheckBox.IsChecked == false)
                    {
                        GeometryDrawing gd = new GeometryDrawing(new SolidColorBrush(selectedFillColor), new Pen(new SolidColorBrush(selectedBorderColor), bthickness), new EllipseGeometry(new Point(0, 0), width, height));   //boja, ivica, sam oblik = elipsa
                        di.Drawing = gd;
                        img.Source = di;
                        MainWindow.transferProperty = img;
                    }
                    else
                    {
                        GeometryDrawing gd = new GeometryDrawing(novaBoja, new Pen(new SolidColorBrush(selectedBorderColor), bthickness), new EllipseGeometry(new Point(0, 0), width, height));   //boja, ivica, sam oblik = elipsa
                        di.Drawing = gd;
                        img.Source = di;
                        MainWindow.transferProperty = img;
                    }
                    Close();
                    break;

                case "TextBtn":

                    Color selectedTextColor;
                    int fontSize;
                    


                    //validacija boje
                    if (!(this.TextColorCB.SelectedIndex > -1))
                    {
                        MessageBox.Show("Error! Select color for text.");
                        return;
                    }
                    else
                    {
                        //----------------------PROVIDNOST----------------------------------
                        if (myCheckBox.IsChecked == false)
                        {
                            string colstr = this.TextColorCB.SelectedItem.ToString().Split(' ')[1];
                            selectedTextColor = (Color)ColorConverter.ConvertFromString(colstr);
                        }
                        else
                        {
                            string colstr = this.TextColorCB.SelectedItem.ToString().Split(' ')[1];
                            selectedTextColor = (Color)ColorConverter.ConvertFromString(colstr);
                            novaBoja = new SolidColorBrush(selectedTextColor);
                            novaBoja.Opacity = 0.25;
                        }
                    }

                    //validacija velicine texta
                    if (!Int32.TryParse(this.FontSizeTb.Text, out fontSize) || fontSize < 0)
                    {
                        MessageBox.Show("Error! Enter valid font size.");
                        this.FontSizeTb.BorderBrush = Brushes.Red;
                        return;
                    }
                    else
                    {
                        this.FontSizeTb.BorderBrush = Brushes.Green;
                    }

                    TextBlock tb = new TextBlock();
                    if (myCheckBox.IsChecked == false)
                    {
                        tb.Text = TextTb.Text;
                        tb.FontSize = fontSize;
                        tb.Foreground = new SolidColorBrush(selectedTextColor);
                        MainWindow.transferProperty = tb;
                        Canvas.SetLeft(tb, cursorPosition.X);
                        Canvas.SetTop(tb, cursorPosition.Y);
                    }
                    else
                    {
                        tb.Text = TextTb.Text;
                        tb.FontSize = fontSize;
                        tb.Foreground = novaBoja;
                        MainWindow.transferProperty = tb;
                        Canvas.SetLeft(tb, cursorPosition.X);
                        Canvas.SetTop(tb, cursorPosition.Y);
                    }
                    

                    Close();

                    break;

                case "PolygonBtn":
                    int bthicknessPol;
                    if (!Int32.TryParse(this.BordThickTb.Text, out bthicknessPol))
                    {
                        this.BordThickTb.BorderBrush = Brushes.Red;
                        MessageBox.Show("Error! Enter valid border thickness.");
                        return;
                    }
                    else
                    {
                        this.BordThickTb.BorderBrush = Brushes.Green;
                    }
                    Color selectedBorderColorPol;
                    Color selectedFillColorPol;
                    if (!(this.BorderColorCB.SelectedIndex > -1))
                    {
                        MessageBox.Show("Error! Select color for border.");
                        return;
                    }
                    else
                    {
                        string colstr = this.BorderColorCB.SelectedItem.ToString().Split(' ')[1];
                        selectedBorderColorPol = (Color)ColorConverter.ConvertFromString(colstr);
                    }
                    if (!(this.FillCB.SelectedIndex > -1))
                    {
                        MessageBox.Show("Error! Select fill color.");
                        return;
                    }
                    else
                    {
                        //----------------------------------PROVIDNOST--------------------------------------
                        if (myCheckBox.IsChecked == false)
                        {
                            string colstr = this.FillCB.SelectedItem.ToString().Split(' ')[1];
                            selectedFillColorPol = (Color)ColorConverter.ConvertFromString(colstr);
                        }
                        else
                        {
                            string colstr = this.FillCB.SelectedItem.ToString().Split(' ')[1];
                            selectedFillColorPol = (Color)ColorConverter.ConvertFromString(colstr);
                            novaBoja = new SolidColorBrush(selectedFillColorPol);
                            novaBoja.Opacity = 0.25;
                        }
                       
                    }

                    Polygon pl = new Polygon();
                    if (myCheckBox.IsChecked == false)
                    {
                        pl.Stroke = new SolidColorBrush(selectedBorderColorPol);
                        pl.StrokeThickness = bthicknessPol;
                        pl.Fill = new SolidColorBrush(selectedFillColorPol);
                        pl.Points = new PointCollection(polPoints);
                        MainWindow.transferProperty = pl;
                    }
                    else 
                    {
                        pl.Stroke = new SolidColorBrush(selectedBorderColorPol);
                        pl.StrokeThickness = bthicknessPol;
                        pl.Fill = novaBoja;
                        pl.Points = new PointCollection(polPoints);
                        MainWindow.transferProperty = pl;
                    }
                    


                    Close();
                    break;

            }
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
