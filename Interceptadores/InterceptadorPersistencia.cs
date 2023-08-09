using DominandoEFCore.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;

namespace Curso.Interceptadores
{
    public class InterceptadorPersistencia : SaveChangesInterceptor
    {

        public override int SavedChanges(SaveChangesCompletedEventData eventData, int result)
        {
            return base.SavedChanges(eventData, result);
        }

        public override InterceptionResult<int> SavingChanges(
            DbContextEventData eventData,
            InterceptionResult<int> result)
        {

            System.Console.WriteLine(eventData.Context.ChangeTracker.DebugView.LongView);
            ////TrackGraph rastrea entidades complexas nao conectadas, mas usa o context para isso(validar possibilidades)
            //eventData.Context.ChangeTracker.TrackGraph(func, e =>
            //{
            //    if (e.Entry.IsKeySet)
            //    {
            //        e.Entry.State = EntityState.Unchanged;
            //    }
            //    else
            //    {
            //        e.Entry.State = EntityState.Added;
            //    }
            //});

            //Pega as entidades e as entidades de navegação
            //foreach (var entry in eventData.Context.ChangeTracker.Entries())
            //{
            //    if (entry.State == EntityState.Modified)
            //    {
            //        //Recupera o valor da entidade corrente do banco para validação de alterações, tem que usar esse metodo para entidades desconectadas
            //        //senão seria possivel pegar com o proprio tracker do EF
            //        var valoresBanco = entry.GetDatabaseValues();
            //        //Itera sobre todas as propriedades
            //        foreach (var property in entry.OriginalValues.Properties)
            //        {

            //            //valor da propriedade do objetos modificado 
            //            var valorCorrente = entry.CurrentValues[property.Name];
            //            //valor do obejto vindo do banco
            //            var valorAtual = valoresBanco[property.Name];
            //            //Compara os valores
            //            if (valorAtual != null && !valorAtual.Equals(valorCorrente))
            //            {
            //              var db =   eventData.Context.Set<Historico>();
            //                var hist = new Historico{Descricao = $"Valor antigo : {property.Name} - {valorAtual} , Valor Novo : {property.Name} - { valorCorrente}"};
            //                db.Add(hist);


            //                Console.WriteLine($"Valor antigo : {property.Name} - {valorAtual} , Valor Novo : {property.Name} - { valorCorrente}");
            //            }
            //        }
            //    }

            //    //var entry2 = entry.Collections;
            //    ////var valoresOriginais = entry.GetDatabaseValues();
            //    //foreach (var property in entry2)
            //    //{

            //    //    var objBanco = property.EntityEntry.GetDatabaseValues();
            //    //    foreach (var property2 in entry.OriginalValues.Properties)
            //    //    {
            //    //        var valorCorrente = entry.CurrentValues[property2.Name];
            //    //        var valorAtual = entry.OriginalValues[property2.Name];
            //    //        if (valorAtual != null && !valorAtual.Equals(valorCorrente))
            //    //        {
            //    //            //Salvar em algum lugar
            //    //        }
            //    //    }
            //    //}


            //    Console.WriteLine($"Entity: {entry.Entity.GetType().Name},State: { entry.State.ToString()}");
            //}



            return result;
        }
    }
}