using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MicroERP.ModelsDB.Models;
using MicroERP.Bridge.Exceptions;
using Flurl.Http;
using Flurl.Http.Configuration;

namespace MicroERP.Bridge.Extensions
{
    public static class ΜicroERPExtensionMethods
    {

        /// <summary>
        /// Add the current object via the µERP Server to the provided connection
        /// </summary>
        /// <param name="microERPObject">microERP current object</param>
        /// <param name="µERPConnection">Database connection ServiceLayer</param>
        /// <returns></returns>
        /// <exception cref="AddException">Throw an exception in case an error occurs in the POST method of the µERP Server</exception>
        public static async Task Add(this IMicroERP microERPObject, MicroERPConnection µERPConnection)
        {
            try
            {
                Type type = microERPObject.GetType();

                PropertyInfo[] properties = type.GetProperties();

                var microERPConcreteObject = (IMicroERP)Activator.CreateInstance(type);

                JObject response = await µERPConnection.Request(microERPConcreteObject.GetType().Name).PostStringAsync<JObject>(microERPObject.MakeRequest());

                var constructor = type.GetConstructor(new[] { typeof(string) });

                if (constructor != null)
                {
                    var newObject = constructor.Invoke(new object[] { response.ToString() });

                    if (newObject is IMicroERP IMicroERPConcrete)
                    {
                        foreach (var property in properties.Where(prop => prop.CanWrite))
                        {
                            object value = property.GetValue(IMicroERPConcrete);
                            property.SetValue(microERPObject, value);
                        }
                    }
                }
            }
            catch (MicroERPException adEx)
            {
                throw new AddException(adEx.Message, adEx, adEx.InnerException);
            }


        }

        /// <summary>
        /// Update the current object via the µERP Server to the provided connection
        /// </summary>
        /// <param name="microERPObject">microERP current object</param>
        /// <param name="µERPConnection">Database connection ServiceLayer</param>
        /// <returns></returns>
        /// <exception cref="UpdateException">Throw an exception in case an error occurs in the PATCH method of the µERP Server</exception>
        public static async Task Update(this IMicroERP microERPObject, MicroERPConnection µERPConnection)
        {
            try
            {
                Type type = microERPObject.GetType();

                PropertyInfo[] properties = type.GetProperties();

                var activator = (IMicroERP)Activator.CreateInstance(type);


                await µERPConnection.Request($"{microERPObject.GetType().Name}{$"({microERPObject.Id})"}").PatchStringAsync(microERPObject.MakeRequest());

                var response = await µERPConnection.Request($"{microERPObject.GetType().Name}{$"({microERPObject.Id})"}").GetStringAsync();

                var constructor = type.GetConstructor(new[] { typeof(string) });

                if (constructor != null)
                {
                    var newObject = constructor.Invoke(new object[] { response });

                    if (newObject is IMicroERP IMicroERPConcrete)
                    {
                        foreach (var property in properties.Where(prop => prop.CanWrite))
                        {
                            object value = property.GetValue(IMicroERPConcrete);
                            property.SetValue(microERPObject, value);
                        }
                    }
                }
            }
            catch (MicroERPException adEx)
            {
                throw new UpdateException(adEx.Message, adEx, adEx.InnerException);
            }

        }

        /// <summary>
        /// Delete the current object via the µERP Server to the provided connection
        /// </summary>
        /// <param name="microERPObject">microERP current object</param>
        /// <param name="µERPConnection">Database connection ServiceLayer</param>
        /// <returns></returns>
        /// <exception cref="DeleteException">Throw an exception in case an error occurs in the DELETE method of the µERP Server</exception>
        public static async Task Delete(this IMicroERP microERPObject, MicroERPConnection µERPConnection)
        {
            try
            {
                Type type = microERPObject.GetType();

                PropertyInfo[] properties = type.GetProperties();

                var microERPConcreteObject = (IMicroERP)Activator.CreateInstance(type);

                await µERPConnection.Request($"{microERPObject.GetType().Name}{$"({microERPObject.Id})"}").DeleteAsync();
            }
            catch (MicroERPException adEx)
            {
                throw new DeleteException(adEx.Message, adEx, adEx.InnerException);
            }

        }

        /// <summary>
        /// Get object with current type via the µERP Server to the provided connection
        /// </summary>
        /// <param name="microERPObject">microERP current object</param>
        /// <param name="µERPConnection">Database connection ServiceLayer</param>
        /// <returns></returns>
        /// <exception cref="SearchException">Throw an exception in case an error occurs in the GET method of the µERP Server</exception>
        public static async Task GetByKey(this IMicroERP microERPObject, string key, MicroERPConnection µERPConnection)
        {
            try
            {
                Type type = microERPObject.GetType();

                PropertyInfo[] properties = type.GetProperties();

                string keyParsed = $"({key})";

                var response = await µERPConnection.Request($"{microERPObject.GetType().Name}{keyParsed}").GetStringAsync();

                var constructor = type.GetConstructor(new[] { typeof(string) });

                if (constructor != null)
                {
                    var newObject = constructor.Invoke(new object[] { response });

                    if (newObject is IMicroERP IMicroERPConcrete)
                    {
                        foreach (var property in properties.Where(prop => prop.CanWrite))
                        {
                            object value = property.GetValue(IMicroERPConcrete);
                            property.SetValue(microERPObject, value);
                        }
                    }
                }
            }
            catch (MicroERPException adEx)
            {
                throw new SearchException(adEx.Message, adEx, adEx.InnerException);
            }
        }

        /// <summary>
        /// Get objects with current type via the µERP Server to the provided connection
        /// </summary>
        /// <typeparam name="IMicroERP">Object with implements IMicroERPInterface</typeparam>
        /// <param name="microERPObject">microERP current object list</param>
        /// <param name="µERPConnection">Database connection ServiceLayer</param>
        /// <param name="keys">Primary keys with want get object</param>
        /// <returns></returns>
        /// <exception cref="SearchException">Throw an exception in case an error occurs in the GET method of the µERP Server</exception>
        public static async Task GetByKeys<IMicroERP>(this ICollection<IMicroERP> microERPObjects, MicroERPConnection µERPConnection, params string[] keys)
        {
            try
            {
                for (int i = 0; i < keys.Length; i++)
                {
                    Type Enumtype = microERPObjects.GetType();

                    Type type = Enumtype.GetGenericArguments()[0];

                    PropertyInfo[] properties = type.GetProperties();

                    var microERPObject = Activator.CreateInstance(type);

                    string keyParsed = $"({keys[i]})";

                    var response = await µERPConnection.Request($"{type.Name}{keyParsed}").GetStringAsync();

                    var constructor = type.GetConstructor(new[] { typeof(string) });

                    if (constructor != null)
                    {
                        var newObject = constructor.Invoke(new object[] { response });

                        if (newObject is IMicroERP IMicroERPConcrete)
                        {
                            foreach (var property in properties.Where(prop => prop.CanWrite))
                            {
                                object value = property.GetValue(IMicroERPConcrete);
                                property.SetValue(microERPObject, value);
                            }
                        }
                    }

                    microERPObjects.Add((IMicroERP)microERPObject);
                }
            }
            catch (MicroERPException adEx)
            {
                throw new SearchException(adEx.Message, adEx, adEx.InnerException);
            }
        }

        /// <summary>
        /// Get objects with current type via the µERP Server to the provided connection
        /// </summary>
        /// <typeparam name="IMicroERP">Object with implements IMicroERPInterface</typeparam>
        /// <param name="microERPObjects">microERP current object list</param>
        /// <param name="parameters">Raw select/filter/orderby/skip and others ServiceLayer filters</param>
        /// <param name="µERPConnection">Database connection ServiceLayer</param>
        /// <returns></returns>
        /// <exception cref="SearchException">Throw an exception in case an error occurs in the GET method of the µERP Server</exception>
        public static async Task GetByFilter<IMicroERP>(this ICollection<IMicroERP> microERPObjects, string parameters, MicroERPConnection µERPConnection)
        {
            try
            {
                Type Enumtype = microERPObjects.GetType();
                Type type = Enumtype.GetGenericArguments()[0];
                PropertyInfo[] properties = type.GetProperties();

                var microERPObject = Activator.CreateInstance(type);


                // Divida a string da consulta em partes separadas por '&'
                string[] queryParts = parameters.Split('&');

                // Use LINQ para criar um dicionário
                Dictionary<string, string> queryDict = queryParts
                    .Select(part => part.Split('='))
                    .Where(keyValue => keyValue.Length == 2)
                    .ToDictionary(keyValue => keyValue[0], keyValue => Uri.UnescapeDataString(keyValue[1]));


                MicroERPRequest request = µERPConnection.Request($"{type.Name}");

                foreach (var kvp in queryDict)
                    request.SetQueryParam(kvp.Key, kvp.Value);

                var responseList = await request.GetAsync<JObject>();

                foreach (var response in responseList)
                {
                    var constructor = type.GetConstructor(new[] { typeof(string) });

                    var newmicroERPObj = Activator.CreateInstance(type);

                    if (constructor != null)
                    {
                        var newObject = constructor.Invoke(new object[] { response.ToString() });

                        if (newObject is IMicroERP IMicroERPConcrete)
                        {
                            foreach (var property in properties.Where(prop => prop.CanWrite))
                            {
                                object value = property.GetValue(IMicroERPConcrete);
                                property.SetValue(newmicroERPObj, value);
                            }
                        }
                    }
                    microERPObjects.Add((IMicroERP)newmicroERPObj);
                }
            }
            catch (MicroERPException adEx)
            {
                throw new SearchException(adEx.Message, adEx, adEx.InnerException);
            }
        }

        internal static void µERPClone(this IMicroERP microERPObject, IMicroERP originalmicroERPObject)
        {
            Type type = originalmicroERPObject.GetType();

            PropertyInfo[] properties = type.GetProperties();

            foreach (var property in properties.Where(prop => prop.CanWrite))
            {
                object value = property.GetValue(originalmicroERPObject);
                property.SetValue(microERPObject, value);
            }
        }

        public static MicroERPRequest SetQueryParam(this MicroERPRequest request, string name, string value)
        {
            request.FlurlRequest.SetQueryParam(name, value);
            return request;
        }

    }

    public class MicroERPManyResquest
    {
        public MicroERPManyResquest(HttpMethod httpMethod, IMicroERP microERPObject)
        {
            HttpMethod = httpMethod;
            MicroERPObject = microERPObject;
        }

        public HttpMethod HttpMethod { get; set; }

        public IMicroERP MicroERPObject { get; set; }
    }

}
