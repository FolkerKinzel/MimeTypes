using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolkerKinzel.MimeTypes.Intls
{
    public static class UnitTestGenerator
    {
        public static void GenerateTests()
        {
            GenerateExtensionTest();
            GenerateMimeTest();
        }

        private static void GenerateExtensionTest() => GenerateTest("ExtensionTest.txt", ReaderFactory.InitExtensionFileReader());

        private static void GenerateMimeTest() => GenerateTest("MimeTest.txt", ReaderFactory.InitMimeFileReader());


        private static void GenerateTest(string fileName, StreamReader reader)
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), fileName);

            using var writeStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
            using var writer = new StreamWriter(writeStream);
            writer.WriteLine("[DataTestMethod]");

            string? line;
            while((line = reader.ReadLine()) != null)
            {
                string[] arr = line.Split(' ');
                writer.Write("[DataRow(\"");
                writer.Write(arr[0]);
                writer.Write("\", \".");
                writer.Write(arr[1]);
                writer.WriteLine("\")]");
            }

            reader.Close();
        }
    }
}
