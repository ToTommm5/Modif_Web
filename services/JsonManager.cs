using Newtonsoft.Json.Linq;

public class JSonManager{
    private string filePath = "data.json";
    public JObject ChargerJson(){
        if(File.Exists(filePath)){
            string json = File.ReadAllText(filePath);
            return JObject.Parse(json);
        }
        return new JObject{
            {
                "concours", new JArray()},
            {
                "photos", new JArray()
            }
        };
    }

    public void AjouterConcours(string nom, string imageUrl){
        var data = ChargerJson();
        var concoursArray = data["concours"] as JArray ?? new JArray();
        var newConcours = new JObject{
            ["id"] = (concoursArray.Count +1 ).ToString(),
            ["name"] = nom,
            ["img_url"] = imageUrl
        };
        concoursArray.Add(newConcours);
        File.WriteAllText(filePath, data.ToString());
    }

}