using SFML.Graphics;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Raven.Utils {
    public class TextureUtil {
        // vars

        // constructor
        private TextureUtil() {

        }

        // public
        public static Texture FromBitmap(Bitmap bitmap) {
            if (bitmap == null) {
                throw new ArgumentNullException("bitmap");
            }

            using (Stream stream = new MemoryStream()) {
                bitmap.Save(stream, ImageFormat.Png);
                return new Texture(stream);
            }
        }

        // private

    }
}
