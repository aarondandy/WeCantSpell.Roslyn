using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

var defaultDictionaryMaps = new Dictionary<string, string>
{
    ["en-US"] = "English (American)"
    ,["en-AU"] = "English (Australian)"
    ,["en-GB"] = "English (British)"
    /*,["en-CA"] = "English (Canadian)"
    ,["en-ZA"] = "English (South African)"*/
};

foreach(var set in defaultDictionaryMaps)
{
    var cultureName = set.Key;

    var destinationFileName = cultureName;
    var destinationFilePath = destinationFileName;
    var destinationFileDic = Path.ChangeExtension(destinationFilePath, "dic.compressed");
    var destinationFileAff = Path.ChangeExtension(destinationFilePath, "aff.compressed");
    var destinationFileTxt = Path.ChangeExtension(destinationFilePath, "txt");

    var sourceFileName = set.Value;
    var sourceFilePath = Path.Combine("../Dictionaries/", sourceFileName);
    var sourceFileDic = Path.ChangeExtension(sourceFilePath, "dic");
    var sourceFileAff = Path.ChangeExtension(sourceFilePath, "aff");
    var sourceFileTxt = Path.ChangeExtension(sourceFilePath, "txt");

    var bytesDic = GetCompressed(File.ReadAllBytes(sourceFileDic));
    var bytesAff = GetCompressed(File.ReadAllBytes(sourceFileAff));

    File.WriteAllBytes(Path.Combine(Path.GetDirectoryName(Output.FilePath), destinationFileDic), bytesDic);
    File.WriteAllBytes(Path.Combine(Path.GetDirectoryName(Output.FilePath), destinationFileAff), bytesAff);
}

byte[] GetCompressed(byte[] raw)
{
    using (var compressedStream = new MemoryStream())
    {
        using (var compressedWriter = new DeflateStream(compressedStream, CompressionMode.Compress))
        {
            compressedWriter.Write(raw, 0, raw.Length);
            compressedWriter.Flush();
        }

        return compressedStream.ToArray();
    }
}