using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq.Expressions;
using Alure.Base.BL.BaseBusinessObject;
using Alure.Base.BL.Interfaces;
using Alure.Base.DA.Helpers;
using AutoMapper;
using AutoMapper.Impl;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestBusinessObject
{
    [TestClass]
    public class AutoMapperBOLite
    {
        [TestMethod]
        public void From_AutoMapper_To()
        {
            Mapper.CreateMap<TestFrom, TestTo>().ForMember(dest => dest.Full, fro => fro.MapFrom(x => x.Thing));
            var testTo = Mapper.Map<TestFrom, TestTo>(new TestFrom { Thing = "Thing" });
            Assert.AreEqual("Thing", testTo.Full);
        }

        [TestMethod]
        public void TestTest()
        {
            AlureDbHelper.AddMapping<TestFrom, TestTo>(fro => fro.MapFrom(x => x.Thing), x => x.Nummer);
            var testFrom = new TestFrom {Thing = "1"};
            var testTo = Mapper.Map<TestFrom, TestTo>(testFrom);
        }
    }

    public class TestTo
    {
        public string Full { get; set; }

        public int Nummer { get; set; }
    }

    public class TestFrom
    {
        public string Thing { get; set; }
    }

    //Database	
    //Tabel		
    //Index		
    //Type		
    //Percentage V
    //Percentage N
    //Aantal V	
    //Aantal N	
    //Pages V		
    //Pages N		
    public class IndexenHerbouwenClass : IViewModelLazyRetriever
    {
        private IViewModelGrid _view;

        public void RegisterView(IViewModelGrid view)
        {
            _view = view;
        }

        public void HaalOp()
        {
            //AlureDbHelper.AddMappingForSqlDataReader<IndexenHerbouwen>("Database", x => x.Database);
            
            
            // haal iets op.
            var executeReaderForBusinessObjectLite = AlureDbHelper.ExecuteReaderForBusinessObjectLite<IndexenHerbouwenClass>("");
            _view.SetDataSource(executeReaderForBusinessObjectLite);
        }
    }


    public static class AlureDbHelper
    {
        public static IEnumerable<TResult> ExecuteReaderForBusinessObjectLite<TResult>(string sql, IDictionary<string, object> parameters = null)
        {
            var result = new Collection<TResult>();

            using (var connection = AlureDatabaseHelper.CreateAndOpenConnection())
            {
                using (var command = AlureDatabaseHelper.CreateCommand(connection, sql, CommandType.StoredProcedure, parameters))
                {
                    using (var reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            result.Add(Mapper.Map<SqlDataReader, TResult>(reader));
                        }
                    }
                }
            }
            return result;
        }

        public static void AddMappingForSqlDataReader<TResult>(string columnName, Expression<Func<TResult, object>> resultExpression) where TResult : BusinessObjectLite
        {
            AddMapping<SqlDataReader, TResult>(sql => sql.MapFrom(x => x[columnName]), resultExpression);
        }

        public static void AddMapping<TFrom, TResult>(Action<IMemberConfigurationExpression<TFrom>> fromAction, Expression<Func<TResult, object>> resultExpression)
        {
            Mapper.CreateMap<TFrom, TResult>().ForMember(resultExpression, fromAction);

            Mapper.CreateMap<string, int>().ConvertUsing<IntTypeConverter>();
            Mapper.CreateMap<string, int?>().ConvertUsing<NullIntTypeConverter>();
            Mapper.CreateMap<string, decimal?>().ConvertUsing<NullDecimalTypeConverter>();
            Mapper.CreateMap<string, decimal>().ConvertUsing<DecimalTypeConverter>();
            Mapper.CreateMap<string, bool?>().ConvertUsing<NullBooleanTypeConverter>();
            Mapper.CreateMap<string, bool>().ConvertUsing<BooleanTypeConverter>();
            Mapper.CreateMap<string, Int64?>().ConvertUsing<NullInt64TypeConverter>();
            Mapper.CreateMap<string, Int64>().ConvertUsing<Int64TypeConverter>();
            Mapper.CreateMap<string, DateTime?>().ConvertUsing<NullDateTimeTypeConverter>();
            Mapper.CreateMap<string, DateTime>().ConvertUsing<DateTimeTypeConverter>();
        }


        private class NullIntTypeConverter : TypeConverter<string, int?>
        {
            protected override int? ConvertCore(string source)
            {
                if (source == null)
                    return null;
                else
                {
                    int result;
                    return Int32.TryParse(source, out result) ? (int?)result : null;
                }
            }
        }
        // Automapper string to int
        private class IntTypeConverter : TypeConverter<string, int>
        {
            protected override int ConvertCore(string source)
            {
                if (source == null)
                    throw new MappingException("null string value cannot convert to non-nullable return type.");
                else
                    return Int32.Parse(source);
            }
        }
        // Automapper string to decimal?
        private class NullDecimalTypeConverter : TypeConverter<string, decimal?>
        {
            protected override decimal? ConvertCore(string source)
            {
                if (source == null)
                    return null;
                else
                {
                    decimal result;
                    return Decimal.TryParse(source, out result) ? (decimal?)result : null;
                }
            }
        }
        // Automapper string to decimal
        private class DecimalTypeConverter : TypeConverter<string, decimal>
        {
            protected override decimal ConvertCore(string source)
            {
                if (source == null)
                    throw new MappingException("null string value cannot convert to non-nullable return type.");
                else
                    return Decimal.Parse(source);
            }
        }
        // Automapper string to bool?
        private class NullBooleanTypeConverter : TypeConverter<string, bool?>
        {
            protected override bool? ConvertCore(string source)
            {
                if (source == null)
                    return null;
                else
                {
                    bool result;
                    return Boolean.TryParse(source, out result) ? (bool?)result : null;
                }
            }
        }
        // Automapper string to bool
        private class BooleanTypeConverter : TypeConverter<string, bool>
        {
            protected override bool ConvertCore(string source)
            {
                if (source == null)
                    throw new MappingException("null string value cannot convert to non-nullable return type.");
                else
                    return Boolean.Parse(source);
            }
        }
        // Automapper string to Int64?
        private class NullInt64TypeConverter : TypeConverter<string, Int64?>
        {
            protected override Int64? ConvertCore(string source)
            {
                if (source == null)
                    return null;
                else
                {
                    Int64 result;
                    return Int64.TryParse(source, out result) ? (Int64?)result : null;
                }
            }
        }
        // Automapper string to Int64
        private class Int64TypeConverter : TypeConverter<string, Int64>
        {
            protected override Int64 ConvertCore(string source)
            {
                if (source == null)
                    throw new MappingException("null string value cannot convert to non-nullable return type.");
                else
                    return Int64.Parse(source);
            }
        }
        // Automapper string to DateTime?
        // In our case, the datetime will be a JSON2.org datetime
        // Example: "/Date(1288296203190)/"
        private class NullDateTimeTypeConverter : TypeConverter<string, DateTime?>
        {
            protected override DateTime? ConvertCore(string source)
            {
                if (source == null)
                    return null;
                else
                {
                    DateTime result;
                    return DateTime.TryParse(source, out result) ? (DateTime?)result : null;
                }
            }
        }
        // Automapper string to DateTime
        private class DateTimeTypeConverter : TypeConverter<string, DateTime>
        {
            protected override DateTime ConvertCore(string source)
            {
                if (source == null)
                    throw new MappingException("null string value cannot convert to non-nullable return type.");
                else
                    return DateTime.Parse(source);
            }
        }
    }
}