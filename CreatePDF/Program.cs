using pdftron.PDF;
using pdftron;
using pdftron.SDF;

PDFNet.Initialize("demo:1638727123163:7b63556e0300000000e733b5223644478c184cc841cc180fb5c16d6939");

PDFDoc doc = new PDFDoc();
Page page = doc.PageCreate(new Rect(0, 0, 595, 842));

int textOffset = 5;
string data = "Dieses Dokument kann nicht im verschlüsselten Format heruntergeladen werden. Bitte klicken Sie #hier#, um das Dokument online anzuschauen.";
//string data = "This document can't be downloaded in encrypted format. Please click #here# to view document online.";

string[] textParts = data.Split('#');

double previousPartLength = 0;
int positionY = 830;
foreach (var textPart in textParts)
{
    var words = textPart.Split(' ');
    foreach (var word in words)
    {
        using ElementBuilder builder = new ElementBuilder();
        using ElementWriter writer = new ElementWriter();
        writer.Begin(page);

        // Begin writing a block of text
        var element = builder.CreateTextBegin(Font.Create(doc, Font.StandardType1Font.e_times_roman), 12);
        writer.WriteElement(element);

        var textElement = builder.CreateTextRun(word);
        var positionX = previousPartLength + textOffset;
        textElement.SetTextMatrix(1, 0, 0, 1, positionX, positionY);
        textElement.SetPosAdjustment(positionX);

        var gstate = textElement.GetGState();
        gstate.SetWordSpacing(positionX);
        writer.WriteElement(textElement);

        previousPartLength = previousPartLength + textElement.GetTextLength();

        if (595 - previousPartLength <= 5)
        {
            //textElement.GetGState().SetLeading(15);      // Set the spacing between lines
            writer.WriteElement(builder.CreateTextNewLine());  // New line
            positionY -= 50;
        }

        writer.WriteElement(builder.CreateTextEnd());
        writer.End();
    }
}

doc.PagePushBack(page);
doc.Save(Path.Combine(Directory.GetCurrentDirectory(), "output.pdf"), SDFDoc.SaveOptions.e_linearized);
