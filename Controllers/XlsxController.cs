using System;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;

namespace H4WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class XlsxController : ControllerBase
    {
        
        private IWebHostEnvironment _webHostEnvironment;

        public XlsxController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

      
        public IActionResult Export()
        {
            string sWebRootFolder = _webHostEnvironment.WebRootPath;
            string sFileName = $"{Guid.NewGuid()}.xlsx";
            FileInfo file = new FileInfo(Path.Combine("d:/excel/", sFileName));
            using (ExcelPackage package = new ExcelPackage(file))
            {
                // 添加worksheet
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("aspnetcore");
                //添加头
                worksheet.Cells[1, 1].Value = "ID";
                worksheet.Cells[1, 2].Value = "Name";
                worksheet.Cells[1, 3].Value = "Url";
                //添加值
                worksheet.Cells["A2"].Value = 1000;
                worksheet.Cells["B2"].Value = "LineZero";
                worksheet.Cells["C2"].Value = "http://www.cnblogs.com/linezero/";

                worksheet.Cells["A3"].Value = 1001;
                worksheet.Cells["B3"].Value = "LineZero GitHub";
                worksheet.Cells["C3"].Value = "https://github.com/linezero";
                worksheet.Cells["C3"].Style.Font.Bold = true;

                package.Save();
            }
            return File(sFileName, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        [HttpPost]
        public IActionResult Import([FromForm] IFormCollection formCollection)
        {
            string sWebRootFolder = "d:/excel/";// _webHostEnvironment.WebRootPath;
            string sFileName = $"{Guid.NewGuid()}.jpg";
            FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
            try
            {
                using (FileStream fs = new FileStream(file.ToString(), FileMode.Create))
                {
                    IFormFile excelfile = formCollection.Files[index: 0];
                    excelfile.CopyTo(fs);
                    fs.Flush();
                }
                using (ExcelPackage package = new ExcelPackage(file))
                {
                    StringBuilder sb = new StringBuilder();
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
                    int rowCount = worksheet.Dimension.Rows;
                    int ColCount = worksheet.Dimension.Columns;
                    bool bHeaderRow = true;
                    for (int row = 1; row <= rowCount; row++)
                    {
                        for (int col = 1; col <= ColCount; col++)
                        {
                            if (bHeaderRow)
                            {
                                sb.Append(worksheet.Cells[row, col].Value.ToString() + "\t");
                            }
                            else
                            {
                                sb.Append(worksheet.Cells[row, col].Value.ToString() + "\t");
                            }
                        }
                        sb.Append(Environment.NewLine);
                    }
                    return Content(sb.ToString());
                }
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }
    }
}
