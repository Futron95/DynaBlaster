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
        public int columns, rows, timeLeft, type;
        private int[] monsterTemplate;
        List<Monster> monsters;

        public Level(int rows, int columns, int type, int[] monsterTemplate)
        {
            this.columns = columns;
            this.rows = rows;
            tiles = new Tile[rows,columns];
            this.type = type;
            this.monsterTemplate = monsterTemplate;
            monsters = new List<Monster>();
            obstacleDestroy = new Point[7];          
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
                    tiles[row, column].draw(sb, x, y);             
                }
            foreach (Monster m in monsters)
            {
                    m.draw(sb);
            }
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
                    tiles[row, column].hasObstacle = false;

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

        public void updateMonsters()
        {
            foreach (Monster m in monsters)
                m.update();
        }

        public void initialize()
        {
            setObstacles();
            setMonsters();
            timeLeft = timeLimit;
        }
    }

}