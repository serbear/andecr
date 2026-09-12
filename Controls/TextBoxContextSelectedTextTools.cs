namespace andecr.Controls;

/// <summary>
/// Selected-text markup toolbar for a vocabulary card's "context" field. Its "Mark" button wraps the current
/// selection of the bound <see cref="SelectedTextToolsControl.TargetTextBox"/> in "&lt;MARK&gt;"/"&lt;/MARK&gt;" tags.
/// Shares its visual tree and behavior entirely with <see cref="SelectedTextToolsControl"/>; only the default
/// button label and tag pair differ.
/// </summary>
public class TextBoxContextSelectedTextTools : SelectedTextToolsControl
{
    static TextBoxContextSelectedTextTools()
    {
        ButtonLabelProperty.OverrideDefaultValue<TextBoxContextSelectedTextTools>("Mark");
        OpenTagProperty.OverrideDefaultValue<TextBoxContextSelectedTextTools>("<MARK>");
        CloseTagProperty.OverrideDefaultValue<TextBoxContextSelectedTextTools>("</MARK>");
    }
}
