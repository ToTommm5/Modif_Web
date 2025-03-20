using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json.Linq;

namespace AppModifJSON_Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly JSonManager _jsonManeger;

        public IndexModel(){
            _jsonManeger = new JSonManager();
        }

        [BindProperty]
        public string NomConcours { get; set; }
        [BindProperty]
        public string ImageUrl { get; set; }

        public JArray Concours { get; set; }

        public void OnGet()
        {
            var data = _jsonManeger.ChargerJson();
            Concours = (JArray)data["concours"];
        }

        public IActionResult OnPost()
        {
            if (!string.IsNullOrWhiteSpace(NomConcours) && !string.IsNullOrWhiteSpace(ImageUrl))
            {
                _jsonManeger.AjouterConcours(NomConcours, ImageUrl);
            }

            return RedirectToPage();
        }
    }
}
