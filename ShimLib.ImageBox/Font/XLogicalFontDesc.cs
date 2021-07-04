using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShimLib {
    public class XLogicalFontDesc {
        public string Foundry;         // Type foundry - vendor or supplier of this font
        public string FamilyName;      // Typeface family
        public string WeightName;      // Weight of type
        public string Slant;           // Slant (upright, italic, oblique, reverse italic, reverse oblique, or "other")
        public string SetwidthName;    // Proportionate width (e.g. normal, condensed, narrow, expanded/double-wide)
        public string AddStyleName;    // Additional style (e.g. (Sans) Serif, Informal, Decorated)
        public int PixelSize;          // Size of characters, in pixels; 0 (Zero) means a scalable font
        public int PointSize;          // Size of characters, in tenths of points
        public int ResolutionX;        // Horizontal resolution in dots per inch (DPI), for which the font was designed
        public int ResolutionY;        // Vertical resolution, in DPI
        public string Spacing;         // monospaced, proportional, or "character cell"
        public int AverageWidth;       // Average width of characters of this font; 0 means scalable font
        public string CharsetRegistry; // Registry defining this character set
        public string CharsetEncoding; // Registry's character encoding scheme for this set
        public XLogicalFontDesc(string text) {
            if (text == null) {
                Foundry = string.Empty;
                FamilyName = string.Empty;
                WeightName = string.Empty;
                Slant = string.Empty;
                SetwidthName = string.Empty;
                AddStyleName = string.Empty;
                PixelSize = 0;
                PointSize = 0;
                ResolutionX = 0;
                ResolutionY = 0;
                Spacing = string.Empty;
                AverageWidth = 0;
                CharsetRegistry = string.Empty;
                CharsetEncoding = string.Empty;
                return;
            }
            var words = text.Split('-');
            Foundry = words[1];
            FamilyName = words[2];
            WeightName = words[3];
            Slant = words[4];
            SetwidthName = words[5];
            AddStyleName = words[6];
            int.TryParse(words[7], out PixelSize);
            int.TryParse(words[8], out PointSize);
            int.TryParse(words[9], out ResolutionX);
            int.TryParse(words[10], out ResolutionY);
            Spacing = words[11];
            int.TryParse(words[12], out AverageWidth);
            CharsetRegistry = words[13];
            CharsetEncoding = words[14];
        }
    }
}
