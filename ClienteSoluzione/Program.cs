using ClientiLibrary;
using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Configuration;
using MySql.Data.MySqlClient;
using System.Reflection;
using AssemblyGestore;
using AssemblyGestoreFile;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Collections;

class Program
{
    static void Main(string[] args)
    {
        // Nuovo oggetto HttpChannel, assegnato alla variabile channel. L'HttpChannel è responsabile della comunicazione tra client e server tramite il protocollo HTTP.
        HttpChannel channel = new HttpChannel();
        // Si registra il canale creato con il metodo ChannelServices.RegisterChannel(channel, false), per la connesione tra client e server
        ChannelServices.RegisterChannel(channel, false);

        IGestoreC gestore = null; // Clero la variabile gestore
        int sceltaArchiviazione;
        do
        {
            Console.WriteLine("Scegli un metodo di archiviazione: \n");
            Console.WriteLine("1. Database");
            Console.WriteLine("2. File di testo");
            int.TryParse(Console.ReadLine(), out sceltaArchiviazione);
        } while (sceltaArchiviazione != 1 && sceltaArchiviazione != 2);

        // Creazione del canale remoto
        if (sceltaArchiviazione == 1)
        {
            // Viene creato un oggetto remoto di tipo IGestoreC che si connette a gestoreclienti
            gestore = (IGestoreC)Activator.GetObject(typeof(IGestoreC), "http://localhost:8080/GestoreClienti");
        }
        else
        {
            gestore = (IGestoreC)Activator.GetObject(typeof(IGestoreC), "http://localhost:8080/GestoreFileClienti");
        }

        while (true)
        {
            Console.WriteLine("Scegli un'opzione:\n");
            Console.WriteLine("1. Cerca cliente");
            Console.WriteLine("2. Aggiungi cliente");
            Console.WriteLine("3. Modifica cliente");
            Console.WriteLine("4. Elimina cliente");
            Console.Write("Inserisci il numero dell'opzione: ");
            //controllo input dell'opzione, legge l'input e lo converte in intero (la conversone in intero mi serve per dei controlli sulla scelta dell'opzione)
            int.TryParse(Console.ReadLine(), out int opzione);

            switch (opzione)
            {
                // CERCA CLIENTE //
                case 1:
                    FindClient(gestore);
                    break;
                // AGGIUNGI CLIENTE //
                case 2:
                    InsertClient(gestore);
                    break;
                // MODIFICA CLIENTE //
                case 3:
                    ModifyClient(gestore);
                    break;
                // ELIMINA CLIENTE //
                case 4:
                    DeleteClient(gestore);
                    break;
                default:
                    Console.WriteLine("Opzione non valida. Riprova.");
                    break;
            }
        }
    }

    //___// (1) CERCA CLIENTE //___//
    private static void FindClient(IGestoreC gestore)
    {
        try
        {
            Console.WriteLine("Scegli l'informazione da cercare:");
            Console.WriteLine("1. ID");
            Console.WriteLine("2. Nome");
            Console.WriteLine("3. Cognome");
            Console.WriteLine("4. Città");
            Console.WriteLine("5. Sesso");
            Console.WriteLine("6. Data di Nascita");

            string scelta;
            int sceltaInt;
            do
            {
                scelta = Console.ReadLine();
                int.TryParse(scelta, out sceltaInt);
                if (sceltaInt < 1 || sceltaInt > 6)
                {
                    Console.WriteLine("Inserisci un numero tra 1 e 6:");
                }
            } while (sceltaInt < 1 || sceltaInt > 6);

            switch (scelta)
            {
                case "1":
                    scelta = "ID";
                    break;
                case "2":
                    scelta = "Nome";
                    break;
                case "3":
                    scelta = "Cognome";
                    break;
                case "4":
                    scelta = "Citta";
                    break;
                case "5":
                    scelta = "Sesso";
                    break;
                case "6":
                    scelta = "DataDiNascita";
                    break;
            }
            Console.WriteLine("Scrivi l'informazione da cercare:");
            string parametroRicerca = Console.ReadLine();

            ArrayList clientiOut = gestore.CercaCliente(parametroRicerca, scelta);

            if (clientiOut.Count > 0)
            {
                Console.WriteLine("Clienti trovati:");
                foreach (Cliente cliente in clientiOut)
                {
                    Console.WriteLine(cliente.ToRead());
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Errore generico: {ex.Message}");
        }
    }

    //___// (2) AGGIUNGI CLIENTE //___//
    private static void InsertClient(IGestoreC gestore)
    {
        ArrayList clientiInseriti = new ArrayList(); // ArrayList in cui memorizza i clienti inseriti
        while (true) // Ciclo per continuare ad aggiungere clienti alla lista finché non viene inserito "N" e si passa al salvataggio
        {
            try
            {
                string id;
                while (true) // Ciclo per controllare che l'id sia valido
                {
                    Console.Write("Inserisci l'ID del cliente: ");
                    id = Console.ReadLine();
                    try
                    {
                        gestore.VerificaIdUnivoco(id);
                        Cliente.ValidaId(id);
                        break;
                    }
                    catch (ArgumentException e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }

                string nome = InputValidationMessage("Inserisci il nome del cliente: ", Cliente.ValidaInput);
                string cognome = InputValidationMessage("Inserisci il cognome del cliente: ", Cliente.ValidaInput);
                string citta = InputValidationMessage("Inserisci la città del cliente: ", Cliente.ValidaInput);
                string sesso = InputValidationMessage("Inserisci il sesso del cliente (M/F): ", Cliente.ValidaSesso);
                DateTime dataDiNascita;
                while (true)
                {
                    Console.Write("Inserisci la data di nascita del cliente (formato: dd/MM/yyyy): ");
                    string dataInput = Console.ReadLine();
                    try
                    {
                        dataDiNascita = Cliente.ValidaData(dataInput);
                        break;
                    }
                    catch (ArgumentException e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }

                Cliente nuovoCliente = new Cliente(id, nome, cognome, citta, sesso, dataDiNascita); // Crea un nuovo oggetto Cliente con i dati inseriti e validati
                                                                                                    //gestore.AggiungiCliente(nuovoCliente);
                clientiInseriti.Add(nuovoCliente); // Aggiunge il nuovo cliente all'ArrayList dei clienti inseriti

                Console.WriteLine("Cliente aggiunto con successo.");
                Console.Write("Premi \"Invio\" per aggiungere un altro cliente o \"N\" per salvare i clienti inseriti ");
                string continua = Console.ReadLine().ToUpper();
                if (continua == "N") // Esce dal ciclo se l'utente inserisce "N", altrimenti continua a chiedere di aggiungere
                {
                    break;
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Errore file: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore: {ex.Message}");
            }
        }
        // Salvataggio di tutti i clienti inseriti
        try
        {
            foreach (var cliente in clientiInseriti)
            {
                gestore.AggiungiCliente((Cliente)cliente);
            }
            Console.WriteLine("Salvataggio completato.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Errore durante il salvataggio dei clienti: " + ex.Message);
        }
    }

    //___// (3) MODIFICA CLIENTE //___//
    private static void ModifyClient(IGestoreC gestore)
    {
        Console.Write("Inserisci l'ID del cliente da modificare: ");
        string idCliente = Console.ReadLine();

        try
        {
            // Trova il cliente da modificare in base a "idCliente" in input
            ArrayList clientiDaModificare = gestore.CercaCliente(idCliente, "ID");

            if (clientiDaModificare.Count == 0)
            {
                throw new Exception($"Il cliente con ID {idCliente} non esiste.");
            }

            Cliente clienteDaModificare = (Cliente)clientiDaModificare[0];

            Console.WriteLine("Inserisci le nuove informazioni del cliente o premi Invio per mantenere le informazioni attuali:"); // Richiesta delle nuove informazioni
            string nuovoNome = UpdateField("nome", clienteDaModificare.Nome, Cliente.ValidaInput);
            string nuovoCognome = UpdateField("cognome", clienteDaModificare.Cognome, Cliente.ValidaInput);
            string nuovaCitta = UpdateField("città", clienteDaModificare.Citta, Cliente.ValidaInput);
            string nuovoSesso = UpdateField("sesso", clienteDaModificare.Sesso, Cliente.ValidaSesso);
            DateTime nuovaDataDiNascita = UpdateDate("nascita", clienteDaModificare.DataDiNascita);

            Cliente clienteModificato = new Cliente(idCliente, nuovoNome, nuovoCognome, nuovaCitta, nuovoSesso, nuovaDataDiNascita); // Crea il nuovo oggetto Cliente

            // Modifica il cliente
            gestore.ModificaCliente(idCliente, clienteModificato);
            Console.WriteLine("Cliente modificato con successo.");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    //___// (4) ELIMINA CLIENTE //___//
    private static void DeleteClient(IGestoreC gestore)
    {
        Console.WriteLine("Inserisci l'ID del cliente da eliminare");
        string outID = Console.ReadLine();
        try
        {
            // Se il Metodo restituisce true, "eliminato" sarà true e quindi non si passerà per il catch
            bool eliminato = gestore.EliminaCliente(outID);
            Console.WriteLine("Cliente eliminato con successo.");
        }
        catch (MySqlException ex)
        {
            Console.WriteLine(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    //___// FUNZIONI //___//
    //___ Funzione per input in ModifyClient ___//
    private static string UpdateField(string fieldName, string currentValue, Func<string, bool> validationFunc)
    {
        Console.WriteLine($"Il {fieldName} attuale è: {currentValue}");
        string prompt = $"Inserisci il nuovo {fieldName} (premi invio per mantenere il valore attuale): ";

        while (true)
        {
            Console.Write(prompt);
            string input = Console.ReadLine().Trim(); // Rimuovo gli spazi iniziali e finali dall'input

            if (string.IsNullOrEmpty(input))
            {
                // L'utente ha premuto invio, mantengo il valore attuale
                return currentValue;
            }
            else
            {
                try
                {
                    if (validationFunc(input))
                    {
                        // L'input è valido, restituisco il nuovo valore
                        return input;
                    }
                    else
                    {
                        // L'input non è valido, stampo il messaggio di errore e continuo il ciclo
                        Console.WriteLine($"Il {fieldName} inserito non è valido.");
                    }
                }
                catch (Exception ex)
                {
                    // Gestisco l'eccezione stampando il messaggio di errore e continuando il ciclo
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
    //___ Funzione per input data in ModifyClient ___//
    private static DateTime UpdateDate(string fieldName, DateTime currentValue)
    {
        Console.WriteLine($"La data di {fieldName} attuale è: {currentValue:dd/MM/yyyy}");
        string prompt = $"Inserisci la nuova data di {fieldName} (premi invio per mantenere il valore attuale): ";

        while (true)
        {
            Console.Write(prompt);
            string input = Console.ReadLine().Trim(); // Rimuovo gli spazi iniziali e finali dall'input

            if (string.IsNullOrEmpty(input))
            {
                // L'utente ha premuto invio, mantengo il valore attuale
                return currentValue;
            }
            else
            {
                try
                {
                    DateTime newData = Cliente.ValidaData(input);
                    return newData;
                }
                catch (Exception ex)
                {
                    // Gestisco l'eccezione stampando il messaggio di errore e continuando il ciclo
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }

    //___ Funzione generale per validazione input e gestione eccezioni ___//
    private static string InputValidationMessage(string prompt, Func<string, bool> validationFunc)
    {
        string input;

        while (true)
        {
            Console.Write(prompt);
            input = Console.ReadLine();

            try
            {
                if (validationFunc(input))
                {
                    break;
                }
                else
                {
                    throw new ArgumentException("Input non valido.");
                }
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        return input;
    }
}