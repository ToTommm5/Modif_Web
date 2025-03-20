using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json.Linq;
using System.IO;

public class AjouterEpreuveModel : PageModel
{
    private readonly string jsonFilePath = "data.json";
    private readonly string imageFolder = "wwwroot/Img";

    public JArray Concours { get; private set; } = new();

    [BindProperty]
    public string SelectedConcoursId { get; set; } = "";

    [BindProperty]
    public string NomEpreuve { get; set; } = "";

    [BindProperty]
    public IFormFile? Photo { get; set; }

    public void OnGet()
    {
        // Charger les concours existants
        if (System.IO.File.Exists(jsonFilePath))
        {
            var jsonData = JObject.Parse(System.IO.File.ReadAllText(jsonFilePath));
            Concours = (JArray)jsonData["concours"]!;
        }
    }

    public IActionResult OnPost()
    {
        if (string.IsNullOrEmpty(SelectedConcoursId) || string.IsNullOrEmpty(NomEpreuve))
        {
            return Page();
        }

        string imageUrl = "";

        if (Photo != null && Photo.Length > 0)
        {
            // Enregistrer l'image
            var fileName = Path.GetFileName(Photo.FileName);
            var filePath = Path.Combine(imageFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                Photo.CopyTo(stream);
            }

            imageUrl = $"/Img/{fileName}";
        }

        // Charger les données JSON
        var jsonData = System.IO.File.Exists(jsonFilePath)
            ? JObject.Parse(System.IO.File.ReadAllText(jsonFilePath))
            : new JObject { { "concours", new JArray() } };

        var concoursArray = (JArray)jsonData["concours"]!;
        var concours = concoursArray.FirstOrDefault(c => c["id"]?.ToString() == SelectedConcoursId);

        if (concours != null)
        {
            var epreuvesArray = (JArray)concours["epreuves"]!;
            var newEpreuve = new JObject
            {
                { "id", (epreuvesArray.Count + 1).ToString() },
                { "name", NomEpreuve },
                { "photos", new JArray() } // 🔹 Ajout de la liste "photos" vide dès la création
            };

            epreuvesArray.Add(newEpreuve);
            System.IO.File.WriteAllText(jsonFilePath, jsonData.ToString());
        }

        return RedirectToPage();
    }
}
