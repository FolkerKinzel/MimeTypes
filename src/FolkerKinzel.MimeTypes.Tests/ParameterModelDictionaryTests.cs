using Microsoft.VisualStudio.TestTools.UnitTesting;
using FolkerKinzel.MimeTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FolkerKinzel.MimeTypes.Intls;

namespace FolkerKinzel.MimeTypes.Tests
{
    [TestClass()]
    public class ParameterModelDictionaryTests
    {
        [TestMethod()]
        public void MimeTypeParameterDataDictionaryTest() 
            => _ = new ParameterModelDictionary
            {
                new ParameterModel("charset", "iso")
            //dic.Add(new MimeTypeParameterModel("Charset", "bla"));
            };
    }
}