using App.Lib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace UnitTesting
{
    public class MappingTestObj
    {
        public string Name { get; set; }
        public Int16 Age { get; set; }
        public bool HasInsurance { get; set; }
        public DateTime BirthDate { get; set; }
        public DateTime? StartDate { get; set; }
        public IEnumerable<string> FavoriteColors { get; set; }
    }

    [TestClass]
    public class MappingUnitTests
    {
        [TestMethod]
        public void TestSimpleMapping()
        {
            var mapObj = new MappingTestObj();
            var formData = new NameValueCollection
            {
                { "Name", "Joshua" },
                { "Age", "28" },
                { "BirthDate", "01/28/1988"},
                { "StartDate", ""},
                { "FavoriteColors", "Red,Blue,Green" },
            };

            mapObj = FormToObjectMapper.FormDataToModel<MappingTestObj>(mapObj, formData);

            Assert.AreEqual("Joshua", mapObj.Name);
        }
    }
}
