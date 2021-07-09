using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShimLib {
    public class PcfFont : IFont {
        int fw = 16;
        int fh = 16;

        class toc_entry {
            public TableType type;      /* See below, indicates which table */
            public TableFormat format;  /* See below, indicates how the data are formatted in the table */
            public int size;            /* In bytes */
            public int offset;          /* from start of file */
        }

        enum TableType {
            PCF_PROPERTIES          = (1<<0),
            PCF_ACCELERATORS        = (1<<1),
            PCF_METRICS             = (1<<2),
            PCF_BITMAPS             = (1<<3),
            PCF_INK_METRICS         = (1<<4),
            PCF_BDF_ENCODINGS       = (1<<5),
            PCF_SWIDTHS             = (1<<6),
            PCF_GLYPH_NAMES         = (1<<7),
            PCF_BDF_ACCELERATORS    = (1<<8),
        }

        enum TableFormat {
            PCF_DEFAULT_FORMAT      = 0x00000000,
            PCF_INKBOUNDS           = 0x00000200,
            PCF_ACCEL_W_INKBOUNDS   = 0x00000100,
            PCF_COMPRESSED_METRICS  = 0x00000100,
            PCF_GLYPH_PAD_MASK      = (3<<0),            /* See the bitmap table for explanation */
            PCF_BYTE_MASK           = (1<<2),            /* If set then Most Sig Byte First */
            PCF_BIT_MASK            = (1<<3),            /* If set then Most Sig Bit First */
            PCF_SCAN_UNIT_MASK      = (3<<4),            /* See the bitmap table for explanation */
        }

        byte[] header;
        int table_count;
        toc_entry[] tables;

        public PcfFont(byte[] pcf) {
            using (var ms = new MemoryStream(pcf))
            using (var br = new BinaryReader(ms)) {
                header = new byte[4];
                br.Read(header, 0, 4);
                table_count = br.ReadInt32();
                tables = new toc_entry[table_count];
                for (int i = 0; i < tables.Length; i++) {
                    var table = new toc_entry();
                    tables[i] = table;
                    table.type = (TableType)br.ReadInt32();
                    table.format = (TableFormat)br.ReadInt32();
                    table.size = br.ReadInt32();
                    table.offset = br.ReadInt32();
                }
            }
        }

        public int FontHeight => fh;

        public void DrawString(string text, IntPtr dispBuf, int dispBW, int dispBH, int dx, int dy, Color color) {
            int icolor = color.ToArgb();
            int x = dx;
            int y = dy;
            foreach (char ch in text) {
                if (ch == '\r') {
                    continue;
                }
                if (ch == '\n') {
                    x = dx;
                    y += fh;
                    continue;
                }
                
                Drawing.DrawRectangle(dispBuf, dispBW, dispBH, x + 4, y + 4, x + 8, y + 8, icolor, false);
                x += fw;
            }
        }

        public Size MeasureString(string text) {
            int maxX = 0;
            int maxY = 0;
            int x = 0;
            int y = 0;
            foreach (char ch in text) {
                if (ch == '\r') {
                    continue;
                }
                if (ch == '\n') {
                    x = 0;
                    y += fh;
                    continue;
                }
                maxX = Math.Max(maxX, x + fw);
                maxY = Math.Max(maxY, y + fh);
                x += fw;
            }

            return new Size(maxX, maxY);
        }
    }
}
