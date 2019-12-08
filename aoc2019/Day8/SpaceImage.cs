using Advent.Of.Code;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day8
{
    internal class SpaceImage : AbstractAocTask
    {
        public SpaceImage()
        {
        }

        public override void First()
        {
            var img = new SiImage("Day8/image.txt", 25, 6);
            var minzerolayer = img.layers
                .Select(l => new { layer = l, zeroes = l.Where(p => p == '0').Count() })
                .OrderBy(l => l.zeroes).First().layer;
            var onesandtwos = minzerolayer.Count(l => l == '1') * minzerolayer.Count(l => l == '2');
            Echo($"#1s * #2s: {onesandtwos}");
            ValidateAnswer(onesandtwos, 1742);
        }

        public override void Second()
        {
            var img = new SiImage("Day8/image.txt", 25, 6);
            var merged = new char[img.width * img.height];
            for (int i = 0; i < img.width*img.height; i++)
            {
                foreach(var layer in img.layers)
                {
                    merged[i] = layer[i];
                    if (layer[i] == '1' || layer[i]=='0') break;
                }
            }
            for (int i = 0; i < img.height; i++)
            {
                Echo($"{merged.Skip(i * img.width).Take(img.width).Aggregate("", (str, c) => str+(c=='0'?' ':'*'))}");
            }
        }
    }

    class SiImage
    {
        public int width;
        public int height;
        public List<char[]> layers = new List<char[]>();
        public SiImage(string path, int width, int height)
        {
            this.width = width;
            this.height = height;
            //var fi = new FileInfo(path);
            //var numLayers = fi.Length / (width * height);
            using (var data = new StreamReader("Day8/image.txt"))
            {
                while (!data.EndOfStream)
                {
                    var buf = new char[width * height];
                    data.ReadBlock(buf, 0, buf.Length);
                    layers.Add(buf);
                }
            }
        }
    }
}