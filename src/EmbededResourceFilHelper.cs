//Copyright (c) 2007 Maxence Dislaire

//Permission is hereby granted, free of charge, to any person
//obtaining a copy of this software and associated documentation
//files (the "Software"), to deal in the Software without
//restriction, including without limitation the rights to use,
//copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the
//Software is furnished to do so, subject to the following
//conditions:

//The above copyright notice and this permission notice shall be
//included in all copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
//EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
//OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
//NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
//HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
//WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
//FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
//OTHER DEALINGS IN THE SOFTWARE.
using System;
using System.IO;
using System.Reflection;

namespace YouWebIt
{
    public class EmbeddedResourceFileHelper
    {
        public static void ExtractFile(string ressourceFileName, string directory,string extratedFileName)
        {
            string extractedFilePath = Path.Combine(directory, extratedFileName);
            if (!File.Exists(extractedFilePath))
            {
                Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(ressourceFileName);
                byte[] buf = new byte[stream.Length];
                stream.Read(buf, 0, (Int32)stream.Length);
                try
                {
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }
                    File.WriteAllBytes(extractedFilePath, buf);
                }
                catch (Exception e)
                {
                    throw new ApplicationException(ressourceFileName + " not found and cant write it to disk", e);
                }
                stream.Dispose();
            }
        }

    }
}