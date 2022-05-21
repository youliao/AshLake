﻿global using AshLake.Services.Archiver;
global using Microsoft.OpenApi.Models;
global using AshLake.Services.Archiver.Domain.Repositories;
global using AshLake.Contracts.Seedwork.Extensions;
global using Microsoft.AspNetCore.Mvc;
global using MediatR;
global using AshLake.Services.Archiver.Infrastructure.Repositories;
global using MongoDB.Bson;
global using AshLake.Contracts.Seedwork.Converts;
global using Hangfire;
global using AshLake.Services.Archiver.Application.BackgroundJobs;
global using AshLake.Contracts.Seedwork;
global using AshLake.Contracts.Seedwork.SourceSites;
global using AshLake.Services.Archiver.Domain.Entities;
global using AshLake.Services.Archiver.Integration.GrabberServices;
global using System.Text.Json.Nodes;
global using Dapr.Client;
global using Ardalis.GuardClauses;