using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Curso.Domain;
using DominandoEFCore.Domain;
using DominandoEFCore.Extensão;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;

namespace DominandoEFCore
{
    class Program
    {
        public static string MensagemLog { get; set; }
        static void Main(string[] args)
        {
            //TesteInterceptacao();
            TesteInterceptacaoSaveChanges();

        }

        static void TesteInterceptacaoSaveChanges()
        {
            var entidadeBanco = new Funcao();
            using (var db = new Curso.Data.ApplicationContext())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                var funcao = new Funcao
                {
                    Nome = "Função teste ",

                };
                for (int i = 0; i < 10; i++)
                {
                    funcao.Teste.Add(new Teste { CampoTeste = "Campos teste " + i });

                }
                db.Funcoes.Add(funcao);

                db.SaveChanges();

                //Desconectado
                entidadeBanco = db.Funcoes.Include(x => x.Teste).AsNoTracking().FirstOrDefault();

                //Conectado
                //entidadeEnditada =  db.Funcoes.Include(x => x.Teste).FirstOrDefault();

                entidadeBanco.Nome = "NOME DA FUNÇÂO EDITADO";
                int contador = 10;
                foreach (var item in entidadeBanco.Teste)
                {
                    item.CampoTeste = "CAMPO EDITADO " + contador;
                    contador += 10;
                }

                //Desconectado
                LogUpdateAoContext<Funcao>("Nome Teste", entidadeBanco.Id, entidadeBanco);

                //Conectado             
                //LogUpdateAoContext<Funcao>("Nome Teste", entidadeEditada.Id);


                db.Update(entidadeBanco);//Talvez tenha q comentar isso no cenraio conectado
                db.SaveChanges();
            }
        }

        /// <summary>
        /// Envia o objeto de log para ser trackeado(Apenas com o uso de AsNoTracking),nome do usuário para o log
        /// Id de referencia para o log, e se é uma atualização ou nao
        /// </summary>
        /// <param name="nomeUsuario"></param>
        /// <param name="idReferencia"></param>
        /// <param name="obj"></param>
        /// <param name="atualizar"></param>

        static void LogUpdateAoContext<T>(string nomeUsuario, Guid idReferencia, object obj = null, bool atualizar = true)
        {
            using (var db = new Curso.Data.ApplicationContext())
            {


                //Validar uso e viabilidade
                if (obj != null)
                {
                    if (atualizar)
                    {
                        //Mapea entidades desconectadas para o contexto 
                        db.ChangeTracker.TrackGraph(obj, e =>
                        {
                            if (e.Entry.IsKeySet)
                            {
                                e.Entry.State = EntityState.Modified;
                            }
                        });
                    }
                    else
                    {
                        //Mapea entidades desconectadas para o contexto 
                        db.ChangeTracker.TrackGraph(obj, e =>
                        {
                            e.Entry.State = EntityState.Added;
                        });
                    }
                }

                var entidades = db.ChangeTracker.Entries();
                //Itera sobre todas as entidades trackeadas
                foreach (var entidade in db.ChangeTracker.Entries())
                {
                    //Recupera o tipo de entidade que esta sendo iterada no momento
                    var tipo = entidade.Entity.GetType();

                    //Recupera o valor da entidade corrente do banco para validação de alterações,
                    // tem que usar esse metodo para entidades desconectadas pois não seria possivel pegar do proprio tracher do EF
                    var valoresBanco = entidade.GetDatabaseValues();
                    //Itera sobre todas as propriedades
                    foreach (var property in entidade.OriginalValues.Properties)
                    {
                        var nomePropriedade = "";
                        //valor da propriedade do objeto modificado 
                        var valorCorrente = entidade.CurrentValues[property.Name]?.ToString();
                        //valor do objeto vindo do banco
                        var valorAtual = valoresBanco[property.Name]?.ToString();
                        //Compara os valores
                        if (valorAtual != null && !valorAtual.Equals(valorCorrente) && property.Name != "DataCadastro" && property.Name != "DataAtualizacao")
                        {
                            //Verifica se a propriedade é um tipo de enum para formatação especifica
                            if (property.ClrType.IsEnum)
                            {
                                //Recupera o valor da propriedade q estava salva no banco
                                var atributoEnumOld = valoresBanco[property.Name]?.ToString();
                                //Recupera o valor da propriedade ja modificada
                                var atributoEnumNovo = entidade.CurrentValues[property.Name]?.ToString();

                                //Recupera a descrição dos enums para formatar melhor o log
                                valorAtual = EnumExtension.GetDescriptionType(property.ClrType, atributoEnumOld);
                                valorCorrente = EnumExtension.GetDescriptionType(property.ClrType, atributoEnumNovo);
                            }
                            //Para mudança de chaves estrangeiras
                            else if (property.Name.ToLower().Contains("id"))
                            {
                                var atributos = GetAttributes(tipo, property.Name);
                                if (!string.IsNullOrEmpty(valorAtual) && Guid.Parse(valorAtual) != Guid.Empty)
                                {
                                    //Recupera o valor da tabela
                                    valorAtual = RecuperaValores(atributos, valorAtual);
                                }
                                else
                                {
                                    valorAtual = "";
                                }
                                if (!string.IsNullOrEmpty(valorCorrente) && Guid.Parse(valorCorrente) != Guid.Empty)
                                {
                                    //Recupera o calor da tabela e da coluna principal determinada no LogAttribute
                                    valorCorrente = RecuperaValores(atributos, valorCorrente);
                                }
                                else
                                {
                                    valorCorrente = "";
                                }
                            }
                            //Retorna o nome da propriedade da class que é igual a propriedade do filtro
                            var description = Util.GetDescriptionPropertyClass<T>(property.Name);
                            //Caso nao existe um parametro com o mesmo nome da propriedade ele mantem o nome da propriedade
                            if (string.IsNullOrEmpty(description))
                            {
                                nomePropriedade = property.Name;
                            }
                            //Caso contrario ele vai escrever a description pois é mais amigavel 
                            else
                            {
                                nomePropriedade = description;
                            }
                            if (!string.IsNullOrWhiteSpace(MensagemLog))
                            {
                                MensagemLog += ",";
                            }
                            MensagemLog += $"{DateTime.Now:dd/MM/yyyy HH:mm:ss} - Campo {nomePropriedade} com valor {valorAtual} atualizado para {valorCorrente} pelo usuário {nomeUsuario}.";

                        }
                    }

                }

                db.Historicos.Add(new Historico()
                {
                    Descricao = MensagemLog

                });
                db.SaveChanges(); //Talvez tenha q comentar isso no cenraio conectado
            }





        }
        public class AuxiliarLog
        {
            public string Tabela { get; set; }
            public string Coluna { get; set; }
            public string ColunaBackup { get; set; }
            public bool IsEnum { get; set; }
        }
        /// <summary>
        /// Recupera os valores inseridos no LogAttribute (Atributo personalizado)
        /// </summary>
        /// <param name="tipoObjeto"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        static AuxiliarLog GetAttributes(Type tipoObjeto, string propertyName)
        {
            //Recupera o nome da propriedade caso exista no filtro 
            var props1 = tipoObjeto.GetProperties().Where(prop => prop.Name.ToLower() == propertyName.Trim().ToLower()).FirstOrDefault();
            object[] attrs = props1.GetCustomAttributes(true);
            var resp = new AuxiliarLog();
            foreach (System.Attribute attr in attrs)
            {
                if (attr is LogAttribute)
                {

                    LogAttribute a = (LogAttribute)attr;
                    resp.Tabela = a.tabela;
                    resp.Coluna = a.coluna;
                    resp.ColunaBackup = a.colunaBackup;
                }
            }
            return resp;
        }

        /// <summary>
        /// Recupera os valores marcado no LogAttribute , caso a coluna principal retorne vazio outra consulta sera executada com a coluna de backup
        /// </summary>
        /// <param name="nomeTabela"></param>
        /// <param name="coluna"></param>
        /// <param name="id"></param>
        /// <param name="colunaBackup"></param>
        /// <returns></returns>
        static string RecuperaValores(AuxiliarLog atributos, string id)
        {
            var colunas = atributos.Coluna;
            if (!string.IsNullOrWhiteSpace(atributos.ColunaBackup))
            {
                colunas += "," + atributos.ColunaBackup;
            }

            //Buscando dados das colunas passadas
            return ExecutaConsulta(atributos.Tabela, colunas, id);


        }

        /// <summary>
        /// Executa a consulta em uma determinada tabela e retorna uma determinada coluna buscando pelo Id
        /// </summary>
        /// <param name="nomeTabela"></param>
        /// <param name="coluna"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        static string ExecutaConsulta(string nomeTabela, string coluna, string id)
        {
            var valorFinal = "";
            if (string.IsNullOrWhiteSpace(nomeTabela) || string.IsNullOrWhiteSpace(coluna))
            {
                return valorFinal;
            }
            var query = "SELECT " + coluna + " FROM " + nomeTabela + " WHERE Id='" + id + "'";



            SqlConnection con = new SqlConnection("Data source=(localdb)\\mssqllocaldb; Initial Catalog=DevIO-02;Integrated Security=true;pooling=true;");
            SqlCommand cmd = new SqlCommand(query, con);
            try
            {
                using (var command = cmd)
                {
                    cmd.Connection.Open();
                    command.CommandText = query;

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (!string.IsNullOrWhiteSpace(reader[0].ToString()))
                            {
                                valorFinal = reader[0]?.ToString();
                            }
                            else
                            {
                                valorFinal = reader[1]?.ToString();
                            }
                        }
                    }
                    cmd.Connection.Close();
                }
                return valorFinal;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                con.Close();
            }

        }



        static class Util
        {
            public static string GetDescriptionPropertyClass<T>(string propriedade)
            {

                //Recupera o nome da propriedade caso exista no filtro 
                var props = typeof(T).GetProperties().Where(prop => prop.Name.ToLower() == propriedade.Trim().ToLower()).FirstOrDefault()?.Name;
                if (!string.IsNullOrEmpty(props))
                {
                    //Recupera o description da classe
                    propriedade = TypeDescriptor.GetProperties(typeof(T))[props].Description;
                }
                return propriedade;
            }
        }



       
    }
}
