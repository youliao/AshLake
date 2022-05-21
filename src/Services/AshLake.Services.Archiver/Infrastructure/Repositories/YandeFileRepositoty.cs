namespace AshLake.Services.Archiver.Infrastructure.Repositories;

public class YandeFileRepositoty : PostObjectRepositoty, IYandeFileRepositoty
{
    public YandeFileRepositoty(DaprClient daprClient) : base(daprClient)
    {
    }

    protected override string BindingName => "yande-file-storage";
}
