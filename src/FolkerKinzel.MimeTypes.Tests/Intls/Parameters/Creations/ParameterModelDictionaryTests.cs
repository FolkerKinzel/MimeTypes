﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolkerKinzel.MimeTypes.Intls.Parameters.Creations.Tests
{
    [TestClass()]
    public class ParameterModelDictionaryTests
    {
        [TestMethod()]
        public void MimeTypeParameterDataDictionaryTest()
            => _ = new ParameterModelDictionary
            {
                new MimeTypeParameter("charset", "iso")
            //dic.Add(new MimeTypeParameterModel("Charset", "bla"));
            };
    }
}