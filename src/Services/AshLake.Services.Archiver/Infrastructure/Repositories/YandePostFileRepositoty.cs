namespace AshLake.Services.Archiver.Infrastructure.Repositories;

public class YandePostFileRepositoty : PostObjectRepositoty, IYandePostFileRepositoty
{
    public YandePostFileRepositoty(DaprClient daprClient) : base(daprClient)
    {
    }

    protected override string BindingName => "yande-file-storage";
}
