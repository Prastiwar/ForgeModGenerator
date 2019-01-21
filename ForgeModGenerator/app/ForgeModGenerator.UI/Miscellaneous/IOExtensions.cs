using System.IO;

namespace ForgeModGenerator.Miscellaneous
{
    public static class IOExtensions
    {
        private const char CR = '\r';
        private const char LF = '\n';
        private const char NULLCHAR = (char)0;

        public static long GetLineCount(this Stream stream)
        {
            long lineCount = 0L;

            byte[] byteBuffer = new byte[1024 * 1024];
            char detectedEOL = NULLCHAR;
            char currentChar = NULLCHAR;

            int bytesRead;
            while ((bytesRead = stream.Read(byteBuffer, 0, byteBuffer.Length)) > 0)
            {
                for (int i = 0; i < bytesRead; i++)
                {
                    currentChar = (char)byteBuffer[i];

                    if (detectedEOL != NULLCHAR)
                    {
                        if (currentChar == detectedEOL)
                        {
                            lineCount++;
                        }
                    }
                    else if (currentChar == LF || currentChar == CR)
                    {
                        detectedEOL = currentChar;
                        lineCount++;
                    }
                }
            }

            if (currentChar != LF && currentChar != CR && currentChar != NULLCHAR)
            {
                lineCount++;
            }
            stream.Position = 0;
            return lineCount;
        }
    }
}
