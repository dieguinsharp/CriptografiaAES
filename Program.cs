using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
namespace Salvar_Dados
{
    class Program
    {
        static void Main(string[] args)
        {
            bool chave = false;
            bool gatilho = false;
            string caminho = "";
            string resp = "";
            while (resp != "N")
            {
                int opc;
                if (gatilho == false)
                {
                    if (File.Exists("caminho.txt"))
                    {
                        StreamReader open2 = new StreamReader("caminho.txt");
                        caminho = open2.ReadLine();
                        open2.Close();
                    }
                    else
                    {
                        caminho = (@"C:\Users\" + Environment.UserName.ToString() + @"\Desktop\usuario.txt");
                    }
                }
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Caminho atual: " + caminho);
                Console.WriteLine("------ Salvar Dados -------");
                Console.WriteLine("------ [1] Abrir Dados ----");
                Console.WriteLine("------ [2] Criar Dados ----");
                Console.WriteLine("------ [3] Mudar Caminho --");
                Console.WriteLine("------ [4] Caminho Padrão -");
                Console.WriteLine("---------------------------");
                Console.Write("Informe o que deseja fazer:");
                opc = Convert.ToInt32(Console.ReadLine());
                switch (opc)
                {
                    case 1:
                        try
                        {
                            int numeroLinhas = System.IO.File.ReadAllLines(caminho).Length;
                            if (numeroLinhas > 4)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("ERRO: Verifique seu arquivo de texto.");
                                Console.WriteLine("ERRO: Existe linhas a mais no arquivo.");
                            }
                            else
                            {
                                StreamReader open = new StreamReader(caminho);
                                string[] txt = open.ReadToEnd().Split('\n');

                                Console.Write("Informe sua senha para descriptografar:");
                                string pass2 = Console.ReadLine();

                                string email2 = txt[0];
                                string senha2 = txt[1];
                                string rg2 = txt[2];
                                string telefone2 = txt[3];

                                open.Close();

                                email2 = decript(pass2, email2);
                                senha2 = decript(pass2, senha2);
                                rg2 = decript(pass2, rg2);
                                telefone2 = decript(pass2, telefone2);

                                if (chave == false)
                                {
                                    Console.WriteLine("Senha de Criptografia:" + pass2);
                                    Console.WriteLine("Email: > " + email2);
                                    Console.WriteLine("Senha: > " + senha2);
                                    Console.WriteLine("RG: > " + rg2);
                                    Console.WriteLine("Telefone: > " + telefone2);
                                }
                            }
                        }
                        catch (System.IO.FileNotFoundException)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Não existe um arquivo de texto nesse caminho!");
                        }
                        break;
                    case 2:
                        Console.Write("Primeiro informe sua senha de criptografia [AES]:");
                        string pass = Console.ReadLine();

                        Console.Write("Digite seu e-mail:");
                        string email = Console.ReadLine();
                        email = cript(pass, email);

                        Console.Write("Digite sua senha:");
                        string senha = Console.ReadLine();
                        senha = cript(pass, senha);

                        Console.Write("Digite seu RG:");
                        string rg = Console.ReadLine();
                        rg = cript(pass, rg);

                        Console.Write("Digite seu telefone:");
                        string telefone = Console.ReadLine();
                        telefone = cript(pass, telefone);

                        if (File.Exists(caminho) == true)
                        {
                            File.Delete(caminho);
                        }

                        StreamWriter save = new StreamWriter(caminho, true);

                        save.WriteLine(email);
                        save.WriteLine(senha);
                        save.WriteLine(rg);
                        save.WriteLine(telefone);
                        save.Close();

                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Dados Gravados com sucesso!");
                        Console.WriteLine("Caminho Atual do Arquivo > " + caminho + ".");
                        break;
                    case 3:
                        Console.Write("Digite CORRETAMENTE o novo caminho:");
                        caminho = Console.ReadLine();

                        StreamWriter save2 = new StreamWriter("caminho.txt", true);
                        save2.WriteLine(caminho);

                        save2.Close();
                        gatilho = false;

                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Seu caminho foi atualizado para " + caminho);

                        break;
                    case 4:
                        caminho = (@"C:\Users\" + Environment.UserName.ToString() + @"\Desktop\usuario.txt");

                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Seu caminho foi atualizado para " + caminho);

                        gatilho = true;
                        break;
                    default:
                        Console.WriteLine("Opção Inválida!");
                        break;
                }
                Console.ForegroundColor = ConsoleColor.Red;

                Console.Write("Deseja usar novamente? [S][N]");
                resp = Console.ReadLine();

                if ((resp != "S") && (resp != "N"))
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("Opção Inválida, lidaremos como se fosse um SIM XD");
                }
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Obrigado pela atenção.");
            Console.ReadKey();

            string cript(string pass, string conteudo)
            {
                byte[] Results;
                System.Text.UTF8Encoding UTF8 = new System.Text.UTF8Encoding();
                MD5CryptoServiceProvider HashProvider = new MD5CryptoServiceProvider();
                byte[] TDESKey = HashProvider.ComputeHash(UTF8.GetBytes(pass));
                TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider();
                TDESAlgorithm.Key = TDESKey;
                TDESAlgorithm.Mode = CipherMode.ECB;
                TDESAlgorithm.Padding = PaddingMode.PKCS7;
                byte[] DataToEncrypt = UTF8.GetBytes(conteudo);

                try
                {
                    ICryptoTransform Encryptor = TDESAlgorithm.CreateEncryptor();
                    Results = Encryptor.TransformFinalBlock(DataToEncrypt, 0, DataToEncrypt.Length);
                    conteudo = Convert.ToBase64String(Results);
                    return conteudo;
                }
                catch (CryptographicException)
                {
                    Console.WriteLine("Senha de criptografia inválida!");
                    return null;
                }
                finally
                {
                    TDESAlgorithm.Clear();
                    HashProvider.Clear();
                }
            }
            Byte[] DataToDecrypt;
            string decript(string pass, string conteudo)
            {
                byte[] Results;
                System.Text.UTF8Encoding UTF8 = new System.Text.UTF8Encoding();
                MD5CryptoServiceProvider HashProvider = new MD5CryptoServiceProvider();
                byte[] TDESKey = HashProvider.ComputeHash(UTF8.GetBytes(pass));
                TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider();
                TDESAlgorithm.Key = TDESKey;
                TDESAlgorithm.Mode = CipherMode.ECB;
                TDESAlgorithm.Padding = PaddingMode.PKCS7;

                try
                {
                    DataToDecrypt = Convert.FromBase64String(conteudo);
                }
                catch (FormatException)
                {
                    Console.WriteLine("Formato de texto inválido para desembaralhar! Tente novamente!");
                    chave = true;
                    return null;
                }
                try
                {
                    ICryptoTransform Decryptor = TDESAlgorithm.CreateDecryptor();
                    Results = Decryptor.TransformFinalBlock(DataToDecrypt, 0, DataToDecrypt.Length);
                    conteudo = UTF8.GetString(Results);
                    chave = false;
                    return conteudo;
                }
                catch (CryptographicException)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("ERRO: Senha de criptografia incorreta!");
                    chave = true;
                    return null;
                }
                catch (NullReferenceException)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("ERRO: Informe sua senha!");
                    chave = true;
                    return null;
                }
                finally
                {
                    TDESAlgorithm.Clear();
                    HashProvider.Clear();
                }
            }
            Console.ReadKey();
        }
    }
}
