﻿#r "C:\Program Files\Tabular Editor 3\TabularEditor3.exe" // *** Needed for C# scripting, remove in TE3 ***
#r "C:\Program Files (x86)\Tabular Editor 3\TabularEditor3.exe" // *** Needed for C# scripting, remove in TE3 ***

using TabularEditor.TOMWrapper; // *** Needed for C# scripting, remove in TE3 ***
using TabularEditor.Scripting; // *** Needed for C# scripting, remove in TE3 ***

Model Model; // *** Needed for C# scripting, remove in TE3 ***
TabularEditor.Shared.Interaction.Selection Selected; // *** Needed for C# scripting, remove in TE3 ***

string defaultQuery =
    "TOPN( 500, VALUES( <column> ) )\r\n" +
    "ORDER BY <column>";

foreach (var c in Selected.Columns)
{
    string daxQuery = defaultQuery.Replace("<column>", c.DaxObjectFullName);
    ScriptHelper.EvaluateDax(daxQuery).Output();
}
