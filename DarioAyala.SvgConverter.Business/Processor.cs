using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Xml;
using Svg;

namespace DarioAyala.SvgConverter.Business
{
    public class Processor
    {
        public string TransformSvg(ProcessorParams param)
        {

            // Float parameters convertion
            float opacity = 1f;
            if (param.AlphaValue != null) opacity = float.Parse(param.AlphaValue, CultureInfo.InvariantCulture);

            float scaleX = 0f;
            float scaleY = 0f;
            if (param.ScaleX != null) scaleX = float.Parse(param.ScaleX, CultureInfo.InvariantCulture);
            if (param.ScaleY != null) scaleY = float.Parse(param.ScaleY, CultureInfo.InvariantCulture);

            float translateX = 0;
            float translateY = 0;
            if (param.TranslateX != null) translateX = float.Parse(param.TranslateX, CultureInfo.InvariantCulture);
            if (param.TranslateY != null) translateY = float.Parse(param.TranslateY, CultureInfo.InvariantCulture);

            double bboxDiag = 0d;
            if (param.BBoxDiag != null) bboxDiag = double.Parse(param.BBoxDiag, CultureInfo.InvariantCulture);


            // Convert string to svgDocument object
            var svgDoc = StringToSvgDocument(param.File);


            // Change opacity of all elements
            svgDoc.ApplyRecursive(x =>
                                    { 
                                        x.Opacity = opacity;
                                        x.FillOpacity = opacity;
                                        x.StrokeOpacity = opacity;
                                    });



            //Transforms

            // Rotate
            if (param.RotateAngle != 0 || param.RotateCenterX != 0 || param.RotateCenterY != 0) 
                Rotate(ref svgDoc, param.RotateAngle, param.RotateCenterX, param.RotateCenterY);

            // Scale
            if (scaleX != 1 || scaleY != 1)
                Scale(ref svgDoc, scaleX, scaleY);

            // Translate
            if (translateX != 0 || translateY != 0) 
                TranslateX(ref svgDoc, translateX, translateY);

            // Bounding Box diagonal
            RemoveBBoxLessThanDiagonal(ref svgDoc, bboxDiag);

            // Export to bitmap
            Bitmap bitmap = svgDoc.Draw();
            
            // Change backgroud color
            Color bgColor = ColorTranslator.FromHtml(param.BgColor);

            Bitmap bg = new Bitmap(bitmap.Width, bitmap.Height);
            using (Graphics grap = Graphics.FromImage(bg))
            {
                grap.Clear(bgColor);
                grap.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                grap.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.None;
                grap.DrawImage(bitmap, Point.Empty);
            }

            MemoryStream stream = new MemoryStream();
            bg.Save(stream, ImageFormat.Bmp);
            byte[] imageBytes = stream.ToArray();

            // Encode to Base64
            return Convert.ToBase64String(imageBytes);
        }

        private void TranslateX(ref SvgDocument svgDoc, float translateX, float translateY)
        {
            var translate = new Svg.Transforms.SvgTranslate(translateX, translateY);
            ApplyTransformAllPaths(ref svgDoc, translate);
        }


        /// <summary>
        /// Get a SvgDocument from string
        /// </summary>
        /// <param name="svgString"></param>
        /// <returns></returns>
        private SvgDocument StringToSvgDocument(string svgString)
        {
            XmlDocument xmlSgv = new XmlDocument();
            xmlSgv.LoadXml(svgString);

            return SvgDocument.Open(xmlSgv);

        }

        private void Rotate(ref SvgDocument doc, int angle, int centerX, int centerY)
        {
            Svg.Transforms.SvgRotate rot;

            rot = new Svg.Transforms.SvgRotate(angle, centerX, centerY);
            ApplyTransformAllPaths(ref doc, rot);
        }

        private void Scale(ref SvgDocument doc, float x, float y)
        {
            var scale = new Svg.Transforms.SvgScale(x, y);
            ApplyTransformAllPaths(ref doc, scale);
        }

        private void ApplyTransformAllPaths(ref SvgDocument doc, Svg.Transforms.SvgTransform transf)
        {
            doc.ApplyRecursive(x =>
            {
                //if (x.GetType() == typeof(SvgPath))
                //{
                    if (x.Transforms == null)
                    {
                        x.Transforms = new Svg.Transforms.SvgTransformCollection();
                    }

                    x.Transforms.Add(transf);
                //}

            });
        }

        private void RemoveBBoxLessThanDiagonal(ref SvgDocument doc, double value)
        {
            doc.ApplyRecursive(x =>
            {
                if (x.GetType() == typeof(SvgPath))
                {
                    var path = (SvgPath)x;
                    var diag = CalculateBBoxDiagonal(path);
                    if (diag < value) x.Parent.Children.Remove(x);
                }
            });

        }

        private double CalculateBBoxDiagonal(SvgPath path)
        {
            var x = path.Bounds.Width;
            var y = path.Bounds.Height;

            var aux = Math.Pow(x, 2) + Math.Pow(y, 2);
            return Math.Sqrt(aux);
        }

    }
}

