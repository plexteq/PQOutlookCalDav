using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalCli.API
{
    public interface ISerializer
    {
        Func<Type, object> DependencyResolver { get; set; }
        
        Encoding Encoding { get; set; }

        T GetService<T>();
        
        T Deserialize<T>(Stream stream, System.Text.Encoding encoding = null) where T : ISerializeToICAL;

        T Deserialize<T>(TextReader rdr) where T : ISerializeToICAL;
        
        void Serialize<T>(Stream stream, T obj, System.Text.Encoding encoding = null) where T : ISerializeToICAL;

        void Serialize<T>(TextWriter wrtr, T obj) where T : ISerializeToICAL;
    }
}
