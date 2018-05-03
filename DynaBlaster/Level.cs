using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace DynaBlaster
{
    public class Level
    {
        public static int timeLimit = 240;
        public Point[] obstacleDestroy;
        public Tile[,] tiles;
        public Tile obstacleTile;
        public int columns, rows, timeLeft, type, monstersNumber;
        private int[] monsterTemplate;
        public List<Monster> monsters;

        public Level(int rows, int columns, int type, int[] monsterTemplate)
        {
            this.columns = columns;
            this.rows = rows;
            tiles = new Tile[rows,columns];
            this.type = type;
            this.monsterTemplate = monsterTemplate;
            monstersNumber = monsterTemplate.Length;
            monsters = new List<Monster>();
            obstacleDestroy = new Point[7];
            loadLevelTiles();
            initialize();
        }

        public void draw(SpriteBatch sb)
        {
            int x, y;
            for (int row = 0; row < rows; row++)
                for (int column = 0; column < columns; column++)
                {
                    x = column * 16 + Game1.dynOrigin.X - 8;
                    if (column == 0)
                        x += 8;
                    y = row * 16 + Game1.dynOrigin.Y + 24;
                    tiles[row, column].draw(sb, x, y, this);             
                }
            foreach (Monster m in monsters)
                m.draw(sb);
        }

        public void setMonsters()
        {
            monsters.Clear();
            Random r = new Random();
            double chance = monsterTemplate.Length/(double)(rows * columns)*3;
            int monsterNr = 0;
            while (true)                                                                //pętla nieskończona z której program wychodzi dopiero jak wylosuje pozycje przeszkód dla danego poziomu
                for (int row = 1; row < rows - 1; row += 1)
                    for (int column = 2; column < columns - 2; column++)
                    {
                        if (tiles[row, column].walkable == true &&      //tile musi być do chodzenia
                            tiles[row, column].hasObstacle != true &&      //nie może mieć już przeszkody
                            !((column == 2 && row < 4) || (row == 1 && column < 5))) //nie moze byc kolo spawnu postaci
                        {
                            if (r.NextDouble() < chance)
                            {
                                monsters.Add(new Monster(column * 16 - 8, row * 16 + 22, monsterTemplate[monsterNr]));
                                monsterNr++;
                                if (monsterNr == monsterTemplate.Length)
                                    return;
                            }
                        }
                    }
        }

        public void setObstacles()
        {
            for (int row = 1; row < rows - 1; row += 1)
                for (int column = 2; column < columns - 2; column++)
                {
                    tiles[row, column].hasObstacle = false;
                    tiles[row, column].hasTeleport = false;
                }

                    int obstacles = 0, obstaclesLimit = (rows * columns) / 7;
            double chance = 1.0 / obstaclesLimit;
            Random r = new Random();

            while (true)                                                                //pętla nieskończona z której program wychodzi dopiero jak wylosuje pozycje przeszkód dla danego poziomu
                for (int row = 1; row < rows - 1; row += 1)
                    for (int column = 2; column < columns - 2; column++)
                    {
                        if (tiles[row, column].walkable == true &&      //tile musi być do chodzenia
                            tiles[row, column].hasObstacle != true &&      //nie może mieć już przeszkody
                            !((column == 2 && row < 4) || (row == 1 && column < 5)))    //nie może być koło spawnu postaci
                            if (r.NextDouble() < chance)
                            {
                                tiles[row, column].hasObstacle = true;
                                obstacles++;
                                if (obstacles >= obstaclesLimit)
                                    return;
                            }
                    }
        }

        private void placeTeleport()
        {
            Random r = new Random();
            int targetRow = r.Next(1, rows - 1);
            int targetColumn = r.Next(2, columns - 2);
            while(true)
            {
                if (tiles[targetRow, targetColumn].hasObstacle)
                {
                    tiles[targetRow, targetColumn].hasTeleport = true;
                    return;
                }
                targetColumn++;
                if (targetColumn > columns -3)
                {
                    targetColumn = 2;
                    targetRow++;
                    if (targetRow > rows - 2)
                        targetRow = 1;
                }
            }
        }

        public void updateMonsters(Character character)
        {
            for (int i = monsters.Count-1; i >= 0; i--)
            {
                Monster m = monsters.ElementAt(i);
                if (m.scoreDisplay && Game1.gameMiliseconds - m.deathTime > 4000)
                    monsters.Remove(m);
                else
                {
                    if (!m.dead && character.row == m.row && character.column == m.column && !character.dead)
                        character.die();
                    m.update();
                }                   
            }
        }

        private void loadLevelTiles()   //wypelnia tablicę tiles zwracając uwagę na wymiary i typ poziomu
        {
            obstacleTile = Tile.getTile(type, 2);
            for (int row = 0; row < rows - 1; row++)            //lewa sciana
                tiles[row, 0] = Tile.getTile(type, 26);
            tiles[rows - 1, 0] = Tile.getTile(type, 27);        //lewy dolny róg
            for (int row = 0; row < rows - 1; row++)            //prawa ściana
                tiles[row, columns - 1] = Tile.getTile(type, 19);
            tiles[rows - 1, columns - 1] = Tile.getTile(type, 20);  //prawy dolny róg
            for (int column = 2; column < columns - 2; column++)    //góra
                tiles[0, column] = Tile.getTile(type, column % 2 + 5);
            tiles[0, 1] = Tile.getTile(type, 4);       //lewy górny róg
            tiles[0, columns - 2] = Tile.getTile(type, 7);   //prawy górny róg
            for (int column = 2; column < columns - 2; column++)    //dół
                tiles[rows - 1, column] = Tile.getTile(type, (column - 2) % 3 + 12);
            if (type == 2)
            {
                for (int row = 1; row < rows - 1;)                                                                //lewa wewnętrzna ściana
                {
                    tiles[row++, 1] = Tile.getTile(type, 16);
                    tiles[row++, 1] = Tile.getTile(type, 17);
                }
            }
            else
                for (int row = 1; row < rows - 1; row++)                                                                //lewa wewnętrzna ściana
                {
                    if ((row - 2) % 7 == 0)       //wyjątkowy element jak kratka raz na 7 tilesów
                    {
                        tiles[row++, 1] = Tile.getTile(type, 17);
                        tiles[row, 1] = Tile.getTile(type, 18);
                    }
                    else
                        tiles[row, 1] = Tile.getTile(type, 16);
                }
            tiles[0, 1] = Tile.getTile(type, 4);             //lewy górny róg wewnętrznej ściany
            tiles[rows - 1, 1] = Tile.getTile(type, 15);    //lewy dolny róg wewnętrznej ściany
            if (type == 2)
            {
                for (int row = 1; row < rows - 1;)                                                                //lewa wewnętrzna ściana
                {
                    tiles[row++, columns - 2] = Tile.getTile(type, 8);
                    tiles[row++, columns - 2] = Tile.getTile(type, 9);
                }
            }
            else
                for (int row = 1; row < rows - 1; row++)                                                                //prawa wewnętrzna ściana
                {
                    if ((row - 2) % 7 == 0)       //wyjątkowy element jak kratka raz na 7 tilesów
                    {
                        tiles[row++, columns - 2] = Tile.getTile(type, 9);
                        tiles[row, columns - 2] = Tile.getTile(type, 10);
                    }
                    else
                        tiles[row, columns - 2] = Tile.getTile(type, 8);
                }
            tiles[0, columns - 2] = Tile.getTile(type, 7); //prawy górny róg wewnętrznej ściany
            tiles[rows - 1, columns - 2] = Tile.getTile(type, 11); //prawy dolny róg wewnętrznej ściany
            for (int column = 2; column < columns - 2; column++)
                tiles[1, column] = Tile.getTile(type, 0);
            for (int row = 2; row < rows - 2; row += 2)
                for (int column = 2; column < columns - 2; column++)
                {
                    if (column % 2 == 0)
                    {
                        tiles[row, column] = Tile.getTile(type, 0);
                        tiles[row + 1, column] = Tile.getTile(type, 0);
                    }
                    else
                    {
                        tiles[row, column] = Tile.getTile(type, 3);
                        tiles[row + 1, column] = Tile.getTile(type, 1);
                    }
                }

            for (int i = 0; i < 7; i++)
                obstacleDestroy[i] = Tile.getTile(type, 28 + i).sourceRectangle.Location;


        }

        public void initialize()
        {
            setObstacles();
            setMonsters();
            placeTeleport();
            timeLeft = timeLimit;
        }
    }

}