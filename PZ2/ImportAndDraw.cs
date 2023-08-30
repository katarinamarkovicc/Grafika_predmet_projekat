using PZ2.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;

namespace PZ2
{
    public class ImportAndDraw
    {
        public static List<PowerEntity> AllResources;
        private List<LineEntity> lines = new List<LineEntity>();
        private List<SubstationEntity> substations = new List<SubstationEntity>();
        public List<SwitchEntity> switches = new List<SwitchEntity>();
        private List<NodeEntity> nodes = new List<NodeEntity>();
        private List<NodeEntity> nodesToDelete = new List<NodeEntity>(); 
        private double xs;
        private double ys;
        private double minX;
        private double minY;
        private PZ2.Models.Grid drawingGrid;    
        private int blockSize = 10;
        private List<PZ2.Models.Point> usedPoints = new List<PZ2.Models.Point>();

        public ImportAndDraw()
        {
           AllResources = new List<PowerEntity>();
        }

        public void LoadAndParseXML(string location)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(location);
            XmlNodeList lineNodes = xmlDoc.SelectNodes("/NetworkModel/Lines/LineEntity");   
            XmlNodeList substationNodes = xmlDoc.SelectNodes("/NetworkModel/Substations/SubstationEntity");
            XmlNodeList switchNodes = xmlDoc.SelectNodes("/NetworkModel/Switches/SwitchEntity");
            XmlNodeList nodeNodes = xmlDoc.SelectNodes("/NetworkModel/Nodes/NodeEntity");
            lines = createLineEntities(lineNodes);  
            substations = createSubstationEntities(substationNodes);
            switches = createSwitchEntities(switchNodes);
            nodes = createNodeEntities(nodeNodes);
            
        }

        private static List<LineEntity> createLineEntities(XmlNodeList lineNodes)  
        {
            List<LineEntity> ret = new List<LineEntity>();

            foreach(XmlNode node in lineNodes)   //prolazim kroz nodove koje sam izvadila iz xml-a i popunjavam polja svojih entiteta, odnosno ja pravim svoje objekte i popunjavam njihova polja vrednostima iz xml-a
            {
                LineEntity line = new LineEntity();
                line.ConductorMaterial = node.SelectSingleNode("ConductorMaterial").InnerText;
                line.FirstEnd = long.Parse(node.SelectSingleNode("FirstEnd").InnerText);
                line.Id = long.Parse(node.SelectSingleNode("Id").InnerText);
                line.IsUnderground = bool.Parse(node.SelectSingleNode("IsUnderground").InnerText);
                line.LineType = node.SelectSingleNode("LineType").InnerText;
                line.Name = node.SelectSingleNode("Name").InnerText;
                line.R = float.Parse(node.SelectSingleNode("R").InnerText);
                line.SecondEnd = long.Parse(node.SelectSingleNode("SecondEnd").InnerText);
                line.ThermalConstantHeat = long.Parse(node.SelectSingleNode("ThermalConstantHeat").InnerText);

                bool postoji_dupla_suprotni_smer = false;

                foreach(LineEntity templine in ret) // treba ignorisati ponovno iscrtavanje vodova izmedju dva ista entiteta.
                {
                    if((templine.FirstEnd == line.SecondEnd && templine.SecondEnd == line.FirstEnd) || (templine.FirstEnd == line.FirstEnd && templine.SecondEnd == line.SecondEnd))
                    {
                        postoji_dupla_suprotni_smer = true;
                    }
                }

                if(postoji_dupla_suprotni_smer == false)
                {
                    ret.Add(line);
                }
                
            }

            return ret;
        }


        private static List<SubstationEntity> createSubstationEntities(XmlNodeList substationNodes)
        {
            List<SubstationEntity> ret = new List<SubstationEntity>();
            foreach(XmlNode node in substationNodes)
            {
               
                SubstationEntity sub = new SubstationEntity();
                sub.Id = long.Parse(node.SelectSingleNode("Id").InnerText, CultureInfo.InvariantCulture);
                sub.Name = node.SelectSingleNode("Name").InnerText;
                sub.X = double.Parse(node.SelectSingleNode("X").InnerText, CultureInfo.InvariantCulture);
                sub.Y = double.Parse(node.SelectSingleNode("Y").InnerText, CultureInfo.InvariantCulture);


                double noviX, noviY;
                ToLatLon(sub.X, sub.Y, 34, out noviX, out noviY);
                sub.X = noviX;
                sub.Y = noviY;
                ret.Add(sub);

            }
            return ret;
        }

        private static List<SwitchEntity> createSwitchEntities(XmlNodeList switchNodes)
        {
            List<SwitchEntity> ret = new List<SwitchEntity>();
            foreach(XmlNode node in switchNodes)
            {
                SwitchEntity swtch = new SwitchEntity();
                swtch.Id = long.Parse(node.SelectSingleNode("Id").InnerText, CultureInfo.InvariantCulture);
                swtch.Name = node.SelectSingleNode("Name").InnerText;
                swtch.Status = node.SelectSingleNode("Status").InnerText;
                swtch.X = double.Parse(node.SelectSingleNode("X").InnerText, CultureInfo.InvariantCulture);
                swtch.Y = double.Parse(node.SelectSingleNode("Y").InnerText, CultureInfo.InvariantCulture);
                double noviX, noviY;
                ToLatLon(swtch.X, swtch.Y, 34, out noviX, out noviY);
                swtch.X = noviX;
                swtch.Y = noviY;
                ret.Add(swtch);
            }


            return ret;
        }

        private static List<NodeEntity> createNodeEntities(XmlNodeList nodeNodes)
        {
            List<NodeEntity> ret = new List<NodeEntity>();
            foreach(XmlNode node in nodeNodes)
            {
                NodeEntity nEntity = new NodeEntity();
                nEntity.Id = long.Parse(node.SelectSingleNode("Id").InnerText, CultureInfo.InvariantCulture);
                nEntity.Name = node.SelectSingleNode("Name").InnerText;
                nEntity.X = double.Parse(node.SelectSingleNode("X").InnerText, CultureInfo.InvariantCulture);
                nEntity.Y = double.Parse(node.SelectSingleNode("Y").InnerText, CultureInfo.InvariantCulture);
                double noviX, noviY;
                ToLatLon(nEntity.X, nEntity.Y, 34, out noviX, out noviY);   //u xml-u su zapisane u nekom utm? formatu, pomocu ove f-je dobijam decimalan zapis od tog koji je zapisan u xml-u; 34 za lokaciju Novog Sada !!!
                nEntity.X = noviX;
                nEntity.Y = noviY;
                ret.Add(nEntity);
            }



            return ret;
        }

        public void ScaleFromLatLonToCanvas(double canvasWidth,double canvasHeight)
        {
            //da bi lepo se skalirale tacke na ceo kanvas
            double substationMinX, substationMinY, nodeMinX, nodeMinY, switchMinX, switchMinY, substationNodeMinX, substationNodeMinY;
            substationMinX = substations.Min((obj) => obj.X);
            nodeMinX = nodes.Min((obj) => obj.X);
            switchMinX = switches.Min((obj) => obj.X);
            substationMinY = substations.Min((obj) => obj.Y);
            nodeMinY = nodes.Min((obj) => obj.Y);
            switchMinY = switches.Min((obj) => obj.Y);
            substationNodeMinX = Math.Min(substationMinX, nodeMinX);
            minX = Math.Min(substationNodeMinX, switchMinX);    //mininimalna vrednost za x od svih entiteta

            substationNodeMinY = Math.Min(substationMinY, nodeMinY);
            minY = Math.Min(substationNodeMinY, switchMinY);    //mininimalna vrednost za y od svih entiteta


            // trazim element, nebitno da l je node substation switch da ima najmanju X i onaj sa najmanjom Y koordinatom
            double substationMaxX, substationMaxY, nodeMaxX, NodeMaxY, switchMaxX, switchMaxY, substationNodeMaxX, substationNodeMaxY,maxX,maxY;

            substationMaxX = substations.Max((obj) => obj.X);
            nodeMaxX = nodes.Max((obj) => obj.X);
            switchMaxX = switches.Max((obj) => obj.X); 

            substationMaxY = substations.Max((obj) => obj.Y);
            NodeMaxY = nodes.Max((obj) => obj.Y);
            switchMaxY = switches.Max((obj) => obj.Y);
            substationNodeMaxX = Math.Min(substationMaxX, nodeMaxX);
            substationNodeMaxY = Math.Min(substationMaxY, NodeMaxY);
            maxX = Math.Max(substationNodeMaxX, switchMaxX);    //maksimalna vrednost x i y od svih entiteta
            maxY = Math.Max(substationNodeMaxY, switchMaxY);

            //trazim element, nebitno koji, onaj sa najvecom X i onaj sa najvecom Y koordinatom

            xs = (canvasWidth/2) / (maxX - minX); //podeoci po x 
            ys = (canvasHeight / 2) / (maxY - minY); //podeoci po y    //zasto se podeoci ovako racunaju??


            // citava poenta ove funkije da nadje element koji ima najmanju x,najmanju y,najvecu x,najvecu y koordinatu i uradi ovo racunaje
            // za xs i ys - x scale valjda y scale

            CreateNewDrawingGrid(); // pozivam funkciju da kreira grid, odnosno matrixu blokova 500x500

        }

        private void CreateNewDrawingGrid()
        {
            drawingGrid = new PZ2.Models.Grid(501, 501);
            for (int x = 0; x <= 500; x++)
            {
                for (int y = 0; y <= 500; y++)
                {
                   drawingGrid.BlockMatrix[x, y] = new Block(x * blockSize, y * blockSize, BlockType.EMPTY, null, x, y); //5000x5000 je kanvas, 500x500 blokova,block size je 10, znaci grid coord je 10*x i 10*y
                   //inicijalizujem blokove, gde svaki blok ima svoje x i y koordinate u kontekstu canvasa 5000x5000 i svoje x i y koordinate u kontekstu
                   // 500x500 grida

                    //DEFINISEMO U STVARI MATRICU BLOKOVA GDE CE NA SVAKOM RAZL PARU X I Y BITI NOVI BLOK

                }
            }
        }

        public void ConvertFromLatLonToCanvasCoord() 
        {
            foreach(SubstationEntity substation in substations)
            {
                double x = Math.Round((substation.X - minX) * xs / blockSize) * blockSize;
                double y = Math.Round((substation.Y - minY) * ys / blockSize) * blockSize; // x i y su blokovskki
                // ovim formulama pronalazim koordinate u kontekstu grida 500x500, znaci blokova
                // i prosledjujem to FinClosestUnusedPoint funkciji koja vraca stvarne canvas koordinate u kontekstu 5000x5000
                PZ2.Models.Point ret = FindClosestUnusedPoint(x, y); // ova funkcija dobija blokovske
                substation.X = ret.X;
                substation.Y = ret.Y;
                drawingGrid.AddToGrid(ret.X, ret.Y, BlockType.SUBSTATION);// ovde samo pozivam funkciju za dodavanje na grid, kojoj prosledjujem
                //stvare koordinate u kontestu 5000x5000 i tip bloka; ADDTOGRID -> PROLAZIM KROZ MATRICU I POSTAVLJAM TIP KOJI PROSLEDJUJEM
                //prolazim kroz sve entitete i nalazim za njih najblizi blok u koji mogu da se smeste ???

                // !!! u xmlu sam imala x, y i onda smo konvertovala taj x i y u decmalni zapis onda sam decimalni zapis konvertovala u blokove i gledala gde taj blok mogu da smestim ???

            }

            foreach (NodeEntity node in nodes)
            {
                double x = Math.Round((node.X - minX) * xs / blockSize) * blockSize;
                double y = Math.Round((node.Y - minY) * ys / blockSize) * blockSize;
                PZ2.Models.Point ret = FindClosestUnusedPoint(x, y);
                drawingGrid.AddToGrid(ret.X, ret.Y, BlockType.NODE);
                node.X = ret.X;
                node.Y = ret.Y;
            }

            foreach(SwitchEntity sw in switches)
            {
                double x = Math.Round((sw.X - minX) * xs / blockSize) * blockSize;
                double y = Math.Round((sw.Y - minY) * ys / blockSize) * blockSize;
                PZ2.Models.Point ret = FindClosestUnusedPoint(x, y); // U RET SU STVARNE KOORDINATE
                drawingGrid.AddToGrid(ret.X, ret.Y, BlockType.SWITCH);
                sw.X = ret.X; // u ret SU STVARNE KOORDINATE
                sw.Y = ret.Y;
            }
        }


        public PZ2.Models.Point FindClosestUnusedPoint(double x,double y)
        {
            bool pointUsed = IsPointUsed(x, y); 

            if (pointUsed == false) 
            {
                PZ2.Models.Point p = new PZ2.Models.Point();
                p.X = x;
                p.Y = y;
                usedPoints.Add(p);
                PZ2.Models.Point ret = new PZ2.Models.Point();
                ret.X = x * 2;
                ret.Y = y * 2;
                return ret;
            }
            double closestX = x - blockSize; 
            closestX = (closestX < 0) ? closestX + blockSize : closestX; 
            double closestY = y - blockSize;
            closestY = (closestY < 0) ? closestY + blockSize : closestY; 

            while (IsPointUsed(closestX, closestY)){ 
                for(int j = 0;j< 2; j++)
                {
                    for(int i = 0; i < 2; i++) 
                    {
                        if (!IsPointUsed(closestX, closestY)) 
                        {
                            usedPoints.Add(new PZ2.Models.Point() { X = closestX, Y = closestY });
                            return new PZ2.Models.Point() { X = closestX * 2, Y = closestY * 2 };
                        }
                        closestY = closestY + blockSize;                       
                    }    
                    if (!IsPointUsed(closestX, closestY))
                    {
                        usedPoints.Add(new PZ2.Models.Point() { X = closestX, Y = closestY });
                        return new PZ2.Models.Point() { X = closestX * 2, Y = closestY * 2 };
                    }
                    closestX = closestX + blockSize;
                    closestY = closestY - 2 * blockSize; 
                }

            }

            usedPoints.Add(new PZ2.Models.Point() { X = closestX, Y = closestY });
            return new PZ2.Models.Point() { X = closestX * 2, Y = closestY * 2 };
        }


        public bool IsPointUsed(double x,double y)
        {
            foreach (PZ2.Models.Point ptemp in usedPoints)
            {
                if (ptemp.X == x && ptemp.Y == y)
                {
                    return true;
                }
            }
            return false;
        }

        public void DrawElements(Canvas drawingCanvas, bool cekVodovi, bool cekiraniVodoviOpseg)
        {
            DrawSubstations(drawingCanvas);
            DrawNodes(drawingCanvas);
            DrawSwitches(drawingCanvas);
            DrawLines(drawingCanvas, cekVodovi, cekiraniVodoviOpseg);
            DrawCrossMarks(drawingCanvas);
        }


        public void DrawSubstations(Canvas drawingCanvas)
        {
            
            foreach(SubstationEntity ent in substations)
            {
                int brKonekcijaSub = 0;
                foreach (LineEntity line in lines)
                {
                    if (ent.Id == line.FirstEnd)
                    {
                        brKonekcijaSub++;
                        
                    }
                    if (ent.Id == line.SecondEnd)
                    {
                        brKonekcijaSub++;
                    }
                }

                Ellipse shape = new Ellipse() { Height = 6, Width = 6}; 

                if (brKonekcijaSub >= 0 && brKonekcijaSub < 3)
                {
                    shape.Fill = Brushes.IndianRed;
                }
                else if (brKonekcijaSub >= 3 && brKonekcijaSub <= 5)
                {
                    shape.Fill = Brushes.Red;
                }
                else
                {
                    shape.Fill = Brushes.DarkRed;
                }

                shape.ToolTip = "Substation: \n" + "ID:" + ent.Id + "\nName: " + ent.Name + "\nBroj konekcija: " + brKonekcijaSub;
                Canvas.SetLeft(shape, ent.X + 2); 
                Canvas.SetTop(shape, ent.Y + 2);
                ent.PowerEntityShape = shape; 
                AllResources.Add(ent); 
                drawingCanvas.Children.Add(ent.PowerEntityShape);
                
            }
        }

        public void DrawNodes(Canvas drawingCanvas)
        {
            foreach (NodeEntity ent in nodes)
            {
                int brKonekcijaNode = 0;
                foreach (LineEntity line in lines)
                {
                    if (ent.Id == line.FirstEnd)
                    {
                        brKonekcijaNode++;

                    }
                    if (ent.Id == line.SecondEnd)
                    {
                        brKonekcijaNode++;
                    }
                }

                Ellipse shape = new Ellipse() { Height = 6, Width = 6 }; 

                if (brKonekcijaNode >= 0 && brKonekcijaNode < 3)
                {
                    shape.Fill = Brushes.IndianRed;
                }
                else if (brKonekcijaNode >= 3 && brKonekcijaNode <= 5)
                {
                    shape.Fill = Brushes.Red;
                }
                else
                {
                    shape.Fill = Brushes.DarkRed;
                }
                
                shape.ToolTip = "Node: \n" + "ID:" + ent.Id + "\nName: " + ent.Name + "\nBroj konekcija: " + brKonekcijaNode;
                Canvas.SetLeft(shape, ent.X + 2);
                Canvas.SetTop(shape, ent.Y + 2);
                ent.PowerEntityShape = shape;
                AllResources.Add(ent);
                drawingCanvas.Children.Add(ent.PowerEntityShape);
            }
        }

        public void DeleteNode(List<NodeEntity> nds, Canvas drawingCanvas)
        {
            if (nds != null)
            {
                foreach (NodeEntity ent in nds)
                {
                   
                        drawingCanvas.Children.Remove(ent.PowerEntityShape);

                }
            }
          
        }

        public List<NodeEntity> NodesToDelete()
        {
            return nodesToDelete;
        }

        public void DrawSwitches(Canvas drawingCanvas)
        {
            int brKonekcijaSw;

            foreach (SwitchEntity ent in switches)
            {
                brKonekcijaSw = 0;

                foreach (LineEntity line in lines)
                {

                    if (ent.Id == line.FirstEnd)
                    {
                        brKonekcijaSw++;

                    }
                    if (ent.Id == line.SecondEnd)
                    {
                        brKonekcijaSw++;
                    }
                }

                Ellipse shape = new Ellipse() { Height = 6, Width = 6 };


                if (brKonekcijaSw >= 0 && brKonekcijaSw < 3)
                {
                    shape.Fill = Brushes.IndianRed;
                }
                else if (brKonekcijaSw >= 3 && brKonekcijaSw <= 5)
                {
                    shape.Fill = Brushes.Red;
                }
                else
                {
                    shape.Fill = Brushes.DarkRed;
                }

                
                

                shape.ToolTip = "Switch: \n" + "ID:" + ent.Id + "\nName: " + ent.Name + "\nStatus: "+ ent.Status + "\nBroj konekcija: " + brKonekcijaSw;
                Canvas.SetLeft(shape, ent.X + 2);
                Canvas.SetTop(shape, ent.Y + 2);
                ent.PowerEntityShape = shape;
                AllResources.Add(ent);
                drawingCanvas.Children.Add(ent.PowerEntityShape);

            }
        }

        public List<SwitchEntity> SwchToDelete()
        {
            return switches;
        }

        public void DrawLines(Canvas drawingCanvas, bool cekVodovi, bool cekiraniVodoviOpseg)
        {
            //Linije se iscrtavaju drugacije od ostalih entiteta iz razloga sto postoje uslovi u zadatku u vezi presecanja
            // i bfs algoritma

            List<Tuple<double, double, double, double, PowerEntity, PowerEntity, LineEntity>> connectedCoords = findAndSortConnectedCoords(cekVodovi);//zbog ubrzanja, kreni od najblizih tacaka pa nadalje <<X1,Y1,X2,Y2,startEnt,endEnt,Line>>
            // funkcija findAndSortConnectedCoords() vraca sve linije razbijene tako da buud u obliku Tuple
            // gde je <<X1,Y1,X2,Y2,startEnt,endEnt,Line>> 

            foreach (Tuple<double, double, double, double,PowerEntity,PowerEntity, LineEntity> ent in connectedCoords)
            {
                // prolazi se kroz vracenu listu tupli, odnosno linija razbijenih na koordinate pocetnog,krajnjeg entiteta, samih entiteta i linije
                PowerEntity startEntity = ent.Item5, endEntity = ent.Item6; // 
                double x1=ent.Item1, y1=ent.Item2, x2=ent.Item3, y2=ent.Item4; //       vrednosti iz tuple se dodeljuju lokalnim varijablama da bi
                //                                                              njima moglo da se rukuje valjd
                LineEntity conline = ent.Item7;//                                  
                
                if((x1 == 0 || x2 == 0 || y1 == 0 || y2 == 0) || (x1==x2 && y1==y2))    //postoje reference na neke tipove koji ne postoje
                {
                    continue;   // ukoliko je neka od koordinata 0 ili su koordinate x1==x2 i y1==2 iteracija se preskace iz nekog razloga
                }

                List<Block> lineBlocks = drawingGrid.createLineUsingBFS(x1, y1, x2, y2, false); //LISTA BLOKOVA, odnosno blokovi kroz koju ta linija prolazi
                // poziva se funkcija za kreiranje linije koriscenjem BFS algoritma, sa zadnjim parametrom false, da se linije ne seku

                if (lineBlocks.Count < 2)//ako nisam nasla put bez presecanja ukljucujem presecanje linija
                {
                    lineBlocks = drawingGrid.createLineUsingBFS(x1, y1, x2, y2, true);
                }

                /*Polyline ugaona_linija = new Polyline();
                 ugaona_linija.Stroke = new SolidColorBrush(Colors.Black);
                 ugaona_linija.StrokeThickness = 1.5;*/

                //----------------------------------------------------------------------------------
                Polyline ugaona_linija = new Polyline();
                if (cekiraniVodoviOpseg)
                {
                    
                    if (ent.Item7.R < 1)
                    {
                        ugaona_linija.Stroke = new SolidColorBrush(Colors.Red);
                    }
                    else if (ent.Item7.R >= 1 && ent.Item7.R <= 2)
                    {
                        ugaona_linija.Stroke = new SolidColorBrush(Colors.Orange);
                    }
                    else 
                    {
                        ugaona_linija.Stroke = new SolidColorBrush(Colors.Yellow);
                    }
                    ugaona_linija.StrokeThickness = 1.5;
                }
                else 
                {
                    if (ent.Item7.ConductorMaterial == "Acsr") 
                    {
                        ugaona_linija.Stroke = new SolidColorBrush(Colors.Black);
                        ugaona_linija.StrokeThickness = 1.5;
                    }
                    else if (ent.Item7.ConductorMaterial == "Steel")    
                    {
                        ugaona_linija.Stroke = new SolidColorBrush(Colors.Gray);
                        ugaona_linija.StrokeThickness = 1.5;
                    }
                    else if (ent.Item7.ConductorMaterial == "Copper")
                    {
                        ugaona_linija.Stroke = new SolidColorBrush(Colors.Brown);
                        ugaona_linija.StrokeThickness = 1.5;
                    }
                    else
                    {
                        ugaona_linija.Stroke = new SolidColorBrush(Colors.Purple);
                        ugaona_linija.StrokeThickness = 1.5;
                    }


                    //ugaona_linija.Stroke = new SolidColorBrush(Colors.Black);
                    //ugaona_linija.StrokeThickness = 1.5;
                }

                
                //----------------------------------------------------------------------------------

                for (int i = 0; i < lineBlocks.Count; i++)
                {
                    BlockType lineType = BlockType.EMPTY;
                    //horizontalne linije
                    if (i < lineBlocks.Count - 1) //ne smemo uporediti psolednji sa sledecim (nepostojecim)
                    {
                        if (lineBlocks[i].XCoo != lineBlocks[i + 1].XCoo)
                        {
                            lineType = BlockType.HLINE;
                        }
                        else if (lineBlocks[i].YCoo != lineBlocks[i + 1].YCoo)
                        {
                            lineType = BlockType.VLINE;
                        }
                        if (lineType != BlockType.EMPTY)
                        {
                            drawingGrid.AddLineToGrid(lineBlocks[i].XCoo, lineBlocks[i].YCoo, lineType); //oznaci polja u gridu sa odgovarajucim tipom
                        }
                    }
                    System.Windows.Point linePoint = new System.Windows.Point(lineBlocks[i].XCoo + 5, lineBlocks[i].YCoo + 5);
                    ugaona_linija.Points.Add(linePoint);
                   


                }
                ugaona_linija.MouseRightButtonDown += SetElementColors;
                ugaona_linija.MouseRightButtonDown += endEntity.OnClick;
                ugaona_linija.MouseRightButtonDown += startEntity.OnClick;
                
                ugaona_linija.ToolTip = "Power line\n" + "ID: " + conline.Id + "\nName: " + conline.Name + "\nTyle: " + conline.LineType + "\nConductor material: " + conline.ConductorMaterial + "\nUnderground: " + conline.IsUnderground.ToString();
                drawingCanvas.Children.Add(ugaona_linija);

            }
        }


        List<Tuple<double,double,double,double, PowerEntity, PowerEntity, LineEntity>> findAndSortConnectedCoords(bool cekiranoVodovi)
        {

            List<Tuple<double, double, double, double, PowerEntity, PowerEntity, LineEntity>> ret = new List<Tuple<double, double, double, double, PowerEntity, PowerEntity, LineEntity>>();
            
            foreach (LineEntity ent in lines)
            {

                PowerEntity startEntity = new PowerEntity(), endEntity = new PowerEntity();

                double x1 = 0, y1 = 0, x2 = 0, y2 = 0; 

                foreach (SubstationEntity temp in substations)
                {
                    if (temp.Id == ent.FirstEnd)
                    {
                        x1 = temp.X;
                        y1 = temp.Y;
                        startEntity = temp;
                    }
                    if (temp.Id == ent.SecondEnd)
                    {
                        x2 = temp.X;
                        y2 = temp.Y;
                        endEntity = temp;
                    }
                }

                foreach (NodeEntity temp in nodes)
                {
                    if (temp.Id == ent.FirstEnd)
                    {
                        x1 = temp.X;
                        y1 = temp.Y;
                        startEntity = temp;
                    }
                    if (temp.Id == ent.SecondEnd)
                    {
                      
                        x2 = temp.X;
                        y2 = temp.Y;
                        endEntity = temp;
                    }
                }

                foreach (SwitchEntity temp in switches)
                {
                    if (cekiranoVodovi)
                    {
                        if (temp.Id == ent.FirstEnd)
                        {
                            x1 = temp.X;
                            y1 = temp.Y;
                            startEntity = temp;
                        }
                        if (temp.Id == ent.SecondEnd)
                        {
                            x2 = temp.X;
                            y2 = temp.Y;
                            endEntity = temp;
                        }
                    }
                    else
                    {
                        if (temp.Id == ent.FirstEnd && temp.Status == "Closed")
                        {
                            x1 = temp.X;
                            y1 = temp.Y;
                            startEntity = temp;
                           
                        }
                        else if(temp.Status == "Open" && temp.Id==ent.FirstEnd)
                        {
                            
                            
                                foreach (NodeEntity node in nodes)
                                {
                                    if (node.Id == ent.SecondEnd && temp.Id==ent.FirstEnd)
                                    {
                                        nodesToDelete.Add(node);
                                       
                                    }
                                }
                            
                           
                        }

                        if (temp.Id == ent.SecondEnd && temp.Status == "Closed")
                        {
                            x2 = temp.X;
                            y2 = temp.Y;
                            endEntity = temp;
                        }
                    }
                   
                }
                
              

                
         
                // na kraju kad se zavrsi for, prodje se kroz svaku liniju, napravimo konkretan objekat tuple i popunimo ga
                //sa gore definisanim i poppunjenim objektima entiteta koje linije spaja i koordinatama njihovinm
                Tuple<double, double, double, double, PowerEntity, PowerEntity, LineEntity> oneLineTuple = new Tuple<double, double, double, double, PowerEntity, PowerEntity, LineEntity>(x1, y1, x2, y2, startEntity, endEntity, ent);
                ret.Add(oneLineTuple); // i dodamo tuplu u listu tupli koju ova funkcija i vraca
        
            }
            var result = ret.OrderBy(tup => ((tup.Item1 - tup.Item3) * (tup.Item1 - tup.Item3) + (tup.Item2 - tup.Item4) * (tup.Item2 - tup.Item4))).ToList();
            // pre vracanja liste sortiramo je,ne znam zasto

            return result;
        }

        void DrawCrossMarks(Canvas drawingCanvas)
        {
            int br = 0;
            foreach (Block block in drawingGrid.BlockMatrix)
            {
                if(block.BType == BlockType.CROSS_LINE)
                {
                    Ellipse tempEllipse = new Ellipse() { Width = 5, Height = 5, Fill = Brushes.Black };
                    Canvas.SetLeft(tempEllipse, block.XCoo + 3);
                    Canvas.SetTop(tempEllipse, block.YCoo + 3);
                    drawingCanvas.Children.Add(tempEllipse);
                    br++;
                }
            }

        }


        public void SetElementColors(object sender, EventArgs e)
        {
            foreach (var resource in AllResources)
            {
                resource.SetElementColor();
            }
        }

        public static void ToLatLon(double utmX, double utmY, int zoneUTM, out double latitude, out double longitude)
        {
            bool isNorthHemisphere = true;

            var diflat = -0.00066286966871111111111111111111111111;
            var diflon = -0.0003868060578;

            var zone = zoneUTM;
            var c_sa = 6378137.000000;
            var c_sb = 6356752.314245;
            var e2 = Math.Pow((Math.Pow(c_sa, 2) - Math.Pow(c_sb, 2)), 0.5) / c_sb;
            var e2cuadrada = Math.Pow(e2, 2);
            var c = Math.Pow(c_sa, 2) / c_sb;
            var x = utmX - 500000;
            var y = isNorthHemisphere ? utmY : utmY - 10000000;

            var s = ((zone * 6.0) - 183.0);
            var lat = y / (c_sa * 0.9996);
            var v = (c / Math.Pow(1 + (e2cuadrada * Math.Pow(Math.Cos(lat), 2)), 0.5)) * 0.9996;
            var a = x / v;
            var a1 = Math.Sin(2 * lat);
            var a2 = a1 * Math.Pow((Math.Cos(lat)), 2);
            var j2 = lat + (a1 / 2.0);
            var j4 = ((3 * j2) + a2) / 4.0;
            var j6 = ((5 * j4) + Math.Pow(a2 * (Math.Cos(lat)), 2)) / 3.0;
            var alfa = (3.0 / 4.0) * e2cuadrada;
            var beta = (5.0 / 3.0) * Math.Pow(alfa, 2);
            var gama = (35.0 / 27.0) * Math.Pow(alfa, 3);
            var bm = 0.9996 * c * (lat - alfa * j2 + beta * j4 - gama * j6);
            var b = (y - bm) / v;
            var epsi = ((e2cuadrada * Math.Pow(a, 2)) / 2.0) * Math.Pow((Math.Cos(lat)), 2);
            var eps = a * (1 - (epsi / 3.0));
            var nab = (b * (1 - epsi)) + lat;
            var senoheps = (Math.Exp(eps) - Math.Exp(-eps)) / 2.0;
            var delt = Math.Atan(senoheps / (Math.Cos(nab)));
            var tao = Math.Atan(Math.Cos(delt) * Math.Tan(nab));

            longitude = ((delt * (180.0 / Math.PI)) + s) + diflon;
            latitude = ((lat + (1 + e2cuadrada * Math.Pow(Math.Cos(lat), 2) - (3.0 / 2.0) * e2cuadrada * Math.Sin(lat) * Math.Cos(lat) * (tao - lat)) * (tao - lat)) * (180.0 / Math.PI)) + diflat;
        }
    }

}
