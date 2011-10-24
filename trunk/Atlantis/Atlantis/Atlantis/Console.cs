using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xen.Ex.Graphics2D;
using Microsoft.Xna.Framework;
using Xen;
using Microsoft.Xna.Framework.Graphics;

namespace Atlantis
{
    public class Console
    {
        private TextElementRect consoleRect;
  
         /// <summary>
         /// create a console to show attributes etc
         /// </summary>
         /// <param name="width"></param>
         /// <param name="height"></param>
        public Console(int width,int height)
        {
            var sizeInPixels = new Vector2(width, height);

            this.consoleRect = new TextElementRect(sizeInPixels);
            this.consoleRect.Colour = Color.Gainsboro;

            //align the element rectangle to the bottom centre of the screen
            this.consoleRect.VerticalAlignment = VerticalAlignment.Bottom;
            this.consoleRect.HorizontalAlignment = HorizontalAlignment.Centre;
        }


        public TextElementRect getTextElementRect()
        {
            return this.consoleRect;
        }

        public void setFont(SpriteFont font)
        {
            this.consoleRect.Font = font;
        }

        public void setColor(Color color)
        {
            this.consoleRect.Colour = color;
        }

        public void add(String text)
        {
            consoleRect.Text.AppendLine(text);
        }

        public void clear()
        {
            consoleRect.Text.Clear();
        }

      
    }
}
