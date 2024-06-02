using Microsoft.AspNetCore.Mvc;
using ServiceContracts;

namespace CRUDExample.Controllers
{
	[Route("[controller]")]
	public class CountriesController : Controller
	{
		private readonly ICountriesService countriesService;

		public CountriesController( ICountriesService countriesService)
        {
			this.countriesService = countriesService;
		}

        [Route("UploadFromExcel")]
		public IActionResult UploadFromExcel()
		{
			return View();



		}
		[Route("UploadFromExcel")]
		[HttpPost]
		public async Task<IActionResult> UploadFromExcel(IFormFile excelFile)
		{
			if(excelFile == null || excelFile.Length == 0)
			{
				ViewBag.ErrorMessage = "Please select an xlsx file";
				return View();
			}
			var x = Path.GetExtension(excelFile.FileName);
			if (!Path.GetExtension(excelFile.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
			{
				ViewBag.ErrorMessage = "Selected File is NOT xlsx file";
				return View();
			}
			
			var insertedCountries = await countriesService.UploadCountriesFromExcelFile(excelFile);

			ViewBag.Message = $"{insertedCountries} Countries Uploaded";

			return View();
		}
	}
}
