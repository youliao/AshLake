﻿global using Dapr;
global using FluentValidation;
global using FluentValidation.AspNetCore;
global using Polly;
global using MediatR;
global using System.Text.Json.Serialization;
global using Microsoft.Extensions.Options;
global using Microsoft.OpenApi.Models;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Diagnostics.HealthChecks;
global using Microsoft.Extensions.FileProviders;
global using Microsoft.Extensions.Hosting;
global using Microsoft.EntityFrameworkCore;
global using System.Globalization;
global using System.IO.Compression;
global using System.Linq.Expressions;
global using System.Text.RegularExpressions;
global using HealthChecks.UI.Client;
global using Microsoft.AspNetCore.Diagnostics.HealthChecks;
global using AshLake.Services.Yande.Domain.Posts;
global using AshLake.Services.Yande.Application.Posts.Commands;
global using AshLake.Services.Yande.Application.Posts.Queries;
global using AshLake.Services.Yande.Infrastructure;
global using AshLake.Services.Yande;
global using AshLake.Services.Yande.Infrastructure.Repositories;