using TaskFlowApi.Infra.DemoSession;

namespace TaskFlowApi.infra.DemoSession
{
    public class DemoSessionWrapper
    {
        public DemoSessionData Data { get; set; } = null!;
        public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    }
}
