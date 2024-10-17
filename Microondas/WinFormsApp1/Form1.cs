using System.Timers;
using WinFormsApp1.Models;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        private System.Timers.Timer timer, limp;
        public List<ProgramasPadroes> programasPadroes { get; set; } = new List<ProgramasPadroes>();

        // Construtor da classe Form1
        public Form1()
        {
            // Inicialização do timer principal que controla o aquecimento
            timer = new System.Timers.Timer();
            timer.Interval = 1000; 
            timer.Elapsed += timerChama; 

            // Inicialização do timer de limpeza
            limp = new System.Timers.Timer();
            limp.Interval = 1000; 
            limp.Elapsed += limpeza; 
            limp.AutoReset = false; 

            // Inicialização dos Programas Padrões
            programasPadroes.Add(new ProgramasPadroes(1, "Pipoca", 3, 7,
                "Observar o barulho de estouros do milho, caso houver um intervalo de mais de 10 segundos entre um estouro e outro, interrompa o aquecimento.", 'p'));
            programasPadroes.Add(new ProgramasPadroes(2, "Leite", 5, 5,
                "Cuidado com aquecimento de líquidos, o choque térmico aliado ao movimento do recipiente pode causar fervura imediata causando risco de queimaduras.", 'l'));
            programasPadroes.Add(new ProgramasPadroes(3, "Carnes de boi", 14, 4,
                "Interrompa o processo na metade e vire o conteúdo com a parte de baixo para cima para o descongelamento uniforme.", 'c'));
            programasPadroes.Add(new ProgramasPadroes(4, "Frango", 8, 7,
                "Interrompa o processo na metade e vire o conteúdo com a parte de baixo para cima para o descongelamento uniforme.", 'f'));
            programasPadroes.Add(new ProgramasPadroes(6, "Feijão", 8, 9,
                "Deixe o recipiente destampado e, em casos de plástico, cuidado ao retirar o recipiente, pois o mesmo pode perder resistência em altas temperaturas.", 'b'));

            InitializeComponent(); // Inicializa os componentes do formulário
        }

        // Variáveis e propriedades usadas ao longo da aplicação
        public int tempoS = 0; 
        public int potenciaM = 10; 
        public int contador = 0; 
        public bool parado = false; 
        public char stringAquecimento = '.'; 
        public bool init = true; 

        // Método que valida o tempo de aquecimento
        public bool validaTempo(int tempo)
        {
            if (tempo > 120 || tempo < 1)
            {
                MessageBox.Show("O tempo deve ser menor que 2 minutos e no mínimo 1 segundo");
                return false;
            }
            return true;
        }

        // Método que limpa os campos da interface
        public void limpaCampos()
        {
            if (displayResult.InvokeRequired)
            {
                // Chama o método na thread correta
                displayResult.Invoke(new Action(limpaCampos));
            }
            else
            {
                displayResult.Text = "";
                displayMain.Text = "";
                power.Text = "";
                powerResult.Text = "";
                stringInformativa.Text = "";
                tempoS = 0;
                potenciaM = 10;
                contador = 0;
                stringAquecimento = '.';
                init = true;
            }
        }


        // Método que valida a potência informada
        public bool validaPotencia(int potencia)
        {
            return potencia >= 0 && potencia <= 10;
        }

        // Método que inicia o aquecimento rápido (30 segundos, potência 10)
        public void inicioRapido()
        {
            tempoS = 30;
            displayResult.Text = "00:30";
            powerResult.Text = potenciaM.ToString();
            parado = false;
            timer.Start();
        }

        // Método que inicia o aquecimento com tempo e potência definidos
        public void IniciarAquecimento(int tempo, int potencia)
        {
            if (!validaTempo(tempo)) return;

            tempoS = tempo;
            displayResult.Text = SegundosParaMinutos(tempoS);
            displayMain.Text = "";

            if (validaPotencia(potencia))
            {
                potenciaM = potencia;
            }
            else
            {
                potenciaM = 10; 
                MessageBox.Show("A potência informada é inválida. Usando potência padrão de 10.");
            }
            powerResult.Text = potenciaM.ToString();
            timer.Start();
            parado = false;
        }

        // Método que cria a string informativa de aquecimento
        public void criaStringInformativa()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(criaStringInformativa)); // Garante que a operação seja feita na thread da UI
            }
            else
            {
                // Aqui você pode apenas manter o progresso visual sem repetir constantemente
                string informativa = $"Aquecendo... Tempo restante: {SegundosParaMinutos(tempoS)}";
                stringInformativa.Text = informativa;

                if (tempoS <= 0)
                {
                    stringInformativa.Text = "Aquecimento concluído";
                    limp.Start(); 
                    timer.Stop(); 
                }
            }
        }


        // Método chamado pelo timer principal a cada segundo
        private void timerChama(object sender, ElapsedEventArgs e)
        {
            if (tempoS > 0)
            {
                tempoS--; 

                // Garante que a atualização dos controles seja feita na thread da UI
                if (displayResult.InvokeRequired)
                {
                    displayResult.Invoke(new Action(() =>
                    {
                        displayResult.Text = SegundosParaMinutos(tempoS); 
                        criaStringInformativa(); 
                    }));
                }
                else
                {
                    displayResult.Text = SegundosParaMinutos(tempoS); 
                    criaStringInformativa(); 
                }
            }
            else
            {
                timer.Stop(); 

                if (displayResult.InvokeRequired)
                {
                    displayResult.Invoke(new Action(() =>
                    {
                        stringInformativa.Text = "Aquecimento concluído";
                    }));
                }
                else
                {
                    stringInformativa.Text = "Aquecimento concluído";
                }
                limp.Start(); 
            }
        }



       
        private void limpeza(object sender, ElapsedEventArgs e)
        
            limpaCampos();
        }

        
        public int minutosParaSegundos(int tempo)
        {
            return tempo * 60;
        }

        
        public string SegundosParaMinutos(int tempo)
        {
            int minutos = tempo / 60;
            int segundosRestantes = tempo % 60;
            return $"{minutos:D2}:{segundosRestantes:D2}";
        }

       
        private void InitAquecimento_Click(object sender, EventArgs e)
        {
            if (init)
            {
                stringAquecimento = '.';
                if (parado)
                {
                    timer.Start();
                    parado = false;
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(displayMain.Text))
                    {
                        MessageBox.Show("Quando o tempo não é definido, o início rápido é iniciado");
                        inicioRapido(); 
                    }
                    else
                    {
                        int tempo = Int32.Parse(displayMain.Text);
                        int pot = string.IsNullOrWhiteSpace(power.Text) ? 10 : Int32.Parse(power.Text);
                        potenciaM = pot;
                        if (string.IsNullOrWhiteSpace(power.Text))
                        {
                            MessageBox.Show("Em caso de potência não informada, será inserido em tela o valor 10 como padrão");
                        }
                        IniciarAquecimento(tempo, pot);
                    }
                }
            }
        }

        // Evento de clique no botão de parar ou cancelar
        private void pararCancelar_Click(object sender, EventArgs e)
        {
            if (parado)
            {
                limpaCampos(); 
                parado = false;
            }
            else
            {
                timer.Stop(); 
                parado = true;
                init = false;
            }
        }

        
        private void BotaoZero_Click(object sender, EventArgs e)
        {
            displayMain.Text += "0";
        }

        private void ButtonUm_Click(object sender, EventArgs e)
        {
            displayMain.Text += "1";
        }

        private void ButtonDois_Click(object sender, EventArgs e)
        {
            displayMain.Text += "2";
        }

        private void ButtonTrez_Click(object sender, EventArgs e)
        {
            displayMain.Text += "3";
        }

        private void ButtonQuatro_Click(object sender, EventArgs e)
        {
            displayMain.Text += "4";
        }

        private void ButtonCinco_Click(object sender, EventArgs e)
        {
            displayMain.Text += "5";
        }

        private void ButtonSeis_Click(object sender, EventArgs e)
        {
            displayMain.Text += "6";
        }

        private void ButtonSete_Click(object sender, EventArgs e)
        {
            displayMain.Text += "7";
        }

        private void ButtonOito_Click(object sender, EventArgs e)
        {
            displayMain.Text += "8";
        }

        private void ButtonNove_Click(object sender, EventArgs e)
        {
            displayMain.Text += "9";
        }

        // Cria botões para os programas padrões (pré-definidos)
        private void CriarBotoesProgramas()
        {
            foreach (var programa in programasPadroes)
            {
                Button novoBotao = new Button
                {
                    Text = $"{programa.Nome}\n Potência: {programa.Potencia}\n Tempo: {programa.Tempo}\n\n{programa.Instrucoes}",
                    Tag = programa,
                    Dock = DockStyle.Left,
                    Size = new Size(120, 80)
                };
                novoBotao.Click += botaoPrograma_Click;

                Padroes.Controls.Add(novoBotao); 
            }
        }

      
        private void Form1_Load(object sender, EventArgs e)
        {
            CriarBotoesProgramas(); 
            criaBotoesCustom(); 
        }

        // Evento de clique em um botão de programa padrão
        private void botaoPrograma_Click(object sender, EventArgs e)
        {
            limpaCampos(); 
            init = false;
            if (sender is Button botao && botao.Tag is ProgramasPadroes programa)
            {
                tempoS = minutosParaSegundos(programa.Tempo); 
                potenciaM = programa.Potencia; 
                powerResult.Text = potenciaM.ToString();
                displayResult.Text = SegundosParaMinutos(tempoS);
                stringAquecimento = programa.stringAquecimento;
                timer.Start(); 
            }
        }

        
        private void CadastraNovo_Click(object sender, EventArgs e)
        {
            Form2 abrirForm = new Form2();
            abrirForm.ShowDialog(); 
        }

        // Cria botões personalizados a partir dos programas salvos
        public void criaBotoesCustom()
        {
            Form2 salvos = new Form2();
            List<ProgramasPadroes> novosBotoes = salvos.listarTodos(Form2.caminho); 
            CriarBotoesCustom(novosBotoes); 
        }

        
        private void CriarBotoesCustom(List<ProgramasPadroes> novosBotoes)
        {
            foreach (var programa in novosBotoes)
            {
                Button novoBotao = new Button
                {
                    Text = $"{programa.Nome}\n Potência: {programa.Potencia}\n Tempo: {programa.Tempo}\n\n{programa.Instrucoes}",
                    Tag = programa,
                    Dock = DockStyle.Right,
                    Font = new Font(DefaultFont, FontStyle.Italic),
                    Size = new Size(120, 80)
                };
                novoBotao.Click += botaoPrograma_Click;

                panelCustom.Controls.Add(novoBotao); 
            }
        }
    }
}
