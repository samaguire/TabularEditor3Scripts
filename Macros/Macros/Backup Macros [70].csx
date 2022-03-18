#r "C:\Program Files\Tabular Editor 3\TabularEditor3.exe" // *** Needed for C# scripting, remove in TE3 ***
#r "C:\Program Files (x86)\Tabular Editor 3\TabularEditor3.exe" // *** Needed for C# scripting, remove in TE3 ***

using TabularEditor.TOMWrapper; // *** Needed for C# scripting, remove in TE3 ***
using TabularEditor.Scripting; // *** Needed for C# scripting, remove in TE3 ***
using System.IO;
using System.Windows.Forms;

Model Model; // *** Needed for C# scripting, remove in TE3 ***
TabularEditor.Shared.Interaction.Selection Selected; // *** Needed for C# scripting, remove in TE3 ***

var jsonFile = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\TabularEditor3\MacroActions.json";

var fbd = new FolderBrowserDialog();
if (fbd.ShowDialog() == DialogResult.Cancel) { return; }
var outFolder = fbd.SelectedPath;

File.Copy(jsonFile, Path.Combine(outFolder, Path.GetFileName(jsonFile)), true);

ScriptHelper.Info("Script finished.");
