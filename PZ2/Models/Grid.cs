using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PZ2.Models
{
     class Grid
    {
        private Block[,] blockMatrix;
        private readonly static List<int> dr = new List<int> { -1, +1, 0, 0 };
        private readonly static List<int> dc = new List<int> { 0, 0, +1, -1 };
        internal Block[,] BlockMatrix { get => blockMatrix; set => blockMatrix = value; }

        public Grid(int xBlocks,int yBlocks)
        {
            BlockMatrix = new Block[xBlocks, yBlocks];
        }

        public void AddToGrid(double xC,double yC, BlockType bType) // funkcija prima stvarne canvas 5000x5000 koordinate i tip bloka
        {
            for(int x = 0; x < BlockMatrix.GetLength(0); x++) // prolazi kroz matricu 500x500, znaci kroz grid blokova
            {
                if(xC == blockMatrix[x, 0].XCoo)// i proverava ako se poklapa x koordinata prosledjena sa poljem xCoo, znaci stvarna x koja se cuva
                    // kao u polje u objektu bloka onda se vrti sledeci for za trazenje y koordinate
                {
                    for (int y = 0; y < BlockMatrix.GetLength(1); y++)
                    {
                        if(yC == BlockMatrix[x, y].YCoo) // kad se i y koordinata nadje
                        {
                            BlockMatrix[x, y].BType = bType; // tom objektu u matrici, na tom indeksu, tim koordinatama se stavi prosledjeni type
                        }
                    }
                }
            }
        }


        //BFS algoritam
        public List<Block> createLineUsingBFS(double X1,double Y1,double X2,double Y2,bool cross) //funkcija dobija koordinate4 pocetnog i krajnjem entiteta
            // koje linija spaja
        {
            List<Block> shortestWay = new List<Block>(); // lista blokova koja ce predstavljati put
            Queue<Block> searchQueue = new Queue<Block>(); // queue za algoritam gde se smestaju blokovi koji se obradjuju
            bool pathFound = false; // fleg da li je put nadjen
            Block[,] visited = new Block[blockMatrix.GetLength(0), BlockMatrix.GetLength(1)];
            // matrica blokova 500x500 koji su poseceni

            //moramo pronaci tacno x,y indekse bloka na koji smo aproksimirali tj indekse u gridu.. blokovske indekse
             int approxX1 = -1, approxY1 = -1 , approxX2 = -1, approxY2 = -1;  


            //forovima ispod prolazim kroz matricu BlockMatrix i poredim parametre X1,Y1.. sa blockMatric[].XCoo, da bi nasao njihove blokovske koordinate
            //jer mi blokovske koordinate tj indeksi trebaju da bih dole instancirala blokove
             for(int x = 0; x < BlockMatrix.GetLength(0); x++)
             {
                 if(X1 == blockMatrix[x, 0].XCoo)   //ovi forovi sluze da predjemo na koordinate u kontekstu blokova, jer su prosledjene u kontekstu canvasa 
                 {
                     approxX1 = x;
                 }
             }

             for (int y = 0; y < BlockMatrix.GetLength(1); y++)
             {
                 if (Y1 == blockMatrix[0 , y].YCoo)
                 {
                     approxY1 = y;
                 }
             }

            for (int x = 0; x < BlockMatrix.GetLength(0); x++)
            {
                if (X2 == blockMatrix[x, 0].XCoo)
                {
                    approxX2 = x;
                }
            }

            for (int y = 0; y < BlockMatrix.GetLength(1); y++)
            {
                if (Y2 == blockMatrix[0, y].YCoo)
                {
                    approxY2 = y;
                }
            }


                //instanciram start i end blok
            Block starRouteBlock = new Block(X1, Y1, approxX1, approxY1);
            Block endRouteBlock = new Block(X2, Y2, approxX2, approxY2);
            
            // u visited matricu na indeksu [approxX1,approxY1] znaci na indeksu pocetnog entiteta stavljam taj pocetni entitet
            visited[approxX1, approxY1] = starRouteBlock; // visited[x1,xs2] = start
            searchQueue.Enqueue(starRouteBlock); // ubacujem ga u queue



            while (searchQueue.Count > 0)
            {
                //while se vrti sve dok u queue ima nesto

                Block tBlock = searchQueue.Dequeue(); // sa queue-a se skida element, 0 sa snimka!!!

                if(tBlock.Approx_X == approxX2 && tBlock.Approx_Y == approxY2)// da li sam pronasla put do end tacke, odnosno da li sam stigla do bloka koji ima koordinate kraja
                 {
                    //proveravam da li indeksi tj koordinate skinutom elementa odgovaraju approxX2 i aprproxY2 tj krajnjem entitetu 
                    // ako odgovaraju to znaci da smo dosli do kraja i nasli put kojim ce linija da ide i postavljamo fleg i brejkujemo
                    pathFound = true;
                    break;
                }
                //ukoliko se jos nije doslo do krajnjeg entiteta ulazi se u for i obilazi se svuda okolo tog bloka

                for(int i = 0; i < 4; i++)//obidji svuda okolo bloka
                {
                    int nextRow = tBlock.Approx_X + dr[i];
                    int nextColumn = tBlock.Approx_Y + dc[i];
/*                    // obilazenje se radi po principu dodavanja na i oduzimanja jedinice ili nule
 *                    // kako bi se bukvalno obilazilo okolo
 *                    
        private readonly static List<int> dr = new List<int> { -1, +1, 0, 0 };
        private readonly static List<int> dc = new List<int> { 0, 0, +1, -1 };*/
                    

                    //iteracija se preskace ako se desi da smo dosli do ivica canvasa
                    if (nextRow < 0 || nextColumn < 0 || nextRow >= BlockMatrix.GetLength(0) || nextColumn >= BlockMatrix.GetLength(1))//provera opsega
                    {
                        continue;                                           ///???
                    }

                    //iteracija se preskace ukoliko je vec posecen blok
                    if (visited[nextRow, nextColumn] != null) //preskoci posecena
                    {
                        continue;
                    }


                    //ovaj if je za slucaj da linije ne smeju da se presecaju
                    // iteracija se preskace ako blok nije krajnji entitet i ako nije empty, znaci ako je vec popunjen necim, nekim switchem npr ne moze tuda ici linija
                    if(!(nextRow == endRouteBlock.Approx_X && nextColumn == endRouteBlock.Approx_Y) && (blockMatrix[nextRow,nextColumn].BType != BlockType.EMPTY) && cross == false)
                    {
                        continue;
                    }
                    


                    //ukoliko linije smeju da se presecaju
                    //ifovi ispod govore da ukoliko blok koji se posmatra nije krajnji entitet a jeste neki entitet, npr switch,node ili sub, on se preskace
                    // ali se ne preskace ako je npr LINE entitet, jer smeju da se presecaju linije zato sto je cross true
                    //ne zelimo ni linije da prolaze kroz polja sa elementima koje ne povezuju
                    if(!(nextRow == endRouteBlock.Approx_X && nextColumn == endRouteBlock.Approx_Y) && (blockMatrix[nextRow, nextColumn].BType == BlockType.NODE) && cross == true)
                    {
                        continue;
                    }

                    if (!(nextRow == endRouteBlock.Approx_X && nextColumn == endRouteBlock.Approx_Y) && (blockMatrix[nextRow, nextColumn].BType == BlockType.SWITCH) && cross == true)
                    {
                        continue;
                    }

                    if (!(nextRow == endRouteBlock.Approx_X && nextColumn == endRouteBlock.Approx_Y) && (blockMatrix[nextRow, nextColumn].BType == BlockType.SUBSTATION) && cross == true)
                    {
                        continue;
                    }

                    // u searchqueue se dodaje taj neki susedni blok koji zadovoljava sve uslove, znaci nije posecen nije popunjen necim itd
                    searchQueue.Enqueue(new Block(BlockMatrix[nextRow, nextColumn].XCoo, BlockMatrix[nextRow, nextColumn].YCoo, nextRow, nextColumn));

                    //a u visited matricu na indeksu tog koji je stavljen u queue da je sve ok se stavlja prethodno
                    // obradjivani blok
                    visited[nextRow, nextColumn] = tBlock; //stavljamo na sledeci x i y da bi nakon pronalaska puta mogli se vratiti kroz matricu do start bloka
                }
            }

            if (pathFound)
            {
                shortestWay.Add(endRouteBlock);
                Block previousBlock = visited[endRouteBlock.Approx_X, endRouteBlock.Approx_Y];
                while(previousBlock.Approx_X > 0 && !(previousBlock.XCoo == starRouteBlock.XCoo && previousBlock.YCoo == starRouteBlock.YCoo && previousBlock.Approx_X == starRouteBlock.Approx_X && previousBlock.Approx_Y == starRouteBlock.Approx_Y))
                {
                    //pravimo putanju
                    if(BlockMatrix[previousBlock.Approx_X,previousBlock.Approx_Y].BType == BlockType.EMPTY)
                    {
                        BlockMatrix[previousBlock.Approx_X, previousBlock.Approx_Y].BType = BlockType.LINE;
                        
                    }
                    shortestWay.Add(previousBlock);
                    previousBlock = visited[previousBlock.Approx_X, previousBlock.Approx_Y];
                }
                shortestWay.Add(previousBlock);
            }
            return shortestWay;

        }

        

        public void AddLineToGrid(double XC, double YC, BlockType lineType)
        {
            for(int x=0; x < BlockMatrix.GetLength(0); x++)
            {
                if(XC == BlockMatrix[x, 0].XCoo)
                {
                    for (int y = 0; y < BlockMatrix.GetLength(1); y++)
                    {
                        if(YC == blockMatrix[x, y].YCoo)
                        {
                            if(blockMatrix[x, y].BType == BlockType.NODE || blockMatrix[x, y].BType == BlockType.SUBSTATION || blockMatrix[x, y].BType == BlockType.SWITCH)//ako je element ne sme tuda linija
                            {
                                return;
                            }
                            if(blockMatrix[x, y].BType == BlockType.LINE || blockMatrix[x, y].BType == BlockType.EMPTY) //ako je linija ili prazno sme
                            {
                                blockMatrix[x, y].BType = lineType;

                            }
                            else if (blockMatrix[x, y].BType != lineType)
                            {
                                blockMatrix[x, y].BType = BlockType.CROSS_LINE;
                            }
                            return;
                        }
                    }
                }
            }
        }



    }
}
