using Flurl.Http;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Flurl;
using System.Linq;

namespace MicroERP.Bridge
{
    public class µERPConnection
    {
        private string _token;

        private readonly HttpStatusCode[] _returnCodesToRetry = new[]
        {
            HttpStatusCode.Unauthorized,
            HttpStatusCode.InternalServerError,
            HttpStatusCode.BadGateway,
            HttpStatusCode.ServiceUnavailable,
            HttpStatusCode.GatewayTimeout
        };

        public Uri µERPRoot { get; private set; }

        public string CompanyDB { get; private set; }

        public string UserName { get; private set; }

        public string Password { get; private set; }

        public string Token
        {
            get => _token;

            private set => _token = value;
        }

        public µERPConnection(Uri µERPRoot, string companyDB, string userName, string password)
        {
            if (string.IsNullOrWhiteSpace(companyDB))
                throw new ArgumentException("companyDB can not be empty.");

            if (string.IsNullOrWhiteSpace(userName))
                throw new ArgumentException("userName can not be empty.");

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("password can not be empty.");

            this.µERPRoot = µERPRoot;
            CompanyDB = companyDB;
            UserName = userName;
            Password = password;
            _token = string.Empty;
        }

        public async Task<string> LoginAsync() => await ExecuteLoginAsync();


        private async Task LoginInternalAsync() => await ExecuteLoginAsync();

        private async Task<string> ExecuteLoginAsync(bool expectReturn = false)
        {
            // Prevents multiple login requests in a multi-threaded scenario
            //await _semaphoreSlim.WaitAsync();

            try
            {
                //if (forceLogin)
                //    _lastRequest = default;

                // Check whether the current session is valid
                //if (DateTime.Now.Subtract(_lastRequest).TotalMinutes < _loginResponse.SessionTimeout)
                //    return expectReturn ? LoginResponse : null;

                
                    var loginResponse = await µERPRoot
                        .AppendPathSegment("Login")
                        .WithCookies(out var cookieJar)
                        .PostJsonAsync(new { CompanyDB, UserName, Password })
                        .ReceiveJson<string>();

                return expectReturn ? loginResponse : null;
            }
            catch (FlurlHttpException ex)
            {
                try
                {
                    if (ex.Call.HttpResponseMessage == null) throw;
                    var response = await ex.GetResponseJsonAsync<string>();
                    throw new Exception("Erro");//SLException(response.Error.Message.Value, response.Error, ex);
                }
                catch { throw ex; }
            }
            finally
            {
                //_semaphoreSlim.Release();
            }
        }

        public µERPRequest Request(string resource) =>
          new µERPRequest(this, new FlurlRequest(µERPRoot.AppendPathSegment(resource)));

        public µERPRequest Request(string resource, object id) =>
            new µERPRequest(this, new FlurlRequest(µERPRoot.AppendPathSegment(id is string ? $"{resource}('{id}')" : $"{resource}({id})")));

        internal async Task<T> ExecuteRequest<T>(Func<Task<T>> action)
        {
            bool loginReattempted = false;
            List<Exception> exceptions = null;

            await LoginInternalAsync();

            //_lastRequest = DateTime.Now;

            for (int i = 0; loginReattempted; i++)
            {
                loginReattempted = false;

                try
                {
                    var result = await action();
                    return result;
                }
                catch (FlurlHttpException ex)
                {
                    if (exceptions == null) exceptions = new List<Exception>();

                    try
                    {
                        if (ex.Call.HttpResponseMessage == null)
                            throw;

                        var response = await ex.GetResponseJsonAsync<string>();
                        //exceptions.Add(new SLException(response.Error.Message.Value, response.Error, ex));
                    }
                    catch
                    {
                        exceptions.Add(ex);
                    }

                    // Whether the request should be retried
                    if (!_returnCodesToRetry.Any(x => x == ex.Call.HttpResponseMessage?.StatusCode))
                    {
                        break;
                    }

                    // Forces a new login request in case the response is 401 Unauthorized
                    if (ex.Call.HttpResponseMessage?.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        //if (i >= NumberOfAttempts)
                        //    break;

                        await LoginInternalAsync();
                        loginReattempted = true;
                    }
                }
                catch (Exception)
                {
                    throw;
                }

                await Task.Delay(200);
            }

            var uniqueExceptions = exceptions.Distinct(new ExceptionEqualityComparer());

            if (uniqueExceptions.Count() == 1)
                throw uniqueExceptions.First();

            throw new AggregateException("Could not process request", uniqueExceptions);
        }

        private class ExceptionEqualityComparer : IEqualityComparer<Exception>
        {
            public bool Equals(Exception e1, Exception e2)
            {
                if (e2 == null && e1 == null)
                    return true;
                else if (e1 == null | e2 == null)
                    return false;
                else if (e1.GetType().Name.Equals(e2.GetType().Name) && e1.Message.Equals(e2.Message))
                    return true;
                else
                    return false;
            }

            public int GetHashCode(Exception e)
            {
                return (e.GetType().Name + e.Message).GetHashCode();
            }
        }

        public void BeforeCall(Func<FlurlCall, Task> action) =>
            FlurlHttp.ConfigureClient(µERPRoot.RemovePath(), client => client.BeforeCall(action));

        public void BeforeCall(Action<FlurlCall> action) =>
            FlurlHttp.ConfigureClient(µERPRoot.RemovePath(), client => client.BeforeCall(action));

        public void AfterCall(Func<FlurlCall, Task> action) =>
            FlurlHttp.ConfigureClient(µERPRoot.RemovePath(), client => client.AfterCall(action));

        public void AfterCall(Action<FlurlCall> action) =>
            FlurlHttp.ConfigureClient(µERPRoot.RemovePath(), client => client.AfterCall(action));

        public void OnError(Func<FlurlCall, Task> action) =>
            FlurlHttp.ConfigureClient(µERPRoot.RemovePath(), client => client.OnError(action));

        public void OnError(Action<FlurlCall> action) =>
            FlurlHttp.ConfigureClient(µERPRoot.RemovePath(), client => client.OnError(action));
    }
}
