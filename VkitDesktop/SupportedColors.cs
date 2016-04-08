using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace VkitDesktop
{
    public static class SupportedColors
    {

        public static List<System.Drawing.KnownColor> GetSupportedColors()
        {
            List<System.Drawing.KnownColor> colors = new List<System.Drawing.KnownColor>();
            colors.Add(KnownColor.White);
            colors.Add(KnownColor.Pink);
            colors.Add(KnownColor.Brown);
            colors.Add(KnownColor.PeachPuff);
            colors.Add(KnownColor.Yellow);
            colors.Add(KnownColor.GreenYellow);
            colors.Add(KnownColor.DarkGreen);
            colors.Add(KnownColor.Turquoise);
            colors.Add(KnownColor.MediumBlue);
            colors.Add(KnownColor.LightSlateGray);
            colors.Add(KnownColor.MediumOrchid);
            colors.Add(KnownColor.Firebrick);
            colors.Add(KnownColor.Orange);

            return colors;
        }
    }
}
