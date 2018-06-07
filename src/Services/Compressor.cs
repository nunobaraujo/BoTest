using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Services
{
    public static class Compressor
    {
        public static FileInfo CompressFileLZ4(FileInfo OriginFile)
        {
            string destiFullName = string.Format("{0}{1}.l4z", Path.GetTempPath(), OriginFile.Name);
            using (var istream = OriginFile.OpenRead())
            using (var ostream = new FileStream(destiFullName, FileMode.Create))
            using (var lzStream = new LZ4.LZ4Stream(ostream, LZ4.LZ4StreamMode.Compress))
            {
                istream.CopyTo(lzStream);
            }
            return new FileInfo(destiFullName);
        }
        public static byte[] CompressBytesLZ4(byte[] input, int inputOffset, int inputLength)
        {
            return LZ4.LZ4Codec.Encode(input, inputOffset, inputLength);
        }

        public static FileInfo DecompressFileLZ4(FileInfo CompressedFile)
        {
            string destinFile = CompressedFile.FullName.Replace(CompressedFile.Extension, "");
            using (var istream = new FileStream(CompressedFile.FullName, FileMode.Open, FileAccess.Read))
            using (var ostream = new FileStream(destinFile, FileMode.Create))
            using (var lzStream = new LZ4.LZ4Stream(istream, LZ4.LZ4StreamMode.Decompress))
            {
                lzStream.CopyTo(ostream);
            }
            return new FileInfo(destinFile);
        }
        public static byte[] DecompressBytesLZ4(byte[] input, int inputOffset, int inputLength, int outputLength)
        {
            return LZ4.LZ4Codec.Decode(input, inputOffset, inputLength, outputLength);
        }


    }
}
