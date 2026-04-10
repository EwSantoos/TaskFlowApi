namespace TaskFlowApi.Application.Interfaces.Demo
{
    public interface IDemoSessionService
    {
        string CriarSessao();
        IDemoSessionContext ObterSessao(string sessionId);
    }
}
