using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using NHibernate;

using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Automapping;
using FluentNHibernate.Conventions.Helpers;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;

using NhConcurrencyOnAspNetMvc.Models;

namespace NhConcurrencyOnAspNetMvc
{
    public static class Mapper
    {
        static ISessionFactory _sf = null;
        public static ISessionFactory GetSessionFactory()
        {
            if (_sf != null) return _sf;

            var fc = Fluently.Configure()
                    .Database(MsSqlConfiguration.MsSql2008.ConnectionString(@"Data Source=localhost;Initial Catalog=TestConcurrency;User id=sa;Password=P@$$w0rd"))
                    .Mappings
                    (m =>
                            m.AutoMappings.Add
                            (
                                AutoMap.AssemblyOf<MvcApplication>(new CustomConfiguration())
                                   .Conventions.Add(ForeignKey.EndsWith("Id"))
                                   .Conventions.Add<RowversionConvention>()

                                   // Convention-over-configuration, the rowversion field is named Version. But if you want to deviate from that name, use the following
                                   // .Override<Song>(x => x.Version(y => y.TheRowVersion))
                            )
                // .ExportTo(@"C:\_Misc\NH")                
                    );


            // Console.WriteLine( "{0}", string.Join( ";\n", fc.BuildConfiguration().GenerateSchemaCreationScript(new MsSql2008Dialect() ) ) );
            // Console.ReadLine();

            _sf = fc.BuildSessionFactory();
            return _sf;
        }


        class CustomConfiguration : DefaultAutomappingConfiguration
        {
            IList<Type> _objectsToMap = new List<Type>()
            {
                // whitelisted objects to map
                typeof(Song)
            };
            public override bool ShouldMap(Type type) { return _objectsToMap.Any(x => x == type); }
            public override bool IsId(FluentNHibernate.Member member) { return member.Name == member.DeclaringType.Name + "Id"; }


            // Convention-over-configuration, the rowversion field is named Version. 
            // But if you want to deviate from that name and use it on all your class, use the following
            // public override bool IsVersion(FluentNHibernate.Member member) { return member.Name == "TheRowversion"; }
        }


        class RowversionConvention : IVersionConvention
        {
            public void Apply(IVersionInstance instance) { instance.Generated.Always(); }
        }


    }
}