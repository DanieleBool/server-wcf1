using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Globalization;
//using System.ComponentModel.DataAnnotations;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;

namespace ClientiLibrary
{
    [Serializable]
    public class Cliente
    {
        public string ID { get; set; }
        public string Nome { get; set; }
        public string Cognome { get; set; }
        public string Citta { get; set; }
        public string Sesso { get; set; }
        public DateTime DataDiNascita { get; set; }

        public Cliente(string id, string nome, string cognome, string citta, string sesso, DateTime dataDiNascita)
        {
            ID = id;
            Nome = nome;
            Cognome = cognome;
            Citta = citta;
            Sesso = sesso;
            DataDiNascita = dataDiNascita;
        }

        //___// FUNZIONI PER LEGGERE E SCRIVERE SU FILE //___//
        public object ToRead()
        {
            return $"ID: {ID}\nNome: {Nome}\nCognome: {Cognome}\nCittà: {Citta}\nSesso: {Sesso}\nData di Nascita: {DataDiNascita:dd/MM/yyyy}";
            // "\n" serve per andare a capo.
        }
        public object ToWrite()
        {
            return $"{ID};{Nome};{Cognome};{Citta};{Sesso.ToUpper()};{DataDiNascita:dd/MM/yyyy}";
        }

        //___// VALIDAZIONI INPUT //___//
        // Sesso
        public static bool ValidaSesso(string sesso)
        {
            if (string.IsNullOrEmpty(sesso) || (sesso.ToUpper() != "M" && sesso.ToUpper() != "F"))
            {
                throw new ArgumentException("Il sesso del cliente deve essere 'M' (maschio) o 'F' (femmina).", nameof(sesso));
            }

            return true;
        }
        // Stringhe
        public static bool ValidaInput(string input)
        {
            if (string.IsNullOrWhiteSpace(input) || input.Length > 50)
            {
                throw new ArgumentException($"L'input non può essere nullo o avere più di 50 caratteri.", nameof(input));
            }
            return true;
        }
        // Data
        public static DateTime ValidaData(string data)
        {
            string[] formatiData = { "dd/MM/yyyy", "dd-MM-yyyy", "ddMMyyyy" };
            if (DateTime.TryParseExact(data, formatiData, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dataValida))
            {
                if (dataValida > DateTime.Now)
                {
                    throw new ArgumentException("La data di nascita non può essere nel futuro.");
                }
                return dataValida;
            }
            else
            {
                throw new ArgumentException("Il formato della data di nascita non è valido. Utilizzare uno dei seguenti formati: dd/MM/yyyy, dd-MM-yyyy, ddMMyyyy");
            }
        }
        // Id
        public static bool ValidaId(string id)
        {
            if (id.Length < 1 || id.Length > 5 || string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("L'ID deve essere composto da 1 a 5 caratteri alfanumerici.");
            }
            return true;
        }
    }
}