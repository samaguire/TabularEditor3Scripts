#r "C:\Program Files\Tabular Editor 3\TabularEditor3.exe" // *** Needed for C# scripting, remove in TE3 ***
#r "C:\Program Files (x86)\Tabular Editor 3\TabularEditor3.exe" // *** Needed for C# scripting, remove in TE3 ***

using TabularEditor.TOMWrapper; // *** Needed for C# scripting, remove in TE3 ***
using TabularEditor.Scripting; // *** Needed for C# scripting, remove in TE3 ***
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Windows.Forms;

Model Model; // *** Needed for C# scripting, remove in TE3 ***
TabularEditor.Shared.Interaction.Selection Selected; // *** Needed for C# scripting, remove in TE3 ***

// Get output folder
var fbd = new FolderBrowserDialog();
if (fbd.ShowDialog() == DialogResult.Cancel) { return; }
var outFolder = fbd.SelectedPath;

// Delete csx and json files from folder
var mbResult = MessageBox.Show(
    "Do you want to delete all existing csx and json files in the folder first?",
    "Question",
    MessageBoxButtons.YesNoCancel,
    MessageBoxIcon.Question
    );
switch (mbResult)
{
    case DialogResult.Yes:
        foreach (var f in Directory.EnumerateFiles(outFolder, "*.csx", SearchOption.AllDirectories)) { File.Delete(f); }
        foreach (var f in Directory.EnumerateFiles(outFolder, "*.json", SearchOption.AllDirectories)) { File.Delete(f); }
        foreach (var d in Directory.EnumerateDirectories(outFolder, "*", SearchOption.AllDirectories))
        {
            if (!Directory.EnumerateFiles(d).Any() && !Directory.EnumerateDirectories(d).Any()) { Directory.Delete(d); }
        }
        break;
    case DialogResult.No:
        break;
    case DialogResult.Cancel:
        return;
    default:
        return;
}

// Load MacroActions
var jsonFile = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\TabularEditor3\MacroActions.json";
var json = JObject.Parse(File.ReadAllText(jsonFile));

// Export each MacroAction
foreach (var jtokenItem in json["Actions"])
{

    // Generate filename without extension and relataive path
    var fileName = string.Join("_", jtokenItem["Name"].Value<string>().Replace('\\', '~').Split(Path.GetInvalidFileNameChars())).Replace('~', '\\') +
        " [" +
        jtokenItem["Id"].Value<string>() +
        "]";

    // Define default parts
    List<string> assemblyList = new List<string>()
    {
        "#r \"C:\\Program Files\\Tabular Editor 3\\TabularEditor3.exe\" // *** Needed for C# scripting, remove in TE3 ***",
        "#r \"C:\\Program Files (x86)\\Tabular Editor 3\\TabularEditor3.exe\" // *** Needed for C# scripting, remove in TE3 ***"
    };
    List<string> namespaceList = new List<string>()
    {
        "using TabularEditor.TOMWrapper; // *** Needed for C# scripting, remove in TE3 ***",
        "using TabularEditor.Scripting; // *** Needed for C# scripting, remove in TE3 ***"
    };
    List<string> classList = new List<string>()
    {
        "Model Model; // *** Needed for C# scripting, remove in TE3 ***",
        "TabularEditor.Shared.Interaction.Selection Selected; // *** Needed for C# scripting, remove in TE3 ***"
    };
    List<string> bodyList = new List<string>();

    // Get csxContent
    var csxContent = jtokenItem["Execute"].Value<string>();

    // Deconstruct csxContent
    var reader = new StringReader(csxContent);
    var line = "";
    while ((line = reader.ReadLine()) != null)
    {
        if (line.StartsWith("#r ")) { assemblyList.Add(line); }
        else if (line.StartsWith("using ")) { namespaceList.Add(line); }
        else { bodyList.Add(line); }
    };
    reader.Dispose();

    // Reconstruct csxContent with default elements
    var writer = new StringWriter();
    foreach (var listItem in assemblyList) { writer.WriteLine(listItem); }
    writer.WriteLine("");
    foreach (var listItem in namespaceList) { writer.WriteLine(listItem); }
    writer.WriteLine("");
    foreach (var listItem in classList) { writer.WriteLine(listItem); }
    writer.WriteLine("");
    foreach (var listItem in bodyList) { writer.WriteLine(listItem); }
    csxContent = writer.ToString();
    writer.Dispose();

    // Save csxContent
    var csxFilePath = outFolder + "\\" + fileName + ".csx";
    FileInfo fileInfo = new FileInfo(csxFilePath);
    if (!fileInfo.Directory.Exists) { fileInfo.Directory.Create(); }
    File.WriteAllText(csxFilePath, csxContent, System.Text.Encoding.UTF8);

    // Get jsonContent
    jtokenItem["Execute"].Parent.Remove();
    var jsonContent = JsonConvert.SerializeObject(jtokenItem, Newtonsoft.Json.Formatting.Indented);

    // Save jsonContent
    var jsonFilePath = outFolder+"\\"+fileName+".json";
    File.WriteAllText(jsonFilePath, jsonContent, System.Text.Encoding.UTF8);

}

ScriptHelper.Info("Script finished.");
