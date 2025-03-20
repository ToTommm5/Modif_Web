using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json.Linq;
using System.IO;

public class ConcoursModel : PageModel
{
    private readonly string jsonFilePath = "data.json";
    private readonly string imageFolder = "wwwroot/Img";

    public JArray Concours { get; private set; } = new();

    [BindProperty]
    public string NomConcours { get; set; } = "";
    [BindProperty]
    public string ImageUrl { get; set; } = "";

    [BindProperty]
    public IFormFile? Photo { get; set; }

    public void OnGet()
    {
        // Charger les données JSON depuis le fichier
        if (System.IO.File.Exists(jsonFilePath))
        {
            var jsonData = JObject.Parse(System.IO.File.ReadAllText(jsonFilePath));
            Concours = (JArray)jsonData["concours"]!;
        }
    }

    public IActionResult OnPost()
    {
        if (Photo != null && Photo.Length > 0)
        {
            // Générer un nom de fichier unique pour éviter les conflits
            var fileName = Path.GetFileName(Photo.FileName);
            var filePath = Path.Combine(imageFolder, fileName);

            // Sauvegarder le fichier dans le dossier Img
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                Photo.CopyTo(stream);
            }

            ImageUrl = fileName;
        }

        if (!string.IsNullOrEmpty(NomConcours) && !string.IsNullOrEmpty(ImageUrl))
        {
            var jsonData = System.IO.File.Exists(jsonFilePath)
                ? JObject.Parse(System.IO.File.ReadAllText(jsonFilePath))
                : new JObject { { "concours", new JArray() }, { "photos", new JArray() } };

            var concoursArray = (JArray)jsonData["concours"]!;

            var newConcours = new JObject
            {
                { "id", (concoursArray.Count + 1).ToString() },
                { "name", NomConcours },
                { "img_url", $"/Img/{ImageUrl}" },
                { "epreuves", new JArray() } // Initialisation d'un tableau d'épreuves vide pour ce concours
            };

            concoursArray.Add(newConcours);
            System.IO.File.WriteAllText(jsonFilePath, jsonData.ToString());
        }

        return RedirectToPage();
    }

    // Handler pour supprimer un concours
    public IActionResult OnPostDelete(string id)
    {
        var jsonData = System.IO.File.Exists(jsonFilePath)
            ? JObject.Parse(System.IO.File.ReadAllText(jsonFilePath))
            : new JObject { { "concours", new JArray() }, { "photos", new JArray() } };

        var concoursArray = (JArray)jsonData["concours"]!;
        var concoursToDelete = concoursArray.FirstOrDefault(c => c["id"]?.ToString() == id);

        if (concoursToDelete != null)
        {
            concoursArray.Remove(concoursToDelete);
            System.IO.File.WriteAllText(jsonFilePath, jsonData.ToString());
        }

        return RedirectToPage();
    }
}
