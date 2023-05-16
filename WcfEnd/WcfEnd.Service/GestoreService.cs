using System;
using System.Collections.Generic;
using WcfEnd.Common;
using WcfEnd.Common.DTO;

using ClientiLibrary;
using System.IO;
using MySql.Data.MySqlClient;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;
using System.Collections;
using System.Reflection;
using System.Configuration;
using System.ServiceModel.Web;
using System.ServiceModel.Activation;
using AssemblyGestore;
using System.Linq;

namespace WcfEnd.Service
{
    //public class GestoreService : IGestore
    //{
    //    private readonly IGestoreC _gestoreC;

    //    public GestoreService()
    //    {
    //        var assemblyGestore = Assembly.LoadFrom(@"C:\Users\d.dieleuterio\source\repos\DanieleBool\server-wcf1\AssemblyGestore\obj\Debug\AssemblyGestore.dll");
    //        var gestoreClientiType = assemblyGestore.GetType("AssemblyGestore.GestoreClienti");
    //        string connString = ConfigurationManager.ConnectionStrings["DatabaseConnection"]?.ConnectionString;

    //        if (string.IsNullOrEmpty(connString))
    //        {
    //            throw new InvalidOperationException("Database connection string not found");
    //        }
    //        if (gestoreClientiType == null)
    //        {
    //            throw new Exception("Tipo 'AssemblyGestore.GestoreClienti' non trovato nell'assembly.");
    //        }

    //        _gestoreC = (IGestoreC)Activator.CreateInstance(gestoreClientiType, connString);
    //    }

    //    //public GestoreService()
    //    //{
    //    //    var assemblyGestore = Assembly.LoadFrom(@"C:\Users\d.dieleuterio\source\repos\DanieleBool\server-wcf1\AssemlyGestore\obj\Debug\AssemlyGestore.dll");
    //    //    var gestoreClientiType = assemblyGestore.GetType("AssemlyGestore.GestoreClienti");
    //    //    //string connString = ConfigurationManager.ConnectionStrings["DatabaseConnection"].ConnectionString;
    //    //    string connString = ConfigurationManager.ConnectionStrings["DatabaseConnection"].ConnectionString;
    //    //    _gestoreC = (IGestoreC)Activator.CreateInstance(gestoreClientiType, connString);
    //    //}

    //    public void AggiungiCliente(Cliente nuovoCliente)
    //    {
    //        _gestoreC.AggiungiCliente(nuovoCliente);
    //    }

    //    public ArrayList CercaCliente(string parametroRicerca, string scelta)
    //    {
    //        return _gestoreC.CercaCliente(parametroRicerca, scelta);
    //    }

    //    public void ModificaCliente(string id, Cliente clienteModificato)
    //    {
    //        _gestoreC.ModificaCliente(id, clienteModificato);
    //    }

    //    public bool EliminaCliente(string id)
    //    {
    //        return _gestoreC.EliminaCliente(id);
    //    }

    //    public void VerificaIdUnivoco(string id)
    //    {
    //        _gestoreC.VerificaIdUnivoco(id);
    //    }

    //    public void GetOptions()
    //    {
    //        // This method is used to handle CORS preflight requests.
    //        // It will add the necessary CORS headers to the response.

    //        var currentContext = WebOperationContext.Current;
    //        currentContext.OutgoingResponse.Headers.Add("Access-Control-Allow-Origin", "*");
    //        currentContext.OutgoingResponse.Headers.Add("Access-Control-Allow-Methods", "POST, GET, OPTIONS");
    //        currentContext.OutgoingResponse.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Accept, Authorization");
    //    }
    //}


    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class GestoreService : IGestore
    {
        private readonly IGestoreC _gestoreClienti;

        public GestoreService()
        {
            string connectionDB = "your_connection_string_here";
            _gestoreClienti = new GestoreClienti(connectionDB);
        }

        public Cliente[] CercaCliente(string parametroRicerca, string scelta)
        {
            var clientiTrovati = _gestoreClienti.CercaCliente(parametroRicerca, scelta);
            return clientiTrovati.Cast<Cliente>().ToArray();
        }

        public void AggiungiCliente(Cliente nuovoCliente)
        {
            _gestoreClienti.AggiungiCliente(nuovoCliente);
        }

        public void ModificaCliente(string id, Cliente clienteModificato)
        {
            _gestoreClienti.ModificaCliente(id, clienteModificato);
        }

        public bool EliminaCliente(string id)
        {
            return _gestoreClienti.EliminaCliente(id);
        }

    }
}