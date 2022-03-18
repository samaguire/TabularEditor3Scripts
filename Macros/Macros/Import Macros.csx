#r "C:\Program Files\Tabular Editor 3\TabularEditor3.exe" // *** Needed for C# scripting, remove in TE3 ***
#r "C:\Program Files (x86)\Tabular Editor 3\TabularEditor3.exe" // *** Needed for C# scripting, remove in TE3 ***

using TabularEditor.TOMWrapper; // *** Needed for C# scripting, remove in TE3 ***
using TabularEditor.Scripting; // *** Needed for C# scripting, remove in TE3 ***

Model Model; // *** Needed for C# scripting, remove in TE3 ***
TabularEditor.Shared.Interaction.Selection Selected; // *** Needed for C# scripting, remove in TE3 ***

/*
Notes:
Elements relating to exporting json are disabled as importing requires TE3 to be restarted, which is a hindrance in script development (it is easier to copy and paste).
Future dev to impliment an import script; add all as new, or override existing (by id) and add new where id doesn't exist: with a warning about potentially breaking the layout
*/
