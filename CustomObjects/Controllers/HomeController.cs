using CustomObjects.Models;
using CustomObjects.Services;
using Microsoft.AspNetCore.Mvc;
using PartnerAPI;
using System.Diagnostics;

namespace CustomObjects.Controllers
{
    [Route("api")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;


        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)

        {
            _configuration = configuration;
            _logger = logger;
        }

        public IActionResult Index()
        {

            return Ok();
        }

        [Route("createObject")]
        public IActionResult CreateCustomObjectTemplateFromCSV()
        {

            string? password = _configuration.GetValue<string>("LogInInfo:Password");
            string? token = _configuration.GetValue<string>("LogInInfo:SecurityToken");
            string? username = _configuration.GetValue<string>("LogInInfo:Username");

            
            string file = ".\\csv\\office.csv";

            CSVHelper csvHelper = new CSVHelper();
            List<HubSpotModel> data = csvHelper.ParseCsvFile(file);

            string nameOfObject = file.Split('\\').Last().Split('.')[0];

            if (password != null && token != null && username != null)
            {
                LogInInfo info = new LogInInfo(username, password, token);
                PartnerLogin.login(info, data, nameOfObject);
            }


            return Ok();
        }


    }
}