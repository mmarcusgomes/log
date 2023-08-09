using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Curso.Configurations;
using Curso.Domain;
using DominandoEFCore.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Logging;

namespace Curso.Data
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Funcao>  Funcoes { get; set; }
        public DbSet<Historico>  Historicos { get; set; }
        //public DbSet<QualquerModelo> QualquerModelos { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            const string strConnection = "Data source=(localdb)\\mssqllocaldb; Initial Catalog=DevIO-02;Integrated Security=true;pooling=true;";

            optionsBuilder
                .UseSqlServer(strConnection)
                .LogTo(Console.WriteLine, LogLevel.Information)
                .EnableSensitiveDataLogging()
                //.AddInterceptors(new Interceptadores.InterceptadorDeComandos())
                //.AddInterceptors(new Interceptadores.InterceptadorDeConexao())
                .AddInterceptors(new Interceptadores.InterceptadorPersistencia());

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<QualquerModelo>().HasNoKey();
            //modelBuilder.Ignore<QualquerModelo>();
     
            modelBuilder
                .Entity<Funcao>(conf =>
                {
                    conf.Property<string>("PropriedadeSombra")
                        .HasColumnType("VARCHAR(100)")
                        .HasDefaultValueSql("'Teste'");
                });
        }
        //public override int SaveChanges()
        //{
        //    foreach (var entry in ChangeTracker.Entries())
        //    {
               



        //        if (entry.State == EntityState.Modified)
        //        {

        //            var teste = ChangeTracker.HasChanges();

        //            ChangeTracker.TrackGraph(entry, e =>
        //            {
        //                if (e.Entry.IsKeySet)
        //                {
        //                    e.Entry.State = EntityState.Unchanged;
        //                }
        //                else
        //                {
        //                    e.Entry.State = EntityState.Added;
        //                }
        //            });

        //            foreach (var entry2 in ChangeTracker.Entries())
        //            {
        //                Console.WriteLine($"Entity: {entry2.Entity.GetType().Name},State: { entry2.State.ToString()}");
        //            }



        //            //Retorna os valores originais do banco para a entidade (testar com entidade mais complexas(com relacionamentos))
        //            var valoresOriginais = entry.GetDatabaseValues();
        //            foreach (var property in entry.OriginalValues.Properties)
        //            {
        //                var valorCorrente =entry.CurrentValues[property.Name];
        //                var valorAtual = valoresOriginais[property.Name];
        //                if(valorAtual != null && !valorAtual.Equals(valorCorrente))
        //                {
        //                    //Salvar em algum lugar
        //                }                     
        //            }
        //        }
        //    }
        //    return base.SaveChanges();
        //}



        //public override EntityEntry<TEntity> Update<TEntity>(TEntity entity)
        //{
        //    //dbEntityEntry.State = EntityState.Modified; --- I cannot do this.


        //    foreach (var entry in ChangeTracker.Entries())
        //    {
        //        if (entry.State == EntityState.Modified)
        //        {

        //            foreach (var property in entry.OriginalValues.Properties)
        //            {
        //                var original = entry.OriginalValues.GetValue<object>(property);
        //                var current = entry.CurrentValues.GetValue<object>(property);
        //                //if (original != null && !original.Equals(current))
        //                //    entry.Property(property).IsModified = true;
        //            }
        //        }

        //        //if (entry.State == EntityState.Added)
        //        //{
        //        //    entry.Property("DataCadastro").CurrentValue = Brasilia.DataAtual;
        //        //    entry.Property("DataAtualizacao").CurrentValue = Brasilia.DataAtual;
        //        //}
        //        //if (entry.State == EntityState.Modified)
        //        //{
        //        //    entry.Property("DataCadastro").IsModified = false;
        //        //    entry.Property("DataAtualizacao").CurrentValue = Brasilia.DataAtual;
        //        //}
        //    }
        //    //dbEntityEntry.State = EntityState.Modified; --- I cannot do this.

        //    ////Ensure only modified fields are updated.
        //    //var dbEntityEntry = DbContext.Entry(entity);
        //    ////if (updatedProperties.Any())
        //    ////{
        //    ////    //update explicitly mentioned properties
        //    ////    foreach (var property in updatedProperties)
        //    ////    {
        //    ////        dbEntityEntry.Property(property).IsModified = true;
        //    ////    }
        //    ////}
        //    ////else
        //    ////{
        //    ////no items mentioned, so find out the updated entries
        //    //foreach (var property in dbEntityEntry.OriginalValues.PropertyNames)
        //    //{
        //    //    var original = dbEntityEntry.OriginalValues.GetValue<object>(property);
        //    //    var current = dbEntityEntry.CurrentValues.GetValue<object>(property);
        //    //    if (original != null && !original.Equals(current))
        //    //        dbEntityEntry.Property(property).IsModified = true;
        //    //}
        //    //}
        //    return base.Update(entity);
        //}

        //public virtual void Update<T>(T entity, params Expression<Func<T, object>>[] updatedProperties)
        //{
        //    //dbEntityEntry.State = EntityState.Modified; --- I cannot do this.

        //    //Ensure only modified fields are updated.
        //    var dbEntityEntry = DbContext.Entry(entity);
        //    if (updatedProperties.Any())
        //    {
        //        //update explicitly mentioned properties
        //        foreach (var property in updatedProperties)
        //        {
        //            dbEntityEntry.Property(property).IsModified = true;
        //        }
        //    }
        //    else
        //    {
        //        //no items mentioned, so find out the updated entries
        //        foreach (var property in dbEntityEntry.OriginalValues.PropertyNames)
        //        {
        //            var original = dbEntityEntry.OriginalValues.GetValue<object>(property);
        //            var current = dbEntityEntry.CurrentValues.GetValue<object>(property);
        //            if (original != null && !original.Equals(current))
        //                dbEntityEntry.Property(property).IsModified = true;
        //        }
        //    }
        //}
    }
}