namespace andecr.Controls;

/// <summary>
/// Selected-text markup toolbar for a vocabulary card's "transcription" field. Its "Sup" button wraps the
/// current selection of the bound <see cref="SelectedTextToolsControl.TargetTextBox"/> in "&lt;sup&gt;"/"&lt;/sup&gt;"
/// tags. Shares its visual tree and behavior entirely with <see cref="SelectedTextToolsControl"/>; only the
/// default button label and tag pair differ.
/// </summary>
public class TextBoxTranscriptionSelectedTextTools : SelectedTextToolsControl
{
    // todo: Turn into UserControl property.
    private new const string ButtonLabel = "Sup";

    static TextBoxTranscriptionSelectedTextTools()
    {
        ButtonLabelProperty.OverrideDefaultValue<TextBoxTranscriptionSelectedTextTools>(ButtonLabel);
        OpenTagProperty.OverrideDefaultValue<TextBoxTranscriptionSelectedTextTools>(
            Constants.TranscriptionSuperScriptStart
        );
        CloseTagProperty.OverrideDefaultValue<TextBoxTranscriptionSelectedTextTools>(
            Constants.TranscriptionSuperScriptEnd
        );
    }
}