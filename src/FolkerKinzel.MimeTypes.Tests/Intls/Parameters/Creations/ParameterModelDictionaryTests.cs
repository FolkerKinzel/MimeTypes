namespace FolkerKinzel.MimeTypes.Intls.Parameters.Creations.Tests
{
    [TestClass()]
    public class ParameterModelDictionaryTests
    {
        [TestMethod()]
        public void MimeTypeParameterDataDictionaryTest()
            => _ = new ParameterModelDictionary
            {
                new MimeTypeParameter("charset", "iso", null)
            //dic.Add(new MimeTypeParameterModel("Charset", "bla"));
            };
    }
}