using System;
using System.Linq;
using System.Linq.Expressions;

namespace TestBusinessObject
{
    public class DynamicLinqQueriesWithExpression
    {
        public IQueryable<TestClass2> DynamicChainedSyntaxSorted(IQueryable<TestClass2> files, bool pastOnly, string orderBy)
        {
            var query = files.Where(file => file.ImportDate > DateTime.Now.AddDays(-7));
            if (pastOnly)
                query = query.Where(file => file.ImportDate < DateTime.Today);
            //if (!string.IsNullOrEmpty(orderBy))
                //query = query.OrderBy(GenericEvaluateOrderBy<TestClass2>(orderBy));
            return query;
        }

        private Func<TestClass2, object> EvaluateOrderBy(string propertyName)
        {
            if (propertyName == "FileName")
                return f => f.FileName;
            if (propertyName == "ImportDate")
                return f => f.ImportDate;
            return f => f.Id;
        }

        public Func<TSource, object> GenericEvaluateOrderBy<TSource>
            (string propertyName)
        {
            var type = typeof(TSource);
            var parameter = Expression.Parameter(type, "p");
            var propertyReference = Expression.Property(parameter,
                    propertyName);
            return Expression.Lambda<Func<TSource, object>>
                    (propertyReference, new[] { parameter }).Compile();
        }
    }

    public class TestClass2
    {
        public string FileName { get; set; }

        public DateTime ImportDate { get; set; }

        public long Id { get; set; }
    }
}
