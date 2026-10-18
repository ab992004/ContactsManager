using Microsoft.AspNetCore.Mvc;
using ServiceContract;

namespace CRUDEx.Controllers
{
    [Route("[controller]")]
    public class CountriesController : Controller
    {
        private readonly ICountriesService _countriesService;
        public CountriesController(ICountriesService countriesService)
        {
            _countriesService = countriesService;
        }
        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> UploadFromExcel()
        {
            return View();
        }
        [Route("[action]")]
        [HttpPost]
        public async Task<IActionResult> UploadFromExcel(IFormFile excelFile)
        {
            if(excelFile is null || excelFile.Length == 0)
            {
                ViewBag.ErrorMessage = "Please Select an xlsx file";
                return View();
            }
            if(!Path.GetExtension(excelFile.FileName).Equals(".xlsx",StringComparison.OrdinalIgnoreCase))
            {
                ViewBag.ErrorMessage = "Unsupported file. 'xlsx' file is expected";
                return View();
            }
            int countriesInserted =await _countriesService.UploadCountriesFromExcelFile(excelFile);
            return RedirectToAction("Index", "Persons");
        }
    }
}
