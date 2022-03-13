﻿#r "C:\Program Files\Tabular Editor 3\TabularEditor3.exe" // *** Needed for C# scripting ***
#r "C:\Program Files (x86)\Tabular Editor 3\TabularEditor3.exe" // *** Needed for C# scripting ***

using TabularEditor.TOMWrapper; // *** Needed for C# scripting ***
using TabularEditor.Scripting; // *** Needed for C# scripting ***
using System.Windows.Forms;

Model Model; // *** Needed for C# scripting ***
TabularEditor.Shared.Interaction.Selection Selected; // *** Needed for C# scripting ***


// Variables

var promptForVariables = false;                         // Set to true to prompt the user for the defaultTimeIntelligenceName & defaultCurrentPeriodItemName
var defaultTimeIntelligenceName = "Time Intelligence";  // Used to determine the calculation group's suffix
var defaultCurrentPeriodItemName = "CUR";               // Calculation item to excluded from measure creation
var defaultMeasureExpression =                          // The template DAX query
    "\r\n" +
    "CALCULATE(\r\n" +
    "    <measure>,\r\n" +
    "    <column> = \"<item>\"\r\n" +
    ")";

// Custom InputBox (instead of VB InputBox as this returns null instead of "" on cancel)

Func<string, string, string, string> InputBox = (string promptText, string titleText, string defaultText) =>
{

    var labelText = new Label()
    {
        Text = promptText,
        Dock = DockStyle.Fill,
    };

    var textboxText = new TextBox()
    {
        Text = defaultText,
        Dock = DockStyle.Bottom
    };

    var panelButtons = new Panel()
    {
        Height = 30,
        Dock = DockStyle.Bottom
    };
    
    var buttonOK = new Button()
    {
        Text = "OK",
        DialogResult = DialogResult.OK,
        Top = 8,
        Left = 120
    };

    var buttonCancel = new Button()
    {
        Text = "Cancel",
        DialogResult = DialogResult.Cancel,
        Top = 8,
        Left = 204
    };

    var formInputBox = new Form()
    {
        Text = titleText,
        Height = 143,
        Padding = new System.Windows.Forms.Padding(8),
        FormBorderStyle = FormBorderStyle.FixedDialog,
        MinimizeBox = false,
        MaximizeBox = false,
        StartPosition = FormStartPosition.CenterScreen,
        AcceptButton = buttonOK,
        CancelButton = buttonCancel
    };

    formInputBox.Controls.AddRange(new Control[] { labelText, textboxText, panelButtons });
    panelButtons.Controls.AddRange(new Control[] { buttonOK, buttonCancel });

    return formInputBox.ShowDialog() == DialogResult.OK ? textboxText.Text : null;

};

// Check measure(s) are selected

if (!Selected.Measures.Any())
{
    ScriptHelper.Error("No measure(s) selected.");
    return;
}

// Get variables input

if (promptForVariables)
{

    defaultTimeIntelligenceName = InputBox(
        "Provide the common name for time/period intelligence tables. This is used to determine the calculation group's suffix, e.g. '(ISO)' in 'Time Intelligence (ISO)'.",
        "Default Time Intelligence Name",
        defaultTimeIntelligenceName
        );
    if (defaultTimeIntelligenceName == null) { return; }

    defaultCurrentPeriodItemName = InputBox(
        "Provide the name of the 'current' time/period intelligence calculation item. Any calculation item with this name will be excluded from measure creation.",
        "Default 'Current' Name",
        defaultCurrentPeriodItemName
        );
    if (defaultCurrentPeriodItemName == null) { return; }

}

// Get calculation group table

var ts = Model.Tables.Where(x => x.ObjectType == (ObjectType.CalculationGroupTable));
var t = null as CalculationGroupTable;

if (ts.Any())
{
    t = ScriptHelper.SelectTable(ts, label:"Select calculation group table:") as CalculationGroupTable;
    if (t == null) { return; }
}
else
{
    ScriptHelper.Error("No calculation group tables in the model.");
}

// Get calculation group's calculation items data column

var cs = t.DataColumns.Where(x => x.SourceColumn == "Name");
var c = null as DataColumn;

if (cs.Count() != 1)
{
    ScriptHelper.Warning("Cannot identify calculation items column.");
    c = ScriptHelper.SelectColumn(t, label:"Select calculation items column:") as DataColumn;
    if (c == null) { return; }
}
else
{
    c = cs.First();
}

// If calculation group is a time intelligence calculation group get the suffix (if any)

var tableSuffix = "" as string;
if (defaultTimeIntelligenceName.Length < t.Name.Length &&
    t.Name.ToUpper().Substring(0, defaultTimeIntelligenceName.Length) == defaultTimeIntelligenceName.ToUpper())
{
    tableSuffix = " " + t.Name.Substring(defaultTimeIntelligenceName.Length).Trim();
}

// Create measures

foreach (var m in Selected.Measures)
{

    bool isCalculationGroupMeasure = Convert.ToBoolean(m.GetAnnotation("isCalculationGroupMeasure"));
    if (isCalculationGroupMeasure) { continue; }

    foreach (var i in t.CalculationItems.Where(i => !i.Name.ToUpper().Contains(defaultCurrentPeriodItemName.ToUpper())))
    {

        var measureName = m.Name + " " + i.Name + tableSuffix;
        var measureExpression = defaultMeasureExpression
            .Replace("<measure>", m.DaxObjectName)
            .Replace("<column>", c.DaxObjectFullName)
            .Replace("<item>", i.Name);
        var measureDisplayFolder = m.DisplayFolder + "\\🢒" + t.Name + "\\" + m.Name;

        var mms = Model.AllMeasures.Where(x => x.Name == measureName);
        if (mms.Any()) { foreach (var mm in mms.ToList()) { mm.Delete(); } }

        var nm = m.Table.AddMeasure(measureName, measureExpression, measureDisplayFolder);

        var e = i.FormatStringExpression;
        if (String.IsNullOrEmpty(e))
        {
            // Format string expression is blank
            nm.FormatString = m.FormatString;
        }
        else if (e.Trim('\r', '\n', '"').Length < e.Length)
        {
            // Format string expression is a string
            nm.FormatString = e.Trim('\r', '\n', '"');
        }
        // Todo: Add in process to handle DAX expressions

        nm.SetAnnotation("isCalculationGroupMeasure", "true");

        // Todo: Add in logic to add formatting annotations by calling a function
        // Todo: Add in logic to create descriptions

    }
}

// End

ScriptHelper.Info("Script finished.");