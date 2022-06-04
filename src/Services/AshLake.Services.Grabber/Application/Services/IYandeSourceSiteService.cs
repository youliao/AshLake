﻿using Newtonsoft.Json.Linq;

namespace AshLake.Services.Grabber.Domain.Services;

public interface IYandeSourceSiteService
{
    Task<ImageFile> GetFileAsync(int id);
    Task<JToken> GetLatestPostAsync();
    Task<JToken?> GetMetadataAsync(int id);
    Task<IEnumerable<JToken>> GetMetadataListAsync(string tags, int limit, int page);
    Task<IEnumerable<JToken>> GetMetadataListAsync(int startId, int limit, int page);
    Task<ImageFile> GetPreviewAsync(int id);
}