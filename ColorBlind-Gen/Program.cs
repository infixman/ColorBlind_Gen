using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace ColorBlind_Gen
{
    class Program
    {
        public static Circle[] circles = new Circle[5000];
        public static string motiv = string.Empty;
        public static int count = 0;
        public static int maxDiameter = 22;
        public static int minDiameter = 8;
        public static int lastAdded = 0;
        public static int lastAddedTimeout = 100;
        public static Bitmap motive;
        public static Color[] off, on;
        public static Bitmap outPutImage;

        /*Args:
            0. input file path
            1. output image path
            2. output image size (WIDTHxHEIGHT)
        */
        static void Main(string[] args)
        {
            motiv = args[0]; // input image path
            outPutImage = new Bitmap(Convert.ToInt32(args[2].Split(new char[] { 'x', 'X' })[0]), Convert.ToInt32(args[2].Split(new char[] { 'x', 'X' })[1]));
        }

        public void setup()
        {
            //size(600, 600);
            //smooth();
            //background(255);
            //colorMode(RGB);
            //noFill();
            motive = new Bitmap(motiv);

            Color[] _off = {
// style 1
   Color.FromArgb(156, 165, 148),
                Color.FromArgb(172, 180, 165),
                Color.FromArgb(187, 185, 100),
                Color.FromArgb(215, 218, 170),
                Color.FromArgb(229, 213, 125),
                Color.FromArgb(209, 214, 175)
// style 2
/*     Color(#F49427), Color(#C9785D), Color(#E88C6A), Color(#F1B081), 
     Color(#F49427), Color(#C9785D), Color(#E88C6A), Color(#F1B081), 
     Color(#F49427), Color(#C9785D), Color(#E88C6A), Color(#F1B081), Color(#FFCE00)*/
  };

            Color[] _on = {
    Color.FromArgb(249, 187, 130),
                Color.FromArgb(235, 161, 110),
                Color.FromArgb(252, 205, 132)
/*    Color(#89B270), Color(#7AA45E),  Color(#B6C674),  Color(#7AA45E),  Color(#B6C674),  
    Color(#89B270), Color(#7AA45E),  Color(#B6C674),  Color(#7AA45E),  Color(#B6C674), 
     Color(#89B270), Color(#7AA45E),  Color(#B6C674),  Color(#7AA45E),  Color(#B6C674), Color(#FECB05)*/
  };

            on = _on;
            off = _off;


        }

        public void draw()
        {
            if (count < circles.Length)
            {
                circles[count] = new Circle();
                if (circles[count].overlapsAny())
                {
                    circles[count] = null;
                }

                if (circles[count] != null)
                {
                    circles[count].draw();

                    if (count > 1)
                    {
                        float nearest = 100000;
                        float current = 0;
                        int nearestIndex = -1;
                        for (int i = 0; i < count; i++)
                        {
                            current = dist(circles[i].x, circles[i].y, circles[count].x, circles[count].y);
                            if (current < nearest)
                            {
                                nearest = current;
                                nearestIndex = i;
                            }
                        }

                    }

                    count++;
                    lastAdded = 0;
                }
                else
                {
                    if (lastAdded > lastAddedTimeout && maxDiameter > minDiameter)
                    {
                        maxDiameter--;
                        // minDiameter--;
                        lastAdded = 0;
                    }
                    lastAdded++;
                }
            }
            lastX = lastY = -1;
        }

        public class Point
        {
            float x, y;

            Point(float x, float y)
            {
                this.x = x;
                this.y = y;
            }
        }

        public class Circle
        {
            float x, y, radius;
            int[] xs, ys;
            Color bg = Color.FromArgb(255, 255, 255);

            Circle()
            {
                radius = random(minDiameter, maxDiameter) / 2.0;
                float a = random(PI * 2);
                float r = random(0, width * .48 - radius);
                x = lastX < 0 ? width * .5 + cos(a) * r : lastX;
                y = lastY < 0 ? height * .5 + sin(a) * r : lastY;
                init();
            }

            Circle(int x, int y, float rad)
            {
                this.radius = rad;
                this.x = x;
                this.y = y;
                init();
            }

            void init()
            {
                int x = int(this.x), y = int(this.y), r = int(radius);
                int[] xs = { x, x, x, x - r, x + r, int(x - r * .93), int(x - r * .93), int(x + r * .93), int(x + r * .93) };
                int[] ys = { y, y - r, y + r, y, y, int(y + r * .93), int(y - r * .93), int(y + r * .93), int(y - r * .93) };
                this.xs = xs;
                this.ys = ys;
            }

            bool overlapsMotive()
            {
                for (int i = 0; i < xs.length; i++)
                {
                    int col = motive.get(xs[i], ys[i]);
                    if (col != bg)
                    {
                        return true;
                    }
                }
                return false;
            }

            bool overlapsAny()
            {
                for (int i = 0; i < xs.length; i++)
                {
                    int col = get(xs[i], ys[i]);
                    if (col != bg)
                    {
                        return true;
                    }
                }
                if (radius > minDiameter)
                {
                    radius = minDiameter;
                    init();
                    return overlapsAny();
                }
                return false;
            }

            bool intersects(Circle c)
            {
                int dx = int(c.x) - int(x), dy = int(c.y) - int(y);
                return dx * dx + dy * dy < (c.radius + radius) * (c.radius + radius);
            }

            bool inside(Circle c)
            {
                int dx = (int)c.x - (int)x, dy = (int)c.y - (int)y;
                return dx * dx + dy * dy < (c.radius - radius) * (c.radius - radius);
            }

            public void draw()
            {
                if (fg < 0) fg = overlapsMotive() ? on[int(random(0, on.length))] : off[int(random(0, off.length))];

                //fill(fg);
                //noStroke();
                ellipse(x, y, radius * 2, radius * 2);
            }

        }

        int lastX = -1, lastY = -1;
    }
}
