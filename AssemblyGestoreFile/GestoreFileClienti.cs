using ClientiLibrary;
using System.Globalization;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;
using System.Linq;
using System;
using System.Collections;

namespace AssemblyGestoreFile
{
    public class GestoreFileClienti : MarshalByRefObject, IGestoreC
    {
        // Campo privato che memorizza il percorso del file dei clienti
        private string _filePercorso;
        public GestoreFileClienti(string filePercorso) // Costruttore che accetta il percorso dall'esterno come argomento
        {
            _filePercorso = filePercorso;
        }

        //___// (1) CERCA CLIENTE //___//
        public ArrayList CercaCliente(string parametroRicerca, string scelta)
        {
            // Controlla la validità della scelta
            string[] scelteValide = { "ID", "Nome", "Cognome", "Citta", "Sesso", "DataDiNascita" };
            if (!scelteValide.Contains(scelta))
            {
                throw new ArgumentException("La scelta non è valida. Deve essere una delle seguenti opzioni: ID, Nome, Cognome, Citta, Sesso, DataDiNascita.");
            }

            if (string.IsNullOrWhiteSpace(parametroRicerca) || parametroRicerca.Length > 50)
            {
                throw new ArgumentException("Il parametro di ricerca non è valido.");
            }

            if (scelta == "DataDiNascita")
            {
                Cliente.ValidaData(parametroRicerca);
            }

            if (scelta == "ID")
            {
                Cliente.ValidaId(parametroRicerca);
            }
            ArrayList clientiTrovati = new ArrayList();

            try
            {
                using (StreamReader sr = new StreamReader(_filePercorso))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] parti = line.Split(';');
                        Cliente cliente = new Cliente(parti[0], parti[1], parti[2], parti[3], parti[4], DateTime.ParseExact(parti[5], "dd/MM/yyyy", CultureInfo.InvariantCulture));
                        bool isDataDiNascita = DateTime.TryParse(parametroRicerca, out DateTime parametroDataDiNascita);

                        if ((scelta == "ID" && cliente.ID.Equals(parametroRicerca, StringComparison.OrdinalIgnoreCase)) ||
                            (scelta == "Nome" && cliente.Nome.Equals(parametroRicerca, StringComparison.OrdinalIgnoreCase)) ||
                            (scelta == "Cognome" && cliente.Cognome.Equals(parametroRicerca, StringComparison.OrdinalIgnoreCase)) ||
                            (scelta == "Citta" && cliente.Citta.Equals(parametroRicerca, StringComparison.OrdinalIgnoreCase)) ||
                            (scelta == "Sesso" && cliente.Sesso.Equals(parametroRicerca, StringComparison.OrdinalIgnoreCase)) ||
                            (scelta == "DataDiNascita" && isDataDiNascita && DateTime.Compare(cliente.DataDiNascita, parametroDataDiNascita) == 0))
                        {
                            clientiTrovati.Add(cliente);
                        }
                    }
                }
            }
            catch (IOException ex)
            {
                throw new ApplicationException("Si è verificato un errore durante la ricerca del cliente nel file.", ex);
            }
            // Se la lista clientiTrovati è vuota, lancia un'eccezione
            if (clientiTrovati.Count == 0)
            {
                throw new ApplicationException("Nessun cliente trovato con il parametro di ricerca specificato.");
            }
            return clientiTrovati;
        }


        //___// (2) AGGIUNGI CLIENTE //___//
        public void AggiungiCliente(Cliente nuovoCliente)
        {
            Cliente.ValidaId(nuovoCliente.ID);
            VerificaIdUnivocoFile(nuovoCliente.ID);
            Cliente.ValidaInput(nuovoCliente.Nome);
            Cliente.ValidaInput(nuovoCliente.Cognome);
            Cliente.ValidaInput(nuovoCliente.Citta);
            Cliente.ValidaData(nuovoCliente.DataDiNascita.ToString("dd/MM/yyyy"));
            Cliente.ValidaSesso(nuovoCliente.Sesso.ToUpper());

            // Aggiunge il nuovo cliente al file dei clienti
            using (StreamWriter sw = new StreamWriter(_filePercorso, true, Encoding.UTF8))
            {
                sw.WriteLine(nuovoCliente.ToWrite());
            }
        }

        //___// (3) MODIFICA CLIENTE //___//
        public void ModificaCliente(string id, Cliente clienteModificato)
        {
            if (clienteModificato == null)
            {
                throw new ArgumentNullException(nameof(clienteModificato), "Il cliente modificato non può essere nullo.");
            }

            Cliente.ValidaInput(clienteModificato.Nome);
            Cliente.ValidaInput(clienteModificato.Citta);
            Cliente.ValidaInput(clienteModificato.Cognome);
            Cliente.ValidaId(id);
            Cliente.ValidaData(clienteModificato.DataDiNascita.ToString("dd/MM/yyyy"));
            Cliente.ValidaSesso(clienteModificato.Sesso.ToUpper());

            string fileTemporaneo = Path.GetTempFileName();

            try
            {
                // Apri il file dei clienti per la lettura e il file temporaneo per la scrittura
                using (StreamReader sr = new StreamReader(_filePercorso))
                using (StreamWriter sw = new StreamWriter(fileTemporaneo, false, Encoding.UTF8))
                {
                    string line;
                    bool clienteTrovato = false;

                    while ((line = sr.ReadLine()) != null) // Leggi il file una riga alla volta
                    {
                        string[] parti = line.Split(';');
                        string idCorrente = parti[0];
                        // Verifica se l'ID del cliente corrente corrisponde all'ID del cliente da modificare
                        if (idCorrente.Equals(id, StringComparison.OrdinalIgnoreCase))
                        {
                            sw.WriteLine(clienteModificato.ToWrite()); // Scrivi il cliente modificato nel file temporaneo
                            clienteTrovato = true;
                        }
                        else
                        {
                            sw.WriteLine(line); // Scrivi il cliente non modificato nel file temporaneo
                        }
                    }
                    if (!clienteTrovato)
                    {
                        throw new ArgumentException("Nessun cliente trovato con l'ID specificato.");
                    }
                }
                // Sostituisci il file originale con il file temporaneo
                File.Delete(_filePercorso);
                File.Move(fileTemporaneo, _filePercorso);
            }
            catch (IOException ex)
            {
                throw new ApplicationException("Si è verificato un errore durante la modifica del cliente nel file.", ex);
            }
            finally
            {// Elimina il file temporaneo alla fine del processo, PERICOLOSO?
                if (File.Exists(fileTemporaneo))
                {
                    File.Delete(fileTemporaneo);
                }
            }
        }

        //___// (4) ELIMINA CLIENTE //___//
        public bool EliminaCliente(string id)
        {
            Cliente.ValidaId(id);
            bool clienteTrovato = false;
            string tempFile = Path.GetTempFileName();

            try
            {
                using (StreamReader sr = new StreamReader(_filePercorso))
                using (StreamWriter sw = new StreamWriter(tempFile, false, Encoding.UTF8))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] parti = line.Split(';');
                        string idCorrente = parti[0];

                        if (idCorrente.Equals(id, StringComparison.OrdinalIgnoreCase))
                        {
                            clienteTrovato = true;
                        }
                        else
                        {
                            sw.WriteLine(line);
                        }
                    }
                }
                if (!clienteTrovato)
                {
                    throw new ArgumentException("Nessun cliente trovato con l'ID specificato.");
                }

                // Sostituisci il file originale con il file temporaneo
                File.Delete(_filePercorso);
                File.Move(tempFile, _filePercorso);
            }
            catch (IOException ex)
            {
                throw new ApplicationException("Si è verificato un errore durante l'eliminazione del cliente nel file.", ex);
            }
            finally
            {
                // Elimina il file temporaneo se esiste ancora
                if (File.Exists(tempFile))
                {
                    File.Delete(tempFile);
                }
            }
            return clienteTrovato;
        }

        //___// FUNZIONI //___//
        private void VerificaIdUnivocoFile(string id)
        {
            try
            {
                using (StreamReader sr = new StreamReader(_filePercorso))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] parti = line.Split(';');
                        string idCorrente = parti[0];

                        if (idCorrente == id)
                        {
                            throw new InvalidOperationException("L'elemento con l'ID specificato è già presente nel file.");
                        }
                    }
                }
            }
            catch (IOException ex)
            {
                throw new InvalidOperationException($"Errore durante la lettura del file: {ex.Message}");
            }
            catch (InvalidOperationException e)
            {
                throw new InvalidOperationException($" {e.Message}");
            }
        }

        public void VerificaIdUnivoco(string id)
        {
            VerificaIdUnivocoFile(id);
        }
    }
}