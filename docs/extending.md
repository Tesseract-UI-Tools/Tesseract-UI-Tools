## Extending functionalities

### Image types

To add a new image type to the process create a new class in the `Generetors` folder. This class should extend `ATiffPagesGenerator` and declare the property `public static readonly new string[] FORMATS` with the formats it will handle (case-insensitive).

```
namespace Tesseract_UI_Tools.Generators
{
    public class TypeGenerator : ATiffPagesGenerator
    {
        public static readonly new string[] FORMATS = new string[] { <type>, <type>, ... };
```

In the constructor you can define the property CanRun which defaults to true. To specify if a given file can actually run without needing to throw an error later.

This class should overwrite the `public override string[] GenerateTIFFs(string FolderPath, bool Overwrite = false)` method and should create tiff files in the FolderPath given from the original image format, returning a list of file paths to the temporary files created.

### Strategies

To add a new strategy to the process create a new class in the `OcrStrategy` folder. This class should extend `AOcrStrategy` and declare the property `public static readonly string StrategyName` with the name of the strategy.

```
namespace Tesseract_UI_Tools.OcrStrategy
{
    public class FastAndOtsuOcrStartegy : AOcrStrategy
    {
        public static readonly string StrategyName = <name>;
```

This class should overwrite the `public override void GenerateTsv(string TiffPage, string TsvPage)` method and should create a .tsv file using `OCROutput.Save(TsvPage)`.
