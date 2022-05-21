namespace AshLake.Services.Archiver.Infrastructure.Repositories;

public class YandePreviewRepositoty : PostObjectRepositoty, IYandePreviewRepositoty
{
    public YandePreviewRepositoty(DaprClient daprClient) : base(daprClient)
    {
    }

    protected override string BindingName => "yande-preview-storage";
}
