using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Alure;
using Alure.Base.BL;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestBusinessObject
{
    [TestClass]
    public static class ExtensionsTest
    {
        public static T Dupliceer<T>(this T businessObject, ICollection<string> ignoredProperties) where T : BusinessObject, new()
        {
            var kopie = new T();
            ZetDataOver(businessObject, kopie, ignoredProperties);
            kopie.Status = BOStatus.Nieuw;
            return kopie;
        }

        public static void ZetDataOver<T>(T businessObject, T returnedBusinessObject, ICollection<string> ignoredProperties)
            where T : BusinessObject
        {
            using (new ResetScope(returnedBusinessObject, "Loading"))
            {
                returnedBusinessObject.Loading = true;

                foreach (var propertyInfo in from propertyInfo in typeof(T).GetProperties()
                                             where propertyInfo.CanRead && propertyInfo.CanWrite
                                             where !ignoredProperties.Contains(propertyInfo.Name)
                                             let businessPropertyAttribute =
                                                 propertyInfo.GetCustomAttribute<BusinessPropertyAttribute>()
                                             where businessPropertyAttribute != null
                                             select propertyInfo)
                {
                    propertyInfo.SetValue(returnedBusinessObject, businessObject.GetValue(propertyInfo.Name), null);
                }
            }
        }
    }
}
