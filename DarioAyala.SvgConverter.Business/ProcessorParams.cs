using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DarioAyala.SvgConverter.Business
{
    public class ProcessorParams
    {
        public string AlphaValue { get; set; }
        public string File { get; set; }
        public int RotateAngle { get; set; }
        public int RotateCenterX { get; set; }
        public int RotateCenterY { get; set; }
        public string ScaleX { get; set; }
        public string ScaleY { get; set; }
        public string TranslateX { get; set; }
        public string TranslateY { get; set; }
        public string BBoxDiag { get; set; }
        public string BgColor { get; set; }
    }
}
