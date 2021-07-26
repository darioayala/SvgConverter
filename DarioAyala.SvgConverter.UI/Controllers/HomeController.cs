using DarioAyala.SvgConverter.Business;
using DarioAyala.SvgConverter.UI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DarioAyala.SvgConverter.UI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult<string>> Index(InputModel param)
        {

            try
            {

                string processedBase64Bmp;
                string res = "";

                if (param.File != null)
                {
                    string fileContent;

                    if (param.File.Length > 0)
                    {

                        using (var reader = new StreamReader(param.File.OpenReadStream()))
                        {
                            fileContent = await reader.ReadToEndAsync();
                        }

                        var processorParams = Mapper(param, fileContent);

                        var processor = new Processor();
                        processedBase64Bmp = processor.TransformSvg(processorParams);

                        res = processedBase64Bmp;
                    }
                }


                Response.StatusCode = 200;
                return Json(new { res = res });

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Response.StatusCode = 400;
                return null;
            }
        }

        private ProcessorParams Mapper(InputModel model, string file)
        {
            var res = new ProcessorParams()
            {
                AlphaValue = model.AlphaValue,
                File = file,
                RotateAngle = model.RotateAngle,
                RotateCenterX = model.RotateCenterX,
                RotateCenterY = model.RotateCenterY,
                ScaleX = model.ScaleX,
                ScaleY = model.ScaleY,
                TranslateX = model.TranslateX,
                TranslateY = model.TranslateY,
                BBoxDiag = model.BBoxDiag,
                BgColor = model.BgColor
            };

            return res;
        }
    }
}
