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
            var dic = new ParameterDictionary();

            dic.Add(new ParameterModel("charset", "iso"));
            //dic.Add(new MimeTypeParameterData("Charset", "bla"));
        }
    }
}