using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rotativa.AspNetCore;
using ServiceContract;
using ServiceContract.DTO;
using ServiceContract.Enums;
namespace CRUDEx.Controllers
{
    [Route("[controller]")]
    public class PersonsController : Controller
    {
        private readonly ICountriesService _countriesService;
        private readonly IPersonsService _personsService;

        public PersonsController(ICountriesService countriesService, IPersonsService personsService)
        {
            _countriesService = countriesService;
            _personsService = personsService;
        }

        [Route("[action]")]
        [Route("/")]
        public async Task<IActionResult> Index(string searchBy, string? searchString, string sortBy = nameof(PersonResponse.PersonName), SortOrderEnum sortOrder = SortOrderEnum.ASC)
        {
            List<PersonResponse> persons;
            #region Search
            ViewBag.SearchFields = new Dictionary<string, string>()
            {
                { nameof(PersonResponse.PersonName),"Person Name" },
                { nameof(PersonResponse.Address),"Address" },
                { nameof(PersonResponse.DateOfBirth),"Date of birth" },
                { nameof(PersonResponse.Email),"Email" },
                { nameof(PersonResponse.Gender),"Gender" },
                { nameof(PersonResponse.ReceiveNewsletters),"Receive News letters" },
            };
            ViewBag.searchBy = searchBy;
            ViewBag.searchString = searchString;

            persons = await _personsService.GetFilteredPersons(searchBy, searchString);
            #endregion

            #region Sort

            persons =await  _personsService.GetSortedPersons(persons, sortBy, sortOrder);
            ViewBag.SortBy = sortBy;
            if (sortOrder == SortOrderEnum.ASC) ViewBag.SortOrder = SortOrderEnum.DESC;
            else ViewBag.SortOrder = SortOrderEnum.ASC;
            #endregion
            return View(persons);
        }


        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Countries = (await _countriesService.GetAllCountries()).Select(temp =>
            new SelectListItem()
            {
                Text = temp.CountryName,
                Value = temp.CountryID.ToString()
            });

            return View();
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<IActionResult> Create(PersonAddRequest personAddRequest)
        {
            if (!ModelState.IsValid)
            {
                List<CountryResponse> countries = await  _countriesService.GetAllCountries();
                ViewBag.Countries = countries.Select(temp =>
                new SelectListItem() { Text = temp.CountryName, Value = temp.CountryID.ToString() });

                ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return View(personAddRequest);
            }

            PersonResponse personResponse = await  _personsService.AddPerson(personAddRequest);

            return RedirectToAction("Index", "Persons");
        }
        [Route("[action]/{id}")]
        [HttpGet]
        //http://localhost:5227/persons/edit/069D0150-6612-4622-B344-2468AAF388D8
        public async Task<IActionResult> Edit(Guid id)
        {
            PersonResponse? personResponse =await  _personsService.GetPersonById(id);
            if (personResponse == null)
            {
                return RedirectToAction("Index");
            }

            PersonUpdateRequest personUpdateRequest = personResponse.ToPersonUpdateRequest();

            List<CountryResponse> countries = await  _countriesService.GetAllCountries();
            ViewBag.Countries = countries.Select(temp =>
            new SelectListItem() { Text = temp.CountryName, Value = temp.CountryID.ToString() });

            return View(personUpdateRequest);
        }


        [Route("[action]/{id}")]
        [HttpPost]
        public async Task<IActionResult> Edit(PersonUpdateRequest personUpdateRequest)
        {
            PersonResponse? personResponse =await  _personsService.GetPersonById(personUpdateRequest.PersonID);

            if (personResponse == null)
            {
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                PersonResponse? updatedPerson =await  _personsService.UpdatePerson(personUpdateRequest);
                return RedirectToAction("Index");
            }
            else
            {
                List<CountryResponse> countries =await  _countriesService.GetAllCountries();
                ViewBag.Countries = countries.Select(temp =>
                new SelectListItem() { Text = temp.CountryName, Value = temp.CountryID.ToString() });

                ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return View();
            }

        }

        [HttpGet]
        [Route("[action]/{id}")]
        public async Task<IActionResult> Delete(Guid? id)
        {
            PersonResponse? personResponse =await  _personsService.GetPersonById(id);
            if (personResponse == null)
                return RedirectToAction("Index");

            return View(personResponse);
        }

        [HttpPost]
        [Route("[action]/{id}")]
        public async Task<IActionResult> Delete(PersonUpdateRequest personUpdateResult)
        {
            PersonResponse? personResponse =await  _personsService.GetPersonById(personUpdateResult.PersonID);
            if (personResponse == null)
                return RedirectToAction("Index");

            await _personsService.DeletePerson(personUpdateResult.PersonID);
            return RedirectToAction("Index");
        }

        [Route("[action]")]
        public async Task<IActionResult> PersonsPDF(string? searchBy, string? searchString,string sortBy = nameof(PersonResponse.PersonName), SortOrderEnum sortOrder = SortOrderEnum.ASC)
        {
            List<PersonResponse> persons =
                await _personsService.GetFilteredPersons(searchBy, searchString);

            persons = await _personsService.GetSortedPersons(
                persons, sortBy, sortOrder);

            return new ViewAsPdf("PersonsPDF", persons, ViewData)
            {
                PageMargins = new Rotativa.AspNetCore.Options.Margins()
                {
                    Top = 20,
                    Bottom = 20,
                    Left = 20,
                    Right = 20
                },
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Landscape
            };
        }

        [Route("[action]")]
        public async Task<IActionResult> PersonsCSV(string? searchBy,string? searchString, string sortBy = nameof(PersonResponse.PersonName),SortOrderEnum sortOrder = SortOrderEnum.ASC)
        {
            List<PersonResponse> persons =
               await _personsService.GetFilteredPersons(searchBy, searchString);

            persons = await _personsService.GetSortedPersons(
                persons, sortBy, sortOrder);

            MemoryStream memoryStream =await _personsService.GetPersonsCSV(persons);
            return File(memoryStream, "application/octet-stream", "persons.csv");
        }


        [Route("[action]")]
        public async Task<IActionResult> PersonsExcel(string? searchBy,string? searchString,string sortBy = nameof(PersonResponse.PersonName),SortOrderEnum sortOrder = SortOrderEnum.ASC)
        {
            List<PersonResponse> persons =
               await _personsService.GetFilteredPersons(searchBy, searchString);

            persons = await _personsService.GetSortedPersons(
                persons, sortBy, sortOrder);

            MemoryStream memoryStream =await _personsService.GetPersonsExcel(persons);
            return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "persons.xlsx");
        }
    }
}