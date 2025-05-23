using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Tris
{
    public partial class MainWindow : Window
    {
        // Matrice 3x3 che rappresenta la griglia di gioco. 
        // 0 = cella vuota, 1 = giocatore "X", 2 = giocatore "O"
        int[,] griglia = {  { 0, 0, 0 },
                            { 0, 0, 0 },
                            { 0, 0, 0 } };
        // Indica il turno corrente: 1 = "X", 2 = "O"
        int turno = 1;
        // Conta il numero di mosse effettuate per determinare la fine della partita
        int nMosseFatte = 0;
        // Modalità di gioco: true = partita tra due umani, false = partita contro la CPU
        bool modalità = true; // true = Umano vs Umano, false = Umano vs CPU
        //Costruttore della classe MainWindow.
        // Inizializza i componenti dell'interfaccia grafica.
        public MainWindow(){
            InitializeComponent();
        }
        public int ControllaVittoria(int[,] griglia)
        {
            bool vittoria = false; // Flag che indica se c'è una vittoria
            int cellaVincente = 0; // Memorizza il valore del giocatore vincente
            // Controllo delle vittorie nelle colonne e nelle righe
            for (int i = 0; i < 3 && !vittoria; i++){
                // Controllo colonne
                if (griglia[0, i] == griglia[1, i] && griglia[1, i] == griglia[2, i] && griglia[2, i] != 0){
                    vittoria = true;
                    cellaVincente = griglia[0, i]; // Giocatore vincente (X = 1, O = 2)
                }
                // Controllo righe
                if (griglia[i, 0] == griglia[i, 1] && griglia[i, 1] == griglia[i, 2] && griglia[i, 2] != 0){
                    vittoria = true;
                    cellaVincente = griglia[i, 0];
                }
            }
            // Controllo delle vittorie nelle diagonali
            if (!vittoria){
                if (griglia[0, 0] == griglia[1, 1] && griglia[1, 1] == griglia[2, 2] && griglia[2, 2] != 0){
                    vittoria = true;
                    cellaVincente = griglia[1, 1]; // Giocatore vincente 
                }
            if (griglia[0, 2] == griglia[1, 1] && griglia[1, 1] == griglia[2, 0] && griglia[2, 0] != 0){
                    vittoria = true;
                    cellaVincente = griglia[1, 1];
                }
            }
            // Se c'è una vittoria, restituisce il punteggio corrispondente
            if (vittoria){
                if (cellaVincente == 1) return 10; // Vince X
                else return -10; // Vince O
            }
            // Controlla se la griglia è piena (pareggio)
            if (GrigliaPiena(griglia)) return 0;
            // La partita è ancora in corso
            return -1;
        }
        public bool GrigliaPiena(int[,] board){
            // Scansiona tutta la griglia per verificare se ci sono celle vuote
            for (int i = 0; i < 3; i++){
                for (int j = 0; j < 3; j++){
                    if (board[i, j] == 0) return false; // Se trova almeno una cella vuota, la griglia non è piena
                }
            }
            return true; // Se tutte le celle sono occupate, la griglia è piena
        }
        public void InserisciMossa(int x, int y, Button Cella) {
            // Controlla se la cella è vuota prima di inserire la mossa
            if (griglia[x, y] == 0) {
                if (turno == 1) { // Turno del giocatore "X"
                    griglia[x, y] = turno; // Segna la mossa sulla griglia
                    turno = 2; // Passa il turno al giocatore "O"
                    nMosseFatte++; // Incrementa il conteggio delle mosse
                    // Se si sta giocando contro la CPU e la griglia non è piena, fa la mossa della CPU
                    if (!modalità && !GrigliaPiena(griglia)) {
                        (x, y) = MossaCPU(); // La CPU sceglie una mossa ottimale
                        griglia[x, y] = turno; // Segna la mossa della CPU
                        turno = 1; // Passa il turno al giocatore "X"
                        nMosseFatte++;
                    }
                }
                else { // Turno del giocatore "O"
                    griglia[x, y] = turno;
                    turno = 1; // Passa il turno al giocatore "X"
                    nMosseFatte++;
                }
                AggiornaGUI(); // Aggiorna l'interfaccia grafica per riflettere la nuova mossa
                // Controlla se la partita è terminata con una vittoria o un pareggio
                int vincitore = ControllaVittoria(griglia);
                if (vincitore != -1) {
                    if (vincitore == 10) { // Vittoria di "X"
                        lblRisultato.Content = "Ha vinto X!";
                        lblRisultato.Background = Brushes.LimeGreen;
                    }
                    else if (vincitore == -10) { // Vittoria di "O"
                        lblRisultato.Content = "Ha vinto O!";
                        lblRisultato.Background = Brushes.DeepSkyBlue;
                        // Se la CPU vince e la griglia non è piena, il colore cambia a rosso
                        if (!modalità && !GrigliaPiena(griglia)) {
                            lblRisultato.Background = Brushes.Red;
                        }
                    }
                    else { // Pareggio
                        lblRisultato.Content = "Pareggio!";
                        lblRisultato.Background = Brushes.White;
                    }
                    lblRisultato.Visibility = Visibility.Visible; // Mostra il risultato della partita
                    DisabilitaCelle(); // Disabilita le celle per impedire ulteriori mosse
                }
            }
        }
       public void DisabilitaCelle() {
            // Disabilita tutti i pulsanti della griglia, impedendo ulteriori mosse
            btnSopraSinistra.IsEnabled = false;
            btnSopraCentro.IsEnabled = false;
            btnSopraDestra.IsEnabled = false;
            btnMedioSinistra.IsEnabled = false;
            btnMedioCentro.IsEnabled = false;
            btnMedioDestra.IsEnabled = false;
            btnSottoSinistra.IsEnabled = false;
            btnSottoCentro.IsEnabled = false;
            btnSottoDestra.IsEnabled = false;
        }
        public void AggiornaGUI(){
            // Matrice di bottoni che rappresenta la griglia di gioco
            Button[,] celle = {
                { btnSopraSinistra, btnSopraCentro, btnSopraDestra },
                { btnMedioSinistra,  btnMedioCentro,  btnMedioDestra },
                { btnSottoSinistra,  btnSottoCentro,  btnSottoDestra }
            };
            // Scansiona l'intera griglia per aggiornare l'interfaccia grafica
            for (int y = 0; y < 3; y++){
                for (int x = 0; x < 3; x++){
                    if (griglia[x, y] == 1){ // Se la cella contiene una "X"
                        celle[x, y].Content = "X"; // Imposta il testo del bottone
                        celle[x, y].Foreground = Brushes.Green; // Colore del testo per il giocatore X
                    }
                    else if (griglia[x, y] == 2){ // Se la cella contiene una "O"
                        celle[x, y].Content = "O"; // Imposta il testo del bottone
                        celle[x, y].Foreground = Brushes.Blue; // Colore del testo per il giocatore O
                    }
                }
            }
        }
        public (int, int) MossaCPU() {
            // Crea una copia temporanea della griglia per la simulazione della CPU
            int[,] grigliaTemp = new int[3, 3];
            for (int i = 0; i < 3; i++) {
                for (int j = 0; j < 3; j++) {
                    grigliaTemp[i, j] = griglia[i, j];
                }
            }
            int migliorMossa = int.MaxValue; // La CPU cerca la mossa con il punteggio più basso per contrastare X
            int mossaX = -1; // Coordinata X della miglior mossa
            int mossaY = -1; // Coordinata Y della miglior mossa
            // Itera su tutte le celle disponibili per trovare la miglior mossa possibile
            for (int i = 0; i < 3; i++) {
                for (int j = 0; j < 3; j++) {
                    if (grigliaTemp[i, j] == 0) { // Se la cella è vuota
                        grigliaTemp[i, j] = 2; // Simula la mossa della CPU ("O")
                        int score = Minimax(grigliaTemp, true, 9 - nMosseFatte); // Valuta la mossa con l'algoritmo Minimax
                        grigliaTemp[i, j] = 0; // Ripristina la cella allo stato iniziale
                        // Se il punteggio della mossa è inferiore al miglior punteggio trovato, lo aggiorna
                        if (score < migliorMossa) {
                            migliorMossa = score;
                            mossaX = i;
                            mossaY = j;
                        }
                    }
                }
            }
            // Restituisce le coordinate della miglior mossa trovata per la CPU
            return (mossaX, mossaY);
        }
        public int Minimax(int[,] grigliaTemp, bool isMaximizing, int depth) {
            // Controlla se la partita è già terminata (vittoria, sconfitta o pareggio)
            int score = ControllaVittoria(grigliaTemp);
            if (score != -1) {
                // Se "X" ha vinto, restituisce un punteggio positivo
                if (score == 10)
                    return 10 - (9 - depth);  // Penalizza vittorie più tardive per preferire mosse rapide
                else if (score == -10)
                    return -10 + (9 - depth); // Penalizza sconfitte più tardive
                else
                    return score; // Ritorna 0 in caso di pareggio
            }
            // Se il limite di profondità è raggiunto, restituisce 0 (approssimazione per stato neutrale)
            if (depth == 0) {
                return 0;
            }
            // Simulazione delle mosse del giocatore e della CPU
            if (isMaximizing) {
                // Turno del giocatore "X", cerca di ottenere il miglior punteggio possibile
                int bestScore = int.MinValue;
                for (int i = 0; i < 3; i++) {
                    for (int j = 0; j < 3; j++) {
                        if (grigliaTemp[i, j] == 0) { // Se la cella è vuota
                            grigliaTemp[i, j] = 1; // Giocatore "X" prova una mossa
                            bestScore = Math.Max(bestScore, Minimax(grigliaTemp, false, depth - 1)); // Ricorsione
                            grigliaTemp[i, j] = 0; // Ripristina la cella
                        }
                    }
                }
                return bestScore; // Restituisce il miglior punteggio trovato
            } else {
                // Turno della CPU ("O"), cerca di minimizzare il punteggio del giocatore
                int bestScore = int.MaxValue;
                for (int i = 0; i < 3; i++) {
                    for (int j = 0; j < 3; j++) {
                        if (grigliaTemp[i, j] == 0) { // Se la cella è vuota
                            grigliaTemp[i, j] = 2; // La CPU prova una mossa
                            bestScore = Math.Min(bestScore, Minimax(grigliaTemp, true, depth - 1)); // Ricorsione
                            grigliaTemp[i, j] = 0; // Ripristina la cella
                        }
                    }
                }
                return bestScore; // Restituisce il miglior punteggio trovato
            }
        }
        // I gestori di eventi per ogni cella della griglia chiamano InserisciMossa
        // con le coordinate corrispondenti alla posizione del pulsante
        private void btnSopraSinistra_Click(object sender, RoutedEventArgs e){
            InserisciMossa(0, 0, btnSopraSinistra); // Cella in alto a sinistra
        }
        private void btnSopraCentro_Click(object sender, RoutedEventArgs e){
            InserisciMossa(0, 1, btnSopraCentro); // Cella al centro in alto
        }
        private void btnSopraDestra_Click(object sender, RoutedEventArgs e){
            InserisciMossa(0, 2, btnSopraDestra); // Cella in alto a destra
        }
        private void btnMedioSinistra_Click(object sender, RoutedEventArgs e){
            InserisciMossa(1, 0, btnMedioSinistra); // Cella a sinistra al centro
        }
        private void btnMedioCentro_Click(object sender, RoutedEventArgs e){
            InserisciMossa(1, 1, btnMedioCentro); // Cella centrale
        }
        private void btnMedioDestra_Click(object sender, RoutedEventArgs e){
            InserisciMossa(1, 2, btnMedioDestra); // Cella a destra al centro
        }
        private void btnSottoSinistra_Click(object sender, RoutedEventArgs e){
            InserisciMossa(2, 0, btnSottoSinistra); // Cella in basso a sinistra
        }
        private void btnSottoCentro_Click(object sender, RoutedEventArgs e){
            InserisciMossa(2, 1, btnSottoCentro); // Cella al centro in basso
        }
        private void btnSottoDestra_Click(object sender, RoutedEventArgs e){
            InserisciMossa(2, 2, btnSottoDestra); // Cella in basso a destra
        }
        private void btnNuovaPartita_Click(object sender, RoutedEventArgs e){
            NuovaPartita(); // Avvia una nuova partita
        }
        // Metodo per resettare la griglia di gioco e abilitare i pulsanti
        private void NuovaPartita() { 
            nMosseFatte = 0; // Resetta il numero di mosse fatte
            // Riabilita tutti i pulsanti della griglia e resetta il loro contenuto
            btnSopraSinistra.IsEnabled = true;
            btnSopraSinistra.Content = "";
            btnSopraCentro.IsEnabled = true;
            btnSopraCentro.Content = "";
            btnSopraDestra.IsEnabled = true;
            btnSopraDestra.Content = "";
            btnMedioSinistra.IsEnabled = true;
            btnMedioSinistra.Content = "";
            btnMedioCentro.IsEnabled = true;
            btnMedioCentro.Content = "";
            btnMedioDestra.IsEnabled = true;
            btnMedioDestra.Content = "";
            btnSottoSinistra.IsEnabled = true;
            btnSottoSinistra.Content = "";
            btnSottoCentro.IsEnabled = true;
            btnSottoCentro.Content = "";
            btnSottoDestra.IsEnabled = true;
            btnSottoDestra.Content = "";
            // Reset della matrice della griglia di gioco
            for (int i = 0; i < 3; i++) {
                for (int j = 0; j < 3; j++){
                    griglia[i, j] = 0;
                }
            }
            turno = 1; // Reset del turno di gioco
            lblRisultato.Visibility = Visibility.Hidden; // Nasconde il messaggio del risultato
        }
        // Gestisce il cambio modalità tra "Umano vs Umano" e "Umano vs Computer"
        private void btnCambiaModalità_Click(object sender, RoutedEventArgs e){
            NuovaPartita(); // Reset della partita quando si cambia modalità
            // Aggiorna il testo del pulsante per riflettere la modalità di gioco attiva
            if (modalità) 
                btnCambiaModalità.Content = "Umano vs Computer";
            else 
                btnCambiaModalità.Content = "Umano vs Umano";
            modalità = !modalità; // Inverte lo stato della modalità
        }

    }
}