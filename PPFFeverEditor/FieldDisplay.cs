using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Reflection;

namespace PPFFeverEditor
{
    public class FieldDisplay
    {
        Chain chain;
        Bitmap puyoImage, arrowImage;
        Panel fieldPanel;
        Bitmap[,] imageAtPosition = new Bitmap[6, 12];

        int cursorX, cursorY;
        bool displayCursor = false;

        public FieldDisplay(Panel panel)
        {
            // Load our resources
            puyoImage = Resources.puyo;
            arrowImage = Resources.arrow;

            fieldPanel = panel;
            // Little hack to get a double-buffered panel to work
            typeof(Control).InvokeMember("DoubleBuffered", BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic, null, fieldPanel, new object[] { true });
            fieldPanel.Paint += Paint;
            fieldPanel.MouseEnter += MouseEnter;
            fieldPanel.MouseMove += MouseMove;
            fieldPanel.MouseLeave += MouseLeave;
            fieldPanel.MouseClick += MouseClick;
        }

        private void Paint(object sender, PaintEventArgs e)
        {
            if (chain == null)
                return;

            Graphics g = e.Graphics;

            puyoImage.SetResolution(g.DpiX, g.DpiY);
            arrowImage.SetResolution(g.DpiX, g.DpiY);

            // Draw the puyo on the field
            for (int y = 0; y < 12; y++)
            {
                for (int x = 0; x < 6; x++)
                {
                    if (chain.Grid[x, y] != Puyo.None)
                        g.DrawImageUnscaled(imageAtPosition[x, y], x * 32, y * 32);
                }
            }

            // Draw the arrow
            if (chain.Grid[chain.ArrowPosition.X, chain.ArrowPosition.Y] == Puyo.None)
            {
                if (ConnectedL(chain.ArrowPosition.X, chain.ArrowPosition.Y, chain.TriggerColor) &&
                    ConnectedR(chain.ArrowPosition.X, chain.ArrowPosition.Y, chain.TriggerColor) &&
                    ConnectedD(chain.ArrowPosition.X, chain.ArrowPosition.Y, chain.TriggerColor))
                    g.DrawImageUnscaled(RotateBitmap(arrowImage, 90), chain.ArrowPosition.X * 32, chain.ArrowPosition.Y * 32);

                else if (ConnectedL(chain.ArrowPosition.X, chain.ArrowPosition.Y, chain.TriggerColor) &&
                    ConnectedR(chain.ArrowPosition.X, chain.ArrowPosition.Y, chain.TriggerColor))
                    g.DrawImageUnscaled(RotateBitmap(arrowImage, 90), chain.ArrowPosition.X * 32, chain.ArrowPosition.Y * 32);

                else if (ConnectedL(chain.ArrowPosition.X, chain.ArrowPosition.Y, chain.TriggerColor) &&
                    ConnectedD(chain.ArrowPosition.X, chain.ArrowPosition.Y, chain.TriggerColor))
                    g.DrawImageUnscaled(RotateBitmap(arrowImage, 135), chain.ArrowPosition.X * 32, chain.ArrowPosition.Y * 32);

                else if (ConnectedR(chain.ArrowPosition.X, chain.ArrowPosition.Y, chain.TriggerColor) &&
                    ConnectedD(chain.ArrowPosition.X, chain.ArrowPosition.Y, chain.TriggerColor))
                    g.DrawImageUnscaled(RotateBitmap(arrowImage, 45), chain.ArrowPosition.X * 32, chain.ArrowPosition.Y * 32);

                else if (ConnectedL(chain.ArrowPosition.X, chain.ArrowPosition.Y, chain.TriggerColor))
                    g.DrawImageUnscaled(RotateBitmap(arrowImage, 180), chain.ArrowPosition.X * 32, chain.ArrowPosition.Y * 32);

                else if (ConnectedD(chain.ArrowPosition.X, chain.ArrowPosition.Y, chain.TriggerColor))
                    g.DrawImageUnscaled(RotateBitmap(arrowImage, 90), chain.ArrowPosition.X * 32, chain.ArrowPosition.Y * 32);

                else if (ConnectedR(chain.ArrowPosition.X, chain.ArrowPosition.Y, chain.TriggerColor))
                    g.DrawImageUnscaled(arrowImage, chain.ArrowPosition.X * 32, chain.ArrowPosition.Y * 32);
            }

            // Draw the cursor
            if (displayCursor)
                g.DrawRectangle(new Pen(Color.FromArgb(255, 255, 255), 2), cursorX * 32, cursorY * 32, 32, 32);
        }

        // Returns the X position of the puyo
        private int GetXPosition(int x, int y)
        {
            if (chain.Grid[x, y] == Puyo.Nuisance) return 0;
			if (!ConnectedU(x, y, chain.Grid[x, y]) && !ConnectedL(x, y, chain.Grid[x, y]) && !ConnectedD(x, y, chain.Grid[x, y]) && !ConnectedR(x, y, chain.Grid[x, y])) return 0;
			if (!ConnectedU(x, y, chain.Grid[x, y]) && !ConnectedL(x, y, chain.Grid[x, y]) &&  ConnectedD(x, y, chain.Grid[x, y]) && !ConnectedR(x, y, chain.Grid[x, y])) return 1;
			if ( ConnectedU(x, y, chain.Grid[x, y]) && !ConnectedL(x, y, chain.Grid[x, y]) && !ConnectedD(x, y, chain.Grid[x, y]) && !ConnectedR(x, y, chain.Grid[x, y])) return 2;
			if ( ConnectedU(x, y, chain.Grid[x, y]) && !ConnectedL(x, y, chain.Grid[x, y]) &&  ConnectedD(x, y, chain.Grid[x, y]) && !ConnectedR(x, y, chain.Grid[x, y])) return 3;
			if (!ConnectedU(x, y, chain.Grid[x, y]) && !ConnectedL(x, y, chain.Grid[x, y]) && !ConnectedD(x, y, chain.Grid[x, y]) &&  ConnectedR(x, y, chain.Grid[x, y])) return 4;
			if (!ConnectedU(x, y, chain.Grid[x, y]) && !ConnectedL(x, y, chain.Grid[x, y]) &&  ConnectedD(x, y, chain.Grid[x, y]) &&  ConnectedR(x, y, chain.Grid[x, y])) return 5;
			if ( ConnectedU(x, y, chain.Grid[x, y]) && !ConnectedL(x, y, chain.Grid[x, y]) && !ConnectedD(x, y, chain.Grid[x, y]) &&  ConnectedR(x, y, chain.Grid[x, y])) return 6;
			if ( ConnectedU(x, y, chain.Grid[x, y]) && !ConnectedL(x, y, chain.Grid[x, y]) &&  ConnectedD(x, y, chain.Grid[x, y]) &&  ConnectedR(x, y, chain.Grid[x, y])) return 7;
			if (!ConnectedU(x, y, chain.Grid[x, y]) &&  ConnectedL(x, y, chain.Grid[x, y]) && !ConnectedD(x, y, chain.Grid[x, y]) && !ConnectedR(x, y, chain.Grid[x, y])) return 8;
			if (!ConnectedU(x, y, chain.Grid[x, y]) &&  ConnectedL(x, y, chain.Grid[x, y]) &&  ConnectedD(x, y, chain.Grid[x, y]) && !ConnectedR(x, y, chain.Grid[x, y])) return 9;
			if ( ConnectedU(x, y, chain.Grid[x, y]) &&  ConnectedL(x, y, chain.Grid[x, y]) && !ConnectedD(x, y, chain.Grid[x, y]) && !ConnectedR(x, y, chain.Grid[x, y])) return 10;
			if ( ConnectedU(x, y, chain.Grid[x, y]) &&  ConnectedL(x, y, chain.Grid[x, y]) &&  ConnectedD(x, y, chain.Grid[x, y]) && !ConnectedR(x, y, chain.Grid[x, y])) return 11;
			if (!ConnectedU(x, y, chain.Grid[x, y]) &&  ConnectedL(x, y, chain.Grid[x, y]) && !ConnectedD(x, y, chain.Grid[x, y]) &&  ConnectedR(x, y, chain.Grid[x, y])) return 12;
			if (!ConnectedU(x, y, chain.Grid[x, y]) &&  ConnectedL(x, y, chain.Grid[x, y]) &&  ConnectedD(x, y, chain.Grid[x, y]) &&  ConnectedR(x, y, chain.Grid[x, y])) return 13;
			if ( ConnectedU(x, y, chain.Grid[x, y]) &&  ConnectedL(x, y, chain.Grid[x, y]) && !ConnectedD(x, y, chain.Grid[x, y]) &&  ConnectedR(x, y, chain.Grid[x, y])) return 14;
			if ( ConnectedU(x, y, chain.Grid[x, y]) &&  ConnectedL(x, y, chain.Grid[x, y]) &&  ConnectedD(x, y, chain.Grid[x, y]) &&  ConnectedR(x, y, chain.Grid[x, y])) return 15;

            return 0;
        }

        // Returns the Y position of the puyo
        private int GetYPosition(int x, int y)
        {
            switch (chain.Grid[x, y])
            {
                case Puyo.Red: return 0;
                case Puyo.Green: return 1;
                case Puyo.Blue: return 2;
                case Puyo.Yellow: return 3;
                case Puyo.Purple: return 4;
                case Puyo.Nuisance: return 5;
                default: return 0;
            }
        }

        // Returns if puyo is connected on the top
        private bool ConnectedU(int x, int y, Puyo p)
        {
            return (y > 0 && chain.Grid[x, y - 1] == p);
        }
        // Returns if puyo is connected on the left
        private bool ConnectedL(int x, int y, Puyo p)
        {
            return (x > 0 && chain.Grid[x - 1, y] == p);
        }
        // Returns if puyo is connected on the bottom
        private bool ConnectedD(int x, int y, Puyo p)
        {
            return (y < 11 && chain.Grid[x, y + 1] == p);
        }
        // Returns if puyo is connected on the right
        private bool ConnectedR(int x, int y, Puyo p)
        {
            return (x < 5 && chain.Grid[x + 1, y] == p);
        }

        // Sets the field display's field to another field
        public void SetChain(Chain c)
        {
            chain = c;

            if (chain != null)
            {
                for (int y = 0; y < 12; y++)
                {
                    for (int x = 0; x < 6; x++)
                    {
                        if (chain.Grid[x, y] != Puyo.None)
                        {
                            imageAtPosition[x, y] = puyoImage.Clone(new Rectangle(GetXPosition(x, y) * 32, GetYPosition(x, y) * 32, 32, 32), puyoImage.PixelFormat);
                        }
                    }
                }
            }

            fieldPanel.Refresh();
        }

        // Mouse Enter
        private void MouseEnter(object sender, EventArgs e)
        {
            cursorX = (int)(fieldPanel.PointToClient(Cursor.Position).X / 32);
            cursorY = (int)(fieldPanel.PointToClient(Cursor.Position).Y / 32);
            displayCursor = true;
            fieldPanel.Refresh();
        }
        // Mouse Move
        private void MouseMove(object sender, MouseEventArgs e)
        {
            int newCursorX = (int)(fieldPanel.PointToClient(Cursor.Position).X / 32);
            int newCursorY = (int)(fieldPanel.PointToClient(Cursor.Position).Y / 32);

            if (newCursorX != cursorX || newCursorY != cursorY)
            {
                cursorX = newCursorX;
                cursorY = newCursorY;

                fieldPanel.Refresh();
            }
        }
        // Mouse Leave
        private void MouseLeave(object sender, EventArgs e)
        {
            displayCursor = false;
            fieldPanel.Refresh();
        }
        // Mouse Click
        private void MouseClick(object sender, EventArgs e)
        {
            if (displayCursor && chain != null)
            {
                chain.ArrowPosition = new Point(cursorX, cursorY);
                fieldPanel.Refresh();
            }
        }

        // Rotates a Bitmap
        private Bitmap RotateBitmap(Bitmap b, float angle)
        {
            Bitmap dst = new Bitmap(b.Width, b.Height);

            Graphics g = Graphics.FromImage(dst);
            g.TranslateTransform((float)b.Width / 2, (float)b.Height / 2);
            g.RotateTransform(angle);
            g.TranslateTransform(-(float)b.Width / 2, -(float)b.Height / 2);
            g.DrawImage(b, new Point(0, 0));
            return dst;
        }
    }
}