using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Build.Evaluation;

var defaultDictionaryMaps = new Dictionary<string, string>
{
    ["en-US"] = @"..\Dictionaries\English (American)",
    ["en-GB"] = @"..\Dictionaries\English (British)",
    ["en-AU"] = @"..\Dictionaries\English (Australian)",
    ["en-CA"] = @"..\Dictionaries\English (Canadian)",
    ["en-ZA"] = @"..\Dictionaries\English (South African)",
    ["hr-HR"] = @"..\Dictionaries\Croatian",
    ["nl-NL"] = @"..\Dictionaries\Dutch",
    ["et-EE"] = @"..\Dictionaries\Estonian",
    ["el-GR"] = @"..\Dictionaries\Greek",
    ["hu-HU"] = @"..\Dictionaries\Hungarian",
    ["lv-LV"] = @"..\Dictionaries\Latvian",
};

Task.WhenAll(defaultDictionaryMaps.Select(set => CopyDictioanryFiles(set.Key, set.Value))).Wait();

async Task CopyDictioanryFiles(string cultureName, string baseSourceFilePath)
{
    var destinationFileName = cultureName;
    var destinationFilePath = destinationFileName;
    var destinationFileDic = Path.ChangeExtension(destinationFilePath, "dic.compressed");
    var destinationFileAff = Path.ChangeExtension(destinationFilePath, "aff.compressed");
    var destinationFileTxt = Path.ChangeExtension(destinationFilePath, "license.txt");

    var sourceFileDic = Path.ChangeExtension(baseSourceFilePath, "dic");
    var sourceFileAff = Path.ChangeExtension(baseSourceFilePath, "aff");
    var sourceFileTxt = Path.ChangeExtension(baseSourceFilePath, "txt");

    await Task.WhenAll(
        CopyFileAsync(sourceFileDic, destinationFileDic, true),
        CopyFileAsync(sourceFileAff, destinationFileAff, true),
        CopyFileAsync(sourceFileTxt, destinationFileTxt, false));
}

async Task CopyFileAsync(string sourcePath, string destinationPath, bool compress)
{
    using (var sourceStream = File.Open(sourcePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
    using (var destinationStream = File.Open(sourcePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read))
    {
        if (compress)
        {
            using (var compressedWriter = new DeflateStream(destinationStream, CompressionMode.Compress))
            {
                await sourceStream.CopyToAsync(compressedWriter);
            }
        }
        else
        {
            await sourceStream.CopyToAsync(destinationStream);
        }
    }
}