using System.Collections;
namespace ClientiLibrary
{
    public interface IGestoreC
    {
        void AggiungiCliente(Cliente nuovoCliente);
        /*List<Cliente> CercaCliente(string parametroRicerca, string scelta);*/
        ArrayList CercaCliente(string parametroRicerca, string scelta);
        void ModificaCliente(string id, Cliente clienteModificato);
        bool EliminaCliente(string id);
        void VerificaIdUnivoco(string id);
    }
}