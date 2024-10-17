using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;
using WinFormsApp1.Models;

namespace WinFormsApp1
{
    public partial class Form2 : Form
    {
        // Mudança: Caminho atualizado para a pasta "Documentos", que tem menos restrições.
        public static string caminho = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ProgramasSalvos.json");

        public Form2()
        {
            InitializeComponent();
        }

        private void btn_save_Click(object sender, EventArgs e)
        {
            if (ValidaCampos())
            {
                try
                {
                    int id = 0;
                    int tempo = Int32.Parse(tempoInput.Text);
                    string alimento = AlimentoInput.Text;
                    int potencia = Int32.Parse(PotenciaInput.Text);
                    char strAquecimento = StringAquecimentoInput.Text.ToCharArray()[0];
                    string instrucoes = InstrucoesInput.Text.Length == 0 ? "" : InstrucoesInput.Text;

                    
                    ProgramasPadroes novoPrograma = new ProgramasPadroes(id, alimento, tempo, potencia, instrucoes, strAquecimento);

                    
                    SalvarDadosJson(novoPrograma, caminho);

                    MessageBox.Show("Programa salvo com sucesso!");
                    this.Close(); 
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao salvar o programa: {ex.Message}");
                }
            }
        }

        public bool ValidaCampos()
        {
            if (string.IsNullOrEmpty(tempoInput.Text) ||
                string.IsNullOrEmpty(AlimentoInput.Text) ||
                string.IsNullOrEmpty(PotenciaInput.Text) ||
                string.IsNullOrEmpty(StringAquecimentoInput.Text))
            {
                MessageBox.Show("Insira todos os campos");
                return false;
            }

            return true;
        }

        public void SalvarDadosJson(ProgramasPadroes dados, string caminhoArquivo)
        {
            try
            {
               
                string directoryPath = Path.GetDirectoryName(caminhoArquivo);
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

             
                List<ProgramasPadroes> programasJaCriados = ListarTodos(caminhoArquivo) ?? new List<ProgramasPadroes>();

                
                programasJaCriados.Add(dados);

               
                string jsonString = JsonSerializer.Serialize(programasJaCriados, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(caminhoArquivo, jsonString);
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show($"Acesso negado ao caminho: {ex.Message}");
            }
            catch (DirectoryNotFoundException ex)
            {
                MessageBox.Show($"Diretório não encontrado: {ex.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao salvar dados: {ex.Message}");
            }
        }

        public List<ProgramasPadroes> ListarTodos(string caminhoArquivo)
        {
            List<ProgramasPadroes> programasCustom = new List<ProgramasPadroes>();

            if (File.Exists(caminhoArquivo))
            {
                try
                {
                    string json = File.ReadAllText(caminhoArquivo);
                    programasCustom = JsonSerializer.Deserialize<List<ProgramasPadroes>>(json);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao ler o arquivo: {ex.Message}");
                }
            }

            return programasCustom;
        }
    }
}
