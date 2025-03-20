using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json.Linq;
using System.IO;

public class PhotosModel : PageModel
{
    private readonly string jsonFilePath = "data.json";
    private readonly string imageFolder = "wwwroot/Img";

    public JArray Concours { get; private set; } = new();

    [BindProperty]
    public string SelectedConcoursId { get; set; } = "";

    [BindProperty]
    public string SelectedEpreuveId { get; set; } = "";

    [BindProperty]
    public List<IFormFile>? Photos { get; set; }

    public void OnGet()
    {
        // Charger les concours et épreuves existants
        if (System.IO.File.Exists(jsonFilePath))
        {
            var jsonData = JObject.Parse(System.IO.File.ReadAllText(jsonFilePath));
            Concours = (JArray)jsonData["concours"]!;
        }
    }

    public IActionResult OnPost()
    {
        if (string.IsNullOrEmpty(SelectedConcoursId) || string.IsNullOrEmpty(SelectedEpreuveId) || Photos == null || Photos.Count == 0)
        {
            return Page();
        }

        var jsonData = System.IO.File.Exists(jsonFilePath)
            ? JObject.Parse(System.IO.File.ReadAllText(jsonFilePath))
            : new JObject { { "concours", new JArray() } };

        var concoursArray = (JArray)jsonData["concours"]!;
        var concours = concoursArray.FirstOrDefault(c => c["id"]?.ToString() == SelectedConcoursId);

        if (concours != null)
        {
            var epreuvesArray = (JArray)concours["epreuves"]!;
            var epreuve = epreuvesArray.FirstOrDefault(e => e["id"]?.ToString() == SelectedEpreuveId);

            if (epreuve != null)
            {
                var photosArray = (JArray)epreuve["photos"]!;
                foreach (var photo in Photos)
                {
                    // Générer un nom de fichier unique
                    var fileName = Path.GetFileName(photo.FileName);
                    var filePath = Path.Combine("wwwroot/Img", fileName); // Chemin complet vers le dossier 'Img'

                    // Sauvegarder le fichier dans le dossier 'wwwroot/Img'
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        photo.CopyTo(stream);
                    }

                    // Ajouter la photo au tableau d'épreuves
                    var newPhoto = new JObject
    {
        { "id", (photosArray.Count + 1).ToString() },
        { "img_url", $"/Img/{fileName}" }  // L'URL de l'image accessible via HTTP
    };

                    photosArray.Add(newPhoto);
                }

                System.IO.File.WriteAllText(jsonFilePath, jsonData.ToString());
            }
        }

        return RedirectToPage();
    }


    public IActionResult OnGetEpreuves(string concoursId)
    {
        if (string.IsNullOrEmpty(concoursId) || !System.IO.File.Exists(jsonFilePath))
        {
            return NotFound();
        }

        var jsonData = JObject.Parse(System.IO.File.ReadAllText(jsonFilePath));
        var concoursArray = (JArray)jsonData["concours"]!;
        var concours = concoursArray.FirstOrDefault(c => c["id"]?.ToString() == concoursId);

        if (concours == null)
        {
            return NotFound();
        }

        var epreuves = (JArray)concours["epreuves"]!;
        var epreuvesList = epreuves.Select(e => new
        {
            id = e["id"]?.ToString(),
            name = e["name"]?.ToString()  // Assurez-vous que le nom est correctement extrait
        }).ToList();

        return new JsonResult(epreuvesList);
    }

}
