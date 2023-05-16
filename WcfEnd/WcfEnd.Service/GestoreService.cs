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

namespace WcfEnd.Service
{

    public class GestoreService : IGestore
    {
        private readonly IGestoreC _gestoreC;

        public GestoreService()
        {
            var assemblyGestore = Assembly.LoadFrom(@"C:\Users\d.dieleuterio\source\repos\DanieleBool\server-wcf1\AssemblyGestore\obj\Debug\AssemblyGestore.dll");
            var gestoreClientiType = assemblyGestore.GetType("AssemblyGestore.GestoreClienti");
            string connString = ConfigurationManager.ConnectionStrings["DatabaseConnection"]?.ConnectionString;

            if (string.IsNullOrEmpty(connString))
            {
                throw new InvalidOperationException("Database connection string not found");
            }
            if (gestoreClientiType == null)
            {
                throw new Exception("Tipo 'AssemblyGestore.GestoreClienti' non trovato nell'assembly.");
            }

            _gestoreC = (IGestoreC)Activator.CreateInstance(gestoreClientiType, connString);
        }

        //public GestoreService()
        //{
        //    var assemblyGestore = Assembly.LoadFrom(@"C:\Users\d.dieleuterio\source\repos\DanieleBool\server-wcf1\AssemlyGestore\obj\Debug\AssemlyGestore.dll");
        //    var gestoreClientiType = assemblyGestore.GetType("AssemlyGestore.GestoreClienti");
        //    //string connString = ConfigurationManager.ConnectionStrings["DatabaseConnection"].ConnectionString;
        //    string connString = ConfigurationManager.ConnectionStrings["DatabaseConnection"].ConnectionString;
        //    _gestoreC = (IGestoreC)Activator.CreateInstance(gestoreClientiType, connString);
        //}

        public void AggiungiCliente(Cliente nuovoCliente)
        {
            _gestoreC.AggiungiCliente(nuovoCliente);
        }

        public ArrayList CercaCliente(string parametroRicerca, string scelta)
        {
            return _gestoreC.CercaCliente(parametroRicerca, scelta);
        }

        public void ModificaCliente(string id, Cliente clienteModificato)
        {
            _gestoreC.ModificaCliente(id, clienteModificato);
        }

        public bool EliminaCliente(string id)
        {
            return _gestoreC.EliminaCliente(id);
        }

        public void VerificaIdUnivoco(string id)
        {
            _gestoreC.VerificaIdUnivoco(id);
        }
    }
}