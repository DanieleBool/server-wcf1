using System.ServiceModel;
using System.ServiceModel.Web;
using WcfEnd.Common.DTO;

using ClientiLibrary;
using System;
using System.IO;
//using MySql.Data.MySqlClient;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;
using System.Collections.Generic;
using System.Collections;

namespace WcfEnd.Common
{
    //[ServiceContract]
    //[XmlSerializerFormat]
    //public interface IGestore
    //{
    //    [WebGet(UriTemplate = "Ping?message={message}")]
    //    [OperationContract]
    //    CustomData Ping(string message);

    //    [WebInvoke]
    //    [OperationContract]
    //    CustomData Ping2(string message);
    //}

    using System.ServiceModel;
    using System.ServiceModel.Web;
    using ClientiLibrary;

    [ServiceContract]
    public interface IGestore
    {
        //[WebInvoke(Method = "POST", UriTemplate = "AggiungiCliente")]
        //[OperationContract]
        //void AggiungiCliente(Cliente nuovoCliente);

        //[WebGet(UriTemplate = "CercaCliente?parametroRicerca={parametroRicerca}&scelta={scelta}")]
        //[OperationContract]
        //ArrayList CercaCliente(string parametroRicerca, string scelta);

        //[WebInvoke(Method = "PUT", UriTemplate = "ModificaCliente/{id}")]
        //[OperationContract]
        //void ModificaCliente(string id, Cliente clienteModificato);

        //[WebInvoke(Method = "DELETE", UriTemplate = "EliminaCliente/{id}")]
        //[OperationContract]
        //bool EliminaCliente(string id);

        //[WebGet(UriTemplate = "VerificaIdUnivoco/{id}")]
        //[OperationContract]
        //void VerificaIdUnivoco(string id);

        //[OperationContract]
        //[WebInvoke(UriTemplate = "*", Method = "OPTIONS")]
        //void GetOptions();


        [OperationContract]
        [WebGet(UriTemplate = "clienti?parametroRicerca={parametroRicerca}&scelta={scelta}")]
        Cliente[] CercaCliente(string parametroRicerca, string scelta);

        [OperationContract]
        [WebInvoke(UriTemplate = "clienti", Method = "POST")]
        void AggiungiCliente(Cliente nuovoCliente);

        [OperationContract]
        [WebInvoke(UriTemplate = "clienti/{id}", Method = "PUT")]
        void ModificaCliente(string id, Cliente clienteModificato);

        [OperationContract]
        [WebInvoke(UriTemplate = "clienti/{id}", Method = "DELETE")]
        bool EliminaCliente(string id);
    }

}
