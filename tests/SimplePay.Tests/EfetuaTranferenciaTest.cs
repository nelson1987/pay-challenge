using FluentResults;

namespace SimplePay.Tests
{
    public class Lojista : Usuario
    {
        public int Id { get; set; }
        public decimal Saldo { get; set; }
    }
    public class Usuario
    {
        public int Id { get; set; }
        public decimal Saldo { get; set; }
    }
    public class Transferencia
    {
        public required Usuario Debitante { get; init; }
        public required Usuario Creditante { get; init; }
        public required decimal Valor { get; init; }
    }
    public static class TransferenciaBuilder
    {
        public static (Transferencia? transferencia, Exception? erro) Create(Usuario debitante, Usuario creditante, decimal valor)
        {
            if (debitante is Lojista)
            {
                return (null, new Exception("Não foi possivel efetuar a transferência pois o usuário é um Lojista."));
            }

            return (new Transferencia()
            {
                Debitante = debitante,
                Creditante = creditante,
                Valor = valor
            }, null);
        }
    }
    public class TransferenciaService
    {
        public Result Transferir(Transferencia transferencia)
        {
            if (transferencia.Valor > transferencia.Debitante.Saldo)
            {
                return Result.Fail("Não foi possivel efetuar a transferência pois o saldo do debitante é insuficiente.");
            }
            transferencia.Debitante.Saldo -= transferencia.Valor;
            transferencia.Creditante.Saldo += transferencia.Valor;
            return Result.Ok();
        }
    }
    public class EfetuaTransferenciaUnitTests
    {
        private readonly Usuario _debitante;
        private readonly Usuario _creditante;
        private readonly TransferenciaService _servico;

        public EfetuaTransferenciaUnitTests()
        {
            _debitante = new Usuario() { Id = 1, Saldo = 10 };
            _creditante = new Usuario() { Id = 2, Saldo = 10 };
            _servico = new TransferenciaService();
        }

        [Fact]
        public void Efetuar_Transferencia_UsuarioEnviaParaUsuario()
        {
            var builder = TransferenciaBuilder.Create(_debitante, _creditante, 5);
            Assert.NotNull(builder.transferencia);
        }

        [Fact]
        public void Efetuar_Transferencia_UsuarioEnviaParaLojista()
        {
            Lojista creditante = new Lojista() { Id = 2, Saldo = 10 };
            var builder = TransferenciaBuilder.Create(_debitante, creditante, 5);
            Assert.NotNull(builder.transferencia);
        }

        [Fact]
        public void Efetuar_Transferencia_LojistaNaoEnviaParaLojista_Throw_Exception()
        {
            Lojista debitante = new Lojista() { Id = 1, Saldo = 10 };
            Lojista creditante = new Lojista() { Id = 2, Saldo = 10 };
            var builder = TransferenciaBuilder.Create(debitante, creditante, 5);
            Assert.NotNull(builder.erro);
        }

        [Fact]
        public void Efetuar_Transferencia_LojistaNaoEnviaParaUsuario_Throw_Exception()
        {
            Lojista debitante = new Lojista() { Id = 1, Saldo = 10 };
            var builder = TransferenciaBuilder.Create(debitante, _creditante, 5);
            Assert.NotNull(builder.erro);
        }

        [Fact]
        public void Efetuar_Transferencia_ValidarSaldoUsuario_SaldoDisponivel()
        {
            var builder = TransferenciaBuilder.Create(_debitante, _creditante, 5);
            Assert.NotNull(builder.transferencia);
            _servico.Transferir(builder.transferencia);
            Assert.Equal(5, _debitante.Saldo);
            Assert.Equal(15, _creditante.Saldo);
        }

        [Fact]
        public void Efetuar_Transferencia_ValidarSaldoUsuario_SaldoInsuficiente()
        {
            var builder = TransferenciaBuilder.Create(_debitante, _creditante, 15);
            Assert.NotNull(builder.transferencia);
            var result = _servico.Transferir(builder.transferencia);
            Assert.True(result.IsFailed);
        }
    }
    public class EfetuaTranferenciaTest
    {
        [Fact]
        public void Cadastrar_Usuario_PermitirQueApenasUmCPFOuEmail()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void Efetuar_Transferencia_AutorizarServicoExternoAutorizacao()
        {
            throw new NotImplementedException();

        }
        [Fact]
        public void Efetuar_Transferencia_RollbackSeErroTransacao()
        {
            throw new NotImplementedException();

        }
        [Fact]
        public void Receber_Transferencia_ReceberNotificacao()
        {
            throw new NotImplementedException();

        }
        [Fact]
        public void Receber_Transferencia_ReceberNotificacaoComServicoIndisponivel()
        {
            throw new NotImplementedException();

        }
    }
}