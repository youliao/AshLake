﻿using Newtonsoft.Json.Linq;

namespace AshLake.Services.YandeStore.Application.Services;

public interface IYandeArchiverService
{
    Task<BsonDocument> GetPostMetadata(int id);
}