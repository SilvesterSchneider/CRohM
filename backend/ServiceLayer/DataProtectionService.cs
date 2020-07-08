using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using ModelLayer.DataTransferObjects;
using Newtonsoft.Json.Linq;

namespace ServiceLayer
{
    public interface IDataProtectionService
    {
        Task<List<string>> MakeSomethingAsync(JObject carlist, ContactDto car);
    }

    public class DataProtectionService : IDataProtectionService
    {
        public async Task<List<string>> MakeSomethingAsync(JObject obj, ContactDto car)
        {
            // TODO: return await
            await Task.Delay(1);

            var list = new List<string>();

            list = loop(null, obj);

            // TODO: should i really use identity result?
            return list;
        }

        private List<string> loop(string pre, JObject obj)
        {
            var list = new List<string>();
            // TODO: hier weitermachen || sieht noch nicht so aus wie in dem wf008 von julian

            foreach (var x in obj)
            {
                string name = x.Key;
                JToken value = x.Value;

                JObject test = value as JObject;
                if (test != null)
                {
                    list = loop(name, test);
                }
                else
                {
                    string tuple;
                    if (pre != null)
                    {
                        tuple = pre;
                    }
                    //wie bekomme ich den pre vorne hin?
                    tuple = obj.GetValue("type") + " " + obj.GetValue("data");

                    list.Add(tuple);
                }
            }

            return list;
        }
    }

    /*{
    type: this.compareValues(obj1, obj2),
    data: obj1 === undefined? obj2 : obj1
    }*/
}