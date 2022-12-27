using Dapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using AIService.Entities;
using Serilog;

namespace AIService.Persistence
{
    public class DapperPostgresHelper
    {
        private static string label = "Persistence";
        private static string className = "DapperPostgresHelper";
        public static async Task<T> ExecuteSP_SingleClass<T>(AppSettings appSettings, RequestInDTO requestInDTO, string spName, dynamic param = null) where T : class
        {
            string methodName = "ExecuteSP_SingleClass";
            try
            {
                string spValue = appSettings.Queries.First(x => x.Key == spName).Value;
                using (var connection = new NpgsqlConnection(GetConnectionString(appSettings)))
                {
                    var temp = await connection.QueryAsyncWithRetry<T>(appSettings, requestInDTO, spValue, param: (object)param, commandType: System.Data.CommandType.Text);
                    Log.ForContext("Layer", $"{label}.{className}").ForContext("MethodName", methodName).ForContext("Ip", requestInDTO.Ip).ForContext("Guid", requestInDTO.Guid)
                        .Debug($"RESULT: {JsonConvert.SerializeObject(Enumerable.FirstOrDefault<T>(temp))}");
                    return await Task.Run(() =>
                    Enumerable.FirstOrDefault<T>(temp));
                }
            }
            catch (Exception e)
            {
                Log.ForContext("Layer", $"{label}.{className}").ForContext("MethodName", methodName).ForContext("Ip", requestInDTO.Ip).ForContext("Guid", requestInDTO.Guid)
                    .Error(e, $"SQL ERROR: {spName} {(param != null ? JsonConvert.SerializeObject(param) : "")}");
                throw e;
            }
        }

        public static async Task<dynamic> ExecuteSP_MultipleClass<T>(AppSettings appSettings, RequestInDTO requestInDTO, string spName, dynamic param = null) where T : class
        {
            string methodName = "ExecuteSP_MultipleClass";
            try
            {
                string spValue = appSettings.Queries.First(x => x.Key == spName).Value;
                using (var connection = new NpgsqlConnection(GetConnectionString(appSettings)))
                {
                    var temp = await connection.QueryAsyncWithRetry<T>(appSettings, requestInDTO, spValue, param: (object)param, commandType: System.Data.CommandType.Text);
                    Log.ForContext("Layer", $"{label}.{className}").ForContext("MethodName", methodName).ForContext("Ip", requestInDTO.Ip).ForContext("Guid", requestInDTO.Guid)
                        .Debug($"RESULT: {JsonConvert.SerializeObject(temp)}");
                    return temp;
                }
            }
            catch (Exception e)
            {
                Log.ForContext("Layer", $"{label}.{className}").ForContext("MethodName", methodName).ForContext("Ip", requestInDTO.Ip).ForContext("Guid", requestInDTO.Guid)
                    .Error(e, $"SQL ERROR: {spName} {(param != null ? JsonConvert.SerializeObject(param) : "")}");
                throw e;
            }
        }

        public static async Task<dynamic> ExecuteText_SingleClass<T>(AppSettings appSettings, RequestInDTO requestInDTO, string query, dynamic param = null) where T : class
        {
            string methodName = "ExecuteText_SingleClass";
            try
            {
                using (var connection = new NpgsqlConnection(GetConnectionString(appSettings)))
                {
                    var temp = await connection.QueryAsyncWithRetry<T>(appSettings, requestInDTO, query, param: (object)param, commandType: System.Data.CommandType.Text);
                    Log.ForContext("Layer", $"{label}.{className}").ForContext("MethodName", methodName).ForContext("Ip", requestInDTO.Ip).ForContext("Guid", requestInDTO.Guid)
                        .Debug($"RESULT: {JsonConvert.SerializeObject(Enumerable.FirstOrDefault<T>(temp))}");
                    return await Task.Run(() =>
                    Enumerable.FirstOrDefault<T>(temp));
                }
            }
            catch (Exception e)
            {
                Log.ForContext("Layer", $"{label}.{className}").ForContext("MethodName", methodName).ForContext("Ip", requestInDTO.Ip).ForContext("Guid", requestInDTO.Guid)
                    .Error(e, $"SQL ERROR: {query} {(param != null ? JsonConvert.SerializeObject(param) : "")}");
                throw e;
            }
        }

        public static async Task<dynamic> ExecuteText_MultipleClass<T>(AppSettings appSettings, RequestInDTO requestInDTO, string query, dynamic param = null) where T : class
        {
            string methodName = "ExecuteText_MultipleClass";
            try
            {
                using (var connection = new NpgsqlConnection(GetConnectionString(appSettings)))
                {
                    var temp = await connection.QueryAsyncWithRetry<T>(appSettings, requestInDTO, query, param: (object)param, commandType: System.Data.CommandType.Text);
                    Log.ForContext("Layer", $"{label}.{className}").ForContext("MethodName", methodName).ForContext("Ip", requestInDTO.Ip).ForContext("Guid", requestInDTO.Guid)
                        .Debug($"RESULT: {JsonConvert.SerializeObject(temp)}");
                    return temp;
                }
            }
            catch (Exception e)
            {
                Log.ForContext("Layer", $"{label}.{className}").ForContext("MethodName", methodName).ForContext("Ip", requestInDTO.Ip).ForContext("Guid", requestInDTO.Guid)
                    .Error(e, $"SQL ERROR: {query} {(param != null ? JsonConvert.SerializeObject(param) : "")}");
                throw e;
            }
        }

        public static async Task<dynamic> ExecuteSP_SingleDictionary<T>(AppSettings appSettings, RequestInDTO requestInDTO, string spName, dynamic param = null) where T : class
        {
            string methodName = "ExecuteSP_SingleDictionary";
            try
            {
                string spValue = appSettings.Queries.First(x => x.Key == spName).Value;
                IDictionary<string, object> keyValuePairs = FormatKeyValues(param);
                using (var connection = new NpgsqlConnection(GetConnectionString(appSettings)))
                {
                    var temp = await connection.QueryAsyncWithRetry<T>(appSettings, requestInDTO, spValue, param: (object)keyValuePairs, commandType: System.Data.CommandType.Text);
                    Log.ForContext("Layer", $"{label}.{className}").ForContext("MethodName", methodName).ForContext("Ip", requestInDTO.Ip).ForContext("Guid", requestInDTO.Guid)
                        .Debug($"RESULT: {JsonConvert.SerializeObject(Enumerable.FirstOrDefault<T>(temp))}");
                    return await Task.Run(() =>
                    Enumerable.FirstOrDefault<T>(temp));
                }
            }
            catch (Exception e)
            {
                Log.ForContext("Layer", $"{label}.{className}").ForContext("MethodName", methodName).ForContext("Ip", requestInDTO.Ip).ForContext("Guid", requestInDTO.Guid)
                    .Error(e, $"SQL ERROR: {spName} {(param != null ? JsonConvert.SerializeObject(param) : "")}");
                throw e;
            }
        }

        public static async Task<dynamic> ExecuteSP_MultipleDictionary<T>(AppSettings appSettings, RequestInDTO requestInDTO, string spName, dynamic param = null) where T : class
        {
            string methodName = "ExecuteSP_MultipleDictionary";
            try
            {
                string spValue = appSettings.Queries.First(x => x.Key == spName).Value;
                IDictionary<string, object> keyValuePairs = FormatKeyValues(param);
                using (var connection = new NpgsqlConnection(GetConnectionString(appSettings)))
                {
                    var temp = await connection.QueryAsyncWithRetry<T>(appSettings, requestInDTO, spValue, param: (object)keyValuePairs, commandType: System.Data.CommandType.Text);
                    Log.ForContext("Layer", $"{label}.{className}").ForContext("MethodName", methodName).ForContext("Ip", requestInDTO.Ip).ForContext("Guid", requestInDTO.Guid)
                        .Debug($"RESULT: {JsonConvert.SerializeObject(temp)}");
                    return temp;
                }
            }
            catch (Exception e)
            {
                Log.ForContext("Layer", $"{label}.{className}").ForContext("MethodName", methodName).ForContext("Ip", requestInDTO.Ip).ForContext("Guid", requestInDTO.Guid)
                    .Error(e, $"SQL ERROR: {spName} {(param != null ? JsonConvert.SerializeObject(param) : "")}");
                throw e;
            }
        }

        public static async Task<dynamic> ExecuteText_SingleDictionary<T>(AppSettings appSettings, RequestInDTO requestInDTO, string query, dynamic param = null) where T : class
        {
            string methodName = "ExecuteText_SingleDictionary";
            try
            {
                IDictionary<string, object> keyValuePairs = FormatKeyValues(param);
                using (var connection = new NpgsqlConnection(GetConnectionString(appSettings)))
                {
                    var temp = await connection.QueryAsyncWithRetry<T>(appSettings, requestInDTO, query, param: (object)keyValuePairs, commandType: System.Data.CommandType.Text);
                    Log.ForContext("Layer", $"{label}.{className}").ForContext("MethodName", methodName).ForContext("Ip", requestInDTO.Ip).ForContext("Guid", requestInDTO.Guid)
                        .Debug($"RESULT: {JsonConvert.SerializeObject(Enumerable.FirstOrDefault<T>(temp))}");
                    return await Task.Run(() =>
                    Enumerable.FirstOrDefault<T>(temp));
                }
            }
            catch (Exception e)
            {
                Log.ForContext("Layer", $"{label}.{className}").ForContext("MethodName", methodName).ForContext("Ip", requestInDTO.Ip).ForContext("Guid", requestInDTO.Guid)
                    .Error(e, $"SQL ERROR: {query} {(param != null ? JsonConvert.SerializeObject(param) : "")}");
                throw e;
            }
        }

        public static async Task<dynamic> ExecuteText_MultipleDictionary<T>(AppSettings appSettings, RequestInDTO requestInDTO, string query, dynamic param = null) where T : class
        {
            string methodName = "ExecuteText_MultipleDictionary";
            try
            {
                IDictionary<string, object> keyValuePairs = FormatKeyValues(param);
                using (var connection = new NpgsqlConnection(GetConnectionString(appSettings)))
                {
                    var temp = await connection.QueryAsyncWithRetry<T>(appSettings, requestInDTO, query, param: (object)keyValuePairs, commandType: System.Data.CommandType.Text);
                    Log.ForContext("Layer", $"{label}.{className}").ForContext("MethodName", methodName).ForContext("Ip", requestInDTO.Ip).ForContext("Guid", requestInDTO.Guid)
                        .Debug($"RESULT: {JsonConvert.SerializeObject(temp)}");
                    return temp;
                }
            }
            catch (Exception e)
            {
                Log.ForContext("Layer", $"{label}.{className}").ForContext("MethodName", methodName).ForContext("Ip", requestInDTO.Ip).ForContext("Guid", requestInDTO.Guid)
                    .Error(e, $"SQL ERROR: {query} {(param != null ? JsonConvert.SerializeObject(param) : "")}");
                throw e;
            }
        }

        public static async Task<dynamic> ExecuteSP_MultipleDictionaryFree<T>(AppSettings appSettings, RequestInDTO requestInDTO, string spName, dynamic param = null) where T : class
        {
            string methodName = "ExecuteSP_MultipleDictionaryFree";
            try
            {
                string spValue = appSettings.Queries.First(x => x.Key == spName).Value;
                IDictionary<string, object> keyValuePairs = FormatKeyValues(param);
                using (var connection = new NpgsqlConnection(GetConnectionString(appSettings)))
                {
                    var temp = await connection.QueryAsyncWithRetry<T>(appSettings, requestInDTO, spValue, param: (object)keyValuePairs, commandType: System.Data.CommandType.Text);
                    Log.ForContext("Layer", $"{label}.{className}").ForContext("MethodName", methodName).ForContext("Ip", requestInDTO.Ip).ForContext("Guid", requestInDTO.Guid)
                        .Debug($"RESULT: {JsonConvert.SerializeObject(temp)}");
                    return temp;
                }
            }
            catch (Exception e)
            {
                Log.ForContext("Layer", $"{label}.{className}").ForContext("MethodName", methodName).ForContext("Ip", requestInDTO.Ip).ForContext("Guid", requestInDTO.Guid)
                    .Error(e, $"SQL ERROR: {spName} {(param != null ? JsonConvert.SerializeObject(param) : "")}");
                throw e;
            }
        }

        public static string GetConnectionString(AppSettings appSettings)
        {
            return appSettings.Environments.Find(x => x.Label == appSettings.Environment).ConnectionStrings.PostgresSQL;
        }

        public static IDictionary<string, object> FormatKeyValues(dynamic param)
        {
            IDictionary<string, object> keyValuePairs = new ExpandoObject();
            foreach (KeyValuePair<string, object> item in param)
            {
                if (item.Value != null)
                {
                    if (item.Value.GetType().Name == "JArray")
                    {
                        keyValuePairs.Add("@" + item.Key, item.Value.ToString().Replace("[\r\n", "").Replace("\r\n]", "").Replace("\r\n", "").Replace("  ", ""));
                    }
                    else
                    {
                        keyValuePairs.Add("@" + item.Key, item.Value);
                    }
                }
                else
                {
                    keyValuePairs.Add("@" + item.Key, null);
                }
            }
            return keyValuePairs;
        }

        public static Dictionary<string, object> ToDictionary(object value)
        {
            IDictionary<string, object> dapperRowProperties = value as IDictionary<string, object>;

            Dictionary<string, object> expando = new Dictionary<string, object>();

            foreach (KeyValuePair<string, object> property in dapperRowProperties)
                expando.Add(property.Key, property.Value);

            return expando as Dictionary<string, object>;
        }

        public static JObject ToJObject(object value)
        {
            IDictionary<string, object> dapperRowProperties = value as IDictionary<string, object>;

            JObject expando = new JObject();

            if (value != null)
            {
                foreach (KeyValuePair<string, object> property in dapperRowProperties)
                    expando.Add(property.Key, new JValue(property.Value));
            }

            return expando as JObject;
        }

        public static List<JObject> ToJObjectList(List<object> value)
        {
            List<JObject> expando = new List<JObject>();

            foreach (object item in value)
            {
                var temp = ToJObject(item);
                expando.Add(temp);
            }

            return expando as List<JObject>;
        }
    }
}
