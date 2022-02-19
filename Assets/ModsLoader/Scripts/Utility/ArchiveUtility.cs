using System;
using System.IO;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using UnityEngine;
// https://ourcodeworld.com/articles/read/629/how-to-create-and-extract-zip-files-compress-and-decompress-zip-with-sharpziplib-with-csharp-in-winforms
public class ArchiveUtility
{
    public static void CompressDirectory(string DirectoryPath, string OutputFilePath, int CompressionLevel = 9)
    {
        try
        {
            // Depending on the directory this could be very large and would require more attention
            // in a commercial package.
            string[] filenames = Directory.GetFiles(DirectoryPath);

            // 'using' statements guarantee the stream is closed properly which is a big source
            // of problems otherwise.  Its exception safe as well which is great.
            using (ZipOutputStream OutputStream = new ZipOutputStream(File.Create(OutputFilePath)))
            {

                // Define the compression level
                // 0 - store only to 9 - means best compression
                OutputStream.SetLevel(CompressionLevel);

                byte[] buffer = new byte[4096];

                foreach (string file in filenames)
                {

                    // Using GetFileName makes the result compatible with XP
                    // as the resulting path is not absolute.
                    ZipEntry entry = new ZipEntry(Path.GetFileName(file));

                    // Setup the entry data as required.

                    // Crc and size are handled by the library for seakable streams
                    // so no need to do them here.

                    // Could also use the last write time or similar for the file.
                    entry.DateTime = DateTime.Now;
                    entry.Flags |= (int)GeneralBitFlags.UnicodeText;
                    OutputStream.PutNextEntry(entry);

                    using (FileStream fs = File.OpenRead(file))
                    {

                        // Using a fixed size buffer here makes no noticeable difference for output
                        // but keeps a lid on memory usage.
                        int sourceBytes;

                        do
                        {
                            sourceBytes = fs.Read(buffer, 0, buffer.Length);
                            OutputStream.Write(buffer, 0, sourceBytes);
                        } while (sourceBytes > 0);
                    }
                }

                // Finish/Close arent needed strictly as the using statement does this automatically

                // Finish is important to ensure trailing information for a Zip file is appended.  Without this
                // the created file would be invalid.
                OutputStream.Finish();

                // Close is important to wrap things up and unlock the file.
                OutputStream.Close();

                Debug.Log("Files successfully compressed");
            }
        }
        catch (Exception ex)
        {
            // No need to rethrow the exception as for our purposes its handled.
            Debug.Log($"Exception during processing {ex}");
        }
    }



    public static void ExtractZipContent(string FileZipPath, string password, string OutputFolder)
    {
        ZipFile file = null;
        try
        {
            FileStream fs = File.OpenRead(FileZipPath);
            file = new ZipFile(fs);

            if (!String.IsNullOrEmpty(password))
            {
                // AES encrypted entries are handled automatically
                file.Password = password;
            }

            foreach (ZipEntry zipEntry in file)
            {
                if (!zipEntry.IsFile)
                {
                    // Ignore directories
                    continue;
                }

                String entryFileName = zipEntry.Name;
                // to remove the folder from the entry:- entryFileName = Path.GetFileName(entryFileName);
                // Optionally match entrynames against a selection list here to skip as desired.
                // The unpacked length is available in the zipEntry.Size property.
                // 4K is optimum
                byte[] buffer = new byte[4096];
                Stream zipStream = file.GetInputStream(zipEntry);

                // Manipulate the output filename here as desired.
                String fullZipToPath = Path.Combine(OutputFolder, entryFileName);
                string directoryName = Path.GetDirectoryName(fullZipToPath);

                if (directoryName.Length > 0)
                {
                    Directory.CreateDirectory(directoryName);
                }

                // Unzip file in buffered chunks. This is just as fast as unpacking to a buffer the full size
                // of the file, but does not waste memory.
                // The "using" will close the stream even if an exception occurs.
                using (FileStream streamWriter = File.Create(fullZipToPath))
                {
                    StreamUtils.Copy(zipStream, streamWriter, buffer);
                }
            }
        }
        finally
        {
            if (file != null)
            {
                file.IsStreamOwner = true; // Makes close also shut the underlying stream
                file.Close(); // Ensure we release resources
            }
        }
    }
}