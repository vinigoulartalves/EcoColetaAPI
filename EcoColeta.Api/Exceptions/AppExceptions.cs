namespace EcoColeta.Api.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string mensagem) : base(mensagem)
    {
    }
}

public class BusinessException : Exception
{
    public BusinessException(string mensagem) : base(mensagem)
    {
    }
}

public class UnauthorizedAppException : Exception
{
    public UnauthorizedAppException(string mensagem) : base(mensagem)
    {
    }
}
