using Curso.Domain;
using DominandoEFCore.Extensão;

namespace DominandoEFCore
{
    public class Teste
    {
        public Teste()
        {

        }
        [Log("Funcoes", "Nome")]
        public int FuncaoId { get; set; }
        public Funcao Funcao { get; set; }
        public int Id { get; set; }
        public string CampoTeste { get; set; }
    }
}
