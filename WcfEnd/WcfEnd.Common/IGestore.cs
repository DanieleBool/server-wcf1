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
        [WebInvoke(Method = "POST", UriTemplate = "AggiungiCliente")]
        [OperationContract]
        void AggiungiCliente(Cliente nuovoCliente);

        [WebGet(UriTemplate = "CercaCliente?parametroRicerca={parametroRicerca}&scelta={scelta}")]
        [OperationContract]
        ArrayList CercaCliente(string parametroRicerca, string scelta);

        [WebInvoke(Method = "PUT", UriTemplate = "ModificaCliente/{id}")]
        [OperationContract]
        void ModificaCliente(string id, Cliente clienteModificato);

        [WebInvoke(Method = "DELETE", UriTemplate = "EliminaCliente/{id}")]
        [OperationContract]
        bool EliminaCliente(string id);

        [WebGet(UriTemplate = "VerificaIdUnivoco/{id}")]
        [OperationContract]
        void VerificaIdUnivoco(string id);
    }

}
