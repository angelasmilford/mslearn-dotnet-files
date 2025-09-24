using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json; 

var currentDirectory = Directory.GetCurrentDirectory();
var storesDirectory = Path.Combine(currentDirectory, "stores");

var salesTotalDir = Path.Combine(currentDirectory, "salesTotalDir");
Directory.CreateDirectory(salesTotalDir);   

var salesFiles = FindFiles(storesDirectory);

var salesTotal = CalculateSalesTotal(salesFiles);

File.AppendAllText(Path.Combine(salesTotalDir, "totals.txt"), $"{salesTotal}{Environment.NewLine}");

GenerateSalesSummary(salesFiles, salesTotal, salesTotalDir);

IEnumerable<string> FindFiles(string folderName)
{
    List<string> salesFiles = new List<string>();

    var foundFiles = Directory.EnumerateFiles(folderName, "*", SearchOption.AllDirectories);

    foreach (var file in foundFiles)
    {
        var extension = Path.GetExtension(file);
        if (extension == ".json")
        {
            salesFiles.Add(file);
        }
    }

    return salesFiles;
}

double CalculateSalesTotal(IEnumerable<string> salesFiles)
{
    double salesTotal = 0;
    
    // Loop over each file path in salesFiles
    foreach (var file in salesFiles)
    {      
        // Read the contents of the file
        string salesJson = File.ReadAllText(file);
    
        // Parse the contents as JSON
        SalesData? data = JsonConvert.DeserializeObject<SalesData?>(salesJson);
    
        // Add the amount found in the Total field to the salesTotal variable
        salesTotal += data?.Total ?? 0;
    }
    
    return salesTotal;
}

// Sales Summary method
void GenerateSalesSummary(IEnumerable<string> salesFiles, double salesTotal, string salesTotalDir)
{
    var summaryFile = Path.Combine(salesTotalDir, "salesSummary.txt");

    // String Builder
    var sb = new System.Text.StringBuilder();
    sb.AppendLine("Sales Summary");
    sb.AppendLine("----------------------------");
    sb.AppendLine($" Total Sales: {salesTotal.ToString("C")}");
    sb.AppendLine();
    sb.AppendLine(" Details:");

    foreach (var file in salesFiles)
    {
        string salesJson = File.ReadAllText(file);
        SalesData? data = JsonConvert.DeserializeObject<SalesData?>(salesJson);
        string fileName = Path.GetFileName(file);
        double fileTotal = data?.Total ?? 0;

        sb.AppendLine($"  {fileName}: {fileTotal.ToString("C")}");
    }

    File.WriteAllText(summaryFile, sb.ToString());
}


record SalesData(double Total);