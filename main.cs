// Models/Livro.cs
public class Livro
{
    public string Titulo { get; set; }
    public string Autor { get; set; }
    public string ISBN { get; set; }
    public bool Disponivel { get; set; } = true;
}

// Models/Usuario.cs
public class Usuario
{
    public string Nome { get; set; }
    public int ID { get; set; }
}

// Models/Emprestimo.cs
public class Emprestimo
{
    public Livro Livro { get; set; }
    public Usuario Usuario { get; set; }
    public DateTime DataEmprestimo { get; set; }
    public DateTime DataDevolucaoPrevista { get; set; }
    public DateTime? DataDevolucaoEfetiva { get; set; }
}

// Services/Interfaces/INotificador.cs
public interface INotificador
{
    void Notificar(string destinatario, string assunto, string mensagem);
}

// Services/EmailNotificador.cs
public class EmailNotificador : INotificador
{
    public void Notificar(string destinatario, string assunto, string mensagem)
    {
        Console.WriteLine($"[Email] Para: {destinatario} | Assunto: {assunto} | Mensagem: {mensagem}");
    }
}

// Services/SmsNotificador.cs
public class SmsNotificador : INotificador
{
    public void Notificar(string destinatario, string assunto, string mensagem)
    {
        Console.WriteLine($"[SMS] Para: {destinatario} | Mensagem: {mensagem}");
    }
}

// Services/MultaService.cs
public static class MultaService
{
    public static double CalcularMulta(Emprestimo emprestimo)
    {
        if (emprestimo.DataDevolucaoEfetiva > emprestimo.DataDevolucaoPrevista)
        {
            var diasAtraso = (emprestimo.DataDevolucaoEfetiva.Value - emprestimo.DataDevolucaoPrevista).Days;
            return diasAtraso * 1.0;
        }
        return 0;
    }
}

// Services/BibliotecaService.cs
public class BibliotecaService
{
    private readonly List<Livro> livros = new();
    private readonly List<Usuario> usuarios = new();

    public void AdicionarLivro(Livro livro)
    {
        livros.Add(livro);
    }

    public void AdicionarUsuario(Usuario usuario)
    {
        usuarios.Add(usuario);
    }

    public Livro ObterLivroPorISBN(string isbn) => livros.FirstOrDefault(l => l.ISBN == isbn);
    public Usuario ObterUsuarioPorId(int id) => usuarios.FirstOrDefault(u => u.ID == id);

    public List<Livro> ObterTodosLivros() => livros;
    public List<Usuario> ObterTodosUsuarios() => usuarios;
}

// Services/EmprestimoService.cs
public class EmprestimoService
{
    private readonly List<Emprestimo> emprestimos = new();
    private readonly INotificador _emailNotificador;
    private readonly INotificador _smsNotificador;

    public EmprestimoService(INotificador emailNotificador, INotificador smsNotificador)
    {
        _emailNotificador = emailNotificador;
        _smsNotificador = smsNotificador;
    }

    public bool RealizarEmprestimo(Livro livro, Usuario usuario, int dias)
    {
        if (!livro.Disponivel) return false;

        livro.Disponivel = false;
        emprestimos.Add(new Emprestimo
        {
            Livro = livro,
            Usuario = usuario,
            DataEmprestimo = DateTime.Now,
            DataDevolucaoPrevista = DateTime.Now.AddDays(dias)
        });

        _emailNotificador.Notificar(usuario.Nome, "Empréstimo realizado", $"Livro: {livro.Titulo}");
        _smsNotificador.Notificar(usuario.Nome, "", $"Você emprestou: {livro.Titulo}");

        return true;
    }

    public double RealizarDevolucao(Livro livro, Usuario usuario)
    {
        var emprestimo = emprestimos.FirstOrDefault(e =>
            e.Livro.ISBN == livro.ISBN &&
            e.Usuario.ID == usuario.ID &&
            e.DataDevolucaoEfetiva == null);

        if (emprestimo == null) return -1;

        emprestimo.DataDevolucaoEfetiva = DateTime.Now;
        livro.Disponivel = true;

        double multa = MultaService.CalcularMulta(emprestimo);
        if (multa > 0)
        {
            _emailNotificador.Notificar(usuario.Nome, "Multa por Atraso", $"Multa de R$ {multa:0.00}");
        }

        return multa;
    }

    public List<Emprestimo> ObterTodosEmprestimos() => emprestimos;
}

// Program.cs
class Program
{
    static void Main(string[] args)
    {
        var bibliotecaService = new BibliotecaService();
        var emailNotificador = new EmailNotificador();
        var smsNotificador = new SmsNotificador();
        var emprestimoService = new EmprestimoService(emailNotificador, smsNotificador);

        var livro = new Livro { Titulo = "Clean Code", Autor = "Robert C. Martin", ISBN = "978-0132350884" };
        var usuario = new Usuario { Nome = "João Silva", ID = 1 };

        bibliotecaService.AdicionarLivro(livro);
        bibliotecaService.AdicionarUsuario(usuario);

        emprestimoService.RealizarEmprestimo(livro, usuario, 7);
        System.Threading.Thread.Sleep(2000); // Simula atraso
        var multa = emprestimoService.RealizarDevolucao(livro, usuario);
        Console.WriteLine($"Multa calculada: R$ {multa}");

        Console.ReadLine();
    }
}
