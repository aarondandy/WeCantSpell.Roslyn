using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

var defaultDictionaryMaps = new Dictionary<string, string>
{
    ["en-US"] = "../Dictionaries/English (American)",
    ["en-GB"] = "../Dictionaries/English (British)",
    ["en-AU"] = "../Dictionaries/English (Australian)",
    ["en-CA"] = "../Dictionaries/English (Canadian)",
    ["en-ZA"] = "../Dictionaries/English (South African)",
    ["hr-HR"] = "../Dictionaries/Croatian",
    ["nl-NL"] = "../Dictionaries/Dutch",
    ["et-EE"] = "../Dictionaries/Estonian",
    ["el-GR"] = "../Dictionaries/Greek",
    ["hu-HU"] = "../Dictionaries/Hungarian",
    ["lv-LV"] = "../Dictionaries/Latvian",
};

foreach (var set in defaultDictionaryMaps)
{
    var cultureName = set.Key;
    var sourceFilePath = set.Value;

    var destinationFileName = cultureName;
    var destinationFilePath = destinationFileName;
    var destinationFileDic = Path.ChangeExtension(destinationFilePath, "dic.compressed");
    var destinationFileAff = Path.ChangeExtension(destinationFilePath, "aff.compressed");
    var destinationFileTxt = Path.ChangeExtension(destinationFilePath, "license.txt");

    var sourceFileDic = Path.ChangeExtension(sourceFilePath, "dic");
    var sourceFileAff = Path.ChangeExtension(sourceFilePath, "aff");
    var sourceFileTxt = Path.ChangeExtension(sourceFilePath, "txt");

    var bytesDic = GetCompressed(File.ReadAllBytes(sourceFileDic));
    var bytesAff = GetCompressed(File.ReadAllBytes(sourceFileAff));

    File.WriteAllBytes(Path.Combine(Path.GetDirectoryName(Output.FilePath), destinationFileDic), bytesDic);
    File.WriteAllBytes(Path.Combine(Path.GetDirectoryName(Output.FilePath), destinationFileAff), bytesAff);
    File.Copy(sourceFileTxt, destinationFileTxt);
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