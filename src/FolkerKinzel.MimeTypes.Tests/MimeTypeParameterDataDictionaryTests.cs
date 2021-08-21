using Microsoft.VisualStudio.TestTools.UnitTesting;
using FolkerKinzel.MimeTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolkerKinzel.MimeTypes.Tests
{
    [TestClass()]
    public class MimeTypeParameterDataDictionaryTests
    {
        [TestMethod()]
        public void MimeTypeParameterDataDictionaryTest()
        {
            var dic = new MimeTypeParameterDataDictionary();

            dic.Add(new MimeTypeParameterData("charset", "iso"));
            //dic.Add(new MimeTypeParameterData("Charset", "bla"));
        }
    }
}