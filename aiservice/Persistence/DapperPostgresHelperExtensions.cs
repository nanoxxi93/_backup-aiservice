using AIService.Entities;
using Dapper;
using Npgsql;
using Polly;
using Polly.Retry;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace AIService.Persistence
{
    public static class DapperPostgresHelperExtensions
    {
        private static readonly IEnumerable<TimeSpan> RetryTimes = new[]
        {
            TimeSpan.FromSeconds(1),
            TimeSpan.FromSeconds(2),
            TimeSpan.FromSeconds(3)
        };

        private static Func<Exception, bool> PostgresDatabaseTransientErrorDetectionStrategy()
        {
            return (ex) =>
            {
                //if it is not a postgres exception we must assume it will be transient
                if (ex.GetType() != typeof(PostgresException))
                    return true;

                var pgex = ex as PostgresException;
                switch (pgex.SqlState)
                {
                    case "53000":   //insufficient_resources
                    case "53100":   //disk_full
                    case "53200":   //out_of_memory
                    case "53300":   //too_many_connections
                    case "53400":   //configuration_limit_exceeded
                    case "57P03":   //cannot_connect_now
                    case "58000":   //system_error
                    case "58030":   //io_error

                    //These next few I am not sure whether they should be treated as transient or not, but I am guessing so

                    case "55P03":   //lock_not_available
                    case "55006":   //object_in_use
                    case "55000":   //object_not_in_prerequisite_state
                    case "08000":   //connection_exception
                    case "08003":   //connection_does_not_exist
                    case "08006":   //connection_failure
                    case "08001":   //sqlclient_unable_to_establish_sqlconnection
                    case "08004":   //sqlserver_rejected_establishment_of_sqlconnection
                    case "08007":   //transaction_resolution_unknown
                        return true;
                }
                return false;
            };
        }

        private static AsyncRetryPolicy RetryPolicy(AppSettings appSettings, RequestInDTO requestInDTO) {
            return Policy
            .Handle<Exception>(PostgresDatabaseTransientErrorDetectionStrategy())
            .Or<TimeoutException>()
            .WaitAndRetryAsync(RetryTimes,
            (exception, timeSpan, retryCount, context) =>
            {
                Log.ForContext("Layer", $"Persistence.DapperPostgresHelper").ForContext("MethodName", "RetryPolicy").ForContext("Ip", requestInDTO.Ip).ForContext("Guid", requestInDTO.Guid)
                .Error($"WARNING: Error talking to Db, will retry after {timeSpan}. Retry attempt {retryCount}");
            });
        }

        public static async Task<int> ExecuteAsyncWithRetry(this IDbConnection cnn, AppSettings appSettings, RequestInDTO requestInDTO, string sql, object param = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return await RetryPolicy(appSettings, requestInDTO).ExecuteAsync(async () => await cnn.ExecuteAsync(sql, param: param, commandTimeout: commandTimeout, commandType: commandType));
        }
            

        public static async Task<IEnumerable<T>> QueryAsyncWithRetry<T>(this IDbConnection cnn, AppSettings appSettings, RequestInDTO requestInDTO, string sql, object param = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return await RetryPolicy(appSettings, requestInDTO).ExecuteAsync(async () => await cnn.QueryAsync<T>(sql, param: param, commandTimeout: commandTimeout, commandType: commandType));
        }
    }
}
