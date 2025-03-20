using Microsoft.AspNetCore.Mvc;
using System.IO;
using Newtonsoft.Json;

[Route("api/[controller]")]
[ApiController]
public class ConcoursController : ControllerBase
{
    private readonly string _jsonFilePath = "data.json"; // Assure-toi que le fichier est bien dans le dossier racine de l'exécution

    [HttpGet]
    public IActionResult GetConcours()
    {
        if (!System.IO.File.Exists(_jsonFilePath))
        {
            return NotFound("Le fichier JSON n'existe pas.");
        }

        try
        {
            var jsonData = System.IO.File.ReadAllText(_jsonFilePath);

            // Désérialisation en utilisant la classe ConcoursModel
            var concoursData = JsonConvert.DeserializeObject<Root>(jsonData);

            return Ok(concoursData);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erreur lors de la lecture du fichier JSON : {ex.Message}");
        }
    }
}
