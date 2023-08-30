using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
    /// Interaction logic for EditWindow.xaml
    /// </summary>
    public partial class EditWindow : Window
    {

        object currentObject;
        
        string selectedObjectType = "";
        Brush novaBoja;
        public EditWindow(object obj, string objType)
        {
            InitializeComponent();
            
            BorderColorCB.ItemsSource = typeof(Colors).GetProperties();
            FillCB.ItemsSource = typeof(Colors).GetProperties();
            TextColorCB.ItemsSource = typeof(Colors).GetProperties();

            currentObject = obj;
            this.Title = "Edit properties for" + objType;
            selectedObjectType = objType;
            switch (selectedObjectType)
            {
                case "Ellipse":
                    DrawingImage di = (DrawingImage)(obj as Image).Source;
                    GeometryDrawing gd = (GeometryDrawing)di.Drawing;
                    SolidColorBrush brushUsed = (SolidColorBrush)gd.Brush;
                    Color brushUsedColor = brushUsed.Color;
                    string colorBrushUsedString = GetColorName(brushUsedColor);
                    foreach (var cn in FillCB.Items)
                    {
                        string temp = cn.ToString().Split(' ')[1];
                        if (temp == colorBrushUsedString)
                        {
                            FillCB.SelectedItem = cn;
                        }
                    }

                    SolidColorBrush borderColorUsed = (SolidColorBrush)gd.Pen.Brush;
                    Color borderColorUsedColor = borderColorUsed.Color;
                    string borderColorUsedString = GetColorName(borderColorUsedColor);

                    foreach (var cn in BorderColorCB.Items)
                    {
                        string temp = cn.ToString().Split(' ')[1];
                        if (temp == borderColorUsedString)
                        {
                            BorderColorCB.SelectedItem = cn;
                        }
                    }

                    this.BordThickTb.Text = gd.Pen.Thickness.ToString();

                    break;
               
                case "Polygon":
                    Polygon temppol = obj as Polygon;
                    SolidColorBrush brushUsedPol = (SolidColorBrush)temppol.Fill;
                    Color brushUsedColorPol = brushUsedPol.Color;
                    string colorBrushUsedStringPol = GetColorName(brushUsedColorPol);
                    foreach (var cn in FillCB.Items)
                    {
                        string temp = cn.ToString().Split(' ')[1];
                        if (temp == colorBrushUsedStringPol)
                        {
                            FillCB.SelectedItem = cn;
                        }
                    }

                    SolidColorBrush borderColorUsedpol = (SolidColorBrush)temppol.Stroke;
                    Color borderColorUsedpolCol = borderColorUsedpol.Color;
                    string borderColorUsedStringPol = GetColorName(borderColorUsedpolCol);
                    foreach (var cn in BorderColorCB.Items)
                    {
                        string temp = cn.ToString().Split(' ')[1];
                        if (temp == borderColorUsedStringPol)
                        {
                            BorderColorCB.SelectedItem = cn;
                        }
                    }
                    this.BordThickTb.Text = temppol.StrokeThickness.ToString();
                    break;

                case "Text":
                    this.BorderColorCB.IsEnabled = false;
                    this.BordThickTb.IsEnabled = false;
                    this.FillCB.IsEnabled = false;

                    TextBlock tb = obj as TextBlock;
                    FontSizeTb.Text = tb.FontSize.ToString();

                    SolidColorBrush brushUsedText = (SolidColorBrush)tb.Foreground;
                    Color brushUsedColorText = brushUsedText.Color;
                    string colorBrushUsedStringText = GetColorName(brushUsedColorText);
                    foreach (var cn in TextColorCB.Items)
                    {
                        string temp = cn.ToString().Split(' ')[1];
                        if (temp == colorBrushUsedStringText)
                        {
                            TextColorCB.SelectedItem = cn;
                        }
                    }
                    break;


            }
        }

     

        private void DrawBtn_Click(object sender, RoutedEventArgs e)
        {
            int bthickness;
            int fontSize;

            

            Color selectedBorderColor;
            Color selectedFillColor;
            Color selectedTextColor;

            if (selectedObjectType == "Ellipse")
            {
                if (!Int32.TryParse(this.BordThickTb.Text, out bthickness))
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
                    //-----------------------PROVIDNOST-----------------------------
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
                var xaml = System.Windows.Markup.XamlWriter.Save(currentObject as UIElement);   //izvlacim xaml kod za tu komponentu
                var deepCopy = System.Windows.Markup.XamlReader.Parse(xaml) as UIElement;

                DrawingImage direc = (DrawingImage)(currentObject as Image).Source;
                if (myCheckBox.IsChecked == false)
                {
                    GeometryDrawing gdrec = (GeometryDrawing)direc.Drawing;
                    gdrec.Brush = new SolidColorBrush(selectedFillColor);
                    gdrec.Pen.Thickness = bthickness;
                    gdrec.Pen.Brush = new SolidColorBrush(selectedBorderColor);
                }
                else
                {
                    GeometryDrawing gdrec = (GeometryDrawing)direc.Drawing;
                    gdrec.Brush = novaBoja;
                    gdrec.Pen.Thickness = bthickness;
                    gdrec.Pen.Brush = new SolidColorBrush(selectedBorderColor);
                }
                

                
                MainWindow.undoStack.Push(new UndoRedoModel("EditE", currentObject as Image, deepCopy as Image));
                
              

                this.Close();
            }
            else if (selectedObjectType == "Polygon")
            {
                if (!Int32.TryParse(this.BordThickTb.Text, out bthickness))
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
                    //----------------------------------PROVIDNOST--------------------------------------
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
                var xaml = System.Windows.Markup.XamlWriter.Save(currentObject as UIElement);
                var deepCopy = System.Windows.Markup.XamlReader.Parse(xaml) as UIElement;
                Polygon editPol = currentObject as Polygon;

                if (myCheckBox.IsChecked == false)
                {
                    editPol.Fill = new SolidColorBrush(selectedFillColor);
                    editPol.StrokeThickness = bthickness;
                    editPol.Stroke = new SolidColorBrush(selectedBorderColor);
                }
                else
                {
                    editPol.Fill = novaBoja;
                    editPol.StrokeThickness = bthickness;
                    editPol.Stroke = new SolidColorBrush(selectedBorderColor);
                }

                
                MainWindow.undoStack.Push(new UndoRedoModel("EditPol", currentObject as Polygon, deepCopy as Polygon));
                this.Close();
            }

            else if (selectedObjectType == "Text")
            {
                if (Int32.TryParse(FontSizeTb.Text, out fontSize))
                {
                    
                    FontSizeTb.BorderBrush = Brushes.Green;
                }
                else
                {
                    this.FontSizeTb.BorderBrush = Brushes.Red;
                    MessageBox.Show("Error! Enter valid font size.");
                    return;

                }
                if (TextColorCB.SelectedIndex > -1)
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
                else
                {
                    MessageBox.Show("Error! Select color for border.");
                    return;
                }

                
                var xaml = System.Windows.Markup.XamlWriter.Save(currentObject as UIElement);
                var deepCopy = System.Windows.Markup.XamlReader.Parse(xaml) as UIElement;
                TextBlock tb = currentObject as TextBlock;
                if (myCheckBox.IsChecked == false)
                {
                    tb.FontSize = fontSize;
                    tb.Foreground = new SolidColorBrush(selectedTextColor);
                }
                else
                {
                    tb.FontSize = fontSize;
                    tb.Foreground = novaBoja;
                }
                
                MainWindow.undoStack.Push(new UndoRedoModel("EditText", currentObject as TextBlock, deepCopy as TextBlock));
                this.Close();
            }

        }


        static string GetColorName(Color col)   //dobijanje stringa (naziv) od boje
        {
            PropertyInfo colorProperty = typeof(Colors).GetProperties()
                .FirstOrDefault(p => Color.AreClose((Color)p.GetValue(null), col));
            return colorProperty != null ? colorProperty.Name : "unnamed color";
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
