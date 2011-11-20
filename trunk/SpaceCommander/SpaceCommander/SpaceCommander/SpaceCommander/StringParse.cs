namespace SpaceCommander
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using GameApplicationTools.UI;
    using System.Reflection;
    using Microsoft.Xna.Framework;

    public class StringParser : GameConsole.DefaultStringParser
    {
        public override object Parse(Type type, string value)
        {
            if (type == typeof(Color))
            {
                PropertyInfo p = typeof(Color).GetProperty(value, BindingFlags.Static | BindingFlags.Public);
                return p.GetValue(null, null);
            }

            return base.Parse(type, value);
        }
    }
}
