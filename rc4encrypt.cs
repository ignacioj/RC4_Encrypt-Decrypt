using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace RC4ED
{
    class Program
    {
        public static Byte[] Content;
        public static Byte[] Key;
        public static string outFileName;

        static void Main(string[] args)
        {
            bool comprueba = false;
            // decrypt / encrypt
            if (args.Length != 0 && File.Exists(args[0]) && File.Exists(args[1]))
            {
                Content = File.ReadAllBytes(args[0]);
                Key = File.ReadAllBytes(args[1]);
                if (args.Length == 3) { outFileName = args[2]; }
                RC4(ref Content, Key);
                comprueba = true;
            }
            if (!comprueba)
            {
                Console.WriteLine(@"
RC4 encryption/decryption

Copyright 2015 Ignacio J. Perez J.

Licensed under the Apache License, Version 2.0 (the ""License"");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

 http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an ""AS IS"" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.

Arguments:
- data_file: binary file to decode
- key_file: binary file with the key
- output_file: if not specified it will display the output on screen

Usage:

>rc4ed.exe data_file key_file output_file

");
            }
        }

        public static void RC4(ref Byte[] bytes, Byte[] key)
        {
            Byte[] result = new byte[256];
            Byte[] s = new Byte[256];
            Byte[] k = new Byte[256];
            Byte temp;

            int i, j;
            int n = key.Length;
            for (i = 0; i < 256; i++)
            {
                s[i] = (Byte)i;
                //k[i] = key[i % key.GetLength(0)];
                k[i] = key[i % n];
            }

            j = 0;
            for (i = 0; i < 256; i++)
            {
                j = (j + s[i] + k[i]) % 256;
                temp = s[i];
                s[i] = s[j];
                s[j] = temp;
            }

            i = 0; j = 0;
            for (int x = 0; x < bytes.GetLength(0); x++)
            {
                i = (i + 1) % 256;
                j = (j + s[i]) % 256;
                temp = s[i];
                s[i] = s[j];
                s[j] = temp;

                int t = (s[i] + s[j]) % 256;
                bytes[x] ^= s[t];
                //Salida en hex
                //Console.Write("{0:X2}", (bytes[x]));
                //Salida caracteres imprimibles
                if (string.IsNullOrEmpty(outFileName))
                {
                    if ((bytes[x] < 32) | (bytes[x] == 129) | (bytes[x] == 141)
                        | (bytes[x] == 143) | (bytes[x] == 144) | (bytes[x] == 157)
                        | (bytes[x] == 159))
                    {
                        Console.Write(".");
                    }
                    else
                    {
                        Console.Write(Encoding.Default.GetString(bytes, x, 1));
                    }
                }
            }
            //Salida a archivo
            if (!string.IsNullOrEmpty(outFileName)) { File.WriteAllBytes(outFileName, bytes); }
        }
    }
}
