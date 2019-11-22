﻿using GraphQL;
using GraphQL.Http;
using GraphQL.Types;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Heroes.Server.Gql.Core
{
	public class GraphQLMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly GqlMiddlewareOptions _settings;
		private readonly IDocumentExecuter _executer;
		private readonly IDocumentWriter _writer;

		public GraphQLMiddleware(
			RequestDelegate next,
			GqlMiddlewareOptions settings,
			IDocumentExecuter executer,
			IDocumentWriter writer)
		{
			_next = next;
			_settings = settings;
			_executer = executer;
			_writer = writer;
		}

		public async Task Invoke(HttpContext context, ISchema schema)
		{
			if (!IsGqlRequest(context))
			{
				await _next(context);
				return;
			}

			await ExecuteAsync(context, schema);
		}

		private bool IsGqlRequest(HttpContext context)
			=> context.Request.Path.StartsWithSegments(_settings.Path)
			&& string.Equals(context.Request.Method, "POST", StringComparison.OrdinalIgnoreCase);

		private async Task ExecuteAsync(HttpContext context, ISchema schema)
		{
			string body;
			using (var streamReader = new StreamReader(context.Request.Body))
			{
				body = await streamReader.ReadToEndAsync().ConfigureAwait(true);
			}

			var request = JsonConvert.DeserializeObject<GraphQLRequest>(body);

			var result = await _executer.ExecuteAsync(_ =>
			{
				_.Schema = schema;
				_.Query = request.Query;
				_.OperationName = request.OperationName;
				_.Inputs = request.Variables.ToInputs();
				_.UserContext = _settings.BuildUserContext?.Invoke(context);
			});

			await WriteResponseAsync(context, result);
		}

		private async Task WriteResponseAsync(HttpContext context, ExecutionResult result)
		{
			var json = _writer.Write(result);

			context.Response.ContentType = "application/json";
			context.Response.StatusCode = result.Errors?.Any() == true ? (int)HttpStatusCode.BadRequest : (int)HttpStatusCode.OK;

			await context.Response.WriteAsync(json);
		}
	}
}