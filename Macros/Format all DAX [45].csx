﻿#r "C:\Program Files\Tabular Editor 3\TabularEditor3.exe" // *** Needed for C# scripting, remove in TE3 ***
#r "C:\Program Files (x86)\Tabular Editor 3\TabularEditor3.exe" // *** Needed for C# scripting, remove in TE3 ***

using TabularEditor.TOMWrapper; // *** Needed for C# scripting, remove in TE3 ***
using TabularEditor.Scripting; // *** Needed for C# scripting, remove in TE3 ***

Model Model; // *** Needed for C# scripting, remove in TE3 ***
TabularEditor.Shared.Interaction.Selection Selected; // *** Needed for C# scripting, remove in TE3 ***

var useShortFormat = true;
var insertSpaceAfterFunctionName = false;
var insertLineBreakOnFirstLine = true;

Func<string, string> GetFormattedDax = (string daxInput) =>
{
    var formattedDax = ScriptHelper.FormatDax(daxInput, shortFormat: useShortFormat, skipSpaceAfterFunctionName: !insertSpaceAfterFunctionName);
    return insertLineBreakOnFirstLine ? "\r\n" + formattedDax : formattedDax;
};

foreach (var m in Model.AllMeasures) { m.Expression = GetFormattedDax(m.Expression); }
foreach (var i in Model.AllCalculationItems) { i.Expression = GetFormattedDax(i.Expression); if (!String.IsNullOrEmpty(i.FormatStringExpression)) { i.FormatStringExpression = GetFormattedDax(i.FormatStringExpression); } }
foreach (var t in Model.Tables.OfType<CalculatedTable>().Where(x => !x.Name.Contains("DateTableTemplate_") && !x.Name.Contains("LocalDateTable_"))) { t.Expression = GetFormattedDax(t.Expression); }
foreach (var c in Model.AllColumns.OfType<CalculatedColumn>().Where(x => !x.DaxTableName.Contains("DateTableTemplate_") && !x.DaxTableName.Contains("LocalDateTable_"))) { c.Expression = GetFormattedDax(c.Expression); }

ScriptHelper.Info("Script finished.");
